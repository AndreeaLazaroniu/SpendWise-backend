using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;
using SpendWise.DataAccess.Repositories;
using SpendWise.Business.Exceptions;

namespace SpendWise.Business
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoriesRepository;

        public CategoryService(IRepository<Category> categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoriesRepository.GetAllAsync();

            var categoriesDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                })
            }).ToList();

            return categoriesDtos;
        }

        public async Task<IEnumerable<CategoryDisplayDto>> CategoriesAsync()
        {
            var categories = await _categoriesRepository.GetAllAsync();

            var categoriesDtos = categories.Select(c => new CategoryDisplayDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();

            return categoriesDtos;
        }

        public async Task<Category> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name
            };
            return await _categoriesRepository.PostAsync(category);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoriesRepository.FindByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            CategoryDto categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
            };

            return categoryDto;
        }

        public async Task<Category> UpdateAsync(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _categoriesRepository.FindByIdAsync(id);

            if (category == null)
            {
                throw new NotFoundExpetion($"Unable to find category with id {id}");
            }

            category.Name = categoryDto.Name;

            return await _categoriesRepository.UpdateAsync(category);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _categoriesRepository.DeleteAsync(id);
        }

        public async Task<List<CategorySpendingDto>> GetCategoriesSpendingAsync(DateTime? dateFrom, DateTime? dateTo)
        {
            var categories = await _categoriesRepository.GetAllAsync();
            var categoriesSpending = new List<CategorySpendingDto>();

            foreach (var category in categories)
            {
                double sum = 0;

                foreach (var product in category.Products)
                {
                    if (dateFrom != null && dateTo != null)
                    {
                        sum += product.CartProducts.Where(c => c.Cart.Date >= dateFrom && c.Cart.Date <= dateTo)
                            .Sum(c => c.Price * c.Quantity);
                    }
                    else
                    {
                        sum += product.CartProducts.Sum(c => c.Price * c.Quantity);
                    }
                }

                var categorySpending = new CategorySpendingDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    TotalSpent = sum
                };

                categoriesSpending.Add(categorySpending);
            }

            return categoriesSpending;
        }
    }
}
