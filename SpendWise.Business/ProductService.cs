using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;
using SpendWise.DataAccess.Repositories;

namespace SpendWise.Business
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<Category> _categoriesRepository;

        public ProductService(IRepository<Product> productsRepository, IRepository<Category> categoriesRepository)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
        }

        public async Task<IEnumerable<ProductDisplayDto>> GetProductsAsync()
        {
            var products = await _productsRepository.GetAllAsync();

            var productsDtos = products.Select(p => new ProductDisplayDto
            {
                Id = p.Id,
                Name = p.Name,
                Categories = p.Categories.Select(c => c.Name)
            }).ToList();

            return productsDtos;
        }

        public async Task<ProductDisplayDto> CreateProductAsync(ProductCreateDto productDto)
        {
            var categories = await _categoriesRepository.GetAllAsync();

            var product = new Product
            {
                Name = productDto.Name,
                Categories = categories.Where(c => productDto.Categories.Contains(c.Id)).ToList()
            };

            var addedProduct = await _productsRepository.PostAsync(product);

            var addedProductDto = new ProductDisplayDto
            {
                Id = addedProduct.Id,
                Name = addedProduct.Name,
                Categories = addedProduct.Categories.Select(c => c.Name)
            };

            return addedProductDto;
        }

        public async Task<ProductDisplayDto?> GetByIdAsync(int id)
        {
            var product = await _productsRepository.FindByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            var productDto = new ProductDisplayDto
            {
                Id = product.Id,
                Name = product.Name,
                Categories = product.Categories.Select(c => c.Name)
            };

            return productDto;
        }
    }
}
