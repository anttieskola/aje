using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AJE.IntegrationTests;

public class HttpClientFixture : WebApplicationFactory<Program>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpClientFixture()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>()
            ?? throw new Exception("https://github.com/dotnet/aspnetcore/issues?q=is%3Aissue+WebApplicationFactory");
    }

    public IHttpClientFactory HttpClientFactory => _httpClientFactory;
}