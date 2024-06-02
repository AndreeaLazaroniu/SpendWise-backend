using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;

namespace SpendWise.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<IEnumerable<CategoryDisplayDto>> CategoriesAsync();
        Task<Category> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<Category> UpdateAsync(int id, CategoryUpdateDto categoryDto);
        Task DeleteByIdAsync(int id);
        Task<List<CategorySpendingDto>> GetCategoriesSpendingAsync(DateTime? dateFrom, DateTime? dateTo);
    }
}
