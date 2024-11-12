using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PlaywrightTests.Utils;

public static class CreateHostUsingKestrel
{
    public static IHost CreateHost(IHostBuilder builder, IHost testHost, string baseUrl)
    {
        // configure and start the actual host using Kestrel.
        // Configure Kestrel with specific URLs
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel()
                .UseUrls(baseUrl); // Use the baseUrl provided
        });

        var host = builder.Build();
        host.Start();
        
        // Wait until the server is listening
        WaitUntilServerIsAvailable(baseUrl);
        
        // In order to cleanup and properly dispose HTTP server
        // resources we return a composite host object that is
        // actually just a way to intercept the StopAsync and Dispose
        // call and relay to our HTTP host.
        return new CompositeHost(testHost, host);
    }
    
    private static void WaitUntilServerIsAvailable(string url)
    {
        var uri = new Uri(url);
        using var client = new TcpClient();

        var maxAttempts = 5;
        var attempt = 0;

        while (attempt < maxAttempts)
        {
            try
            {
                client.Connect(uri.Host, uri.Port);
                if (client.Connected)
                {
                    break;
                }
            }
            catch
            {
                attempt++;
                Thread.Sleep(1000);
            }
        }

        if (attempt == maxAttempts)
        {
            throw new Exception("Server did not start in time.");
        }
    }
}