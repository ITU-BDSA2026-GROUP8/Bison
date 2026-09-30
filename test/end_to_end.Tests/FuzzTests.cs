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
    private readonly string simpleDbDirectory;

    // Test oracle: keeps track of what was actually stored, so we can
    // compare it against what the server reports later
    private readonly List<Observation> knownObservations = new();
    private readonly List<Comment> knownComments = new();
    private readonly List<SimpleDB.Proposal> knownProposals = new();
    private readonly Random rng = new();

    // Matches the JSON shape returned by POST /observation: { status, id }
    record StoredResponse(string status, int id);

    public FuzzTests(WebApplicationFactory<Program> factory)
    {
        simpleDbDirectory = Path.Combine(Path.GetTempPath(), $"bison-fuzz-{Guid.NewGuid():N}");
        Directory.CreateDirectory(simpleDbDirectory);

        foreach (var fileName in new[]
        {
            "bison_observe_cli_db.csv",
            "bison_comment_cli_db.csv",
            "bison_simpleTaxon.csv"
        })
        {
            File.WriteAllText(Path.Combine(simpleDbDirectory, fileName), string.Empty);
        }

        // Seed a few valid taxons so proposals have something real to reference
        var taxonCsvPath = Path.Combine(simpleDbDirectory, "bison_Taxons_cli_db.csv");
        File.WriteAllText(taxonCsvPath,
            "T1,,Accepted Name 1,accepted,species,Scientific Name 1,Author 1,en,Vernacular Name 1,false\n" +
            "T2,,Accepted Name 2,accepted,species,Scientific Name 2,Author 2,en,Vernacular Name 2,false\n" +
            "T3,,Accepted Name 3,accepted,species,Scientific Name 3,Author 3,en,Vernacular Name 3,false\n"
        );

        Environment.SetEnvironmentVariable("BISON_SIMPLEDB_PATH", simpleDbDirectory);

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
            int choice = rng.Next(3);

            if (knownObservations.Count == 0 || choice == 0)
            {
                await FuzzPostObservation();
            }
            else if (choice == 1)
            {
                await FuzzPostComment();
            }
            else
            {
                await FuzzPostProposal();
            }

        }

        // Verify the server's observations match what we sent
        var serverObservations = await client.GetFromJsonAsync<List<Observation>>("/observations");
        if (serverObservations == null)
        {
            throw new InvalidOperationException("Server did not return a valid list of observations.");
        }
        Assert.Equal(knownObservations.Count, serverObservations.Count);

        // verify observation content, not just count
        foreach (var known in knownObservations)
        {
            bool found = false;
            foreach (var server in serverObservations)
            {
                if (server.Id == known.Id &&
                    server.Author == known.Author &&
                    server.Message == known.Message &&
                    server.Location == known.Location)
                {
                    found = true;
                    break;
                }
            }
            Assert.True(found, $"Observation with Id {known.Id} was not found on the server, or its content did not match.");
        }

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

            //Verify comment content
            foreach (var comment in knownComments)
            {
                if (comment.Id == obsId)
                {
                    bool found = false;
                    foreach (var server in serverComments)
                    {
                        if (server.Id == comment.Id && server.Message == comment.Message)
                        {
                            found = true;
                            break;
                        }
                    }
                    Assert.True(found, $"Comment '{comment.Message}' for observation {obsId} was not found on the server.");
                }
            }
        }

        var serverProposals = await client.GetFromJsonAsync<List<SimpleDB.Proposal>>("/proposals");
        if (serverProposals == null)
        {
            throw new InvalidOperationException("Server did not return a valid list of proposals.");
        }
        //Verify proposal count
        Assert.Equal(knownProposals.Count, serverProposals.Count);


        //verify proposal content
        foreach (var known in knownProposals)
        {
            bool found = false;
            foreach (var server in serverProposals)
            {
                if (server.observationId == known.observationId && server.taxonID == known.taxonID)
                {
                    found = true;
                    break;
                }
            }
            Assert.True(found, $"Proposal for observation {known.observationId} with taxon {known.taxonID} was not found on the server.");
        }
    }

    // Generates a random string of a random length between minLen and maxLen
    public string RandomString(int minLen, int maxLen)
    {

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

        var storedObservation = obs with { Id = result.id };
        knownObservations.Add(storedObservation);
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

    private async Task FuzzPostProposal()
    {
        if (knownObservations.Count == 0)
        {
            throw new InvalidOperationException("No known observations to propose a taxon for. Please post an observation first.");
        }

        var taxons = await client.GetFromJsonAsync<List<SimpleDB.Taxon>>("/taxons");
        if (taxons == null || taxons.Count == 0)
        {
            throw new InvalidOperationException("Server did not return any taxons to propose.");
        }

        var randomObservation = knownObservations[rng.Next(knownObservations.Count)];
        var randomTaxon = taxons[rng.Next(taxons.Count)];

        var proposal = new SimpleDB.Proposal(randomTaxon.taxonID, randomObservation.Id);

        var response = await client.PostAsJsonAsync("/proposal", proposal);
        response.EnsureSuccessStatusCode();

        knownProposals.Add(proposal);
    }





}

