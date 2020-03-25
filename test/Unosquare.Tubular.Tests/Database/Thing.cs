namespace Unosquare.Tubular.Tests.Database
{
    using System;

    public class Thing
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public double Number { get; set; }

        public decimal DecimalNumber { get; set; }

        public string? Category { get; set; }

        public string? Color { get; set; }

        public DateTime Date { get; set; }

        public DateTime? NullableDate { get; set; }

        public bool IsShipped { get; set; }

        public static GridColumn[] GetColumnsWithAggregate() =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Number", Aggregate = AggregationFunction.Sum},
                new GridColumn {Name = "DecimalNumber", Aggregate = AggregationFunction.Sum},
                new GridColumn {Name = "Name", Aggregate = AggregationFunction.Max},
                new GridColumn {Name = "Date", Aggregate = AggregationFunction.Min}
            };

        public static GridColumn[] GetColumnsWithMultipleCounts() =>
            new[]
            {
                new GridColumn {Name = "Id", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "Number", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "DecimalNumber", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "Name", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "Date", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "IsShipped", Aggregate = AggregationFunction.DistinctCount}
            };

        public static GridColumn[] GetColumns() =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name", Searchable = true},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"}
            };

        public static GridColumn[] GetColumnsWithSort() =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date", Sortable = true, SortDirection = SortDirection.Ascending, SortOrder = 1},
                new GridColumn {Name = "IsShipped"}
            };

        public static GridColumn[] GetColumnsWithIdFilter(string filter, CompareOperators oper) =>
            new[]
            {
                new GridColumn
                {
                    Name = "Id",
                    FilterText = filter,
                    FilterOperator = oper,
                    DataType = DataType.Numeric
                },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"}
            };

        public static GridColumn[] GetColumnsWithColorFilter(string filter, CompareOperators oper) =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"},
                new GridColumn
                {
                    Name = "Color",
                    FilterText = filter,
                    FilterOperator = oper,
                    DataType = DataType.String
                }
            };

        public static GridColumn[] GetColumnsWithBetweenFilter(string filter, string[] arguments) =>
            new[]
            {
                new GridColumn
                {
                    Name = "Id",
                    FilterText = filter, FilterArgument = arguments, FilterOperator = CompareOperators.Between,
                    DataType = DataType.Numeric
                },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"}
            };

        public static GridColumn[] GetColumnsWithMultipleFilter(string[] arguments, CompareOperators oper) =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"},
                new GridColumn
                {
                    Name = "Color",
                    FilterArgument = arguments,
                    FilterOperator = oper,
                    DataType = DataType.String
                }
            };

        public static GridColumn[] GetColumnsWithDateFilter(string filter, CompareOperators oper,
            DataType dataType) =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "IsShipped"},
                new GridColumn
                {
                    Name = "Date",
                    FilterText = filter,
                    FilterOperator = oper,
                    DataType = dataType
                }
            };

        public static GridColumn[] GetColumnsWithNumberFilter(string filter, CompareOperators oper) =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"},
                new GridColumn
                {
                    Name = "DecimalNumber",
                    FilterText = filter, FilterOperator = oper,
                    DataType = DataType.Numeric
                }
            };

        public static GridColumn[] GetColumnsWithBooleanFilter(string filter, CompareOperators oper) =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn
                {
                    Name = "IsShipped",
                    Searchable = false,
                    FilterText = filter,
                    FilterOperator = oper,
                    DataType = DataType.Boolean,
                }
            };

        public static GridColumn[] GetColumnsWithAggregateDouble(AggregationFunction aggregation) =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Number", Aggregate = aggregation},
                new GridColumn {Name = "DecimalNumber"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"}
            };

        public static GridColumn[] GetColumnsWithAggregateInt(AggregationFunction aggregation) =>
            new[]
            {
                new GridColumn {Name = "Id", Aggregate = aggregation},
                new GridColumn {Name = "Number"},
                new GridColumn {Name = "DecimalNumber"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"}
            };

        public static GridColumn[] GetColumnsWithAggregateDoubleAndInvalidDate(AggregationFunction aggregation) =>
            new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Number"},
                new GridColumn {Name = "DecimalNumber", Aggregate = aggregation, DataType = DataType.Numeric},
                new GridColumn {Name = "Name"},
                new GridColumn
                {
                    Name = "Date",
                    FilterArgument = new[] {DateTime.Now.AddDays(1).ToShortDateString()},
                    FilterText = DateTime.Now.AddDays(1).ToShortDateString(),
                    FilterOperator = CompareOperators.Equals
                }
            };
    }
}