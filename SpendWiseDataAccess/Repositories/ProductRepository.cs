using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpendWise.DataAccess.Entities;

namespace SpendWise.DataAccess.Repositories
{
    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository(SpendWiseContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await _context.Products.Include(p => p.Categories).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when retrieving data from DB: {ex.Message}", ex);
            }
        }

        public override async Task<Product?> FindByIdAsync(int id)
        {
            try
            {
                return await _context.Products.Include(p => p.Categories).FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when retrieving entity by id {id}, {ex.Message}", ex);
            }
        }
    }
}
