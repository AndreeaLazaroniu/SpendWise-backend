using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpendWise.DataAccess.Entities;

namespace SpendWise.DataAccess.Repositories
{
    public class CategoryRepository : BaseRepository<Category>
    {

        public CategoryRepository(SpendWiseContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                return await _context.Categories.Include(c => c.Products).ThenInclude(p => p.CartProducts).ThenInclude(p => p.Cart).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when retrieving data from DB: {ex.Message}", ex);
            }
        }

        public override async Task<Category?> FindByIdAsync(int id)
        {
            try
            {
                return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when retrieving entity by id {id}, {ex.Message}", ex);
            }
        }
    }
}
