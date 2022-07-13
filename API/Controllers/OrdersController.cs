using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities.OrderAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly StoreContext context;

        public OrdersController(StoreContext storeContext)
        {
            context = storeContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            return await context.Orders
                    .Include(o=>o.OrderItems)
                    .Where(x=>x.BuyerId==User.Identity.Name)
                    .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            return await context.Orders
                    .Include(x=>x.OrderItems)
                    .Where(x=>x.BuyerId==User.Identity.Name && x.Id==id)
                    .FirstOrDefaultAsync();
        }
    }
}