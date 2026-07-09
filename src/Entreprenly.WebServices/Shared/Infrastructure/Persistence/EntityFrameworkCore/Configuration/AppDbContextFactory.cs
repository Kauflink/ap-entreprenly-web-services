using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Design-time factory used by EF Core tooling (e.g. <c>dotnet ef migrations</c>) so it can
///     build the context without running the application. Scaffolding does not open a connection,
///     so a local fallback connection string is sufficient when no configuration is available.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string LocalFallbackConnectionString =
        "server=localhost;port=3306;database=entreprenly;user=root;password=root";

    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var template = configuration.GetConnectionString("DefaultConnection");
        var connectionString = string.IsNullOrWhiteSpace(template)
            ? LocalFallbackConnectionString
            : Environment.ExpandEnvironmentVariables(template);

        // If the template still contains unresolved %PLACEHOLDERS%, use the local fallback.
        if (connectionString.Contains('%'))
            connectionString = LocalFallbackConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 0)));
        return new AppDbContext(optionsBuilder.Options);
    }
}
