namespace Unosquare.Tubular;

/// <summary>
/// Common property types supported by this library.
/// </summary>
internal static class CommonTypes
{
    /// <summary>
    /// Defines primitive types.
    /// </summary>
    public static readonly Type[] PrimitiveTypes =
    [
        typeof(string),
        typeof(DateTime),
        typeof(DateOnly),
        typeof(DateTimeOffset),
        typeof(TimeOnly),
        typeof(bool),
        typeof(byte),
        typeof(sbyte),
        typeof(char),
        typeof(decimal),
        typeof(double),
        typeof(float),
        typeof(int),
        typeof(nint),
        typeof(nuint),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(short),
        typeof(ushort),
        typeof(DateTime?),
        typeof(DateOnly?),
        typeof(DateTimeOffset?),
        typeof(TimeOnly?),
        typeof(bool?),
        typeof(byte?),
        typeof(sbyte?),
        typeof(char?),
        typeof(decimal?),
        typeof(double?),
        typeof(float?),
        typeof(int?),
        typeof(nint?),
        typeof(nuint?),
        typeof(uint?),
        typeof(long?),
        typeof(ulong?),
        typeof(short?),
        typeof(ushort?),
        typeof(Guid),
        typeof(Guid?),
    ];
}