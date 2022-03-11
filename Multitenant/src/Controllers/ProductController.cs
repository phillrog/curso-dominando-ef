using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using src.Data;
using src.Domain;

namespace EFCore.Multitenant.Controllers
{
    [ApiController]
    [Route("{tenant}/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Product> Get([FromServices]ApplicationContext db)
        {
           var products = db.Products.ToArray();

           return products;
        }
    }
}
