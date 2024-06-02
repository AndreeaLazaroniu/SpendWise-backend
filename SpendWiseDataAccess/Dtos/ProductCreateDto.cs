using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.DataAccess.Dtos
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public IEnumerable<int> Categories { get; set; }
    }
}
