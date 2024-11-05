﻿using Chirp.Core;
using Chirp.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Web.UnitTests.Utils;

public class RazorWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfiguration = new Dictionary<string, string>
            {
                ["authentication:github:clientId"] = "test-client-id",
                ["authentication:github:clientSecret"] = "test-client-secret"
            };
            config.AddInMemoryCollection(testConfiguration!);
        });
        
        builder.UseEnvironment("Testing");
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