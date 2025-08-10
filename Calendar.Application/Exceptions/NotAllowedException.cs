using System;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Application.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotAllowedException : Exception
    {
        public NotAllowedException()
            : base("User does not have permission for this action!")
        { }

        public NotAllowedException(string message)
            : base(message)
        { }

        public NotAllowedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
