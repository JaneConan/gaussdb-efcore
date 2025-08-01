// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Provides extension methods supporting NodaTime function translation for GaussDB.
/// </summary>
public static class GaussDBNodaTimeDbFunctionsExtensions
{
    /// <summary>
    ///     Computes the sum of the non-null input intervals. Corresponds to the GaussDB <c>sum</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The input values to be summed.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Sum(DbFunctions, IEnumerable{Period})" /> is only intended for use via SQL translation as part of an EF Core LINQ query.
    /// </exception>
    public static Period? Sum(this DbFunctions _, IEnumerable<Period> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Sum)));

    /// <summary>
    ///     Computes the sum of the non-null input intervals. Corresponds to the GaussDB <c>sum</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The input values to be summed.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Sum(DbFunctions, IEnumerable{Duration})" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static Duration? Sum(this DbFunctions _, IEnumerable<Duration> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Sum)));

    /// <summary>
    ///     Computes the average (arithmetic mean) of the non-null input intervals. Corresponds to the GaussDB <c>avg</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The input values to be computed into an average.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Average(DbFunctions, IEnumerable{Period})" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static Period? Average(this DbFunctions _, IEnumerable<Period> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Average)));

    /// <summary>
    ///     Computes the average (arithmetic mean) of the non-null input intervals. Corresponds to the GaussDB <c>avg</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The input values to be computed into an average.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="Average(DbFunctions, IEnumerable{Duration})" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static Duration? Average(this DbFunctions _, IEnumerable<Duration> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Average)));

    /// <summary>
    ///     Returns the distance between two instants as a <see cref="Period" />, particularly suitable for sorting where the appropriate index
    ///     is defined.
    /// </summary>
    /// <remarks>
    ///     This requires the <c>btree_gist</c> built-in GaussDB extension, see
    ///     <see href="https://www.postgresql.org/docs/current/btree-gist.html" />.
    /// </remarks>
    public static int Distance(this DbFunctions _, Instant a, Instant b)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Distance)));

    /// <summary>
    ///     Returns the distance between two zoned timestamps as a <see cref="Period" />, particularly suitable for sorting where the
    ///     appropriate index is defined.
    /// </summary>
    /// <remarks>
    ///     This requires the <c>btree_gist</c> built-in GaussDB extension, see
    ///     <see href="https://www.postgresql.org/docs/current/btree-gist.html" />.
    /// </remarks>
    public static int Distance(this DbFunctions _, ZonedDateTime a, ZonedDateTime b)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Distance)));

    /// <summary>
    ///     Returns the distance between two local timestamps as a <see cref="Period" />, particularly suitable for sorting where the
    ///     appropriate index is defined.
    /// </summary>
    /// <remarks>
    ///     This requires the <c>btree_gist</c> built-in GaussDB extension, see
    ///     <see href="https://www.postgresql.org/docs/current/btree-gist.html" />.
    /// </remarks>
    public static int Distance(this DbFunctions _, LocalDateTime a, LocalDateTime b)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Distance)));

    /// <summary>
    ///     Returns the distance between two dates as a number of days, particularly suitable for sorting where the appropriate index is
    ///     defined.
    /// </summary>
    /// <remarks>
    ///     This requires the <c>btree_gist</c> built-in GaussDB extension, see
    ///     <see href="https://www.postgresql.org/docs/current/btree-gist.html" />.
    /// </remarks>
    public static int Distance(this DbFunctions _, LocalDate a, LocalDate b)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Distance)));

    #region Aggregate functions

    /// <summary>
    ///     Computes the union of the non-null input intervals. Corresponds to the GaussDB <c>range_agg</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The intervals to be aggregated via union into a multirange.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="RangeAgg(DbFunctions, IEnumerable{Interval})" /> is only intended for use via SQL translation as part of an EF Core LINQ
    ///     query.
    /// </exception>
    public static Interval[] RangeAgg(this DbFunctions _, IEnumerable<Interval> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RangeAgg)));

    /// <summary>
    ///     Computes the union of the non-null input date intervals. Corresponds to the GaussDB <c>range_agg</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The date intervals to be aggregated via union into a multirange.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="RangeAgg(DbFunctions, IEnumerable{DateInterval})" /> is only intended for use via SQL translation as part of an EF Core
    ///     LINQ query.
    /// </exception>
    public static DateInterval[] RangeAgg(this DbFunctions _, IEnumerable<DateInterval> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RangeAgg)));

    /// <summary>
    ///     Computes the intersection of the non-null input intervals. Corresponds to the GaussDB <c>range_intersect_agg</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The intervals on which to perform the intersection operation.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="RangeIntersectAgg(DbFunctions, IEnumerable{Interval})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static Interval RangeIntersectAgg(this DbFunctions _, IEnumerable<Interval> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RangeIntersectAgg)));

    /// <summary>
    ///     Computes the intersection of the non-null input date intervals. Corresponds to the GaussDB <c>range_intersect_agg</c> aggregate
    ///     function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The date intervals on which to perform the intersection operation.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="RangeIntersectAgg(DbFunctions, IEnumerable{DateInterval})" /> is only intended for use via SQL translation as part of an
    ///     EF Core LINQ query.
    /// </exception>
    public static DateInterval RangeIntersectAgg(this DbFunctions _, IEnumerable<DateInterval> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RangeIntersectAgg)));

    /// <summary>
    ///     Computes the intersection of the non-null input interval multiranges.
    ///     Corresponds to the GaussDB <c>range_intersect_agg</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The intervals on which to perform the intersection operation.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="RangeIntersectAgg(DbFunctions, IEnumerable{Interval[]})" /> is only intended for use via SQL translation as part of an EF
    ///     Core LINQ query.
    /// </exception>
    public static Interval[] RangeIntersectAgg(this DbFunctions _, IEnumerable<Interval[]> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RangeIntersectAgg)));

    /// <summary>
    ///     Computes the intersection of the non-null input date interval multiranges.
    ///     Corresponds to the GaussDB <c>range_intersect_agg</c> aggregate function.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions" /> instance.</param>
    /// <param name="input">The date intervals on which to perform the intersection operation.</param>
    /// <seealso href="https://www.postgresql.org/docs/current/functions-aggregate.html">GaussDB documentation for aggregate functions.</seealso>
    /// <exception cref="NotSupportedException">
    ///     <see cref="RangeIntersectAgg(DbFunctions, IEnumerable{DateInterval[]})" /> is only intended for use via SQL translation as part of
    ///     an EF Core LINQ query.
    /// </exception>
    public static DateInterval[] RangeIntersectAgg(this DbFunctions _, IEnumerable<DateInterval[]> input)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RangeIntersectAgg)));

    #endregion Aggregate functions
}
