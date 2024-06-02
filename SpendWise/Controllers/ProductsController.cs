using Microsoft.AspNetCore.Mvc;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;

namespace SpendWise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productsService;

        public ProductsController(IProductService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productsService.GetProductsAsync();

            return Ok(products);
        }

        [HttpGet("GetProduct/{id}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDisplayDto>> GetProduct(int id)
        {
            var product = await _productsService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost(Name = "CreateProduct")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductCreateDto product)
        {
            var addedProduct = await _productsService.CreateProductAsync(product);

            return Ok(addedProduct);
        }
    }
}
