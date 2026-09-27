using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleDB;
using Xunit.Sdk;

namespace end_to_end.Tests;

public class FuzzTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;
    private readonly List<Observation> knownObservations = new();
    private readonly List<Comment> knownComments = new();
    private readonly Random rng = new();
    record StoredResponse(string status, int id);

    public FuzzTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

[Fact]
public async Task FuzzTest()
{

    for (int i = 0; i < 100; i++)
        {
            int choice = rng.Next(2);

            if (knownObservations.Count == 0 || choice == 0)
            {
                await FuzzPostObservation();
            } 
            else
            {
                await FuzzPostComment();
            }
        } 

    var serverObservations = await client.GetFromJsonAsync<List<Observation>>("/observations");
    if (serverObservations == null)
        {
            throw new InvalidOperationException("Server did not return a valid list of observations.");
        }
        Assert.Equal(knownObservations.Count, serverObservations.Count);



    //Find all unique observation-ID's that has comments
    var observationIdsWithComments = new List<int>();
    foreach (var comment in knownComments)
        {
            if (!observationIdsWithComments.Contains(comment.Id))
            {
                observationIdsWithComments.Add(comment.Id);
            }
        }

    //For each of the ID's check that the number of comments matches the server
    foreach (var obsId in observationIdsWithComments)
    {
        var serverComments = await client.GetFromJsonAsync<List<Comment>>($"/comments?id={obsId}");
            if (serverComments == null)
            {
                throw new InvalidOperationException("Server did not return a valid list of comments.");
            }

        int expectedCount = 0;
        foreach (var comment in knownComments)
        {
            if (comment.Id == obsId)
            {
                expectedCount++;
            }
        }
        Assert.Equal(expectedCount, serverComments.Count);
    }
}
        


// Helper method to generate a random string of a given length
public string RandomString(int minLen, int maxLen) {
        
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 æøåÆØÅ!?.,-_'\"";        
    int length = rng.Next(minLen, maxLen + 1);

    var result = new char[length];
    for (int i = 0; i < length; i++)
    {
        result[i] = chars[rng.Next(chars.Length)];
    }
    return new string(result);
    }


// Fuzz test for posting observations
public async Task FuzzPostObservation()
    {
        var obs = new Observation(
            //Author, Message, Timestamp, Id, Location
            RandomString(1, 50),
            RandomString(0, 500),
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            knownObservations.Count + 1,
            RandomString(0, 100)
        );

// Send the observation to the server
    var response = await client.PostAsJsonAsync("/observation", obs);
    response.EnsureSuccessStatusCode();


// Read the response and extract the ID of the stored observation
    var result = await response.Content.ReadFromJsonAsync<StoredResponse>();
        if (result == null)
        {
            throw new InvalidOperationException("Server did not return a valid response for the stored observation.");
        }

        obs.Id = result.id;
        knownObservations.Add(obs);
    }


public async Task FuzzPostComment()
    {

    if (knownObservations.Count == 0)
    {
        throw new InvalidOperationException("No known observations to comment on. Please post an observation first.");
    } 
    else 
    {
        var randomObservation = knownObservations[rng.Next(knownObservations.Count)];

        var comment = new Comment(
            //Message, Id
            RandomString(1, 200),
            randomObservation.Id
        );

        var response = await client.PostAsJsonAsync("/comment", comment);
        response.EnsureSuccessStatusCode();

        knownComments.Add(comment);
    }      
}





}

