using Calendar.Application.Exceptions;
using Calendar.Application.Features.Animals.Commands;
using Calendar.Application.Interfaces;
using Calendar.Domain.Models;
using NSubstitute;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Calendar.Tests.Features.Animals
{
    [ExcludeFromCodeCoverage]
    public class DeleteAnimalCommandTests
    {
        private DeleteAnimalCommand _command;
        private readonly DeleteAnimalCommandValidator _validator;
        private readonly IAnimalRepository _animalRepository;

        public DeleteAnimalCommandTests()
        {
            _validator = new DeleteAnimalCommandValidator();
            _animalRepository = Substitute.For<IAnimalRepository>();
        }

        [Fact]
        public async Task Handle_DeletesAnimal_ReturnsTrue()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            _command = new DeleteAnimalCommand(animalId);
            _animalRepository.GetByIdAsync(animalId, Arg.Any<CancellationToken>()).Returns(new Animal { Id = animalId });
            _animalRepository.DeleteAsync(animalId, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var handler = new DeleteAnimalCommandHandler(_animalRepository);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.True(result);
            await _animalRepository.Received(1).DeleteAsync(animalId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenAnimalDoesNotExist()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            _command = new DeleteAnimalCommand(animalId);
            _animalRepository.GetByIdAsync(animalId, Arg.Any<CancellationToken>()).Returns((Animal)null);
            var handler = new DeleteAnimalCommandHandler(_animalRepository);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(_command, CancellationToken.None));
        }

        [Fact]
        public void DeleteAnimalCommandValidator_Throws_WhenIdIsEmpty()
        {
            // Arrange
            _command = new DeleteAnimalCommand(Guid.Empty); 

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Id");
        }

        [Fact]
        public void DeleteAnimalCommandValidator_Passes_WhenValid()
        {
            // Arrange
            _command = new DeleteAnimalCommand(Guid.NewGuid());

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
