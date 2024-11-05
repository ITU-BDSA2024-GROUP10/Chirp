using System.Data.Common;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TestUtils;

namespace Web.UnitTests.Utils;

public class RazorWebApplicationFactory<TProgram>
    : InMemoryCostumeWebApplicationFactory<TProgram>
    where TProgram : class
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