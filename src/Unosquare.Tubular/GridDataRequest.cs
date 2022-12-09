﻿namespace Unosquare.Tubular;

/// <summary>
/// Represents a Data Request from a Tubular Grid.
/// 
/// This model is how Tubular Grid sends data to any server.
/// </summary>
public class GridDataRequest
{
    /// <summary>
    /// Request's counter.
    /// </summary>
    public int Counter { get; set; }

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    /// <value>
    /// The search text.
    /// </value>
    public string? SearchText { get; set; }

    /// <summary>
    /// Set how many records skip, for pagination.
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    /// Set how many records take, for pagination.
    /// </summary>
    public int Take { get; set; }

    /// <summary>
    /// Defines the columns.
    /// </summary>
    public GridColumn?[]? Columns { get; set; }

    /// <summary>
    /// Sent the minutes difference between UTC and local time.
    /// </summary>
    public int TimezoneOffset { get; set; }
}