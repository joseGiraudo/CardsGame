using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.Exceptions
{
    public abstract class TournamentException : Exception
    {
        public int StatusCode { get; }

        protected TournamentException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : TournamentException
    {
        public NotFoundException(string message)
            : base(message, 404) { }
    }

    public class BadRequestException : TournamentException
    {
        public BadRequestException(string message) : base(message, 400) { }
    }

    public class InconsistentException : TournamentException
    {
        public InconsistentException(string message) : base(message, 409) { }
    }

    public class UnauthorizedException : TournamentException
    {
        public UnauthorizedException(string message)
            : base(message, 401) { }
    }

    public class RegistrationClosedException : TournamentException
    {
        public RegistrationClosedException(string message = "No hay más cupos disponibles para el torneo.")
            : base(message, 409) { }
    }
    public class JudgesNotFoundException : TournamentException
    {
        public JudgesNotFoundException(string message = "No se encontraron jueces asignados al torneo.")
            : base(message, 400) { }
    }

    public class InvalidPlayerException : TournamentException
    {
        public InvalidPlayerException(string message = "El id de jugador no es válido.")
            : base(message, 400) { }
    }
    public class InvalidDeckException : TournamentException
    {
        public InvalidDeckException(string message = "El mazo posee cartas no permitidas en este torneo.")
            : base(message, 400) { }
    }

    public class TournamentCanceledException : TournamentException
    {
        public TournamentCanceledException(string message = "El torneo fue cancelado por un administrador.")
            : base(message, 400) { }
    }
    public class TournamentFinishedException : TournamentException
    {
        public TournamentFinishedException(string message = "El torneo ya se encuentra finalizado.")
            : base(message, 400) { }
    }
}
