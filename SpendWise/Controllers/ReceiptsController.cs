using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SpendWise.Business.Exceptions;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;

namespace SpendWise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptsService;

        public ReceiptsController(IReceiptService receiptsService)
        {
            _receiptsService = receiptsService;
        }

        [HttpPost("ScanReceipt")]
        public async Task<ActionResult<string>> ScanReceipt([FromForm] string categories, [FromForm] IFormFile image)
        {
            List<Category> categoriesList = JsonConvert.DeserializeObject<List<Category>>(categories) ?? new List<Category>();

            if (categoriesList.IsNullOrEmpty() || image.Length <= 0)
            {
                return BadRequest();
            }

            var imageOcr = await _receiptsService.ScanReceipt(categoriesList, image);

            return Ok(imageOcr);
        }

        [HttpPost("SaveCart")]
        public async Task<ActionResult<string>> SaveCart([FromBody] CartCreateDto cart)
        {
            try
            {
                if (cart.CategoryProducts.IsNullOrEmpty())
                {
                    return BadRequest();
                }

                var createdCart = await _receiptsService.SaveCart(cart);
                return Ok();
            } catch (NotFoundExpetion ex)
            {
                return BadRequest(ex.Message);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
