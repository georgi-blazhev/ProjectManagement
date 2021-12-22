using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.BLL.Exceptions
{
    public class AlreadyExistingException : Exception
    {
        public AlreadyExistingException(string message) : base(message)
        {

        }
    }
}
