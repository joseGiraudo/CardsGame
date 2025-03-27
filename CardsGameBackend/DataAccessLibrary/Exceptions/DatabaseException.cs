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

    public class HandleDatabaseException : Exception
    {
        public int StatusCode { get; }
        protected HandleDatabaseException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : HandleDatabaseException
    {

        public NotFoundException(string message)
            : base(message, 404) { }
    }
}
