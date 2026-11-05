using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Bie.IntegrationTests;

public sealed class BieFactory : WebApplicationFactory<Program>
{
    private readonly string folder = Path.Combine(Path.GetTempPath(), "bie-integration-" + Guid.NewGuid());
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?> { ["DatabasePath"] = Path.Combine(folder, "test.db") }));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && Directory.Exists(folder))
            Directory.Delete(folder, true);
    }
}
