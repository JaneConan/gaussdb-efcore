﻿namespace Microsoft.EntityFrameworkCore;

public class PropertyValuesGaussDBTest(PropertyValuesGaussDBTest.PropertyValuesGaussDBFixture fixture)
    : PropertyValuesTestBase<PropertyValuesGaussDBTest.PropertyValuesGaussDBFixture>(fixture)
{
    public class PropertyValuesGaussDBFixture : PropertyValuesFixtureBase
    {
        protected override string StoreName { get; } = "PropertyValues";

        protected override ITestStoreFactory TestStoreFactory
            => GaussDBTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // We default to mapping DateTime to 'timestamp with time zone', but the seeding data has Unspecified DateTimes which aren't
            // supported.
            modelBuilder.Entity<PastEmployee>().Property(e => e.TerminationDate).HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Building>()
                .Property(b => b.Value).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CurrentEmployee>()
                .Property(ce => ce.LeaveBalance).HasColumnType("decimal(18,2)");
        }
    }
}
