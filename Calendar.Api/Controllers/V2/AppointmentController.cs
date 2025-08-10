using Calendar.Application.Features.Appointments.Commands;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Api.Controllers.V2
{
    /// <summary>
    /// Controller for managing appointments (v2).
    /// </summary>
    [ApiController]
    [Route("api/v2/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <param name="createAppointmentCommand">The appointment creation command.</param>
        /// <returns>The created appointment data with its assigned Id.</returns>
        /// <response code="201">Appointment created successfully.</response>
        /// <response code="400">Invalid appointment data provided.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentCommand createAppointmentCommand)
        {
            var appointment = await _mediator.Send(createAppointmentCommand);

            return Ok(appointment);
        }

        /// <summary>
        /// Retrieves an appointment data by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the appointment.</param>
        /// <returns>The appointment if found.</returns>
        /// <response code="200">Appointment found and returned.</response>
        /// <response code="400">Invalid data provided.</response>
        /// <response code="404">Appointment not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(Guid id)
        {
            var getAppointmentByIdQuery = new GetAppointmentByIdQuery(id);
            var appointment = await _mediator.Send(getAppointmentByIdQuery);

            return Ok(appointment);
        }

        /// <summary>
        /// Retrieves appointments for a specific veterinarian within a date range.
        /// </summary>
        /// <param name="vetId">The unique identifier of the veterinarian.</param>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>List of appointments for the veterinarian.</returns>
        /// <response code="200">Appointments found and returned.</response>
        /// <response code="400">Invalid data provided.</response>
        /// <response code="404">No appointments found for the veterinarian.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("vet/{vetId}")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetVetAppointments(
            Guid vetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var GgtVetAppointmentsQuery = new GetVetAppointmentsQuery(vetId, startDate, endDate);
            var appointments = await _mediator.Send(GgtVetAppointmentsQuery);

            return Ok(appointments);
        }

        /// <summary>
        /// Updates the status of an appointment.
        /// </summary>
        /// <param name="updateAppointmentStatusCommand">The command containing the new status.</param>
        /// <returns>No content if update is successful.</returns>
        /// <response code="200">Status updated successfully.</response>
        /// <response code="400">Invalid data provided.</response>
        /// <response code="404">Appointment not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("status")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateAppointmentStatusCommand updateAppointmentStatusCommand)
        {
            var result = await _mediator.Send(updateAppointmentStatusCommand);

            return Ok(result);
        }
    }
}