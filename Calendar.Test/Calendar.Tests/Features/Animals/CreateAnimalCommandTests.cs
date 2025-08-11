using AutoMapper;
using Calendar.Application.Features.Animals.Commands;
using Calendar.Application.Features.Animals.Models;
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
    public class CreateAnimalCommandTests
    {
        private CreateAnimalCommand _command;
        private readonly CreateAnimalCommandValidator _validator;
        private readonly IMapper _mapper;
        private readonly IAnimalRepository _animalRepository;

        public CreateAnimalCommandTests()
        {
            _validator = new CreateAnimalCommandValidator();
            _mapper = Substitute.For<IMapper>();
            _animalRepository = Substitute.For<IAnimalRepository>();
        }

        [Fact]
        public async Task Handle_CreatesAnimal_ReturnsAnimalDto()
        {
            // Arrange
            _command = new CreateAnimalCommand
            {
                Name = "Dog",
                BirthDate = DateTime.Now.AddYears(-2),
                OwnerId = Guid.NewGuid(),
                OwnerName = "Owner",
                OwnerEmail = "owner@example.com"
            };
            _animalRepository.AddAsync(Arg.Any<Animal>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            _mapper.Map<AnimalDto>(Arg.Any<Animal>()).Returns(new AnimalDto { Name = _command.Name });

            var handler = new CreateAnimalCommandHandler(_animalRepository, _mapper);

            // Act
            var result = await handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_command.Name, result.Name);
        }

        [Theory]
        [InlineData(null, "2019-01-01", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "Owner", "owner@example.com", "Animal name is required.")]
        [InlineData("Dog", "2030-01-01", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "Owner", "owner@example.com", "Birth date must be in the past.")]
        [InlineData("Dog", "2019-01-01", "00000000-0000-0000-0000-000000000000", "Owner", "owner@example.com", "OwnerId is required.")]
        [InlineData("Dog", "2019-01-01", "f47ac10b-58cc-4372-a567-0e02b2c3d479", null, "owner@example.com", "Owner name is required.")]
        [InlineData("Dog", "2019-01-01", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "Owner", null, "Owner email is required.")]
        [InlineData("Dog", "2019-01-01", "f47ac10b-58cc-4372-a567-0e02b2c3d479", "Owner", "notanemail", "Owner email must be a valid email address.")]
        public void CreateAnimalCommandValidator_Throws_ForInvalidInputs(string name, string birthDate, string ownerId, string ownerName, string ownerEmail, string expectedError)
        {
            // Arrange
            _command = new CreateAnimalCommand
            {
                Name = name ?? string.Empty,
                BirthDate = DateTime.Parse(birthDate),
                OwnerId = ownerId != null ? Guid.Parse(ownerId) : Guid.Empty,
                OwnerName = ownerName ?? string.Empty,
                OwnerEmail = ownerEmail ?? string.Empty
            };

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == expectedError);
        }

        [Fact]
        public void CreateAnimalCommandValidator_Passes_WhenValid()
        {
            // Arrange
            _command = new CreateAnimalCommand
            {
                Name = "Dog",
                BirthDate = DateTime.Now.AddYears(-2),
                OwnerId = Guid.NewGuid(),
                OwnerName = "Owner",
                OwnerEmail = "owner@example.com"
            };

            // Act
            var result = _validator.Validate(_command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}