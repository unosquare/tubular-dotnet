namespace Unosquare.Tubular;

/// <summary>
/// Supported data types.
/// </summary>
public enum DataType
{
    /// <summary>
    /// String type
    /// </summary>
#pragma warning disable CA1720 // Identifier contains type name
    String,
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Numeric type (int or float)
    /// </summary>
    Numeric,

    /// <summary>
    /// DataTime type
    /// </summary>
    DateTime,

    /// <summary>
    /// Only Date type
    /// </summary>
    Date,

    /// <summary>
    /// Boolean type
    /// </summary>
    Boolean,

    /// <summary>
    /// DataTime UTC type
    /// </summary>
    DateTimeUtc,
}