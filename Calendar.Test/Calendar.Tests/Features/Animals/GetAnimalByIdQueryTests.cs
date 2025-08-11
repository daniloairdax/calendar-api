using AutoMapper;
using Calendar.Application.Exceptions;
using Calendar.Application.Features.Animals.Models;
using Calendar.Application.Features.Animals.Queries;
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
    public class GetAnimalByIdQueryTests
    {
        private GetAnimalByIdQuery _query;
        private readonly GetAnimalByIdQueryValidator _validator;
        private readonly IMapper _mapper;
        private readonly IAnimalRepository _animalRepository;

        public GetAnimalByIdQueryTests()
        {
            _validator = new GetAnimalByIdQueryValidator();
            _mapper = Substitute.For<IMapper>();
            _animalRepository = Substitute.For<IAnimalRepository>();
        }

        [Fact]
        public async Task Handle_ReturnsAnimalDto_WhenAnimalExists()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            _query = new GetAnimalByIdQuery(animalId);
            var animal = new Animal { Id = animalId, Name = "Dog" };
            _animalRepository.GetByIdAsync(animalId, Arg.Any<CancellationToken>()).Returns(animal);
            var animalDto = new AnimalDto { Id = animalId, Name = "Dog" };
            _mapper.Map<AnimalDto>(animal).Returns(animalDto);
            var handler = new Handler(_animalRepository, _mapper);

            // Act
            var result = await handler.Handle(_query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(animalId, result.Id);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenAnimalDoesNotExist()
        {
            // Arrange
            var animalId = Guid.NewGuid();
            _query = new GetAnimalByIdQuery(animalId);
            _animalRepository.GetByIdAsync(animalId, Arg.Any<CancellationToken>()).Returns((Animal)null);
            var handler = new Handler(_animalRepository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(_query, CancellationToken.None));
        }

        [Fact]
        public void GetAnimalByIdQueryValidator_Throws_WhenIdIsEmpty()
        {
            // Arrange
            _query = new GetAnimalByIdQuery(Guid.Empty);

            // Act
            var result = _validator.Validate(_query);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Id");
        }

        [Fact]
        public void GetAnimalByIdQueryValidator_Passes_WhenIdIsValid()
        {
            // Arrange
            var validId = Guid.NewGuid();
            _query = new GetAnimalByIdQuery(validId);

            // Act
            var result = _validator.Validate(_query);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
