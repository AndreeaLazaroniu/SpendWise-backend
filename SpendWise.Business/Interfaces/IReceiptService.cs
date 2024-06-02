using Microsoft.AspNetCore.Http;
using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;

namespace SpendWise.Business.Interfaces
{
    public interface IReceiptService
    {
        public Task<List<CategorizedProductsDto>> ScanReceipt(List<Category> categories, IFormFile image);
        public Task<Cart> SaveCart(CartCreateDto cartDto);
    }
}
