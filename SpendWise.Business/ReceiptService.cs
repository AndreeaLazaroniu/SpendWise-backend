using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SpendWise.Business.Exceptions;
using SpendWise.Business.Interfaces;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;
using SpendWise.DataAccess.Repositories;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace SpendWise.Business
{
    public class ReceiptService : IReceiptService
    {
        private readonly string OCRURL = "https://netrom-ligaaclabs2024-dev.stage04.netromsoftware.ro";
        private const string ChatGptUrl = "https://netrom-ligaaclabs2024chatgpt-dev.stage04.netromsoftware.ro";
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Category> _categoriesRepository;

        public ReceiptService(IRepository<Product> productRepository, IRepository<Cart> cartRepository, IRepository<Category> categoriesRepository)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _categoriesRepository = categoriesRepository;
        }

        public async Task<List<CategorizedProductsDto>> ScanReceipt(List<Category> categories, IFormFile image)
        {
            var imageOcr = await GetImageOcr(image);
            var categorizedProducts = await GetCategorizedProducts(categories, imageOcr);

            var deserializedCategoriesProducts = JsonConvert.DeserializeObject<string>(categorizedProducts);
            var categoriesProducts = JsonConvert.DeserializeObject<Dictionary<string, List<ScanedProductsDto>>>(deserializedCategoriesProducts);

            var categorizedProductsDto = new List<CategorizedProductsDto>();

            foreach (var category in categoriesProducts)
            {
                var products = category.Value.DistinctBy(p => new { p.Name, p.Price }).ToList();

                foreach (var product in products)
                {
                    product.Quantity = category.Value.Where(p => p.Name == product.Name && p.Price == product.Price).Sum(p => p.Quantity);
                }

                categorizedProductsDto.Add(new CategorizedProductsDto
                {
                    Id = categories.First(c => c.Name == category.Key).Id,
                    Name = category.Key,
                    Products = products
                });
            }

            return categorizedProductsDto;
        }

        public async Task<Cart> SaveCart(CartCreateDto cartDto)
        {
            var repoProducts = await _productRepository.GetAllAsync();

            var cart = new Cart
            {
                Date = cartDto.Date,
            };

            var cartProducts = new List<CartProduct>();

            foreach (var categoryProducts in cartDto.CategoryProducts)
            {
                var category = await _categoriesRepository.FindByIdAsync(categoryProducts.Id);

                if (category == null)
                {
                    throw new NotFoundExpetion($"Entity of type {typeof(Category)} not found");
                }


                cartProducts.AddRange(await AddProducts(category, categoryProducts.Products, repoProducts.ToList()));
            }

            cart.CartProducts = cartProducts;

            await _cartRepository.PostAsync(cart);

            return cart;
        }

        #region Private methods

        private async Task<string> GetImageOcr(IFormFile image)
        {
            var imageOcr = string.Empty;

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            var bytes = ms.ToArray();

            HttpClient client = new HttpClient();
            var uri = new Uri(OCRURL);
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(bytes), "image", "image.jpg");

            var response = client.PostAsync(uri, content).Result;

            if (response.IsSuccessStatusCode)
            {
                imageOcr = await response.Content.ReadAsStringAsync();
            }

            await ms.DisposeAsync();
            client.Dispose();

            return imageOcr;
        }

        private async Task<string> GetCategorizedProducts(List<Category> categories, string imageOcr)
        {
            var categorizedProducts = string.Empty;

            HttpClient client = new HttpClient();
            var uri = new Uri(ChatGptUrl);
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(imageOcr), "ocr");
            content.Add(new StringContent(JsonConvert.SerializeObject(categories.Select(c => c.Name))), "categories");

            var response = client.PostAsync(uri, content).Result;

            if (response.IsSuccessStatusCode)
            {
                categorizedProducts = await response.Content.ReadAsStringAsync();
            }

            client.Dispose();

            return categorizedProducts;
        }

        private async Task<List<CartProduct>> AddProducts(Category category, List<ScanedProductsDto> scannedProducts, List<Product> repoProducts)
        {
            var cartProducts = new List<CartProduct>();

            foreach (var scannedProduct in scannedProducts)
            {
                var repoProduct = repoProducts.Find(p => p.Name == scannedProduct.Name);

                if (repoProduct == null)
                {
                    var product = new Product
                    {
                        Name = scannedProduct.Name,
                        Categories = new List<Category> { category }
                    };

                    await _productRepository.PostAsync(product);

                    var cartProduct = new CartProduct
                    {
                        Product = product,
                        Quantity = scannedProduct.Quantity,
                        Price = scannedProduct.Price,
                    };

                    cartProducts.Add(cartProduct);
                }
                else
                {
                    if (!repoProduct.Categories.Contains(category))
                    {
                        repoProduct.Categories.Add(category);

                        await _productRepository.UpdateAsync(repoProduct);
                    }

                    var cartProduct = new CartProduct
                    {
                        Product = repoProduct,
                        Quantity = scannedProduct.Quantity,
                        Price = scannedProduct.Price,
                    };

                    cartProducts.Add(cartProduct);
                }
            }

            return cartProducts;
        }

        #endregion
    }
}