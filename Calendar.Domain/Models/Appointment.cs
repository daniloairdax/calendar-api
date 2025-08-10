using Calendar.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendar.Domain.Models
{
    public class Appointment : BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [ForeignKey("Animal")]
        public Guid AnimalId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid VeterinarianId { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public string? Notes { get; set; }

        public Animal? Animal { get; set; }
    }
}