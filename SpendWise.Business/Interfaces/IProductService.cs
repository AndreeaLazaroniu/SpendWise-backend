using SpendWise.DataAccess.Dtos;
using SpendWise.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Business.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDisplayDto>> GetProductsAsync();
        Task<ProductDisplayDto> CreateProductAsync(ProductCreateDto productDto);
        Task<ProductDisplayDto?> GetByIdAsync(int id);
    }
}
