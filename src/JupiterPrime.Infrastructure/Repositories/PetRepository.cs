using System.Text.Json;
using RestSharp;
using JupiterPrime.Application.Interfaces;
using JupiterPrime.Domain.Entities;
using JupiterPrime.Infrastructure.Clients;

namespace JupiterPrime.Infrastructure.Repositories;

public class PetRepository : IPetRepository
{
    private readonly ApiClient _apiClient;

    public PetRepository(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Pet> AddPetAsync(Pet pet)
    {
        var request = new RestRequest("pet", Method.Post);
        request.AddJsonBody(pet);

        var response = await _apiClient.ExecuteAsync(request);
        Console.WriteLine($"Response Status: {response.StatusCode}");
        Console.WriteLine($"Response Content: {response.Content}");

        if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var deserializedPet = JsonSerializer.Deserialize<Pet>(response.Content, options);
                Console.WriteLine($"Deserialized Pet: {JsonSerializer.Serialize(deserializedPet)}");
                return deserializedPet;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to deserialize the response content: {ex.Message}");
            }
        }
        else
        {
            throw new Exception($"API request failed: {response.ErrorMessage ?? response.StatusDescription}");
        }
    }

    public async Task<Pet> GetPetByIdAsync(long petId)
    {
        var request = new RestRequest($"pet/{petId}", Method.Get);

        var response = await _apiClient.ExecuteAsync(request);
        Console.WriteLine($"GetPetByIdAsync Response Status: {response.StatusCode}");
        Console.WriteLine($"GetPetByIdAsync Response Content: {response.Content}");

        if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var deserializedPet = JsonSerializer.Deserialize<Pet>(response.Content, options);
                Console.WriteLine($"GetPetByIdAsync Deserialized Pet: {JsonSerializer.Serialize(deserializedPet)}");
                return deserializedPet;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to deserialize the response content in GetPetByIdAsync: {ex.Message}");
            }
        }
        else
        {
            throw new Exception($"API request failed in GetPetByIdAsync: {response.ErrorMessage ?? response.StatusDescription}");
        }
    }

    public async Task<Pet> UpdatePetAsync(Pet pet)
    {
        var request = new RestRequest("pet", Method.Put);
        request.AddJsonBody(pet);

        var response = await _apiClient.ExecuteAsync(request);
        Console.WriteLine($"UpdatePetAsync Response Status: {response.StatusCode}");
        Console.WriteLine($"UpdatePetAsync Response Content: {response.Content}");

        if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var deserializedPet = JsonSerializer.Deserialize<Pet>(response.Content, options);
                Console.WriteLine($"UpdatePetAsync Deserialized Pet: {JsonSerializer.Serialize(deserializedPet)}");
                return deserializedPet;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to deserialize the response content in UpdatePetAsync: {ex.Message}");
            }
        }
        else
        {
            throw new Exception($"API request failed in UpdatePetAsync: {response.ErrorMessage ?? response.StatusDescription}");
        }
    }

    public async Task<bool> DeletePetAsync(long petId)
    {
        var request = new RestRequest($"pet/{petId}", Method.Delete);

        var response = await _apiClient.ExecuteAsync(request);
        return response.IsSuccessful;
    }
}

