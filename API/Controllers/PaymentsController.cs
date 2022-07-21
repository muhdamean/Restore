using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly PaymentService paymentService;
        private readonly StoreContext context;
        private readonly IConfiguration config;

        public PaymentsController(PaymentService paymentService, StoreContext context, IConfiguration config)
        {
            this.config = config;
            this.paymentService = paymentService;
            this.context = context;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent()
        {
            var basket =await context.Baskets
                .RetrieveBasketWithItems(User.Identity.Name)
                .FirstOrDefaultAsync();

            if(basket == null) return NotFound();

            var intent=await paymentService.CreateOrUpdatePaymentIntent(basket);

            if(intent==null ) return BadRequest(new ProblemDetails{Title="Problem creating payment intent"});

            basket.PaymentIntentId=basket.PaymentIntentId ?? intent.Id;
            basket.ClientSecret=basket.ClientSecret ?? intent.ClientSecret;

            context.Update(basket);
            var result=await context.SaveChangesAsync()>0;

            if(!result) return BadRequest(new ProblemDetails{Title="Problem updating basket with intent"});

            return basket.MapBasketToDto();
        }
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent=EventUtility.ConstructEvent(json, 
                                                    Request.Headers["Stripe-Signature"],
                                                    config["StripeSettings:WhSecret"]);

            var charge=(Charge)stripeEvent.Data.Object;

            var order=await context.Orders.FirstOrDefaultAsync(x=>
                x.PaymentIntentId==charge.PaymentIntentId);
            
            if(charge.Status=="succeeded") order.OrderStatus=OrderStatus.PaymentRecieved;

            await context.SaveChangesAsync();

            return new EmptyResult();
        }
    }
}