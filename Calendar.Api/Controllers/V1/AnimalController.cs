using Api.Data;
using Calendar.Application.Features.Animals.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Api.Controllers.V1
{
    /// <summary>
    /// Controller for managing animal resources.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController : ControllerBase
    {
        /// <summary>
        /// Creates a new animal.
        /// </summary>
        /// <param name="animal">The animal data to create.</param>
        /// <returns>The created animal data with its assigned Id.</returns>
        /// <response code="201">Animal created successfully.</response>
        /// <response code="400">Invalid animal data provided.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [Obsolete("This endpoint is obsolete. Please use the v2 API instead.")]
        [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AnimalDto> CreateAnimal([FromBody] AnimalDto animal)
        {
            if (animal == null)
            {
                return BadRequest("Animal cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(animal.Name))
            {
                return BadRequest("Animal name is required.");
            }

            animal.Id = Guid.NewGuid();

            AnimalData.Animals.Add(animal);

            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        /// <summary>
        /// Retrieves an animal data by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the animal.</param>
        /// <returns>The animal data if found.</returns>
        /// <response code="200">Animal found and returned.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [Obsolete("This endpoint is obsolete. Please use the v2 API instead.")]
        [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AnimalDto> GetAnimal(Guid id)
        {
            var animal = AnimalData.Animals.FirstOrDefault(a => a.Id == id);

            return Ok(animal);
        }

        /// <summary>
        /// Deletes an animal by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the animal to delete.</param>
        /// <returns>No content if deletion is successful.</returns>
        /// <response code="204">Animal deleted successfully.</response>
        /// <response code="404">Animal not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id}")]
        [Obsolete("This endpoint is obsolete. Please use the v2 API instead.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteAnimal(Guid id)
        {
            var animal = AnimalData.Animals.FirstOrDefault(a => a.Id == id);
            if (animal == null)
            {
                return NotFound("Animal not found.");
            }

            AnimalData.Animals.Remove(animal);

            return NoContent();
        }
    }
}