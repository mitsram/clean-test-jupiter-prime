using NUnit.Framework;
using JupiterPrime.Application.UseCases;
using JupiterPrime.Domain.Entities;
using JupiterPrime.Infrastructure.Clients;
using JupiterPrime.Infrastructure.Repositories;
using System.Text.Json;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace JupiterPrime.SpecFlow.StepDefinitions;

[Binding]
public class PetApiSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly PetApiUseCases _petUseCases;
    private Pet _testPet;

    public PetApiSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        var apiClient = new ApiClient("https://petstore.swagger.io/v2");
        var petRepository = new PetRepository(apiClient);
        _petUseCases = new PetApiUseCases(petRepository);
    }

    [Given(@"I have a new pet with the following details:")]
    public void GivenIHaveANewPetWithTheFollowingDetails(Table table)
    {
        var petDetails = table.CreateInstance<PetDetails>();
        _testPet = new Pet
        {
            Name = petDetails.Name,
            Status = petDetails.Status,
            PhotoUrls = new List<string> { petDetails.PhotoUrl },
            Category = new Category { Name = petDetails.CategoryName },
            Tags = new List<Tag> { new Tag { Name = petDetails.TagName } }
        };
    }

    [When(@"I add the pet to the store")]
    public async Task WhenIAddThePetToTheStore()
    {
        var addedPet = await _petUseCases.AddPetAsync(_testPet);
        _scenarioContext["AddedPet"] = addedPet;
    }

    [Then(@"the pet should be successfully added")]
    public void ThenThePetShouldBeSuccessfullyAdded()
    {
        var addedPet = _scenarioContext.Get<Pet>("AddedPet");
        Assert.That(addedPet, Is.Not.Null);
        Assert.That(addedPet.Id, Is.GreaterThan(0));
    }

    [Then(@"the added pet should have the correct details")]
    public void ThenTheAddedPetShouldHaveTheCorrectDetails()
    {
        var addedPet = _scenarioContext.Get<Pet>("AddedPet");
        Assert.That(addedPet.Name, Is.EqualTo(_testPet.Name));
        Assert.That(addedPet.Status, Is.EqualTo(_testPet.Status));
        Assert.That(addedPet.PhotoUrls, Is.EquivalentTo(_testPet.PhotoUrls));
        Assert.That(addedPet.Category.Name, Is.EqualTo(_testPet.Category.Name));
        Assert.That(addedPet.Tags[0].Name, Is.EqualTo(_testPet.Tags[0].Name));
    }

    [Given(@"I have added a pet named ""(.*)"" to the store")]
    public async Task GivenIHaveAddedAPetNamedToTheStore(string petName)
    {
        var pet = new Pet { Name = petName, Status = "available" };
        var addedPet = await _petUseCases.AddPetAsync(pet);
        _scenarioContext["AddedPet"] = addedPet;
    }

    [When(@"I retrieve the pet by its ID")]
    public async Task WhenIRetrieveThePetByItsID()
    {
        var addedPet = _scenarioContext.Get<Pet>("AddedPet");
        var retrievedPet = await _petUseCases.GetPetByIdAsync(addedPet.Id);
        _scenarioContext["RetrievedPet"] = retrievedPet;
    }

    [Then(@"the retrieved pet should have the name ""(.*)""")]
    public void ThenTheRetrievedPetShouldHaveTheName(string expectedName)
    {
        var retrievedPet = _scenarioContext.Get<Pet>("RetrievedPet");
        Assert.That(retrievedPet.Name, Is.EqualTo(expectedName));
    }

    [When(@"I update the pet's status to ""(.*)""")]
    public async Task WhenIUpdateThePetsStatusTo(string newStatus)
    {
        var addedPet = _scenarioContext.Get<Pet>("AddedPet");
        addedPet.Status = newStatus;
        var updatedPet = await _petUseCases.UpdatePetAsync(addedPet);
        _scenarioContext["UpdatedPet"] = updatedPet;
    }

    [Then(@"the pet's status should be updated to ""(.*)""")]
    public void ThenThePetsStatusShouldBeUpdatedTo(string expectedStatus)
    {
        var updatedPet = _scenarioContext.Get<Pet>("UpdatedPet");
        Assert.That(updatedPet.Status, Is.EqualTo(expectedStatus));
    }

    [When(@"I delete the pet")]
    public async Task WhenIDeleteThePet()
    {
        var addedPet = _scenarioContext.Get<Pet>("AddedPet");
        var deleteResult = await _petUseCases.DeletePetAsync(addedPet.Id);
        _scenarioContext["DeleteResult"] = deleteResult;
    }

    [Then(@"the pet should be successfully deleted")]
    public void ThenThePetShouldBeSuccessfullyDeleted()
    {
        var deleteResult = _scenarioContext.Get<bool>("DeleteResult");
        Assert.That(deleteResult, Is.True);
    }
}

public class PetDetails
{
    public string Name { get; set; }
    public string Status { get; set; }
    public string PhotoUrl { get; set; }
    public string CategoryName { get; set; }
    public string TagName { get; set; }
}

