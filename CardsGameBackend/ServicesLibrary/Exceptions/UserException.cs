using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.Exceptions
{
    public class UserException : Exception
    {
        public int StatusCode { get; }

        public UserException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class DuplicateEmailException : UserException
    {
        public DuplicateEmailException(string email)
            : base($"El email {email} ya está registrado", 400) { }
    }

    public class UnauthorizedRoleException : UserException
    {
        public UnauthorizedRoleException(string message)
            : base(message, 403) { }
    }
}
