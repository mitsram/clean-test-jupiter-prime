using NUnit.Framework;
using JupiterPrime.Application.UseCases;
using JupiterPrime.Domain.Entities;
using JupiterPrime.Infrastructure.Clients;
using JupiterPrime.Infrastructure.Repositories;
using System.Text.Json;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace JupiterPrime.SpecFlow.StepDefinitions;

[Binding]
public class PetApiWithJsonSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly PetApiUseCases _petUseCases;
    private Pet _testPet;

    public PetApiWithJsonSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        var apiClient = new ApiClient("https://petstore.swagger.io/v2");
        var petRepository = new PetRepository(apiClient);
        _petUseCases = new PetApiUseCases(petRepository);
    }

    [Given(@"I have a new pet with the following details:")]
    [Given(@"I have added a pet with the following details:")]
    public void GivenIHaveANewPetWithTheFollowingDetails(string petJson)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _testPet = JsonSerializer.Deserialize<Pet>(petJson, options);
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

    [Then(@"the response should contain:")]
    public void ThenTheResponseShouldContain(string expectedJson)
    {
        var actualPet = _scenarioContext.Get<Pet>("AddedPet");
        var actualJson = JsonSerializer.Serialize(actualPet);

        // Replace placeholders with actual values
        expectedJson = expectedJson.Replace("{id}", actualPet.Id.ToString());
        if (actualPet.Category != null)
            expectedJson = expectedJson.Replace("{category_id}", actualPet.Category.Id.ToString());
        if (actualPet.Tags != null && actualPet.Tags.Count > 0)
            expectedJson = expectedJson.Replace("{tag_id}", actualPet.Tags[0].Id.ToString());

        // Remove whitespace for comparison
        expectedJson = Regex.Replace(expectedJson, @"\s", "");
        actualJson = Regex.Replace(actualJson, @"\s", "");

        Assert.That(actualJson, Is.EqualTo(expectedJson));
    }

    [When(@"I retrieve the pet by its ID")]
    public async Task WhenIRetrieveThePetByItsID()
    {
        var addedPet = _scenarioContext.Get<Pet>("AddedPet");
        var retrievedPet = await _petUseCases.GetPetByIdAsync(addedPet.Id);
        _scenarioContext["RetrievedPet"] = retrievedPet;
    }

    [When(@"I update the pet with the following details:")]
    public async Task WhenIUpdateThePetWithTheFollowingDetails(string updateJson)
    {
        var updateData = JsonSerializer.Deserialize<Pet>(updateJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        var addedPet = _scenarioContext.Get<Pet>("AddedPet");
        addedPet.Status = updateData!.Status;
        var updatedPet = await _petUseCases.UpdatePetAsync(addedPet);
        _scenarioContext["UpdatedPet"] = updatedPet;
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

    [Then(@"the response should indicate successful deletion")]
    public void ThenTheResponseShouldIndicateSuccessfulDeletion()
    {
        var deleteResult = _scenarioContext.Get<bool>("DeleteResult");
        Assert.That(deleteResult, Is.True);
    }
}

