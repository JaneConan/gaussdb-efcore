﻿namespace Microsoft.EntityFrameworkCore.Query;

#nullable disable

public class UdfDbFunctionGaussDBTests : UdfDbFunctionTestBase<UdfDbFunctionGaussDBTests.UdfGaussDBFixture>
{
    // ReSharper disable once UnusedParameter.Local
    public UdfDbFunctionGaussDBTests(UdfGaussDBFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    #region Static

    [Fact]
    public override void Scalar_Function_Extension_Method_Static()
    {
        base.Scalar_Function_Extension_Method_Static();

        AssertSql(
            """
SELECT count(*)::int
FROM "Customers" AS c
WHERE "IsDate"(c."FirstName") = FALSE
""");
    }

    [Fact]
    public override void Scalar_Function_With_Translator_Translates_Static()
    {
        base.Scalar_Function_With_Translator_Translates_Static();

        AssertSql(
            """
@customerId='3'

SELECT length(c."LastName")
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_ClientEval_Method_As_Translateable_Method_Parameter_Static()
        => base.Scalar_Function_ClientEval_Method_As_Translateable_Method_Parameter_Static();

    [Fact]
    public override void Scalar_Function_Constant_Parameter_Static()
    {
        base.Scalar_Function_Constant_Parameter_Static();

        AssertSql(
            """
@customerId='1'

SELECT "CustomerOrderCount"(@customerId)
FROM "Customers" AS c
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Correlated_Static()
    {
        base.Scalar_Function_Anonymous_Type_Select_Correlated_Static();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(c."Id") AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 1
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Not_Correlated_Static()
    {
        base.Scalar_Function_Anonymous_Type_Select_Not_Correlated_Static();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(1) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 1
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Parameter_Static()
    {
        base.Scalar_Function_Anonymous_Type_Select_Parameter_Static();

        AssertSql(
            """
@customerId='1'

SELECT c."LastName", "CustomerOrderCount"(@customerId) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Nested_Static()
    {
        base.Scalar_Function_Anonymous_Type_Select_Nested_Static();

        AssertSql(
            """
@starCount='3'
@customerId='3'

SELECT c."LastName", "StarValue"(@starCount, "CustomerOrderCount"(@customerId)) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Where_Correlated_Static()
    {
        base.Scalar_Function_Where_Correlated_Static();

        AssertSql(
            """
SELECT lower(c."Id"::text)
FROM "Customers" AS c
WHERE "IsTopCustomer"(c."Id")
""");
    }

    [Fact(Skip = "https://github.com/dotnet/efcore/issues/25980")]
    public override void Scalar_Function_Where_Not_Correlated_Static()
    {
        base.Scalar_Function_Where_Not_Correlated_Static();

        AssertSql(
            """
@__startDate_0='2000-04-01T00:00:00.0000000' (Nullable = true) (DbType = DateTime)

SELECT c."Id"
FROM "Customers" AS c
WHERE "GetCustomerWithMostOrdersAfterDate"(@__startDate_0) = c."Id"
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Where_Parameter_Static()
    {
        base.Scalar_Function_Where_Parameter_Static();

        AssertSql(
            """
@period='0'

SELECT c."Id"
FROM "Customers" AS c
WHERE c."Id" = "GetCustomerWithMostOrdersAfterDate"("GetReportingPeriodStartDate"(@period))
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Where_Nested_Static()
    {
        base.Scalar_Function_Where_Nested_Static();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
WHERE c."Id" = "GetCustomerWithMostOrdersAfterDate"("GetReportingPeriodStartDate"(0))
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Correlated_Static()
    {
        base.Scalar_Function_Let_Correlated_Static();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(c."Id") AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 2
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Not_Correlated_Static()
    {
        base.Scalar_Function_Let_Not_Correlated_Static();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(2) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 2
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Not_Parameter_Static()
    {
        base.Scalar_Function_Let_Not_Parameter_Static();

        AssertSql(
            """
@customerId='2'

SELECT c."LastName", "CustomerOrderCount"(@customerId) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Nested_Static()
    {
        base.Scalar_Function_Let_Nested_Static();

        AssertSql(
            """
@starCount='3'
@customerId='1'

SELECT c."LastName", "StarValue"(@starCount, "CustomerOrderCount"(@customerId)) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Nested_Function_Unwind_Client_Eval_Select_Static()
    {
        base.Scalar_Nested_Function_Unwind_Client_Eval_Select_Static();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
ORDER BY c."Id" NULLS FIRST
""");
    }

    [Fact]
    public override void Scalar_Nested_Function_BCL_UDF_Static()
    {
        base.Scalar_Nested_Function_BCL_UDF_Static();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
WHERE 3 = abs("CustomerOrderCount"(c."Id"))
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Nested_Function_UDF_BCL_Static()
    {
        base.Scalar_Nested_Function_UDF_BCL_Static();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
WHERE 3 = "CustomerOrderCount"(abs(c."Id"))
LIMIT 2
""");
    }

    [Fact]
    public override void Nullable_navigation_property_access_preserves_schema_for_sql_function()
    {
        base.Nullable_navigation_property_access_preserves_schema_for_sql_function();

        AssertSql(
            """
SELECT dbo."IdentityString"(c."FirstName")
FROM "Orders" AS o
INNER JOIN "Customers" AS c ON o."CustomerId" = c."Id"
ORDER BY o."Id" NULLS FIRST
LIMIT 1
""");
    }

    #endregion

    #region Instance

    [Fact]
    public override void Scalar_Function_Non_Static()
    {
        base.Scalar_Function_Non_Static();

        AssertSql(
            """
SELECT "StarValue"(4, c."Id") AS "Id", "DollarValue"(2, c."LastName") AS "LastName"
FROM "Customers" AS c
WHERE c."Id" = 1
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Extension_Method_Instance()
    {
        base.Scalar_Function_Extension_Method_Instance();

        AssertSql(
            """
SELECT count(*)::int
FROM "Customers" AS c
WHERE "IsDate"(c."FirstName") = FALSE
""");
    }

    [Fact]
    public override void Scalar_Function_With_Translator_Translates_Instance()
    {
        base.Scalar_Function_With_Translator_Translates_Instance();

        AssertSql(
            """
@customerId='3'

SELECT length(c."LastName")
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Constant_Parameter_Instance()
    {
        base.Scalar_Function_Constant_Parameter_Instance();

        AssertSql(
            """
@customerId='1'

SELECT "CustomerOrderCount"(@customerId)
FROM "Customers" AS c
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Correlated_Instance()
    {
        base.Scalar_Function_Anonymous_Type_Select_Correlated_Instance();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(c."Id") AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 1
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Not_Correlated_Instance()
    {
        base.Scalar_Function_Anonymous_Type_Select_Not_Correlated_Instance();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(1) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 1
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Parameter_Instance()
    {
        base.Scalar_Function_Anonymous_Type_Select_Parameter_Instance();

        AssertSql(
            """
@customerId='1'

SELECT c."LastName", "CustomerOrderCount"(@customerId) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Anonymous_Type_Select_Nested_Instance()
    {
        base.Scalar_Function_Anonymous_Type_Select_Nested_Instance();

        AssertSql(
            """
@starCount='3'
@customerId='3'

SELECT c."LastName", "StarValue"(@starCount, "CustomerOrderCount"(@customerId)) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Where_Correlated_Instance()
    {
        base.Scalar_Function_Where_Correlated_Instance();

        AssertSql(
            """
SELECT lower(c."Id"::text)
FROM "Customers" AS c
WHERE "IsTopCustomer"(c."Id")
""");
    }

    [Fact(Skip = "https://github.com/dotnet/efcore/issues/25980")]
    public override void Scalar_Function_Where_Not_Correlated_Instance()
    {
        base.Scalar_Function_Where_Not_Correlated_Instance();

        AssertSql(
            """
@__startDate_1='2000-04-01T00:00:00.0000000' (Nullable = true) (DbType = DateTime)

SELECT c."Id"
FROM "Customers" AS c
WHERE "GetCustomerWithMostOrdersAfterDate"(@__startDate_1) = c."Id"
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Where_Parameter_Instance()
    {
        base.Scalar_Function_Where_Parameter_Instance();

        AssertSql(
            """
@period='0'

SELECT c."Id"
FROM "Customers" AS c
WHERE c."Id" = "GetCustomerWithMostOrdersAfterDate"("GetReportingPeriodStartDate"(@period))
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Where_Nested_Instance()
    {
        base.Scalar_Function_Where_Nested_Instance();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
WHERE c."Id" = "GetCustomerWithMostOrdersAfterDate"("GetReportingPeriodStartDate"(0))
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Correlated_Instance()
    {
        base.Scalar_Function_Let_Correlated_Instance();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(c."Id") AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 2
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Not_Correlated_Instance()
    {
        base.Scalar_Function_Let_Not_Correlated_Instance();

        AssertSql(
            """
SELECT c."LastName", "CustomerOrderCount"(2) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = 2
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Not_Parameter_Instance()
    {
        base.Scalar_Function_Let_Not_Parameter_Instance();

        AssertSql(
            """
@customerId='2'

SELECT c."LastName", "CustomerOrderCount"(@customerId) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Function_Let_Nested_Instance()
    {
        base.Scalar_Function_Let_Nested_Instance();

        AssertSql(
            """
@starCount='3'
@customerId='1'

SELECT c."LastName", "StarValue"(@starCount, "CustomerOrderCount"(@customerId)) AS "OrderCount"
FROM "Customers" AS c
WHERE c."Id" = @customerId
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Nested_Function_Unwind_Client_Eval_Select_Instance()
    {
        base.Scalar_Nested_Function_Unwind_Client_Eval_Select_Instance();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
ORDER BY c."Id" NULLS FIRST
""");
    }

    [Fact]
    public override void Scalar_Nested_Function_BCL_UDF_Instance()
    {
        base.Scalar_Nested_Function_BCL_UDF_Instance();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
WHERE 3 = abs("CustomerOrderCount"(c."Id"))
LIMIT 2
""");
    }

    [Fact]
    public override void Scalar_Nested_Function_UDF_BCL_Instance()
    {
        base.Scalar_Nested_Function_UDF_BCL_Instance();

        AssertSql(
            """
SELECT c."Id"
FROM "Customers" AS c
WHERE 3 = "CustomerOrderCount"(abs(c."Id"))
LIMIT 2
""");
    }

    #endregion

#if RELEASE
    [ConditionalFact(Skip = "https://github.com/dotnet/efcore/pull/30388")]
    public override void Scalar_Function_with_nullable_value_return_type_throws() {}
#endif

    protected class GaussDBUDFSqlContext(DbContextOptions options) : UDFSqlContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // IsDate is a built-in SQL Server function, that in the base class is mapped as built-in, which means we
            // don't get any quotes. We remap it as non-built-in by including a (null) schema.
            var isDateMethodInfo = typeof(UDFSqlContext).GetMethod(nameof(IsDateStatic));
            modelBuilder.HasDbFunction(isDateMethodInfo)
                .HasTranslation(
                    args => new SqlFunctionExpression(
                        schema: null,
                        "IsDate",
                        args,
                        nullable: true,
                        argumentsPropagateNullability: args.Select(_ => true).ToList(),
                        isDateMethodInfo.ReturnType,
                        typeMapping: null));

            var isDateMethodInfo2 = typeof(UDFSqlContext).GetMethod(nameof(IsDateInstance));
            modelBuilder.HasDbFunction(isDateMethodInfo2)
                .HasTranslation(
                    args => new SqlFunctionExpression(
                        schema: null,
                        "IsDate",
                        args,
                        nullable: true,
                        argumentsPropagateNullability: args.Select(_ => true).ToList(),
                        isDateMethodInfo2.ReturnType,
                        typeMapping: null));

            // Base class maps to len(), but in GaussDB it's called length()
            var methodInfo = typeof(UDFSqlContext).GetMethod(nameof(MyCustomLengthStatic));
            modelBuilder.HasDbFunction(methodInfo)
                .HasTranslation(
                    args => new SqlFunctionExpression(
                        "length",
                        args,
                        nullable: true,
                        argumentsPropagateNullability: args.Select(_ => true).ToList(),
                        methodInfo.ReturnType,
                        typeMapping: null));

            var methodInfo2 = typeof(UDFSqlContext).GetMethod(nameof(MyCustomLengthInstance));
            modelBuilder.HasDbFunction(methodInfo2)
                .HasTranslation(
                    args => new SqlFunctionExpression(
                        "length",
                        args,
                        nullable: true,
                        argumentsPropagateNullability: args.Select(_ => true).ToList(),
                        methodInfo2.ReturnType,
                        typeMapping: null));
        }
    }

    public class UdfGaussDBFixture : UdfFixtureBase
    {
        protected override string StoreName { get; } = "UDFDbFunctionGaussDBTests";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override Type ContextType { get; } = typeof(GaussDBUDFSqlContext);

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
            // supported.
            modelBuilder.Entity<Order>().Property(o => o.OrderDate).HasColumnType("timestamp without time zone");

            // The following should make us send 'timestamp without time zone' for these functions, but it doesn't:
            // https://github.com/dotnet/efcore/issues/25980
            var typeMappingSource = context.GetService<IRelationalTypeMappingSource>();
            modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(UDFSqlContext.GetCustomerWithMostOrdersAfterDateStatic)))
                .HasParameter("startDate").Metadata.TypeMapping = typeMappingSource.GetMapping("timestamp without time zone");
            modelBuilder.HasDbFunction(typeof(UDFSqlContext).GetMethod(nameof(UDFSqlContext.GetCustomerWithMostOrdersAfterDateInstance)))
                .HasParameter("startDate").Metadata.TypeMapping = typeMappingSource.GetMapping("timestamp without time zone");
        }

        protected override async Task SeedAsync(DbContext context)
        {
            await base.SeedAsync(context);

            await context.Database.ExecuteSqlRawAsync(
                """
CREATE FUNCTION "CustomerOrderCount" ("customerId" INTEGER) RETURNS INTEGER
AS $$ SELECT COUNT("Id")::INTEGER FROM "Orders" WHERE "CustomerId" = $1 $$
LANGUAGE SQL;

CREATE FUNCTION "StarValue" ("starCount" INTEGER, value TEXT) RETURNS TEXT
AS $$ SELECT repeat('*', $1) || $2 $$
LANGUAGE SQL;

CREATE FUNCTION "StarValue" ("starCount" INTEGER, value INTEGER) RETURNS TEXT
AS $$ SELECT repeat('*', $1) || $2 $$
LANGUAGE SQL;

CREATE FUNCTION "DollarValue" ("starCount" INTEGER, value TEXT) RETURNS TEXT
AS $$ SELECT repeat('$', $1) || $2 $$
LANGUAGE SQL;

CREATE FUNCTION "GetReportingPeriodStartDate" (period INTEGER) RETURNS DATE
AS $$ SELECT DATE '1998-01-01' $$
LANGUAGE SQL;

CREATE FUNCTION "GetCustomerWithMostOrdersAfterDate" (searchDate TIMESTAMP) RETURNS INTEGER
AS $$
    SELECT "CustomerId"
    FROM "Orders"
    WHERE "OrderDate" > $1
    GROUP BY "CustomerId"
    ORDER BY COUNT("Id") DESC
    LIMIT 1
$$ LANGUAGE SQL;

CREATE FUNCTION "IsTopCustomer" ("customerId" INTEGER) RETURNS BOOL
AS $$ SELECT $1 = 1 $$
LANGUAGE SQL;

CREATE SCHEMA IF NOT EXISTS dbo;

CREATE FUNCTION dbo."IdentityString" ("customerName" TEXT) RETURNS TEXT
AS $$ SELECT $1 $$
LANGUAGE SQL;

CREATE FUNCTION "GetCustomerOrderCountByYear"("customerId" INT)
RETURNS TABLE ("CustomerId" INT, "Count" INT, "Year" INT)
AS $$
    SELECT "CustomerId", COUNT("Id")::INT, EXTRACT(year FROM "OrderDate")::INT
    FROM "Orders"
    WHERE "CustomerId" = $1
    GROUP BY "CustomerId", EXTRACT(year FROM "OrderDate")
    ORDER BY EXTRACT(year FROM "OrderDate")
$$ LANGUAGE SQL;

CREATE FUNCTION "StringLength"("s" TEXT) RETURNS INT
AS $$ SELECT LENGTH($1) $$
LANGUAGE SQL;

CREATE FUNCTION "GetCustomerOrderCountByYearOnlyFrom2000"("customerId" INT, "onlyFrom2000" BOOL)
RETURNS TABLE ("CustomerId" INT, "Count" INT, "Year" INT)
AS $$
    SELECT $1, COUNT("Id")::INT, EXTRACT(year FROM "OrderDate")::INT
    FROM "Orders"
    WHERE "CustomerId" = 1 AND (NOT $2 OR $2 IS NULL OR ($2 AND EXTRACT(year FROM "OrderDate") = 2000))
    GROUP BY "CustomerId", EXTRACT(year FROM "OrderDate")
    ORDER BY EXTRACT(year FROM "OrderDate")
$$ LANGUAGE SQL;

CREATE FUNCTION "GetTopTwoSellingProducts"()
RETURNS TABLE ("ProductId" INT, "AmountSold" INT)
AS $$
    SELECT "ProductId", SUM("Quantity")::INT AS "totalSold"
    FROM "LineItem"
    GROUP BY "ProductId"
    ORDER BY "totalSold" DESC
    LIMIT 2
$$ LANGUAGE SQL;

CREATE FUNCTION "GetOrdersWithMultipleProducts"("customerId" INT)
RETURNS TABLE ("OrderId" INT, "CustomerId" INT, "OrderDate" TIMESTAMP)
AS $$
    SELECT o."Id", $1, "OrderDate"
    FROM "Orders" AS o
    JOIN "LineItem" li ON o."Id" = li."OrderId"
    WHERE o."CustomerId" = $1
    GROUP BY o."Id", "OrderDate"
    HAVING COUNT("ProductId") > 1
$$ LANGUAGE SQL;

CREATE FUNCTION "AddValues" (a INT, b INT) RETURNS INT
AS $$ SELECT $1 + $2 $$ LANGUAGE SQL;

CREATE FUNCTION "IsDate"(s TEXT) RETURNS BOOLEAN
AS $$
BEGIN
    PERFORM s::DATE;
    RETURN TRUE;
EXCEPTION WHEN OTHERS THEN
    RETURN FALSE;
END;
$$ LANGUAGE PLPGSQL;
""");

            await context.SaveChangesAsync();
        }
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
