using Asp.Versioning;
using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ContosoOnlineOrders.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiVersion("1.2")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ShopController(IStoreDataService storeServices) : ControllerBase
    {
        public IStoreDataService StoreServices { get; } = storeServices;

        [HttpPost("/orders", Name = nameof(CreateOrder))]
        public Task<ActionResult<Order>> CreateOrder(Order order)
        {
            ActionResult<Order> result = Conflict();

            try
            {
                StoreServices.CreateOrder(order);
                result = Created($"/orders/{order.Id}", order);
            }
            catch
            {
                result = Conflict();
            }

            return Task.FromResult(result);
        }

        [HttpGet("/products", Name = nameof(GetProducts))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await Task.FromResult(Ok(StoreServices.GetProducts()));
        }

        [HttpGet("/products/page/{page}", Name = nameof(GetProductsPage))]
        [MapToApiVersion("1.1")]
        [MapToApiVersion("1.2")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsPage([FromRoute] int page = 0)
        {
            var pageSize = 5;
            var productsPage = StoreServices.GetProducts().Skip(page * pageSize).Take(pageSize);
            return await Task.FromResult(Ok(productsPage));
        }

        [HttpGet("/products/{id}", Name = nameof(GetProduct))]
        public Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = StoreServices.GetProduct(id);
            ActionResult<Product> result = NotFound();

            if (product != null)
            {
                result = Ok(product);
            }

            return Task.FromResult(result);
        }
    }
}