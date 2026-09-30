using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleDB;
using Xunit.Sdk;
namespace end_to_end.Tests;

/// <summary>
/// This test sets up an in-memory instance of the API (via WebApplicationFactory)
/// and repeatedly sends randomly generated observations and comments to it over HTTP
/// Keeps track of what we sent (our "oracle") and checks
// it against what the server reports back at the end.
// Comments always use a real observation ID so they're not just rejected as invalid.
/// </summary>

public class FuzzTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    // Test oracle: keeps track of what was actually stored, so we can
    // compare it against what the server reports later
    private readonly List<Observation> knownObservations = new();
    private readonly List<Comment> knownComments = new();
    private readonly Random rng = new();

    // Matches the JSON shape returned by POST /observation: { status, id }
    record StoredResponse(string status, int id);

    public FuzzTests(WebApplicationFactory<Program> factory)
    {
        // Starts an in-memory instance of the API so we don't need
        // to run it manually in a separate terminal
        client = factory.CreateClient();
    }

[Fact]
public async Task FuzzTest()
{
    // Randomly post observations and comments 100 times.
    // Always post an observation first if none exist yet,
    // since a comment needs a valid observation ID to reference.
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
    
    // Verify the server's observations match what we sent
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
        
// Generates a random string of a random length between minLen and maxLen
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


// Generates a random observation and posts it to the server.
// Reads the server-assigned ID from the response and adds it to our oracle.
private async Task FuzzPostObservation()
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

// Generates a random comment, always referencing a real, known observation ID,
// so we don't trigger the "fuzz blocker" of only sending invalid IDs.
private async Task FuzzPostComment()
    {

    if (knownObservations.Count == 0)
    {
        throw new InvalidOperationException("No known observations to comment on. Please post an observation first.");
    } 
    else 
    {
        var randomObservation = knownObservations[rng.Next(knownObservations.Count)];

        var comment = new Comment(
            RandomString(1, 200), //message
            randomObservation.Id // Reference to a valid observation
        );

        var response = await client.PostAsJsonAsync("/comment", comment);
        response.EnsureSuccessStatusCode();

        knownComments.Add(comment);
    }      
}





}

