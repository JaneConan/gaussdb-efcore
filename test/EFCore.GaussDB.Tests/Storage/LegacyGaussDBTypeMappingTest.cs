#if DEBUG

using Microsoft.EntityFrameworkCore.Storage.Json;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage;

[Collection("LegacyDateTimeTest")]
public class LegacyGaussDBTypeMappingTest : IClassFixture<LegacyGaussDBTypeMappingTest.LegacyGaussDBTypeMappingFixture>
{
    [Fact]
    public void DateTime_type_maps_to_timestamp_by_default()
        => Assert.Equal("timestamp without time zone", GetMapping(typeof(DateTime)).StoreType);

    [Fact]
    public void Timestamp_maps_to_DateTime_by_default()
        => Assert.Same(typeof(DateTime), GetMapping("timestamp without time zone").ClrType);

    [Fact]
    public void Timestamptz_maps_to_DateTime_by_default()
        => Assert.Same(typeof(DateTime), GetMapping("timestamp with time zone").ClrType);

    [Fact]
    public void GenerateSqlLiteral_returns_timestamptz_datetime_literal()
    {
        var mapping = GetMapping("timestamptz");
        Assert.Equal(
            "TIMESTAMPTZ '1997-12-17T07:37:16Z'",
            mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Utc)));
        Assert.Equal(
            "TIMESTAMPTZ '1997-12-17T07:37:16Z'",
            mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Unspecified)));

        var offset = TimeZoneInfo.Local.BaseUtcOffset;
        var offsetStr = (offset < TimeSpan.Zero ? '-' : '+') + offset.ToString(@"hh\:mm");
        Assert.StartsWith(
            $"TIMESTAMPTZ '1997-12-17T07:37:16{offsetStr}",
            mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, DateTimeKind.Local)));

        Assert.Equal(
            "TIMESTAMPTZ '1997-12-17T07:37:16.345678Z'",
            mapping.GenerateSqlLiteral(new DateTime(1997, 12, 17, 7, 37, 16, 345, DateTimeKind.Utc).AddTicks(6780)));
    }

    #region Support

    private static readonly GaussDBTypeMappingSource Mapper = new(
        new TypeMappingSourceDependencies(
            new ValueConverterSelector(new ValueConverterSelectorDependencies()),
            new JsonValueReaderWriterSource(new JsonValueReaderWriterSourceDependencies()),
            []
        ),
        new RelationalTypeMappingSourceDependencies([]),
        new GaussDBSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
        new GaussDBSingletonOptions());

    private static RelationalTypeMapping GetMapping(string storeType)
        => Mapper.FindMapping(storeType);

    private static RelationalTypeMapping GetMapping(Type clrType)
        => Mapper.FindMapping(clrType);

    public class LegacyGaussDBTypeMappingFixture : IDisposable
    {
        public LegacyGaussDBTypeMappingFixture()
        {
            GaussDBTypeMappingSource.LegacyTimestampBehavior = true;
        }

        public void Dispose()
            => GaussDBTypeMappingSource.LegacyTimestampBehavior = false;
    }

    #endregion Support
}

[CollectionDefinition("LegacyDateTimeTest", DisableParallelization = true)]
public class EventSourceTestCollection;

#endif
