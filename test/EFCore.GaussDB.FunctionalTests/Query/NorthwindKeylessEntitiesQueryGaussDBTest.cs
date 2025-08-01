namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindKeylessEntitiesQueryGaussDBTest : NorthwindKeylessEntitiesQueryRelationalTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindKeylessEntitiesQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    public override async Task KeylessEntity_with_nav_defining_query(bool async)
    {
        // FromSql mapping. Issue #21627.
        await Assert.ThrowsAsync<PostgresException>(() => base.KeylessEntity_with_nav_defining_query(async));

        AssertSql(
            """
SELECT c."CompanyName", c."OrderCount", c."SearchTerm"
FROM "CustomerQueryWithQueryFilter" AS c
WHERE c."OrderCount" > 0
""");
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
