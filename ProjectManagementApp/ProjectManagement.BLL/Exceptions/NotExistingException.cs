using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Exceptions
{
    public class NotExistingException : Exception
    {
        public NotExistingException(string message) : base(message)
        {

        }
    }
}
