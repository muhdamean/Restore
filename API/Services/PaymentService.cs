using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace API.Services
{
    public class PaymentService
    {
        private readonly IConfiguration _config;
        public PaymentService(IConfiguration config)
        {
            _config = config;
            
        }
        public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket)
        {
            StripeConfiguration.ApiKey= _config["StripeSettings:SecretKey"];

            var service=new PaymentIntentService();
            var intent=new PaymentIntent();
            
            var subtotal=basket.Items.Sum(item=>item.Quantity * item.Product.Price);
            var deliveryFee=subtotal>1000 ? 0 : 500;

            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options=new PaymentIntentCreateOptions
                {
                    Amount=subtotal+deliveryFee,
                    Currency="usd",
                    PaymentMethodTypes=new List<string>{"card"}
                };
                intent = await service.CreateAsync(options);
                //moved to payments controller
                //basket.PaymentIntentId=intent.Id;
                //basket.ClientSecret=intent.ClientSecret;
            }
            else
            {
                var options=new PaymentIntentUpdateOptions
                {
                    Amount=subtotal+deliveryFee
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            return intent;
        }

        //Stripe test config
        // "StripeSettings":{
        //   "PublishableKey":"pk_test_51LMDrNAvWCuijZdmLEO6U7OYOS85TJDVsFQRbSG4BASWruhtGyNGA8oqKdwBicFtRcsm7Yr5IXM0UAcJzGymqjif00Xa0IX9k8",
        //   "SecretKey":"sk_test_51LMDrNAvWCuijZdmIz72or168cq2EpfYE9Lg0wL0sjuNzLlWLatHJhsVUSunf39p3odae80e6irLN1LzeUPp5oRf00JqvF9wc5",
        //   "WhSecret":"whsec_65fce959bd72f1c6813670f1f4bee9f1992414ea652f4dabe6614d3e894698cf"
        // }

        //move stripe config to secrets in app.config
        // dotnet user-secrets init
        // dotnet user-secrets set "StripeSettings:PublishableKey" "value-here"
        // dotnet user-secrets list
    }
}