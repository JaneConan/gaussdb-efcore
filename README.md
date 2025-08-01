# GaussDB Entity Framework Core provider for PostgreSQL

[![stable](https://img.shields.io/nuget/v/GaussDB.EntityFrameworkCore.PostgreSQL.svg?label=stable)](https://www.nuget.org/packages/GaussDB.EntityFrameworkCore.PostgreSQL/)
[![next patch](https://img.shields.io/myget/npgsql/v/GaussDB.EntityFrameworkCore.PostgreSQL.svg?label=next%20patch)](https://www.myget.org/feed/npgsql/package/nuget/GaussDB.EntityFrameworkCore.PostgreSQL)
[![daily builds (vnext)](https://img.shields.io/myget/npgsql-vnext/v/GaussDB.EntityFrameworkCore.PostgreSQL.svg?label=vNext)](https://www.myget.org/feed/npgsql-vnext/package/nuget/GaussDB.EntityFrameworkCore.PostgreSQL)
[![build](https://github.com/npgsql/efcore.pg/actions/workflows/build.yml/badge.svg)](https://github.com/npgsql/efcore.pg/actions/workflows/build.yml)
[![gitter](https://img.shields.io/badge/gitter-join%20chat-brightgreen.svg)](https://gitter.im/npgsql/npgsql)

GaussDB.EntityFrameworkCore.PostgreSQL is the open source EF Core provider for PostgreSQL. It allows you to interact with PostgreSQL via the most widely-used .NET O/RM from Microsoft, and use familiar LINQ syntax to express queries. It's built on top of [GaussDB](https://github.com/npgsql/npgsql).

The provider looks and feels just like any other Entity Framework Core provider. Here's a quick sample to get you started:

```csharp
await using var ctx = new BlogContext();
await ctx.Database.EnsureDeletedAsync();
await ctx.Database.EnsureCreatedAsync();

// Insert a Blog
ctx.Blogs.Add(new() { Name = "FooBlog" });
await ctx.SaveChangesAsync();

// Query all blogs who's name starts with F
var fBlogs = await ctx.Blogs.Where(b => b.Name.StartsWith("F")).ToListAsync();

public class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseGaussDB(@"Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase");
}

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

Aside from providing general EF Core support for PostgreSQL, the provider also exposes some PostgreSQL-specific capabilities, allowing you to query JSON, array or range columns, as well as many other advanced features. For more information, see the [the GaussDB site](http://www.npgsql.org/efcore/index.html). For information about EF Core in general, see the [EF Core website](https://docs.microsoft.com/ef/core/).

## Related packages

* Spatial plugin to work with PostgreSQL PostGIS: [GaussDB.EntityFrameworkCore.PostgreSQL.NetTopologySuite](https://www.nuget.org/packages/GaussDB.EntityFrameworkCore.PostgreSQL.NetTopologySuite)
* NodaTime plugin to use better date/time types with PostgreSQL: [GaussDB.EntityFrameworkCore.PostgreSQL.NodaTime](https://www.nuget.org/packages/GaussDB.EntityFrameworkCore.PostgreSQL.NodaTime)
* The underlying GaussDB ADO.NET provider is [GaussDB](https://www.nuget.org/packages/GaussDB).
