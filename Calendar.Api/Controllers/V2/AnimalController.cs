using Calendar.Application.Features.Animals.Commands;
using Calendar.Application.Features.Animals.Models;
using Calendar.Application.Features.Animals.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Api.Controllers.V2
{
    /// <summary>
    /// Controller for managing animal resources (v2).
    /// </summary>
    [ApiController]
    [Route("api/v2/[controller]")]
    public class AnimalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnimalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new animal.
        /// </summary>
        /// <param name="createAnimalCommand">The animal creation command.</param>
        /// <returns>The created animal data with its assigned Id.</returns>
        /// <response code="201">Animal created successfully.</response>
        /// <response code="400">Invalid animal data provided.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AnimalDto>> CreateAnimal([FromBody] CreateAnimalCommand createAnimalCommand)
        {
            var animal = await _mediator.Send(createAnimalCommand);

            return Ok(animal);
        }

        /// <summary>
        /// Retrieves an animal by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the animal.</param>
        /// <returns>The animal data if found.</returns>
        /// <response code="200">Animal found and returned.</response>
        /// <response code="404">Animal not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AnimalDto>> GetAnimal(Guid id)
        {
            var getAnimalByIdQuery = new GetAnimalByIdQuery(id);
            var animal = await _mediator.Send(getAnimalByIdQuery);

            return Ok(animal);
        }

        /// <summary>
        /// Deletes an animal by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the animal to delete.</param>
        /// <returns>No content if deletion is successful.</returns>
        /// <response code="200">Animal deleted successfully.</response>
        /// <response code="404">Animal not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAnimal(Guid id)
        {
            var deleteAnimalCommand = new DeleteAnimalCommand(id);
            var result = await _mediator.Send(deleteAnimalCommand);

            return Ok(result);
        }
    }
}