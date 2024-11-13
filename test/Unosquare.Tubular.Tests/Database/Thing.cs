namespace Unosquare.Tubular.Tests.Database;

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
    [
        new() {Name = "Id"},
        new() {Name = "Number", Aggregate = AggregationFunction.Sum},
        new() {Name = "DecimalNumber", Aggregate = AggregationFunction.Sum},
        new() {Name = "Name", Aggregate = AggregationFunction.Max},
        new() {Name = "Date", Aggregate = AggregationFunction.Min},
    ];

    public static GridColumn[] GetColumnsWithMultipleCounts() =>
    [
        new() {Name = "Id", Aggregate = AggregationFunction.DistinctCount},
        new() {Name = "Number", Aggregate = AggregationFunction.DistinctCount},
        new() {Name = "DecimalNumber", Aggregate = AggregationFunction.DistinctCount},
        new() {Name = "Name", Aggregate = AggregationFunction.DistinctCount},
        new() {Name = "Date", Aggregate = AggregationFunction.DistinctCount},
        new() {Name = "IsShipped", Aggregate = AggregationFunction.DistinctCount},
    ];

    public static GridColumn[] GetColumns() =>
    [
        new() {Name = "Id"},
        new() {Name = "Name", Searchable = true},
        new() {Name = "Date"},
        new() {Name = "IsShipped"},
    ];

    public static GridColumn[] GetColumnsWithSort() =>
    [
        new() {Name = "Id"},
        new() {Name = "Name"},
        new() {Name = "Date", Sortable = true, SortDirection = SortDirection.Ascending, SortOrder = 1},
        new() {Name = "IsShipped"},
    ];

    public static GridColumn[] GetColumnsWithIdFilter(string filter, CompareOperators oper) =>
    [
        new()
        {
            Name = "Id", FilterText = filter, FilterOperator = oper, DataType = DataType.Numeric,
        },
        new() {Name = "Name"},
        new() {Name = "Date"},
        new() {Name = "IsShipped"},
    ];

    public static GridColumn[] GetColumnsWithColorFilter(string filter, CompareOperators oper) =>
    [
        new() {Name = "Id"},
        new() {Name = "Name"},
        new() {Name = "Date"},
        new() {Name = "IsShipped"},
        new()
        {
            Name = "Color", FilterText = filter, FilterOperator = oper, DataType = DataType.String,
        },
    ];

    public static GridColumn[] GetColumnsWithBetweenFilter(string filter, string[] arguments) =>
    [
        new()
        {
            Name = "Id",
            FilterText = filter,
            FilterArgument = arguments,
            FilterOperator = CompareOperators.Between,
            DataType = DataType.Numeric,
        },
        new() {Name = "Name"},
        new() {Name = "Date"},
        new() {Name = "IsShipped"},
    ];

    public static GridColumn[] GetColumnsWithMultipleFilter(string[] arguments, CompareOperators oper) =>
    [
        new() {Name = "Id"},
        new() {Name = "Name"},
        new() {Name = "Date"},
        new() {Name = "IsShipped"},
        new()
        {
            Name = "Color", FilterArgument = arguments, FilterOperator = oper, DataType = DataType.String,
        },
    ];

    public static GridColumn[] GetColumnsWithDateFilter(string filter, CompareOperators oper,
        DataType dataType) =>
    [
        new() {Name = "Id"},
        new() {Name = "Name"},
        new() {Name = "IsShipped"},
        new()
        {
            Name = "Date", FilterText = filter, FilterOperator = oper, DataType = dataType,
        },
    ];

    public static GridColumn[] GetColumnsWithNumberFilter(string filter, CompareOperators oper) =>
    [
        new() {Name = "Id"},
        new() {Name = "Name"},
        new() {Name = "Date"},
        new() {Name = "IsShipped"},
        new()
        {
            Name = "DecimalNumber", FilterText = filter, FilterOperator = oper, DataType = DataType.Numeric,
        },
    ];

    public static GridColumn[] GetColumnsWithBooleanFilter(string filter, CompareOperators oper) =>
    [
        new() {Name = "Id"},
        new() {Name = "Name"},
        new() {Name = "Date"},
        new()
        {
            Name = "IsShipped",
            Searchable = false,
            FilterText = filter,
            FilterOperator = oper,
            DataType = DataType.Boolean,
        },
    ];

    public static GridColumn[] GetColumnsWithAggregateDouble(AggregationFunction aggregation) =>
    [
        new() {Name = "Id"},
        new() {Name = "Number", Aggregate = aggregation},
        new() {Name = "DecimalNumber"},
        new() {Name = "Name"},
        new() {Name = "Date"},
    ];

    public static GridColumn[] GetColumnsWithAggregateInt(AggregationFunction aggregation) =>
    [
        new() {Name = "Id", Aggregate = aggregation},
        new() {Name = "Number"},
        new() {Name = "DecimalNumber"},
        new() {Name = "Name"},
        new() {Name = "Date"},
    ];
}