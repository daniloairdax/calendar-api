using Api.Data;
using Calendar.Application.Features.Appointments.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Api.Controllers.V1
{
    /// <summary>
    /// Controller for managing appointments.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <param name="appointment">The appointment data to create.</param>
        /// <returns>The created appointment data with its assigned Id.</returns>
        /// <response code="201">Appointment created successfully.</response>
        /// <response code="400">Invalid appointment data provided.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Obsolete("This endpoint is obsolete. Please use the v2 API instead.")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AppointmentDto> CreateAppointment([FromBody] AppointmentDto appointment)
        {
            if (appointment == null)
            {
                return BadRequest("Appointment cannot be null.");
            }

            if (appointment.AnimalId == Guid.Empty || appointment.CustomerId == Guid.Empty)
            {
                return BadRequest("AnimalId and CustomerId are required.");
            }

            appointment.Id = Guid.NewGuid();

            AppointmentData.Appointments.Add(appointment);

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        /// <summary>
        /// Retrieves an appointment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the appointment.</param>
        /// <returns>The appointment data if found.</returns>
        /// <response code="200">Appointment found and returned.</response>
        /// <response code="404">Appointment not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [Obsolete("This endpoint is obsolete. Please use the v2 API instead.")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AppointmentDto> GetAppointment(Guid id)
        {
            var appointment = AppointmentData.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }
    }
}