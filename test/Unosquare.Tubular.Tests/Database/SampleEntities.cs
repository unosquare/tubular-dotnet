namespace Unosquare.Tubular.Tests.Database
{
    using System;
    using System.Collections.Generic;

    public static class SampleEntities
    { 
        private static readonly string[] Colors = { "red", "yellow", "blue" };
        private static readonly string[] Category = { "A", "B" };

        public static IEnumerable<Thing> GenerateData(int count = 100)
        {
            var rand = new Random();

            for (var i = 0; i < count; i++)
            {
                yield return new Thing
                {
                    Date = DateTime.UtcNow.AddDays(-i),
                    NullableDate = (i % 2) == 0 ? (DateTime?)null : DateTime.UtcNow.AddDays(-i),
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
}