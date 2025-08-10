using System;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
