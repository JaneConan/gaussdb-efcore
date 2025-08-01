﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.Json;
using NetTopologySuite.IO;

// ReSharper disable once CheckNamespace
namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

/// <summary>
///     Reads and writes JSON using the well-known-text format for <see cref="Geometry" /> values.
/// </summary>
public sealed class GaussDBJsonGeometryWktReaderWriter : JsonValueReaderWriter<Geometry>
{
    private static readonly PropertyInfo InstanceProperty = typeof(GaussDBJsonGeometryWktReaderWriter).GetProperty(nameof(Instance))!;

    private static readonly WKTReader WktReader = new();

    /// <summary>
    ///     The singleton instance of this stateless reader/writer.
    /// </summary>
    public static GaussDBJsonGeometryWktReaderWriter Instance { get; } = new();

    private GaussDBJsonGeometryWktReaderWriter()
    {
    }

    /// <inheritdoc />
    public override Geometry FromJsonTyped(ref Utf8JsonReaderManager manager, object? existingObject = null)
        => WktReader.Read(manager.CurrentReader.GetString());

    /// <inheritdoc />
    public override void ToJsonTyped(Utf8JsonWriter writer, Geometry value)
    {
        var wkt = value.ToText();

        // If the SRID is defined, prefix the WKT with it (SRID=4326;POINT(-44.3 60.1))
        // Although this is a GaussDB extension, NetTopologySuite supports it (see #3236)
        if (value.SRID > 0)
        {
            wkt = $"SRID={value.SRID};{wkt}";
        }

        writer.WriteStringValue(wkt);
    }

    /// <inheritdoc />
    public override Expression ConstructorExpression => Expression.Property(null, InstanceProperty);
}
