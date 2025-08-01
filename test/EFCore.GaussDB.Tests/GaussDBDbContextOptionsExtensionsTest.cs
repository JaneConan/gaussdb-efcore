using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class GaussDBDbContextOptionsExtensionsTest
{
    [ConditionalFact]
    public void Can_add_extension_with_max_batch_size()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseGaussDB("Database=Crunchie", b => b.MaxBatchSize(123));

        var extension = optionsBuilder.Options.Extensions.OfType<GaussDBOptionsExtension>().Single();

        Assert.Equal(123, extension.MaxBatchSize);
    }

    [ConditionalFact]
    public void Can_add_extension_with_command_timeout()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseGaussDB("Database=Crunchie", b => b.CommandTimeout(30));

        var extension = optionsBuilder.Options.Extensions.OfType<GaussDBOptionsExtension>().Single();

        Assert.Equal(30, extension.CommandTimeout);
    }

    [ConditionalFact]
    public void Can_add_extension_with_connection_string()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseGaussDB("Database=Crunchie");

        var extension = optionsBuilder.Options.Extensions.OfType<GaussDBOptionsExtension>().Single();

        Assert.Equal("Database=Crunchie", extension.ConnectionString);
        Assert.Null(extension.Connection);
    }

    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void Can_add_extension_with_connection_string_using_generic_options(bool nullConnectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
        optionsBuilder.UseGaussDB(nullConnectionString ? null : "Database=Whisper");

        var extension = optionsBuilder.Options.Extensions.OfType<GaussDBOptionsExtension>().Single();

        Assert.Equal(nullConnectionString ? null : "Database=Whisper", extension.ConnectionString);
        Assert.Null(extension.Connection);
    }

    [ConditionalFact]
    public void Can_add_extension_with_connection()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        var connection = new GaussDBConnection();

        optionsBuilder.UseGaussDB(connection);

        var extension = optionsBuilder.Options.Extensions.OfType<GaussDBOptionsExtension>().Single();

        Assert.Same(connection, extension.Connection);
        Assert.Null(extension.ConnectionString);
    }

    [ConditionalFact]
    public void Can_add_extension_with_connection_using_generic_options()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
        var connection = new GaussDBConnection();

        optionsBuilder.UseGaussDB(connection);

        var extension = optionsBuilder.Options.Extensions.OfType<GaussDBOptionsExtension>().Single();

        Assert.Same(connection, extension.Connection);
        Assert.Null(extension.ConnectionString);
    }

    [ConditionalTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void Service_collection_extension_method_can_configure_npgsql_options(bool nullConnectionString)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddGaussDB<ApplicationDbContext>(
            nullConnectionString ? null : "Database=Crunchie",
            npgsqlOption =>
            {
                npgsqlOption.MaxBatchSize(123);
                npgsqlOption.CommandTimeout(30);
            },
            dbContextOption =>
            {
                dbContextOption.EnableDetailedErrors();
            });

        var services = serviceCollection.BuildServiceProvider();

        using (var serviceScope = services
                   .GetRequiredService<IServiceScopeFactory>()
                   .CreateScope())
        {
            var coreOptions = serviceScope.ServiceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()
                .GetExtension<CoreOptionsExtension>();
            Assert.True(coreOptions.DetailedErrorsEnabled);

            var npgsqlOptions = serviceScope.ServiceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()
                .GetExtension<GaussDBOptionsExtension>();
            Assert.Equal(123, npgsqlOptions.MaxBatchSize);
            Assert.Equal(30, npgsqlOptions.CommandTimeout);
            Assert.Equal(nullConnectionString ? null : "Database=Crunchie", npgsqlOptions.ConnectionString);
        }
    }

    [ConditionalFact]
    public void Varying_data_source_connection_strings_do_not_cause_multiple_service_providers()
    {
        for (var i = 0; i < 21; i++)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
            var dataSource = GaussDBDataSource.Create($"Host=localhost;Database={i};Include Error Detail=true;Username=admin;Password=admin");
            optionsBuilder.UseGaussDB(dataSource);

            using var context = new ApplicationDbContext(optionsBuilder.Options);
            _ = context.Model;
        }
    }

    private class ApplicationDbContext(DbContextOptions options) : DbContext(options);
}
