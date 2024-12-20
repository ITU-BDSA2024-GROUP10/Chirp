﻿using Chirp.Core;
using Chirp.Core.DTO;
using Moq;
using Web.UnitTests.Utils;

namespace Web.UnitTests;

public class RazorPageIntegrationTest(RazorWebApplicationFactory factory)
    : IClassFixture<RazorWebApplicationFactory>
{
    [Fact]
    public async void DisplayCheeps_On_PublicTimeline()
    {
        // Arrange
        var cheeps = new List<CheepDTO>();

        for (int i = 0; i < 10; i++)
        {
            cheeps.Add(
                new CheepDTO(
                    null,
                    "mr. " + i + "test",
                    "test: " + i + "test!",
                    new DateTimeOffset(DateTime.Now.AddHours(i)).ToUnixTimeSeconds()));
        }

        var serviceMock = new Mock<ICheepService>();
        serviceMock.Setup(service => service.GetCheepsByPage(It.IsAny<int>(), It.IsAny<int>())).Returns(cheeps);

        var client = factory.GetClientFromCheepServiceMock(serviceMock);

        // Act
        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);

        foreach (var cheep in cheeps)
        {
            Assert.Contains($"<a href=\"/{cheep.Author}\">{cheep.Author}</a>", content);
            Assert.Contains(cheep.Message, content);
            Assert.Contains(
                DateTimeOffset.FromUnixTimeSeconds(cheep.UnixTimestamp).DateTime.ToString("dd/MM/yy H:mm:ss"),
                content);
        }
    }

    [Fact]
    public async void Dont_Display_Cheeps_Public_Timeline()
    {
        // Arrange
        var serviceMock = new Mock<ICheepService>();
        serviceMock.Setup(service => service.GetCheepsByPage(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new List<CheepDTO>());

        var client = factory.GetClientFromCheepServiceMock(serviceMock);

        // Act
        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
        Assert.Contains("There are no cheeps so far.", content);
        Assert.DoesNotContain("<li>", content);
    }
}