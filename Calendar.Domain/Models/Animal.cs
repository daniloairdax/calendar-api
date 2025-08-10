using System;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Domain.Models
{
    public class Animal : BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        public string OwnerEmail { get; set; } = string.Empty;
    }
}