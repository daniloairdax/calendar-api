using System;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Application.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class MissingClientHeaderException : Exception
    {
        public MissingClientHeaderException(string message)
            : base(message)
        { }
    }
}
