namespace Unosquare.Tubular;

/// <summary>
/// Filtering operator definitions.
/// </summary>
public enum CompareOperators
{
    /// <summary>
    /// None operator
    /// </summary>
    None,

    /// <summary>
    /// Auto-filtering
    /// </summary>
    Auto,

    /// <summary>
    /// Equals operator
    /// </summary>
    Equals,

    /// <summary>
    /// Not Equals operator
    /// </summary>
    NotEquals,

    /// <summary>
    /// Contains filter
    /// </summary>
    Contains,

    /// <summary>
    /// StartsWith filter
    /// </summary>
    StartsWith,

    /// <summary>
    /// EndsWith filter
    /// </summary>
    EndsWith,

    /// <summary>
    /// Greater than or equal filter
    /// </summary>
    Gte,

    /// <summary>
    /// Greater than filter
    /// </summary>
    Gt,

    /// <summary>
    /// Less than or equal filter
    /// </summary>
    Lte,

    /// <summary>
    /// Less than filter
    /// </summary>
    Lt,

    /// <summary>
    /// Multiple options filter
    /// </summary>
    Multiple,

    /// <summary>
    /// Between values filter
    /// </summary>
    Between,

    /// <summary>
    /// Not Contains filter
    /// </summary>
    NotContains,

    /// <summary>
    /// Not StartsWith filter
    /// </summary>
    NotStartsWith,

    /// <summary>
    /// Not EndsWith filter
    /// </summary>
    NotEndsWith,
}