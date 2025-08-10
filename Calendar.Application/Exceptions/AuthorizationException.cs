using System;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Application
{
    [ExcludeFromCodeCoverage]
    public class AuthorizationException : Exception
    {
        public AuthorizationException()
            : base("User Authorization failed!")
        { }

        public AuthorizationException(string message)
            : base(message)
        { }

        public AuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
