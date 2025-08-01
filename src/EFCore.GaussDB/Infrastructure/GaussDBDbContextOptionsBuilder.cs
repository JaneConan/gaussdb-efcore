using System.Net.Security;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure.Internal;
using HuaweiCloud.GaussDB;
using HuaweiCloud.GaussDBTypes;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;

/// <summary>
///     Allows for options specific to GaussDB to be configured for a <see cref="DbContext" />.
/// </summary>
public class GaussDBDbContextOptionsBuilder
    : RelationalDbContextOptionsBuilder<GaussDBDbContextOptionsBuilder, GaussDBOptionsExtension>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussDBDbContextOptionsBuilder" /> class.
    /// </summary>
    /// <param name="optionsBuilder"> The core options builder.</param>
    public GaussDBDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        : base(optionsBuilder)
    {
    }

    /// <summary>
    ///     Configures lower-level GaussDB options at the ADO.NET driver level.
    /// </summary>
    /// <param name="dataSourceBuilderAction">A lambda to configure GaussDB options on <see cref="GaussDBDataSourceBuilder" />.</param>
    /// <remarks>
    ///     Changes made by <see cref="ConfigureDataSource" /> are untracked; When using <see cref="DbContext.OnConfiguring" />, EF Core
    ///     will by default resolve the same <see cref="GaussDBDataSource" /> internally, disregarding differing configuration across calls
    ///     to <see cref="ConfigureDataSource" />. Either make sure that <see cref="ConfigureDataSource" /> always sets the same
    ///     configuration, or pass externally-provided, pre-configured data source instances when configuring the provider.
    /// </remarks>
    public virtual GaussDBDbContextOptionsBuilder ConfigureDataSource(Action<GaussDBDataSourceBuilder> dataSourceBuilderAction)
        => WithOption(e => e.WithDataSourceConfiguration(dataSourceBuilderAction));

    /// <summary>
    ///     Connect to this database for administrative operations (creating/dropping databases).
    /// </summary>
    /// <param name="dbName">The name of the database for administrative operations.</param>
    public virtual GaussDBDbContextOptionsBuilder UseAdminDatabase(string? dbName)
        => WithOption(e => e.WithAdminDatabase(dbName));

    /// <summary>
    ///     Configures the backend version to target.
    /// </summary>
    /// <param name="postgresVersion">The backend version to target.</param>
    public virtual GaussDBDbContextOptionsBuilder SetPostgresVersion(Version? postgresVersion)
        => WithOption(e => e.WithPostgresVersion(postgresVersion));

    /// <summary>
    ///     Configures the backend version to target.
    /// </summary>
    /// <param name="major">The GaussDB major version to target.</param>
    /// <param name="minor">The GaussDB minor version to target.</param>
    public virtual GaussDBDbContextOptionsBuilder SetPostgresVersion(int major, int minor)
        => SetPostgresVersion(new Version(major, minor));

    /// <summary>
    ///     Configures the provider to work in Redshift compatibility mode, which avoids certain unsupported features from modern
    ///     GaussDB versions.
    /// </summary>
    /// <param name="useRedshift">Whether to target Redshift.</param>
    public virtual GaussDBDbContextOptionsBuilder UseRedshift(bool useRedshift = true)
        => WithOption(e => e.WithRedshift(useRedshift));

    #region MapRange

    /// <summary>
    ///     Maps a user-defined GaussDB range type for use.
    /// </summary>
    /// <typeparam name="TSubtype">
    ///     The CLR type of the range's subtype (or element).
    ///     The actual mapped type will be an <see cref="GaussDBRange{T}" /> over this type.
    /// </typeparam>
    /// <param name="rangeName">The name of the GaussDB range type to be mapped.</param>
    /// <param name="schemaName">The name of the GaussDB schema in which the range is defined.</param>
    /// <param name="subtypeName">
    ///     Optionally, the name of the range's GaussDB subtype (or element).
    ///     This is usually not needed - the subtype will be inferred based on <typeparamref name="TSubtype" />.
    /// </param>
    /// <example>
    ///     To map a range of GaussDB real, use the following:
    ///     <code>GaussDBTypeMappingSource.MapRange{float}("floatrange");</code>
    /// </example>
    public virtual GaussDBDbContextOptionsBuilder MapRange<TSubtype>(
        string rangeName,
        string? schemaName = null,
        string? subtypeName = null)
        => MapRange(rangeName, typeof(TSubtype), schemaName, subtypeName);

    /// <summary>
    ///     Maps a user-defined GaussDB range type for use.
    /// </summary>
    /// <param name="rangeName">The name of the GaussDB range type to be mapped.</param>
    /// <param name="schemaName">The name of the GaussDB schema in which the range is defined.</param>
    /// <param name="subtypeClrType">
    ///     The CLR type of the range's subtype (or element).
    ///     The actual mapped type will be an <see cref="GaussDBRange{T}" /> over this type.
    /// </param>
    /// <param name="subtypeName">
    ///     Optionally, the name of the range's GaussDB subtype (or element).
    ///     This is usually not needed - the subtype will be inferred based on <paramref name="subtypeClrType" />.
    /// </param>
    /// <example>
    ///     To map a range of GaussDB real, use the following:
    ///     <code>GaussDBTypeMappingSource.MapRange("floatrange", typeof(float));</code>
    /// </example>
    public virtual GaussDBDbContextOptionsBuilder MapRange(
        string rangeName,
        Type subtypeClrType,
        string? schemaName = null,
        string? subtypeName = null)
        => WithOption(e => e.WithUserRangeDefinition(rangeName, schemaName, subtypeClrType, subtypeName));

    #endregion MapRange

    #region MapEnum

    /// <summary>
    ///     Maps a GaussDB enum type for use.
    /// </summary>
    /// <param name="enumName">The name of the GaussDB enum type to be mapped.</param>
    /// <param name="schemaName">The name of the GaussDB schema in which the range is defined.</param>
    /// <param name="nameTranslator">The name translator used to map enum value names to GaussDB enum values.</param>
    public virtual GaussDBDbContextOptionsBuilder MapEnum<T>(
        string? enumName = null,
        string? schemaName = null,
        IGaussDBNameTranslator? nameTranslator = null)
        where T : struct, Enum
        => MapEnum(typeof(T), enumName, schemaName, nameTranslator);

    /// <summary>
    ///     Maps a GaussDB enum type for use.
    /// </summary>
    /// <param name="clrType">The CLR type of the enum.</param>
    /// <param name="enumName">The name of the GaussDB enum type to be mapped.</param>
    /// <param name="schemaName">The name of the GaussDB schema in which the range is defined.</param>
    /// <param name="nameTranslator">The name translator used to map enum value names to GaussDB enum values.</param>
    public virtual GaussDBDbContextOptionsBuilder MapEnum(
        Type clrType,
        string? enumName = null,
        string? schemaName = null,
        IGaussDBNameTranslator? nameTranslator = null)
        => WithOption(e => e.WithEnumMapping(clrType, enumName, schemaName, nameTranslator));

    #endregion MapEnum

    /// <summary>
    ///     Appends NULLS FIRST to all ORDER BY clauses. This is important for the tests which were written
    ///     for SQL Server. Note that to fully implement null-first ordering indexes also need to be generated
    ///     accordingly, and since this isn't done this feature isn't publicly exposed.
    /// </summary>
    /// <param name="reverseNullOrdering">True to enable reverse null ordering; otherwise, false.</param>
    internal virtual GaussDBDbContextOptionsBuilder ReverseNullOrdering(bool reverseNullOrdering = true)
        => WithOption(e => e.WithReverseNullOrdering(reverseNullOrdering));

    #region Authentication (obsolete)

    /// <summary>
    ///     Configures the <see cref="DbContext" /> to use the specified <see cref="ProvideClientCertificatesCallback" />.
    /// </summary>
    /// <param name="callback">The callback to use.</param>
    [Obsolete("Call ConfigureDataSource() and configure the client certificates on the GaussDBDataSourceBuilder, or pass an externally-built, pre-configured GaussDBDataSource to UseGaussDB().")]
    public virtual GaussDBDbContextOptionsBuilder ProvideClientCertificatesCallback(ProvideClientCertificatesCallback? callback)
        => WithOption(e => e.WithProvideClientCertificatesCallback(callback));

    /// <summary>
    ///     Configures the <see cref="DbContext" /> to use the specified <see cref="RemoteCertificateValidationCallback" />.
    /// </summary>
    /// <param name="callback">The callback to use.</param>
    [Obsolete("Call ConfigureDataSource() and configure remote certificate validation on the GaussDBDataSourceBuilder, or pass an externally-built, pre-configured GaussDBDataSource to UseGaussDB().")]
    public virtual GaussDBDbContextOptionsBuilder RemoteCertificateValidationCallback(RemoteCertificateValidationCallback? callback)
        => WithOption(e => e.WithRemoteCertificateValidationCallback(callback));

    /// <summary>
    ///     Configures the <see cref="DbContext" /> to use the specified <see cref="ProvidePasswordCallback" />.
    /// </summary>
    /// <param name="callback">The callback to use.</param>
    [Obsolete("Call ConfigureDataSource() and configure the password callback on the GaussDBDataSourceBuilder, or pass an externally-built, pre-configured GaussDBDataSource to UseGaussDB().")]
    public virtual GaussDBDbContextOptionsBuilder ProvidePasswordCallback(ProvidePasswordCallback? callback)
        => WithOption(e => e.WithProvidePasswordCallback(callback));

    #endregion Authentication (obsolete)

    #region Retrying execution strategy

    /// <summary>
    ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="GaussDBDbContextOptionsBuilder" /> configured to use
    ///     the default retrying <see cref="IExecutionStrategy" />.
    /// </returns>
    public virtual GaussDBDbContextOptionsBuilder EnableRetryOnFailure()
        => ExecutionStrategy(c => new GaussDBRetryingExecutionStrategy(c));

    /// <summary>
    ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="GaussDBDbContextOptionsBuilder" /> with the specified parameters.
    /// </returns>
    public virtual GaussDBDbContextOptionsBuilder EnableRetryOnFailure(int maxRetryCount)
        => ExecutionStrategy(c => new GaussDBRetryingExecutionStrategy(c, maxRetryCount));

    /// <summary>
    ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
    /// </summary>
    /// <param name="errorCodesToAdd">Additional error codes that should be considered transient.</param>
    /// <returns>
    ///     An instance of <see cref="GaussDBDbContextOptionsBuilder" /> with the specified parameters.
    /// </returns>
    public virtual GaussDBDbContextOptionsBuilder EnableRetryOnFailure(ICollection<string>? errorCodesToAdd)
        => ExecutionStrategy(c => new GaussDBRetryingExecutionStrategy(c, errorCodesToAdd));

    /// <summary>
    ///     Configures the context to use the default retrying <see cref="IExecutionStrategy" />.
    /// </summary>
    /// <param name="maxRetryCount">The maximum number of retry attempts.</param>
    /// <param name="maxRetryDelay">The maximum delay between retries.</param>
    /// <param name="errorCodesToAdd">Additional error codes that should be considered transient.</param>
    /// <returns>
    ///     An instance of <see cref="GaussDBDbContextOptionsBuilder" /> with the specified parameters.
    /// </returns>
    public virtual GaussDBDbContextOptionsBuilder EnableRetryOnFailure(
        int maxRetryCount,
        TimeSpan maxRetryDelay,
        ICollection<string>? errorCodesToAdd)
        => ExecutionStrategy(c => new GaussDBRetryingExecutionStrategy(c, maxRetryCount, maxRetryDelay, errorCodesToAdd));

    #endregion Retrying execution strategy
}
