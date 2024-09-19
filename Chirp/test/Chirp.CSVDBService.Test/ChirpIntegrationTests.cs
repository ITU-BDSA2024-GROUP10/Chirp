using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;

namespace Chirp.CSVDBService.Test;

//From - To is code created in collaboration with ChatGPT
//From
public class ChirpIntegrationTests
{
    [Fact]
    public async Task webWriteCheep_SendsPostRequest()
    {
        // Arrange
        var message = "Hello, this is a test cheep!";
        var author = Environment.UserName;
        var time = DateTime.Now;
        var cheep = new Cheep(author, message, time);

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://bdsa2024group10chirpremotedb.azurewebsites.net/")
        };

        // Act
        await httpClient.PostAsJsonAsync("/cheep", cheep);

        // Assert
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri == new Uri("https://bdsa2024group10chirpremotedb.azurewebsites.net/cheep")),
            ItExpr.IsAny<CancellationToken>()
        );
    }
    
    [Fact]
    public async Task webDisplayCheeps_GetsListOfCheeps()
    {
        // Arrange
        var cheeps = new[]
        {
            new Cheep("user1", "Test Cheep 1", DateTime.Now),
            new Cheep("user2", "Test Cheep 2", DateTime.Now)
        };

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(cheeps)
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://bdsa2024group10chirpremotedb.azurewebsites.net/")
        };

        // Act
        var result = await httpClient.GetFromJsonAsync<Cheep[]>("/cheeps");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Equal("Test Cheep 1", result[0].Message);
        Assert.Equal("Test Cheep 2", result[1].Message);
    }
// To
}
