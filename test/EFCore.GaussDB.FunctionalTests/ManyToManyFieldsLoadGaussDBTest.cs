using Microsoft.EntityFrameworkCore.TestModels.ManyToManyFieldsModel;

namespace Microsoft.EntityFrameworkCore;

public class ManyToManyFieldsLoadGaussDBTest(ManyToManyFieldsLoadGaussDBTest.ManyToManyFieldsLoadGaussDBFixture fixture)
    : ManyToManyFieldsLoadTestBase<ManyToManyFieldsLoadGaussDBTest.ManyToManyFieldsLoadGaussDBFixture>(fixture)
{
    public class ManyToManyFieldsLoadGaussDBFixture : ManyToManyFieldsLoadFixtureBase, ITestSqlLoggerFactory
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
            // supported.
            modelBuilder.Entity<EntityCompositeKey>()
                .Property(e => e.Key3)
                .HasColumnType("timestamp without time zone");

            modelBuilder
                .Entity<JoinOneSelfPayload>()
                .Property(e => e.Payload)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder
                .SharedTypeEntity<Dictionary<string, object>>("JoinOneToThreePayloadFullShared")
                .IndexerProperty<string>("Payload")
                .HasDefaultValue("Generated");

            modelBuilder
                .Entity<JoinOneToThreePayloadFull>()
                .Property(e => e.Payload)
                .HasDefaultValue("Generated");
        }
    }
}
