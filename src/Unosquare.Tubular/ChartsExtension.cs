namespace Unosquare.Tubular
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Linq.Expressions;
    using ObjectModel;

    /// <summary>
    /// Extension methods to create chart responses
    /// </summary>
    public static class ChartsExtension
    {
        /// <summary>
        /// Creates a single serie chart from a IQueryable
        /// </summary>
        /// <typeparam name="T">The type of object to query</typeparam>
        /// <typeparam name="TR">The type of the result.</typeparam>
        /// <param name="datasource">The datasource.</param>
        /// <param name="label">The label.</param>
        /// <param name="value">The value.</param>
        /// <param name="serieName">Name of the serie.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <returns>A response with single serie for a Tubular Chart</returns>
        public static SingleSerieChartResponse<TR> ProvideSingleSerieChartResponse<T, TR>(
            this IQueryable<T> datasource,
            Expression<Func<T, string>> label, 
            Expression<Func<T, TR>> value, 
            string serieName = null,
            AggregationFunction aggregation = AggregationFunction.Sum)
        {
            var labelExpression = label.Body as MemberExpression ??
                                  (MemberExpression) ((UnaryExpression) label.Body).Operand;
            var valueExpression = value.Body as MemberExpression ??
                                  (MemberExpression) ((UnaryExpression) value.Body).Operand;

            var dataSelector = GenerateDataSelector(aggregation, valueExpression);

            var data =
                datasource.GroupBy(labelExpression.Member.Name, "it")
                    .Select($"new (it.Key as Label, {dataSelector} as Data)");

            return new SingleSerieChartResponse<TR>
            {
                Data = data.Select("Data").Cast<TR>().ToArray(),
                SerieName = serieName,
                Labels = data.Select("Label").Cast<string>().ToArray(),
            };
        }

        /// <summary>
        /// Creates a multiple series chart from a IQueryable
        /// </summary>
        /// <typeparam name="T">The type of object to query</typeparam>
        /// <typeparam name="TR">The type of the result.</typeparam>
        /// <param name="datasource">The datasource.</param>
        /// <param name="label">The label.</param>
        /// <param name="serie">The serie.</param>
        /// <param name="value">The value.</param>
        /// <param name="aggregation">The aggregation.</param>
        /// <returns>A response with multiple series for a Tubular Chart</returns>
        public static MultipleSerieChartResponse<TR> ProvideMultipleSerieChartResponse<T, TR>(
            this IQueryable<T> datasource, 
            Expression<Func<T, string>> label, 
            Expression<Func<T, string>> serie,
            Expression<Func<T, TR>> value, 
            AggregationFunction aggregation = AggregationFunction.Sum)
        {
            // Series are filters
            var labelExpression = label.Body as MemberExpression ??
                                  (MemberExpression) ((UnaryExpression) label.Body).Operand;

            var serieExpression = serie.Body as MemberExpression ??
                                  (MemberExpression) ((UnaryExpression) serie.Body).Operand;

            var valueExpression = value.Body as MemberExpression ??
                                  (MemberExpression) ((UnaryExpression) value.Body).Operand;

            var dataSelector = GenerateDataSelector(aggregation, valueExpression);

            var series =
                datasource.OrderBy(serieExpression.Member.Name)
                    .Select(serieExpression.Member.Name)
                    .Cast<string>()
                    .Distinct()
                    .ToList();
            var labels =
                datasource.OrderBy(labelExpression.Member.Name)
                    .Select(labelExpression.Member.Name)
                    .Cast<string>()
                    .Distinct()
                    .ToList();

            var data = new List<List<decimal>>();

            foreach (var serieValue in series)
            {
                var subset = datasource.Where($"{serieExpression.Member.Name} == @0", serieValue)
                    .GroupBy(labelExpression.Member.Name, "it")
                    .Select($"new (it.Key as Label, {dataSelector} as Data)");

                var tempData = subset.Select("Data").Cast<decimal>().ToList();
                var subsetLabels = subset.Select("Label").Cast<string>().ToList();

                var unkwnownLabels = labels.Except(subsetLabels);

                foreach (var index in unkwnownLabels.Select(item => labels.IndexOf(item)))
                {
                    tempData.Insert(index, 0);
                }

                data.Add(tempData);
            }

            return new MultipleSerieChartResponse<TR>
            {
                Data = data.Select(x => x.Cast<TR>().ToArray()).ToArray(),
                Labels = labels.ToArray(),
                Series = series.ToArray()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GenerateDataSelector(AggregationFunction aggregation, MemberExpression valueExpression)
        {
            if (aggregation == AggregationFunction.Count || aggregation == AggregationFunction.DistinctCount)
            {
                // TODO: DISTINCT is tricky and Ricky is a friend of mine
                return "COUNT()";
            }

            return $"{aggregation.ToString().ToUpper()}(it.{valueExpression.Member.Name})";
        }
    }
}