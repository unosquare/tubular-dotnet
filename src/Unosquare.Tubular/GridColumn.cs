﻿namespace Unosquare.Tubular;

/// <summary>
/// Represents a Tubular Grid column.
/// 
/// This object is only used to be serialized/deserialized between
/// the backend and Tubular Grid.
/// </summary>
public class GridColumn
{
    /// <summary>
    /// Column Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Set if column is sortable.
    /// </summary>
    public bool Sortable { get; set; }

    /// <summary>
    /// Set the sort order, zero or less are ignored.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Set the sort direction.
    /// </summary>
    public SortDirection SortDirection { get; set; }

    /// <summary>
    /// Set if the column is searchable in free-text search.
    /// </summary>
    public bool Searchable { get; set; }

    /// <summary>
    /// Column data type.
    /// </summary>
    public DataType DataType { get; set; }

    /// <summary>
    /// The Aggregation Function.
    /// </summary>
    public AggregationFunction Aggregate { get; set; }

    /// <summary>
    /// Filter search text.
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// Filter search params.
    /// </summary>
    public string?[]? FilterArgument { get; set; }

    /// <summary>
    /// Filter operator.
    /// </summary>
    public CompareOperators FilterOperator { get; set; }
}