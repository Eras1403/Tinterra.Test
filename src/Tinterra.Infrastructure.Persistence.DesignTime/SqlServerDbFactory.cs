using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Tinterra.Infrastructure.Persistence.SqlServer;

namespace Tinterra.Infrastructure.Persistence.DesignTime;

public class SqlServerDbFactory : IDesignTimeDbContextFactory<SqlServerDb>
{
    public SqlServerDb CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlServerDb>();
        var connectionString = Environment.GetEnvironmentVariable("TINTERRA_SQL_CONNECTION")
            ?? "Server=localhost;Database=Tinterra.Test;Trusted_Connection=True;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(SqlServerDbFactory).Assembly.FullName);
        });

        return new SqlServerDb(optionsBuilder.Options);
    }
}
