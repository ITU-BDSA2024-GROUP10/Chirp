using Chirp.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TestUtilities;

namespace Web.UnitTests.Utils;

public class RazorWebApplicationFactory : InMemoryCostumeWebApplicationFactory
{
    public HttpClient GetClientFromCheepServiceMock(Mock<ICheepService> serviceMock)
    {
        return WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var serviceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(ICheepService));

                if (serviceDescriptor != null) services.Remove(serviceDescriptor);
                services.AddScoped<ICheepService>(serviceProvider => serviceMock.Object);
            });
        }).CreateClient();
    }
}