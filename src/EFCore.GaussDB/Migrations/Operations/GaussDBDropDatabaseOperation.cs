﻿namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Migrations.Operations;

/// <summary>
///     A GaussDB-specific <see cref="MigrationOperation" /> to drop a database.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see>.
/// </remarks>
public class GaussDBDropDatabaseOperation : MigrationOperation
{
    /// <summary>
    ///     The name of the database.
    /// </summary>
    public virtual string Name { get; set; } = null!;
}
