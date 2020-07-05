using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wali_Express.Core.Models;

namespace Wali_Express.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseContext _db;

        public ProductsController(DatabaseContext context)
        {
            _db = context;
            if (!_db.Products.Any())
            {
                _db.Products.Add(new Product() {Name = "Товар 1", Amount = 100});
                _db.Products.Add(new Product() {Name = "Товар 2", Amount = 100});
                _db.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            return await _db.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            Product good = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (good == null)
            {
                return NotFound();
            }
            return new ObjectResult(good);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }
            ShopStorage shopStorage = new ShopStorage(_db);
            await shopStorage.AddProduct(product);
            return Ok(product);
        }

        [HttpPut]
        public async Task<ActionResult<Product>> Put(Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (!_db.Products.Any(x => x.Id == product.Id))
            {
                return NotFound();
            }
            ShopStorage shopStorage = new ShopStorage(_db);
            await shopStorage.UpdateProduct(product.Name, product.Amount);
            return Ok(product);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            Product product = _db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Ok(product);
        }
    }
}