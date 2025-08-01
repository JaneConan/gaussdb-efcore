﻿namespace Microsoft.EntityFrameworkCore.Query;

public class IncludeGaussDBFixture : NorthwindQueryGaussDBFixture<NoopModelCustomizer>
{
    protected override bool ShouldLogCategory(string logCategory)
        => base.ShouldLogCategory(logCategory) || logCategory == DbLoggerCategory.Query.Name;
}
