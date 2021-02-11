namespace Unosquare.Tubular
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Extensions methods.
    /// </summary>
    public static class Extensions
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.f";
        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Adjust a timezone data in a object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="timezoneOffset">The timezone offset.</param>
        /// <returns>The same object with DateTime properties adjusted to the timezone specified.</returns>
        public static object AdjustTimeZone(this object data, int timezoneOffset)
        {
            if (data == null) 
                throw new ArgumentNullException(nameof(data));

            var dateTimeProperties = data.GetType().GetDateProperties();

            foreach (var prop in dateTimeProperties)
                data.AdjustTimeZoneForProperty(timezoneOffset, prop);

            return data;
        }

        /// <summary>
        /// Generates a GridDataResponse using the GridDataRequest and an IQueryable source,
        /// like a DataSet in Entity Framework.
        /// </summary>
        /// <param name="request">The Tubular Grid Request.</param>
        /// <param name="dataSource">The IQueryable source.</param>
        /// <param name="preProcessSubset">The subset's process delegate.</param>
        /// <returns>A grid response.</returns>
        public static GridDataResponse CreateGridDataResponse(
            this GridDataRequest request,
            IQueryable dataSource,
            Func<IQueryable, IQueryable>? preProcessSubset = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (dataSource == null) 
                throw new ArgumentNullException(nameof(dataSource));
            if (request.Columns.Any() != true)
                throw new ArgumentOutOfRangeException(nameof(request), "Missing column information");

            var response = new GridDataResponse
            {
                Counter = request.Counter,
                TotalRecordCount = dataSource.Count(),
                FilteredRecordCount = dataSource.Count(),
            };

            var properties = dataSource.ElementType.ExtractProperties();
            var columnMap = MapColumnsToProperties(request.Columns, properties);

            var subset = FilterResponse(request, dataSource, response);

            // Perform Sorting
            var orderingExpression = request.Columns.Where(x => x.SortOrder > 0)
                .OrderBy(x => x.SortOrder)
                .Aggregate(string.Empty,
                    (current, column) =>
                        current +
                        (column.Name + " " + (column.SortDirection == SortDirection.Ascending ? "ASC" : "DESC") +
                         ", "));

            // Apply the sorting expression if supplied
            subset = !string.IsNullOrWhiteSpace(orderingExpression)
                ? subset.OrderBy(orderingExpression.Substring(0, orderingExpression.Length - 2))
                : subset.OrderBy($"{request.Columns.First().Name} ASC");

            // Check aggregations before paging
            // Should it aggregate before filtering too?
            response.AggregationPayload = AggregateSubset(request.Columns, subset);

            var pageSize = request.Take;

            // Take with value -1 represents entire set
            if (request.Take == -1)
            {
                response.TotalPages = 1;
                response.CurrentPage = 1;
                pageSize = subset.Count();
                subset = subset.Skip(request.Skip);
            }
            else
            {
                var filteredCount = subset.Count();
                var totalPages = (int)Math.Ceiling((double)filteredCount / pageSize);

                if (totalPages > 0)
                {
                    response.TotalPages = totalPages;
                    var currentPage = (request.Skip / pageSize) + 1;
                    response.CurrentPage = Math.Min(currentPage, totalPages);

                    if (request.Skip > 0)
                    {
                        var maxSkip = pageSize *
                                      (response.CurrentPage == response.TotalPages
                                          ? response.CurrentPage - 1
                                          : response.CurrentPage);

                        subset = subset.Skip(Math.Min(request.Skip, maxSkip));
                    }
                }
                else
                {
                    response.TotalPages = 1;
                    response.CurrentPage = 1;
                }

                subset = subset.Take(pageSize);
            }

            // Generate the response data in a suitable format
            if (preProcessSubset != null)
                subset = preProcessSubset(subset);

            response.Payload = CreateGridPayload(subset, columnMap, pageSize, request.TimezoneOffset);
            return response;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<GridColumn, PropertyInfo> MapColumnsToProperties(
            IReadOnlyCollection<GridColumn> columns,
            IReadOnlyDictionary<string, PropertyInfo> properties)
        {
            var columnMap = new Dictionary<GridColumn, PropertyInfo>(columns.Count);

            foreach (var column in columns.Where(column => properties.ContainsKey(column.Name)))
            {
                columnMap[column] = properties[column.Name];
            }

            return columnMap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<List<object?>> CreateGridPayload(
            IEnumerable subset,
            Dictionary<GridColumn, PropertyInfo> columnMap,
            int initialCapacity,
            int timezoneOffset)
        {
            var payload = new List<List<object?>>(initialCapacity);

            foreach (var item in subset)
            {
                var payloadItem = new List<object?>(columnMap.Keys.Count);

                foreach (var column in columnMap.Select(m => new { Value = m.Value.GetValue(item), m.Key }))
                {
                    if (column.Value is DateTime time)
                    {
                        // DateTimeUtc and no-time dates should not be timezone shifted
                        // only DateTime (local) should be
                        payloadItem.Add(column.Key.DataType == DataType.DateTime
                            ? time.AddMinutes(-timezoneOffset)
                            : time);
                    }
                    else
                    {
                        payloadItem.Add(column.Value);
                    }
                }

                payload.Add(payloadItem);
            }

            return payload;
        }

        private static void AdjustTimeZoneForProperty(this object data, int timezoneOffset, PropertyInfo prop)
        {
            DateTime value;

            if (prop.PropertyType == typeof(DateTime?))
            {
                var nullableValue = (DateTime?)prop.GetValue(data);
                if (!nullableValue.HasValue) return;
                value = nullableValue.Value;
            }
            else
            {
                value = (DateTime)(prop.GetValue(data) ?? throw new InvalidOperationException());
            }

            value = value.AddMinutes(-timezoneOffset);
            prop.SetValue(data, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<string, object> AggregateSubset(IEnumerable<GridColumn> columns, IQueryable subset)
        {
            var aggregateColumns = columns.Where(c => c.Aggregate != AggregationFunction.None);
            var payload = new Dictionary<string, object>(aggregateColumns.Count());

            foreach (var column in aggregateColumns)
            {
                try
                {
                    AggregateColumn(subset, column, payload);
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException(
                        $"Invalid casting using column {column.Name} with aggregate {column.Aggregate}");
                }
            }

            return payload;
        }

        private static void AggregateColumn(IQueryable subset, GridColumn gridColumn, IDictionary<string, object> payload)
        {
            void Aggregate(GridColumn column,
                Func<IQueryable<double>, double> doubleF,
                Func<IQueryable<decimal>, decimal> decimalF,
                Func<IQueryable<int>, int> intF,
                Func<IQueryable<string?>, string?>? stringF,
                Func<IQueryable<DateTime>, DateTime>? dateF)
            {
                try
                {
                    var propertyType = subset.ElementType.ExtractPropertyType(column.Name);

                    if (propertyType == typeof(double))
                    {
                        payload.Add(column.Name, doubleF(subset.Select(column.Name).Cast<double>()));
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        payload.Add(column.Name, decimalF(subset.Select(column.Name).Cast<decimal>()));
                    }
                    else if (propertyType == typeof(int))
                    {
                        payload.Add(column.Name, intF(subset.Select(column.Name).Cast<int>()));
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        if (dateF == null) return;

                        payload.Add(column.Name, dateF(subset.Select(column.Name).Cast<DateTime>()));
                    }
                    else
                    {
                        if (stringF == null) return;

                        payload.Add(column.Name, stringF(subset.Select(column.Name).Cast<string?>()));
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // EF6 can't materialize a no-nullable value aggregate function returning NULL, so the logic path is return the ONLY value ZERO
                    if (ex.Source != "EntityFramework") throw;
                    payload.Add(column.Name, 0);
                }
            }

            {
                switch (gridColumn.Aggregate)
                {
                    case AggregationFunction.Sum:
                        Aggregate(gridColumn, x => x.Sum(), x => x.Sum(), x => x.Sum(), null, null);

                        break;
                    case AggregationFunction.Average:
                        Aggregate(gridColumn, x => x.Average(), x => x.Average(), x => x.Sum() / x.Count(), null, null);

                        break;
                    case AggregationFunction.Max:
                        Aggregate(gridColumn, x => x.Max(), x => x.Max(), x => x.Max(), x => x.Max(), x => x.Max());

                        break;
                    case AggregationFunction.Min:
                        Aggregate(gridColumn, x => x.Min(), x => x.Min(), x => x.Min(), x => x.Min(), x => x.Min());

                        break;

                    case AggregationFunction.Count:
                        payload.Add(gridColumn.Name, subset.Select(gridColumn.Name).Count());
                        break;
                    case AggregationFunction.DistinctCount:
                        payload.Add(gridColumn.Name,
                            subset.Select(gridColumn.Name).Distinct().Count());
                        break;

                    case AggregationFunction.None:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            $"The AggregationFunction in column {gridColumn.Name} is not valid");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string? GetSqlOperator(CompareOperators op) =>
            op switch
            {
                CompareOperators.Equals => "==",
                CompareOperators.NotEquals => "!=",
                CompareOperators.Gte => ">=",
                CompareOperators.Gt => ">",
                CompareOperators.Lte => "<=",
                CompareOperators.Lt => "<",
                _ => null
            };

        private static IQueryable FilterResponse(GridDataRequest request, IQueryable subset, GridDataResponse response)
        {
            var isDbQuery = subset.GetType().IsDbQuery();

            // Perform Searching
            var searchLambda = new StringBuilder();
            var searchParamArgs = new List<object?>();

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchValue = isDbQuery ? request.SearchText : request.SearchText.ToLowerInvariant();

                if (!string.IsNullOrWhiteSpace(searchValue))
                    GetSearchFilter(request, isDbQuery, searchValue!, searchLambda, searchParamArgs);
            }

            // Perform Filtering
            foreach (var column in request.Columns
                .Where(column => !string.IsNullOrWhiteSpace(column.FilterText) || column.FilterArgument != null))
            {
                FilterColumn(column, searchLambda, searchParamArgs, isDbQuery);
            }

            if (searchLambda.Length <= 0) return subset;

            subset = subset.Where(
                searchLambda.Remove(searchLambda.Length - 3, 3).ToString(),
                searchParamArgs.ToArray());

            response.FilteredRecordCount = subset.Count();

            return subset;
        }

        private static void FilterColumn(
            GridColumn column,
            StringBuilder searchLambda,
            ICollection<object?> searchParamArgs,
            bool isDbQuery)
        {
            switch (column.FilterOperator)
            {
                case CompareOperators.Equals:
                case CompareOperators.NotEquals:

                    if (string.IsNullOrWhiteSpace(column.FilterText)) return;

                    if (column.DataType == DataType.Date || column.DataType == DataType.DateTime || column.DataType == DataType.DateTimeUtc)
                    {
                        searchLambda.AppendFormat(
                            column.FilterOperator == CompareOperators.Equals
                                ? "({0} >= @{1} && {0} <= @{2}) &&"
                                : "({0} < @{1} || {0} > @{2}) &&",
                            column.Name,
                            searchParamArgs.Count,
                            searchParamArgs.Count + 1);
                    }
                    else
                    {
                        searchLambda.AppendFormat("{0} {2} @{1} &&",
                            column.Name,
                            searchParamArgs.Count,
                            GetSqlOperator(column.FilterOperator));
                    }

                    switch (column.DataType)
                    {
                        case DataType.Numeric:
                            searchParamArgs.Add(decimal.Parse(column.FilterText));
                            break;
                        case DataType.DateTime:
                        case DataType.DateTimeUtc:
                        case DataType.Date:
                            searchParamArgs.Add(
                                DateTime.Parse(column.FilterText).Date.ToString(DateTimeFormat));
                            searchParamArgs.Add(DateTime.Parse(column.FilterText)
                                .Date.AddDays(1)
                                .AddMinutes(-1).ToString(DateTimeFormat));

                            break;
                        case DataType.Boolean:
                            searchParamArgs.Add(bool.Parse(column.FilterText));
                            break;
                        default:
                            searchParamArgs.Add(column.FilterText);
                            break;
                    }

                    break;
                case CompareOperators.Contains:
                    searchLambda.AppendFormat(
                        isDbQuery
                            ? "{0}.Contains(@{1}) &&"
                            : "({0} != null && {0}.ToLowerInvariant().Contains(@{1})) &&", column.Name,
                        searchParamArgs.Count);

                    searchParamArgs.Add(column.FilterText?.ToLowerInvariant());
                    break;
                case CompareOperators.StartsWith:
                    searchLambda.AppendFormat(
                        isDbQuery
                            ? "{0}.StartsWith(@{1}) &&"
                            : "({0} != null && {0}.ToLowerInvariant().StartsWith(@{1})) &&", column.Name,
                        searchParamArgs.Count);

                    searchParamArgs.Add(column.FilterText?.ToLowerInvariant());
                    break;
                case CompareOperators.EndsWith:
                    searchLambda.AppendFormat(
                        isDbQuery
                            ? "{0}.EndsWith(@{1}) &&"
                            : "({0} != null && {0}.ToLowerInvariant().EndsWith(@{1})) &&", column.Name,
                        searchParamArgs.Count);

                    searchParamArgs.Add(column.FilterText?.ToLowerInvariant());
                    break;
                case CompareOperators.NotContains:
                    searchLambda.AppendFormat(
                        isDbQuery
                            ? "{0}.Contains(@{1}) == false &&"
                            : "({0} != null && {0}.ToLowerInvariant().Contains(@{1}) == false) &&", column.Name,
                        searchParamArgs.Count);

                    searchParamArgs.Add(column.FilterText?.ToLowerInvariant());
                    break;
                case CompareOperators.NotStartsWith:
                    searchLambda.AppendFormat(
                        isDbQuery
                            ? "{0}.StartsWith(@{1}) == false &&"
                            : "({0} != null && {0}.ToLowerInvariant().StartsWith(@{1}) == false) &&", column.Name,
                        searchParamArgs.Count);

                    searchParamArgs.Add(column.FilterText?.ToLowerInvariant());
                    break;
                case CompareOperators.NotEndsWith:
                    searchLambda.AppendFormat(
                        isDbQuery
                            ? "{0}.EndsWith(@{1}) == false &&"
                            : "({0} != null && {0}.ToLowerInvariant().EndsWith(@{1}) == false) &&", column.Name,
                        searchParamArgs.Count);

                    searchParamArgs.Add(column.FilterText?.ToLowerInvariant());
                    break;
                case CompareOperators.Gte:
                case CompareOperators.Gt:
                case CompareOperators.Lte:
                case CompareOperators.Lt:
                    searchLambda.AppendFormat("{0} {2} @{1} &&",
                        column.Name,
                        searchParamArgs.Count,
                        GetSqlOperator(column.FilterOperator));

                    if (column.DataType == DataType.Numeric)
                        searchParamArgs.Add(decimal.Parse(column.FilterText));
                    else
                        searchParamArgs.Add(DateTime.Parse(column.FilterText).ToString(DateFormat));

                    break;
                case CompareOperators.Multiple:
                    if (column.FilterArgument == null || column.FilterArgument.Length == 0) return;

                    var filterString = new StringBuilder("(");
                    foreach (var filter in column.FilterArgument)
                    {
                        filterString.AppendFormat(" {0} == @{1} ||", column.Name, searchParamArgs.Count);
                        searchParamArgs.Add(filter);
                    }

                    if (filterString.Length == 1) return;

                    searchLambda.AppendFormat(filterString.Remove(filterString.Length - 3, 3) + ") &&");

                    break;
                case CompareOperators.Between:
                    if (column.FilterArgument == null || column.FilterArgument.Length == 0) return;

                    searchLambda.AppendFormat("(({0} >= @{1}) &&  ({0} <= @{2})) &&",
                        column.Name,
                        searchParamArgs.Count,
                        searchParamArgs.Count + 1);

                    if (column.DataType == DataType.Numeric)
                    {
                        searchParamArgs.Add(decimal.Parse(column.FilterText));
                        searchParamArgs.Add(decimal.Parse(column.FilterArgument[0]));
                    }
                    else
                    {
                        searchParamArgs.Add(DateTime.Parse(column.FilterText).ToString(DateFormat));
                        searchParamArgs.Add(DateTime.Parse(column.FilterArgument[0]).ToString(DateFormat));
                    }

                    break;
            }
        }

        private static void GetSearchFilter(
            GridDataRequest request,
            bool isDbQuery,
            string searchValue,
            StringBuilder searchLambda,
            List<object?> searchParamArgs)
        {
            var filter = new StringBuilder();
            var values = new List<object?>();

            if (request.Columns.Any(x => x.Searchable))
                filter.Append("(");

            foreach (var column in request.Columns.Where(x => x.Searchable))
            {
                filter.AppendFormat(isDbQuery
                        ? "{0}.Contains(@{1}) ||"
                        : "({0} != null && {0}.ToLowerInvariant().Contains(@{1})) ||",
                    column.Name,
                    values.Count);

                values.Add(searchValue);
            }

            if (filter.Length > 0)
            {
                searchLambda.Append(filter.Remove(filter.Length - 3, 3) + ") &&");
                searchParamArgs.AddRange(values);
            }
        }
    }
}