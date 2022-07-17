using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly PaymentService paymentService;
        private readonly StoreContext context;

        public PaymentsController(PaymentService paymentService, StoreContext context)
        {
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
    }
}