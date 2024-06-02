using Microsoft.AspNetCore.Mvc;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;
using SpendWise.DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SpendWise.Business.Exceptions;

namespace SpendWise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoriesService;

        public CategoriesController(ICategoryService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoriesService.GetCategoriesAsync();

            return Ok(categories);
        }
        
        [HttpGet()]
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoriesService.CategoriesAsync();

            return Ok(categories);
        }

        [HttpGet("GetCategory/{id}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoriesService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto category)
        {
            var createdCategory = await _categoriesService.CreateCategoryAsync(category);

            return Ok(createdCategory);
        }

        [HttpPut("{id}", Name = "UpdateCategory")]
        public async Task<ActionResult<Category>> UpdateCategory(int id, [FromBody] CategoryUpdateDto category)
        {
            if (category.Name.IsNullOrEmpty())
            {
                return BadRequest("Category ID mismatch");
            }

            try
            {
                var updatedCategory = await _categoriesService.UpdateAsync(id, category);

                return Ok(updatedCategory);
            }
            catch (NotFoundExpetion e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpDelete("{id}", Name = "DeleteCategory")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            await _categoriesService.DeleteByIdAsync(id);

            return Ok();
        }

        [HttpGet("GetCategoriesSpending")]
        public async Task<IActionResult> GetCategoriesSpending([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
        {
            var categoriesSpending = await _categoriesService.GetCategoriesSpendingAsync(dateFrom, dateTo);

            return Ok(JsonConvert.SerializeObject(categoriesSpending));
        }
    }
}
