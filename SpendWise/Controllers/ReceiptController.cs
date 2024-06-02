using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess.Entities;
using System.Collections.Generic;

namespace SpendWise.Controllers
{
    [Route(template: "api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet(template: "ScanReceipt")]
        public async Task<ActionResult<string>> ScanReceipt([FromForm] string categories, [FromForm] IFormFile image)
        { 
            List<Category> categoriesList = JsonConvert.DeserializeObject<List<Category>>(categories) ?? new List<Category>(); //verifies if the categories are null

            if (categoriesList.IsNullOrEmpty() || image.Length<=0)
            {
                return BadRequest();
            }

            var imageOCR = await _receiptService.ScanReceipt(categoriesList, image);
            return Ok(imageOCR);
        }
    }
}
