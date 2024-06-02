using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.DataAccess.Dtos
{
    public class CategorySpendingDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double TotalSpent { get; set; }
    }
}