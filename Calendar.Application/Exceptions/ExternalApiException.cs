using System;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Application
{
    [ExcludeFromCodeCoverage]
    public class ExternalApiException : Exception
    {
        public ExternalApiException()
            : base("There was a problem accessing external API.")
        { }

        public ExternalApiException(string message)
            : base(message)
        { }

        public ExternalApiException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
