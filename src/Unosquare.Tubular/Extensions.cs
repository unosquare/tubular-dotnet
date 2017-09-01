﻿namespace Unosquare.Tubular
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using ObjectModel;
#if NET452
    using System.Text.RegularExpressions;
    using System.Net.Http;
#endif

    /// <summary>
    /// Extensions methods
    /// </summary>
    public static class Extensions
    {
        private const string DateTimeFormat = "yyyy-MM-dd hh:mm:ss.f";
        private const string DateFormat = "yyyy-MM-dd";

#if NET452
        private static readonly Regex TimezoneOffset = new Regex(@"timezoneOffset=(\d[^&]*)");
#endif

        private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> TypePropertyCache =
            new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// Delegates a process to format a subset response
        /// </summary>
        /// <param name="dataSource">The datasource</param>
        /// <returns>A subset</returns>
        public delegate IQueryable ProcessResponseSubset(IQueryable dataSource);

        /// <summary>
        /// Adjust a timezone data in a object
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="timezoneOffset">The timezone offset.</param>
        /// <returns>The same object with DateTime properties adjusted to the timezone specified.</returns>
        public static object AdjustTimeZone(object data, int timezoneOffset)
        {
            var dateTimeProperties = data.GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

            foreach (var prop in dateTimeProperties)
            {
                AdjustTimeZoneForProperty(data, timezoneOffset, prop);
            }

            return data;
        }

#if NET452
        /// <summary>
        /// Checks the datetime properties in an object and adjust the timezone.
        /// </summary>
        /// <param name="request">The Http Request</param>
        /// <param name="data">The output object</param>
        /// <param name="fromLocal">Set if the adjustment is from local time</param>
        /// <returns>The same object with DateTime properties adjusted to the timezone specified.</returns>
        public static object AdjustObjectTimeZone(this HttpRequestMessage request, object data, bool fromLocal = false)
        {
            var query = request.RequestUri.Query;

            if (string.IsNullOrWhiteSpace(query)) return data;

            var match = TimezoneOffset.Match(query);

            if (!match.Success) return data;
            var timeDiff = int.Parse(match.Groups[1].Value);
            if (fromLocal) timeDiff *= -1;

            return AdjustTimeZone(data, timeDiff);
        }
#endif

        /// <summary>
        /// Generates a GridDataReponse using the GridDataRequest and an IQueryable source,
        /// like a DataSet in Entity Framework.
        /// </summary>
        /// <param name="request">The Tubular's grid request</param>
        /// <param name="dataSource">The IQueryable source</param>
        /// <param name="preProcessSubset">The subset's process delegate</param>
        /// <returns>A grid response</returns>
        public static GridDataResponse CreateGridDataResponse(
            this GridDataRequest request, 
            IQueryable dataSource,
            ProcessResponseSubset preProcessSubset = null)
        {
            if (request?.Columns == null || request.Columns.Any() == false)
                throw new ArgumentNullException(nameof(request));

            var response = new GridDataResponse
            {
                Counter = request.Counter,
                TotalRecordCount = dataSource.Count(),
                FilteredRecordCount = dataSource.Count()
            };

            var properties = ExtractProperties(dataSource.ElementType);
            var columnMap = MapColumnsToProperties(request.Columns, properties);

            var subset = FilterResponse(request, dataSource, response);
            
            // Perform Sorting
            var orderingExpression = request.Columns.Where(x => x.SortOrder > 0).OrderBy(x => x.SortOrder)
                .Aggregate(string.Empty,
                    (current, column) =>
                        current +
                        (column.Name + " " + (column.SortDirection == SortDirection.Ascending ? "ASC" : "DESC") + ", "));

            // Apply the sorting expression if supplied
            subset = !string.IsNullOrWhiteSpace(orderingExpression)
                ? subset.OrderBy(orderingExpression.Substring(0, orderingExpression.Length - 2))
                : subset.OrderBy(request.Columns.First().Name + " ASC");

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
                var totalPages = response.TotalPages = filteredCount/pageSize;

                if (totalPages > 0)
                {
                    response.CurrentPage = (request.Skip/pageSize) + 1;

                    if (request.Skip > 0) subset = subset.Skip(request.Skip);
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
        private static Dictionary<string, PropertyInfo> ExtractProperties(Type t)
        {
            return TypePropertyCache.GetOrAdd(t, GetTypeProperties);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<string, PropertyInfo> GetTypeProperties(Type t)
        {
            return t.GetProperties()
                .Where(p => Common.PrimitiveTypes.Contains(p.PropertyType) && p.CanRead)
                .ToDictionary(k => k.Name, v => v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<GridColumn, PropertyInfo> MapColumnsToProperties(
            GridColumn[] columns,
            Dictionary<string, PropertyInfo> properties)
        {
            var columnMap = new Dictionary<GridColumn, PropertyInfo>(columns.Length);

            foreach (var column in columns)
            {
                if (properties.ContainsKey(column.Name))
                    columnMap[column] = properties[column.Name];
            }

            return columnMap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<List<object>> CreateGridPayload(
            IQueryable subset,
            Dictionary<GridColumn, PropertyInfo> columnMap, 
            int initialCapacity, 
            int timezoneOffset)
        {
            var payload = new List<List<object>>(initialCapacity);

            foreach (var item in subset)
            {
                var payloadItem = new List<object>(columnMap.Keys.Count);

                foreach (var column in columnMap.Select(m => new { Value = m.Value.GetValue(item), m.Key }))
                {
                    if (column.Value is DateTime)
                    {
                        if (column.Key.DataType == DataType.DateTimeUtc ||
                            TubularDefaultSettings.AdjustTimezoneOffset == false)
                            payloadItem.Add((DateTime)column.Value);
                        else
                            payloadItem.Add(((DateTime)column.Value).AddMinutes(-timezoneOffset));
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

        private static void AdjustTimeZoneForProperty(object data, int timezoneOffset, PropertyInfo prop)
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
                value = (DateTime)prop.GetValue(data);
            }

            value = value.AddMinutes(-timezoneOffset);
            prop.SetValue(data, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<string, object> AggregateSubset(GridColumn[] columns, IQueryable subset)
        {
            var aggregateColumns = columns.Where(c => c.Aggregate != AggregationFunction.None).ToArray();
            var payload = new Dictionary<string, object>(aggregateColumns.Length);

            Action<GridColumn, Func<IQueryable<double>, double>, Func<IQueryable<decimal>, decimal>, Func<IQueryable<int>, int>, Func<IQueryable<string>, string>, Func<IQueryable<DateTime>, DateTime>> aggregate =
                (column, doubleF, decimalF, intF, stringF, dateF) =>
                {
                    if (subset.ElementType.GetProperty(column.Name).PropertyType == typeof(double))
                    {
                        payload.Add(column.Name,
                            doubleF(subset.Select(column.Name).Cast<double>()));
                    }
                    else if (subset.ElementType.GetProperty(column.Name).PropertyType == typeof(decimal))
                    {
                        payload.Add(column.Name,
                            decimalF(subset.Select(column.Name).Cast<decimal>()));
                    }
                    else if (subset.ElementType.GetProperty(column.Name).PropertyType == typeof(int))
                    {
                        payload.Add(column.Name,
                            intF(subset.Select(column.Name).Cast<int>()));
                    }
                    else if (subset.ElementType.GetProperty(column.Name).PropertyType == typeof(DateTime))
                    {
                        if (dateF == null) return;

                        payload.Add(column.Name,
                            dateF(subset.Select(column.Name).Cast<DateTime>()));
                    }
                    else
                    {
                        if (stringF == null) return;

                        payload.Add(column.Name,
                            stringF(subset.Select(column.Name).Cast<string>()));
                    }
                };

            foreach (var column in aggregateColumns)
            {
                try
                {
                    switch (column.Aggregate)
                    {
                        case AggregationFunction.Sum:
                            aggregate(column, x => x.Sum(), x => x.Sum(), x => x.Sum(), null, null);

                            break;
                        case AggregationFunction.Average:
                            aggregate(column, x => x.Average(), x => x.Average(), x => x.Sum() / x.Count(), null, null);

                            break;
                        case AggregationFunction.Max:
                            aggregate(column, x => x.Max(), x => x.Max(), x => x.Max(), x => x.Max(), x => x.Max());

                            break;
                        case AggregationFunction.Min:
                            aggregate(column, x => x.Min(), x => x.Min(), x => x.Min(), x => x.Min(), x => x.Min());

                            break;

                        case AggregationFunction.Count:
                            payload.Add(column.Name, subset.Select(column.Name).Count());
                            break;
                        case AggregationFunction.DistinctCount:
                            payload.Add(column.Name,
                                subset.Select(column.Name).Distinct().Count());
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(
                                $"The AggregationFunction in column {column.Name} is not valid");
                    }
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException(
                        $"Invalid casting using column {column.Name} with aggregate {column.Aggregate}");
                }
            }

            return payload;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetSqlOperator(CompareOperators op)
        {
            switch (op)
            {
                case CompareOperators.Equals:
                    return "==";
                case CompareOperators.NotEquals:
                    return "!=";
                case CompareOperators.Gte:
                    return ">=";
                case CompareOperators.Gt:
                    return ">";
                case CompareOperators.Lte:
                    return "<=";
                case CompareOperators.Lt:
                    return "<";
                default:
                    return null;
            }
        }

        private static IQueryable FilterResponse(GridDataRequest request, IQueryable subset, GridDataResponse response)
        {
            var isDbQuery = subset.GetType().GetTypeInfo().IsGenericType &&
                            subset.GetType().GetInterfaces().Any(y => y == typeof(IListSource));

            // Perform Searching
            var searchLambda = new StringBuilder();
            var searchParamArgs = new List<object>();
            var searchValue = isDbQuery ? request.Search.Text : request.Search.Text.ToLowerInvariant();

            switch (request.Search.Operator)
            {
                case CompareOperators.Auto:
                    var filter = string.Empty;
                    var values = new List<object>();

                    if (request.Columns.Any(x => x.Searchable))
                        filter = "(";

                    foreach (var column in request.Columns.Where(x => x.Searchable))
                    {
                        filter += string.Format(isDbQuery
                            ? "{0}.Contains(@{1}) ||"
                            : "({0} != null && {0}.ToLowerInvariant().Contains(@{1})) ||", 
                            column.Name, 
                            values.Count);

                        values.Add(searchValue);
                    }

                    if (string.IsNullOrEmpty(filter) == false)
                    {
                        searchLambda.Append(filter.Remove(filter.Length - 3, 3) + ") &&");
                        searchParamArgs.AddRange(values);
                    }

                    break;
            }

            // Perform Filtering
            foreach (
                var column in
                request.Columns.Where(x => x.Filter != null)
                    .Where(
                        column => !string.IsNullOrWhiteSpace(column.Filter.Text) || column.Filter.Argument != null))
            {
                column.Filter.HasFilter = true;

                switch (column.Filter.Operator)
                {
                    case CompareOperators.Equals:
                    case CompareOperators.NotEquals:
                        if (string.IsNullOrWhiteSpace(column.Filter.Text)) continue;

                        if (column.DataType == DataType.Date)
                        {
                            searchLambda.AppendFormat(
                                column.Filter.Operator == CompareOperators.Equals
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
                                GetSqlOperator(column.Filter.Operator));
                        }

                        switch (column.DataType)
                        {
                            case DataType.Numeric:
                                searchParamArgs.Add(decimal.Parse(column.Filter.Text));
                                break;
                            case DataType.DateTime:
                            case DataType.DateTimeUtc:
                                searchParamArgs.Add(DateTime.Parse(column.Filter.Text).ToString(DateFormat));
                                break;
                            case DataType.Date:
                                if (TubularDefaultSettings.AdjustTimezoneOffset)
                                {
                                    searchParamArgs.Add(DateTime.Parse(column.Filter.Text).Date.ToUniversalTime().ToString(DateTimeFormat));
                                    searchParamArgs.Add(
                                        DateTime.Parse(column.Filter.Text)
                                            .Date.ToUniversalTime()
                                            .AddDays(1)
                                            .AddMinutes(-1).ToString(DateTimeFormat));
                                }
                                else
                                {
                                    searchParamArgs.Add(DateTime.Parse(column.Filter.Text).Date.ToString(DateTimeFormat));
                                    searchParamArgs.Add(DateTime.Parse(column.Filter.Text)
                                        .Date.AddDays(1)
                                        .AddMinutes(-1).ToString(DateTimeFormat));
                                }

                                break;
                            case DataType.Boolean:
                                searchParamArgs.Add(bool.Parse(column.Filter.Text));
                                break;
                            default:
                                searchParamArgs.Add(column.Filter.Text);
                                break;
                        }

                        break;
                    case CompareOperators.Contains:
                        searchLambda.AppendFormat(
                            isDbQuery
                                ? "{0}.Contains(@{1}) &&"
                                : "({0} != null && {0}.ToLowerInvariant().Contains(@{1})) &&", column.Name,
                            searchParamArgs.Count);

                        searchParamArgs.Add(column.Filter.Text.ToLowerInvariant());
                        break;
                    case CompareOperators.StartsWith:
                        searchLambda.AppendFormat(
                            isDbQuery
                                ? "{0}.StartsWith(@{1}) &&"
                                : "({0} != null && {0}.ToLowerInvariant().StartsWith(@{1})) &&", column.Name,
                            searchParamArgs.Count);

                        searchParamArgs.Add(column.Filter.Text.ToLowerInvariant());
                        break;
                    case CompareOperators.EndsWith:
                        searchLambda.AppendFormat(
                            isDbQuery
                                ? "{0}.EndsWith(@{1}) &&"
                                : "({0} != null && {0}.ToLowerInvariant().EndsWith(@{1})) &&", column.Name,
                            searchParamArgs.Count);

                        searchParamArgs.Add(column.Filter.Text.ToLowerInvariant());
                        break;
                    case CompareOperators.NotContains:
                        searchLambda.AppendFormat(
                            isDbQuery
                                ? "{0}.Contains(@{1}) == false &&"
                                : "({0} != null && {0}.ToLowerInvariant().Contains(@{1}) == false) &&", column.Name,
                            searchParamArgs.Count);

                        searchParamArgs.Add(column.Filter.Text.ToLowerInvariant());
                        break;
                    case CompareOperators.NotStartsWith:
                        searchLambda.AppendFormat(
                            isDbQuery
                                ? "{0}.StartsWith(@{1}) == false &&"
                                : "({0} != null && {0}.ToLowerInvariant().StartsWith(@{1}) == false) &&", column.Name,
                            searchParamArgs.Count);

                        searchParamArgs.Add(column.Filter.Text.ToLowerInvariant());
                        break;
                    case CompareOperators.NotEndsWith:
                        searchLambda.AppendFormat(
                            isDbQuery
                                ? "{0}.EndsWith(@{1}) == false &&"
                                : "({0} != null && {0}.ToLowerInvariant().EndsWith(@{1}) == false) &&", column.Name,
                            searchParamArgs.Count);

                        searchParamArgs.Add(column.Filter.Text.ToLowerInvariant());
                        break;
                    case CompareOperators.Gte:
                    case CompareOperators.Gt:
                    case CompareOperators.Lte:
                    case CompareOperators.Lt:
                        searchLambda.AppendFormat("{0} {2} @{1} &&", 
                            column.Name, 
                            searchParamArgs.Count,
                            GetSqlOperator(column.Filter.Operator));

                        if (column.DataType == DataType.Numeric)
                            searchParamArgs.Add(decimal.Parse(column.Filter.Text));
                        else
                            searchParamArgs.Add(DateTime.Parse(column.Filter.Text).ToString(DateFormat));

                        break;
                    case CompareOperators.Multiple:
                        if (column.Filter.Argument == null || column.Filter.Argument.Length == 0) continue;

                        var filterString = "(";
                        foreach (var filter in column.Filter.Argument)
                        {
                            filterString += $" {column.Name} == @{searchParamArgs.Count} ||";
                            searchParamArgs.Add(filter);
                        }

                        if (filterString.Length == 1) continue;

                        searchLambda.AppendFormat(filterString.Remove(filterString.Length - 3, 3) + ") &&");

                        break;
                    case CompareOperators.Between:
                        if (column.Filter.Argument == null || column.Filter.Argument.Length == 0) continue;

                        searchLambda.AppendFormat("(({0} >= @{1}) &&  ({0} <= @{2})) &&", 
                            column.Name,
                            searchParamArgs.Count, 
                            searchParamArgs.Count + 1);

                        if (column.DataType == DataType.Numeric)
                        {
                            searchParamArgs.Add(decimal.Parse(column.Filter.Text));
                            searchParamArgs.Add(decimal.Parse(column.Filter.Argument[0]));
                        }
                        else
                        {
                            searchParamArgs.Add(DateTime.Parse(column.Filter.Text).ToString(DateFormat));
                            searchParamArgs.Add(DateTime.Parse(column.Filter.Argument[0]).ToString(DateFormat));
                        }

                        break;
                }
            }

            if (searchLambda.Length <= 0) return subset;

            subset = subset.Where(searchLambda.Remove(searchLambda.Length - 3, 3).ToString(),
                searchParamArgs.ToArray());

            if (subset != null)
                response.FilteredRecordCount = subset.Count();

            return subset;
        }
    }
}