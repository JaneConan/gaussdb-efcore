﻿using System.Data.Common;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.Query;

public class FromSqlQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture)
    : FromSqlQueryTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>(fixture)
{
    [ConditionalTheory(Skip = "https://github.com/aspnet/EntityFramework/issues/{6563,20364}")]
    public override Task Bad_data_error_handling_invalid_cast(bool async)
        => base.Bad_data_error_handling_invalid_cast(async);

    [ConditionalTheory(Skip = "https://github.com/aspnet/EntityFramework/issues/{6563,20364}")]
    public override Task Bad_data_error_handling_invalid_cast_projection(bool async)
        => base.Bad_data_error_handling_invalid_cast_projection(async);

    // The test attempts to project out a column with the wrong case; this works on other databases, and fails when EF tries to materialize.
    // But in PG this fails at the database since PG is case-sensitive and the column does not exist.
    public override Task FromSqlRaw_queryable_simple_different_cased_columns_and_not_enough_columns_throws(bool async)
        => Assert.ThrowsAsync<ThrowsException>(
            () => base.FromSqlRaw_queryable_simple_different_cased_columns_and_not_enough_columns_throws(async));

    public override async Task FromSqlInterpolated_queryable_multiple_composed_with_parameters_and_closure_parameters_interpolated(
        bool async)
    {
        // We default to mapping DateTime to 'timestamp with time zone', but here we need to send `timestamp without time zone` to match
        // the database data.
        var city = "London";
        var startDate = new GaussDBParameter { Value = new DateTime(1997, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };
        var endDate = new GaussDBParameter { Value = new DateTime(1998, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };

        await using var context = CreateContext();
        var query
            = from c in context.Set<Customer>().FromSqlRaw(
                  NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), city)
              from o in context.Set<Order>().FromSqlInterpolated(
                  NormalizeDelimitersInInterpolatedString(
                      $"SELECT * FROM [Orders] WHERE [OrderDate] BETWEEN {startDate} AND {endDate}"))
              where c.CustomerID == o.CustomerID
              select new { c, o };

        var actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Equal(25, actual.Length);

        city = "Berlin";
        startDate = new GaussDBParameter { Value = new DateTime(1998, 4, 1), GaussDBDbType = GaussDBDbType.Timestamp };
        endDate = new GaussDBParameter { Value = new DateTime(1998, 5, 1), GaussDBDbType = GaussDBDbType.Timestamp };

        query
            = (from c in context.Set<Customer>().FromSqlRaw(
                   NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), city)
               from o in context.Set<Order>().FromSqlInterpolated(
                   NormalizeDelimitersInInterpolatedString(
                       $"SELECT * FROM [Orders] WHERE [OrderDate] BETWEEN {startDate} AND {endDate}"))
               where c.CustomerID == o.CustomerID
               select new { c, o });

        actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Single(actual);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task FromSql_queryable_multiple_composed_with_parameters_and_closure_parameters_interpolated(bool async)
    {
        // We default to mapping DateTime to 'timestamp with time zone', but here we need to send `timestamp without time zone` to match
        // the database data.
        var city = "London";
        var startDate = new GaussDBParameter { Value = new DateTime(1997, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };
        var endDate = new GaussDBParameter { Value = new DateTime(1998, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };

        await using var context = CreateContext();
        var query
            = from c in context.Set<Customer>().FromSqlRaw(
                  NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), city)
              from o in context.Set<Order>().FromSqlInterpolated(
                  NormalizeDelimitersInInterpolatedString(
                      $"SELECT * FROM [Orders] WHERE [OrderDate] BETWEEN {startDate} AND {endDate}"))
              where c.CustomerID == o.CustomerID
              select new { c, o };

        var actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Equal(25, actual.Length);

        city = "Berlin";
        startDate = new GaussDBParameter { Value = new DateTime(1998, 4, 1), GaussDBDbType = GaussDBDbType.Timestamp };
        endDate = new GaussDBParameter { Value = new DateTime(1998, 5, 1), GaussDBDbType = GaussDBDbType.Timestamp };

        query
            = (from c in context.Set<Customer>().FromSqlRaw(
                   NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), city)
               from o in context.Set<Order>().FromSqlInterpolated(
                   NormalizeDelimitersInInterpolatedString(
                       $"SELECT * FROM [Orders] WHERE [OrderDate] BETWEEN {startDate} AND {endDate}"))
               where c.CustomerID == o.CustomerID
               select new { c, o });

        actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Single(actual);
    }

    public override async Task FromSqlRaw_queryable_multiple_composed_with_parameters_and_closure_parameters(bool async)
    {
        // We default to mapping DateTime to 'timestamp with time zone', but here we need to send `timestamp without time zone` to match
        // the database data.
        var city = "London";
        var startDate = new GaussDBParameter { Value = new DateTime(1997, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };
        var endDate = new GaussDBParameter { Value = new DateTime(1998, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };

        await using var context = CreateContext();
        var query = from c in context.Set<Customer>().FromSqlRaw(
                        NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), city)
                    from o in context.Set<Order>().FromSqlRaw(
                        NormalizeDelimitersInRawString("SELECT * FROM [Orders] WHERE [OrderDate] BETWEEN {0} AND {1}"),
                        startDate,
                        endDate)
                    where c.CustomerID == o.CustomerID
                    select new { c, o };

        var actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Equal(25, actual.Length);

        city = "Berlin";
        startDate = new GaussDBParameter { Value = new DateTime(1998, 4, 1), GaussDBDbType = GaussDBDbType.Timestamp };
        endDate = new GaussDBParameter { Value = new DateTime(1998, 5, 1), GaussDBDbType = GaussDBDbType.Timestamp };

        query = (from c in context.Set<Customer>().FromSqlRaw(
                     NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), city)
                 from o in context.Set<Order>().FromSqlRaw(
                     NormalizeDelimitersInRawString("SELECT * FROM [Orders] WHERE [OrderDate] BETWEEN {0} AND {1}"),
                     startDate,
                     endDate)
                 where c.CustomerID == o.CustomerID
                 select new { c, o });

        actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Single(actual);
    }

    public override async Task FromSqlRaw_queryable_multiple_composed_with_closure_parameters(bool async)
    {
        // We default to mapping DateTime to 'timestamp with time zone', but here we need to send `timestamp without time zone` to match
        // the database data.
        var startDate = new GaussDBParameter { Value = new DateTime(1997, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };
        var endDate = new GaussDBParameter { Value = new DateTime(1998, 1, 1), GaussDBDbType = GaussDBDbType.Timestamp };

        await using var context = CreateContext();
        var query = from c in context.Set<Customer>().FromSqlRaw(NormalizeDelimitersInRawString("SELECT * FROM [Customers]"))
                    from o in context.Set<Order>().FromSqlRaw(
                        NormalizeDelimitersInRawString("SELECT * FROM [Orders] WHERE [OrderDate] BETWEEN {0} AND {1}"),
                        startDate,
                        endDate)
                    where c.CustomerID == o.CustomerID
                    select new { c, o };

        var actual = async
            ? await query.ToArrayAsync()
            : query.ToArray();

        Assert.Equal(411, actual.Length);
    }

    protected override DbParameter CreateDbParameter(string name, object value)
        => new GaussDBParameter { ParameterName = name, Value = value };
}
