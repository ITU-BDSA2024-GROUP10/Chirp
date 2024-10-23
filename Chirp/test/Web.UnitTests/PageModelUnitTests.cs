using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Web.UnitTests;

public class PageModelUnitTests
{
    [Fact]
    public void PublicTimeLine()
    {
        // Arrange
        var cheeps = new List<CheepDTO>();

        for (int i = 0; i < 10; i++)
        {
            cheeps.Add(
                new CheepDTO("mr. test",
                    "test",
                    new DateTimeOffset(DateTime.Now.AddHours(i)).ToUnixTimeSeconds()));
        }

        var serviceMock = new Mock<ICheepService>();
        serviceMock.Setup(x => x.GetCheepsByPage(It.IsAny<int>(), It.IsAny<int>())).Returns(cheeps);

        var pageModel = new PublicModel(serviceMock.Object);

        // Act
        var result = pageModel.OnGet(1);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(10, pageModel.Cheeps.Count);
        Assert.Equal(cheeps, pageModel.Cheeps);
    }
}