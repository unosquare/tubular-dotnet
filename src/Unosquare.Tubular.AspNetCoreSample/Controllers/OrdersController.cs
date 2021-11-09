using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unosquare.Tubular.AspNetCoreSample.ApiModel;
using Unosquare.Tubular.AspNetCoreSample.Models;

namespace Unosquare.Tubular.AspNetCoreSample.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly SampleDbContext _context;

        public OrdersController(SampleDbContext context)
        {
            _context = context;
        }

        [HttpGet, Route("{id}")]
        public object Get(string id)
        {
            var orderId = int.Parse(id);
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null) return NotFound();

            return order;
        }

        [HttpPost, Route("paged")]
        public object GridData([FromBody] GridDataRequest request) => request.CreateGridDataResponse(_context.Orders);

        private static IQueryable FormatOutput(IQueryable q)
        {
            var list = new List<OrderDto>();

            foreach (var i in q)
            {
                var item = i as OrderDto;

                if (item?.CustomerName == "Super La Playa")
                {
                    item.ShippedDate = "Blocked";
                    item.ShipperCity = "Blocked";
                    item.Amount = "Blocked";
                }

                list.Add(item);
            }

            return list.AsQueryable();
        }

        [HttpPost, Route("pagedwithformat")]
        public Task<GridDataResponse> GridDataWithFormat([FromBody] GridDataRequest request) =>
            Task.Run(() =>
                request.CreateGridDataResponse(
                    _context.Orders.Select(x => new OrderDto
                    {
                        Amount = x.Amount.ToString(),
                        CustomerName = x.CustomerName,
                        IsShipped = x.IsShipped.ToString(),
                        OrderId = x.OrderId,
                        ShippedDate = x.ShippedDate.ToString(),
                        ShipperCity = x.ShipperCity
                    }), FormatOutput));

        [HttpPut, Route("save")]
        public async Task<object> UpdateOrder([FromBody] Order request)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == request.OrderId);

            if (order == null)
                return null;

            order.Amount = request.Amount;
            order.CustomerName = request.CustomerName;
            order.IsShipped = request.IsShipped;
            order.ShippedDate = request.ShippedDate.Date;
            order.ShipperCity = request.ShipperCity;
            order.OrderType = request.OrderType;
            order.Comments = request.Comments;
            order.CarrierName = request.CarrierName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost, Route("save")]
        public async Task<object> CreateOrder([FromBody] Order order)
        {
            var fixedOrder = order;  // TODO: (Order)Request.AdjustObjectTimeZone(order);
            _context.Orders.Add(fixedOrder);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete, Route("save/{id}")]
        public async Task<object> DeleteOrder(string id)
        {
            var orderId = int.Parse(id);
            var orderDb = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (orderDb == null)
                return NotFound();

            _context.Orders.Remove(orderDb);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet, Route("cities")]
        public IEnumerable<object> GetCities() =>
            _context.Orders
                .Select(x => new
                {
                    Key = x.ShipperCity,
                    Label = x.ShipperCity.ToUpper()
                })
                .Distinct()
                .OrderBy(x => x.Label)
                .ToList();
    }
}