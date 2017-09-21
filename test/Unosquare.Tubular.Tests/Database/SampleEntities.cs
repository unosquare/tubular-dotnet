﻿namespace Unosquare.Tubular.Tests.Database
{
    using System;
    using System.Collections.Generic;

    public class SampleEntities
    { 
        public static IEnumerable<Thing> GenerateData(int count = 100)
        {
            var colors = new[] { "red", "yellow", "blue" };
            var category = new[] { "A", "B" };
            var data = new List<Thing>(count);
            var rand = new Random();
            for (var i = 0; i < count; i++)
            {
                data.Add(new Thing()
                {
                    Date = DateTime.UtcNow.AddDays(-i),
                    NullableDate = (i % 2) == 0 ? (DateTime?)null : DateTime.UtcNow.AddDays(-i),
                    Id = i,
                    Name = "Name - " + i,
                    Bool = (i % 2) == 0,
                    DecimalNumber = (i % 3 == 0) ? 10.100m : 20.2002m,
                    Number = rand.NextDouble() * 20,
                    Category = category[rand.Next(0, category.Length)],
                    Color = colors[rand.Next(0, colors.Length)],
                });
            }
            return data;
        }
    }
}