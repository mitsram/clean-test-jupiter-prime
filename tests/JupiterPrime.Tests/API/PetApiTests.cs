using NUnit.Framework;
using JupiterPrime.Application.UseCases;
using JupiterPrime.Domain.Entities;
using JupiterPrime.Infrastructure.Clients;
using JupiterPrime.Infrastructure.Repositories;

namespace JupiterPrime.Tests.API;

[TestFixture]
[Ignore("This is how you skip a test")]
public class PetApiTests
{
    private PetApiUseCases _petUseCases;

    [SetUp]
    public void Setup()
    {
        var apiClient = new ApiClient("https://petstore.swagger.io/v2");
        var petRepository = new PetRepository(apiClient);
        _petUseCases = new PetApiUseCases(petRepository);
    }

    [Test]
    public async Task AddPet_ShouldReturnAddedPet()
    {
        // Arrange
        var newPet = new Pet
        {
            Name = "Fluffy",
            Status = "available",
            PhotoUrls = new List<string> { "http://example.com/fluffy.jpg" },
            Categories = new List<Category> { new Category { Id = 1, Name = "Dogs" } },
            Tags = new List<Tag> { new Tag { Id = 1, Name = "Friendly" } }
        };

        // Act
        var addedPet = await _petUseCases.AddPetAsync(newPet);

        // Assert
        Assert.That(addedPet, Is.Not.Null);
        Assert.That(addedPet.Id, Is.GreaterThan(0));
        Assert.That(addedPet.Name, Is.EqualTo(newPet.Name));
        Assert.That(addedPet.Status, Is.EqualTo(newPet.Status));
    }

    [Test]
    public async Task GetPetById_ShouldReturnCorrectPet()
    {
        // Arrange
        var newPet = new Pet
        {
            Name = "Buddy",
            Status = "available"
        };
        var addedPet = await _petUseCases.AddPetAsync(newPet);        

        // Act
        var retrievedPet = await _petUseCases.GetPetByIdAsync(addedPet.Id);

        // Assert
        Assert.That(retrievedPet, Is.Not.Null);
        Assert.That(retrievedPet.Id, Is.EqualTo(addedPet.Id));
        Assert.That(retrievedPet.Name, Is.EqualTo(addedPet.Name));
        Assert.That(retrievedPet.Status, Is.EqualTo(addedPet.Status));
    }

    [Test]
    public async Task UpdatePet_ShouldReturnUpdatedPet()
    {
        // Arrange
        var newPet = new Pet
        {
            Name = "Max",
            Status = "available"
        };
        var addedPet = await _petUseCases.AddPetAsync(newPet);

        addedPet.Status = "sold";

        // Act
        var updatedPet = await _petUseCases.UpdatePetAsync(addedPet);

        // Assert
        Assert.That(updatedPet, Is.Not.Null);
        Assert.That(updatedPet.Id, Is.EqualTo(addedPet.Id));
        Assert.That(updatedPet.Name, Is.EqualTo(addedPet.Name));
        Assert.That(updatedPet.Status, Is.EqualTo("sold"));
    }

    [Test]
    public async Task DeletePet_ShouldReturnTrue()
    {
        // Arrange
        var newPet = new Pet
        {
            Name = "Charlie",
            Status = "available"
        };
        var addedPet = await _petUseCases.AddPetAsync(newPet);

        // Act
        var deleteResult = await _petUseCases.DeletePetAsync(addedPet.Id);

        // Assert
        Assert.That(deleteResult, Is.True);
    }
}
