using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace LABOFMUTANTAPI.Context;
public sealed class AplicacionBdContextFactory : IDesignTimeDbContextFactory<AplicacionBdContext>
{
    public AplicacionBdContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true).AddEnvironmentVariables().Build();
        var connection = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configura ConnectionStrings:DefaultConnection.");
        return new AplicacionBdContext(new DbContextOptionsBuilder<AplicacionBdContext>().UseSqlServer(connection).Options);
    }
}
