using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.DataAccess.Dtos
{
    public class CartCreateDto
    {
        public DateTime Date { get; set; }
        public List<CategorizedProductsDto> CategoryProducts { get; set; }
    }
}
