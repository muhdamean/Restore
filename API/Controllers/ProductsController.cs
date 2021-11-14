using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
   
    public class ProductsController : BaseApiController
    {
        
        private readonly StoreContext _context;

        public ProductsController(StoreContext context)
        {
            _context = context;
           
        }
        [HttpGet]
        public ActionResult<List<Product>> GetProducts()
        {
            var products=_context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product= _context.Products.Find(id);
            if(product==null) return NotFound();
            return product;
        }
    }
}