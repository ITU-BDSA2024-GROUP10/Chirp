using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Web.UnitTests.Utils;

public class RazorWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }

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