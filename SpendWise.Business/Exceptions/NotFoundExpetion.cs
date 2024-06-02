using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Business.Exceptions
{
    public class NotFoundExpetion : Exception
    {
        public NotFoundExpetion(string message) : base(message)
        {
        }



    }
}
