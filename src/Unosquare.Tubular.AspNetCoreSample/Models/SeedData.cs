using Microsoft.EntityFrameworkCore;

namespace Unosquare.Tubular.AspNetCoreSample.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var dbContext = new SampleDbContext(serviceProvider.GetRequiredService<DbContextOptions<SampleDbContext>>());
        if (dbContext.Orders.Any()) return;
        var shipperCities = new[]
        {
            "Guadalajara, JAL, Mexico", "Los Angeles, CA, USA", "Portland, OR, USA", "Leon, GTO, Mexico",
            "Boston, MA, USA"
        };

        var companies = new[]
        {
            "Unosquare LLC", "Advanced Technology Systems", "Super La Playa", "Vesta", "Microsoft", "Oxxo",
            "Simian"
        };

        dbContext.Products.AddRange(new Product { Name = "CocaCola" }, new Product { Name = "Pepsi" },
            new Product { Name = "Starbucks" }, new Product { Name = "Donut" });

        dbContext.SaveChanges();

        var rand = new Random();
        var products = dbContext.Products.ToArray();

        for (var i = 0; i < 500; i++)
        {
            var order = new Order
            {
                //CreatedUserId = users[rand.Next(users.Count - 1)].Id,
                CustomerName = companies[rand.Next(companies.Length - 1)],
                IsShipped = rand.Next(10) > 5,
                ShipperCity = shipperCities[rand.Next(shipperCities.Length - 1)],
                ShippedDate = DateTime.Now.AddDays(1 - rand.Next(10)),
                OrderType = rand.Next(30),
                Address = "Address " + i,
                PostalCode = "500-" + i,
                PhoneNumber = "1-800-123-1" + i
            };

            for (var k = 0; k < rand.Next(10); k++)
            {
                order.Details.Add(new()
                {
                    Price = rand.Next(10),
                    Description = "Product ID" + rand.Next(1000),
                    Quantity = rand.Next(10),
                    ProductID = products[rand.Next(products.Length - 1)].ProductID
                });
            }

            order.Amount = order.Details.Sum(x => x.Price * x.Quantity);

            dbContext.Orders.Add(order);
        }

        dbContext.SaveChanges();
    }
}