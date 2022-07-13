using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
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

        [HttpGet("{id}", Name="GetOrder")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            return await context.Orders
                    .Include(x=>x.OrderItems)
                    .Where(x=>x.BuyerId==User.Identity.Name && x.Id==id)
                    .FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(CreateOrderDto orderDto){
            var basket=await context.Baskets
                        .RetrieveBasketWithItems(User.Identity.Name)
                        .FirstOrDefaultAsync();

            if(basket ==null) return BadRequest(new ProblemDetails{Title="Could not locate basket"});

            var items =new List<OrderItems>();

            foreach (var item in basket.Items)
            {
                var productItem=await context.Products.FindAsync(item.ProductId);
                var itemOrdered=new ProductItemOrdered
                {
                    ProductId=productItem.Id,
                    Name= productItem.Name,
                    PictureUrl=productItem.PictureUrl
                };
                var orderItem=new OrderItems
                {
                    ItemOrdered=itemOrdered,
                    Price=productItem.Price,
                    Quantity=item.Quantity
                };
                items.Add(orderItem);
                productItem.QuantityInStock-=item.Quantity;
            }

            var subtotal=items.Sum(item=>item.Price * item.Quantity);
            var deliveryFee=subtotal>1000 ? 0 : 500;

            var order=new Order
            {
                OrderItems=items,
                BuyerId=User.Identity.Name,
                ShippingAddress=orderDto.ShippingAddress,
                SubTotal=subtotal,
                DeliveryFee=deliveryFee
            };

            context.Orders.Add(order);
            context.Baskets.Remove(basket);

            if(orderDto.SaveAddress)
            {
                var user=await context.Users.FirstOrDefaultAsync(x=>x.UserName==User.Identity.Name);
                user.Address=new UserAddress
                {
                    FulName=orderDto.ShippingAddress.FulName,
                    Address1=orderDto.ShippingAddress.Address1,
                    Address2=orderDto.ShippingAddress.Address2,
                    City=orderDto.ShippingAddress.City,
                    Zip=orderDto.ShippingAddress.Zip,
                    Country=orderDto.ShippingAddress.Country
                };
                context.Update(user);
            }
            var result=await context.SaveChangesAsync()>0;

            if(result) return CreatedAtRoute("GetOrder",new {id=order.Id}, order.Id);

            return BadRequest("Problem creating order");
        }
    }
}