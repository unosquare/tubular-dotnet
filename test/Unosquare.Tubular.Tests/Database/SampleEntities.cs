namespace Unosquare.Tubular.Tests.Database;

internal static class SampleEntities
{
    private static readonly string[] Colors = ["red", "yellow", "blue"];
    private static readonly string[] Category = ["A", "B"];

    public static IEnumerable<Thing> GenerateData(int count = 100)
    {
        var rand = new Random();
        var nowBase = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);

        for (var i = 0; i < count; i++)
        {
            yield return new()
            {
                Date = nowBase.AddDays(-i),
                NullableDate = (i % 2) == 0 ? null : DateTime.UtcNow.AddDays(-i),
                Id = i,
                Name = "Name - " + i,
                IsShipped = (i % 2) == 0,
                DecimalNumber = i * 1.101m,
                Number = i * 2,
                Category = Category[rand.Next(0, Category.Length)],
                Color = Colors[rand.Next(0, Colors.Length)],
            };
        }
    }
}