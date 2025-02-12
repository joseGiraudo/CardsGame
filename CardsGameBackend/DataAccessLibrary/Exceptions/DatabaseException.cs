using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception inner)
            : base(message, inner) { }
    }
}
