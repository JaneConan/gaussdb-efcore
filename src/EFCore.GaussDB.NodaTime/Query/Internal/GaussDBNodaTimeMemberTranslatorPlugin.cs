﻿using HuaweiCloud.EntityFrameworkCore.GaussDB.Query;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime.Query.Internal;

/// <summary>
///     Provides translation services for <see cref="NodaTime" /> members.
/// </summary>
/// <remarks>
///     See: https://www.postgresql.org/docs/current/static/functions-datetime.html
/// </remarks>
public class GaussDBNodaTimeMemberTranslatorPlugin : IMemberTranslatorPlugin
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBNodaTimeMemberTranslatorPlugin(
        IRelationalTypeMappingSource typeMappingSource,
        ISqlExpressionFactory sqlExpressionFactory)
    {
        Translators =
        [
            new GaussDBNodaTimeMemberTranslator(typeMappingSource, (GaussDBExpressionFactory)sqlExpressionFactory)
        ];
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual IEnumerable<IMemberTranslator> Translators { get; }
}

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBNodaTimeMemberTranslator : IMemberTranslator
{
    private static readonly MemberInfo SystemClock_Instance =
        typeof(SystemClock).GetRuntimeProperty(nameof(SystemClock.Instance))!;

    private static readonly MemberInfo ZonedDateTime_LocalDateTime =
        typeof(ZonedDateTime).GetRuntimeProperty(nameof(ZonedDateTime.LocalDateTime))!;

    private static readonly MemberInfo Interval_Start =
        typeof(Interval).GetRuntimeProperty(nameof(Interval.Start))!;

    private static readonly MemberInfo Interval_End =
        typeof(Interval).GetRuntimeProperty(nameof(Interval.End))!;

    private static readonly MemberInfo Interval_HasStart =
        typeof(Interval).GetRuntimeProperty(nameof(Interval.HasStart))!;

    private static readonly MemberInfo Interval_HasEnd =
        typeof(Interval).GetRuntimeProperty(nameof(Interval.HasEnd))!;

    private static readonly MemberInfo Interval_Duration =
        typeof(Interval).GetRuntimeProperty(nameof(Interval.Duration))!;

    private static readonly MemberInfo DateInterval_Start =
        typeof(DateInterval).GetRuntimeProperty(nameof(DateInterval.Start))!;

    private static readonly MemberInfo DateInterval_End =
        typeof(DateInterval).GetRuntimeProperty(nameof(DateInterval.End))!;

    private static readonly MemberInfo DateInterval_Length =
        typeof(DateInterval).GetRuntimeProperty(nameof(DateInterval.Length))!;

    private static readonly MemberInfo DateTimeZoneProviders_TzDb =
        typeof(DateTimeZoneProviders).GetRuntimeProperty(nameof(DateTimeZoneProviders.Tzdb))!;

    private readonly GaussDBExpressionFactory _sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly RelationalTypeMapping _dateTypeMapping;
    private readonly RelationalTypeMapping _periodTypeMapping;
    private readonly RelationalTypeMapping _localDateTimeTypeMapping;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBNodaTimeMemberTranslator(
        IRelationalTypeMappingSource typeMappingSource,
        GaussDBExpressionFactory sqlExpressionFactory)
    {
        _typeMappingSource = typeMappingSource;
        _sqlExpressionFactory = sqlExpressionFactory;
        _dateTypeMapping = typeMappingSource.FindMapping(typeof(LocalDate))!;
        _periodTypeMapping = typeMappingSource.FindMapping(typeof(Period))!;
        _localDateTimeTypeMapping = typeMappingSource.FindMapping(typeof(LocalDateTime))!;
    }

    private static readonly bool[][] TrueArrays = [[], [true], [true, true]];

    /// <inheritdoc />
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MemberInfo member,
        Type returnType,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        // This is necessary to allow translation of methods on SystemClock.Instance
        if (member == SystemClock_Instance)
        {
            return _sqlExpressionFactory.Constant(SystemClock.Instance);
        }

        if (member == DateTimeZoneProviders_TzDb)
        {
            return PendingDateTimeZoneProviderExpression.Instance;
        }

        if (instance is null)
        {
            return null;
        }

        var declaringType = member.DeclaringType;

        if (declaringType == typeof(LocalDateTime)
            || declaringType == typeof(LocalDate)
            || declaringType == typeof(LocalTime)
            || declaringType == typeof(Period))
        {
            return TranslateDateTime(instance, member);
        }

        if (declaringType == typeof(ZonedDateTime))
        {
            return TranslateZonedDateTime(instance, member, returnType);
        }

        if (declaringType == typeof(Duration))
        {
            return TranslateDuration(instance, member);
        }

        if (declaringType == typeof(Interval))
        {
            return TranslateInterval(instance, member);
        }

        if (declaringType == typeof(DateInterval))
        {
            return TranslateDateInterval(instance, member);
        }

        return null;
    }

    private SqlExpression? TranslateDuration(SqlExpression instance, MemberInfo member)
    {
        return member.Name switch
        {
            nameof(Duration.TotalDays) => TranslateDurationTotalMember(instance, 86400),
            nameof(Duration.TotalHours) => TranslateDurationTotalMember(instance, 3600),
            nameof(Duration.TotalMinutes) => TranslateDurationTotalMember(instance, 60),
            nameof(Duration.TotalSeconds) => GetDatePartExpressionDouble(instance, "epoch"),
            nameof(Duration.TotalMilliseconds) => TranslateDurationTotalMember(instance, 0.001),
            nameof(Duration.Days) => GetDatePartExpression(instance, "day"),
            nameof(Duration.Hours) => GetDatePartExpression(instance, "hour"),
            nameof(Duration.Minutes) => GetDatePartExpression(instance, "minute"),
            nameof(Duration.Seconds) => GetDatePartExpression(instance, "second", true),
            nameof(Duration.Milliseconds) => null, // Too annoying, floating point and sub-millisecond handling
            _ => null,
        };

        SqlExpression TranslateDurationTotalMember(SqlExpression instance, double divisor)
            => _sqlExpressionFactory.Divide(GetDatePartExpressionDouble(instance, "epoch"), _sqlExpressionFactory.Constant(divisor));
    }

    private SqlExpression? TranslateInterval(SqlExpression instance, MemberInfo member)
    {
        if (member == Interval_Start)
        {
            return Lower();
        }

        if (member == Interval_End)
        {
            return Upper();
        }

        if (member == Interval_HasStart)
        {
            return _sqlExpressionFactory.Not(
                _sqlExpressionFactory.Function(
                    "lower_inf",
                    [instance],
                    nullable: true,
                    argumentsPropagateNullability: TrueArrays[1],
                    typeof(bool)));
        }

        if (member == Interval_HasEnd)
        {
            return _sqlExpressionFactory.Not(
                _sqlExpressionFactory.Function(
                    "upper_inf",
                    [instance],
                    nullable: true,
                    argumentsPropagateNullability: TrueArrays[1],
                    typeof(bool)));
        }

        if (member == Interval_Duration)
        {
            return _sqlExpressionFactory.Subtract(Upper(), Lower(), _typeMappingSource.FindMapping(typeof(Duration)));
        }

        return null;

        SqlExpression Lower()
            => _sqlExpressionFactory.Function(
                "lower",
                [instance],
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                typeof(Interval),
                _typeMappingSource.FindMapping(typeof(Instant)));

        SqlExpression Upper()
            => _sqlExpressionFactory.Function(
                "upper",
                [instance],
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                typeof(Interval),
                _typeMappingSource.FindMapping(typeof(Instant)));
    }

    private SqlExpression? TranslateDateInterval(SqlExpression instance, MemberInfo member)
    {
        // NodaTime DateInterval is inclusive on both ends.
        // GaussDB daterange is a discrete range type; this means it gets normalized to inclusive lower bound, exclusive upper bound.
        // So we can translate Start as-is, but need to subtract a day for End.
        if (member == DateInterval_Start)
        {
            return Lower();
        }

        if (member == DateInterval_End)
        {
            // GaussDB creates a result of type 'timestamp without time zone' when subtracting intervals from dates, so add a cast back
            // to date.
            return _sqlExpressionFactory.Convert(
                _sqlExpressionFactory.Subtract(
                    Upper(),
                    _sqlExpressionFactory.Constant(Period.FromDays(1), _periodTypeMapping)), typeof(LocalDate),
                _typeMappingSource.FindMapping(typeof(LocalDate)));
        }

        if (member == DateInterval_Length)
        {
            return _sqlExpressionFactory.Subtract(Upper(), Lower());
        }

        return null;

        SqlExpression Lower()
            => _sqlExpressionFactory.Function(
                "lower",
                [instance],
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                typeof(LocalDate),
                _dateTypeMapping);

        SqlExpression Upper()
            => _sqlExpressionFactory.Function(
                "upper",
                [instance],
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                typeof(LocalDate),
                _dateTypeMapping);
    }

    private SqlExpression? TranslateDateTime(SqlExpression instance, MemberInfo member)
        => member.Name switch
        {
            "Year" or "Years" => GetDatePartExpression(instance, "year"),
            "Month" or "Months" => GetDatePartExpression(instance, "month"),
            "DayOfYear" => GetDatePartExpression(instance, "doy"),
            "Day" or "Days" => GetDatePartExpression(instance, "day"),
            "Hour" or "Hours" => GetDatePartExpression(instance, "hour"),
            "Minute" or "Minutes" => GetDatePartExpression(instance, "minute"),
            "Second" or "Seconds" => GetDatePartExpression(instance, "second", true),
            "Millisecond" or "Milliseconds" => null, // Too annoying

            // Unlike DateTime.DayOfWeek, NodaTime's IsoDayOfWeek enum doesn't exactly correspond to GaussDB's
            // values returned by date_part('dow', ...): in NodaTime Sunday is 7 and not 0, which is None.
            // So we generate a CASE WHEN expression to translate GaussDB's 0 to 7.
            "DayOfWeek" when GetDatePartExpression(instance, "dow", true) is var getValueExpression
                => _sqlExpressionFactory.Case(
                    getValueExpression,
                    [new CaseWhenClause(_sqlExpressionFactory.Constant(0), _sqlExpressionFactory.Constant(7))],
                    getValueExpression),

            // PG allows converting a timestamp directly to date, truncating the time; but given a timestamptz, it performs a time zone
            // conversion (based on TimeZone), which we don't want (so avoid translating except on timestamp).
            // The translation for ZonedDateTime.Date converts to timestamp before ending up here.
            "Date" when instance.TypeMapping is TimestampLocalDateTimeMapping or LegacyTimestampInstantMapping
                => _sqlExpressionFactory.Convert(instance, typeof(LocalDate), _typeMappingSource.FindMapping(typeof(LocalDate))!),

            "TimeOfDay" => _sqlExpressionFactory.Convert(
                instance,
                typeof(LocalTime),
                _typeMappingSource.FindMapping(typeof(LocalTime), storeTypeName: "time")),

            _ => null
        };

    /// <summary>
    ///     Constructs the date_part expression.
    /// </summary>
    /// <param name="instance">The expression.</param>
    /// <param name="partName">The name of the date_part to construct.</param>
    /// <param name="floor">True if the result should be wrapped with floor(...); otherwise, false.</param>
    /// <returns>
    ///     The date_part expression.
    /// </returns>
    /// <remarks>
    ///     date_part returns doubles, which we floor and cast into ints
    ///     This also gets rid of sub-second components when retrieving seconds.
    /// </remarks>
    private SqlExpression GetDatePartExpression(
        SqlExpression instance,
        string partName,
        bool floor = false)
    {
        var result = GetDatePartExpressionDouble(instance, partName, floor);
        return _sqlExpressionFactory.Convert(result, typeof(int));
    }

    private SqlExpression GetDatePartExpressionDouble(
        SqlExpression instance,
        string partName,
        bool floor = false)
    {
        var result = _sqlExpressionFactory.Function(
            "date_part",
            [_sqlExpressionFactory.Constant(partName), instance],
            nullable: true,
            argumentsPropagateNullability: TrueArrays[2],
            typeof(double));

        if (floor)
        {
            result = _sqlExpressionFactory.Function(
                "floor",
                [result],
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                typeof(double));
        }

        return result;
    }

    private SqlExpression? TranslateZonedDateTime(SqlExpression instance, MemberInfo member, Type returnType)
    {
        if (instance is PendingZonedDateTimeExpression pendingZonedDateTime)
        {
            instance = _sqlExpressionFactory.AtTimeZone(
                pendingZonedDateTime.Operand,
                pendingZonedDateTime.TimeZoneId,
                typeof(LocalDateTime),
                _localDateTimeTypeMapping);

            return member == ZonedDateTime_LocalDateTime
                ? instance
                : TranslateDateTime(instance, member);
        }

        // date_part, which is used to extract most components, doesn't have an overload for timestamptz, so passing one directly
        // converts it to the local timezone as per TimeZone. Explicitly convert it to a 'timestamp without time zone' in UTC.
        // The same works also for the LocalDateTime member.
        instance = _sqlExpressionFactory.AtUtc(instance);

        return member == ZonedDateTime_LocalDateTime
            ? instance
            : TranslateDateTime(instance, member);
    }
}
