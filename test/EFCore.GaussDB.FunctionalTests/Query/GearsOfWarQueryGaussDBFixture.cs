﻿using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;

namespace Microsoft.EntityFrameworkCore.Query;


public class GearsOfWarQueryGaussDBFixture : GearsOfWarQueryRelationalFixture
{
    protected override string StoreName
        => "GearsOfWarQueryTest";

    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    private GearsOfWarData? _expectedData;

    static GearsOfWarQueryGaussDBFixture()
    {
        // TODO: Switch to using GaussDBDataSource
#pragma warning disable CS0618 // Type or member is obsolete
        GaussDBConnection.GlobalTypeMapper.EnableRecordsAsTuples();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
    {
        base.OnModelCreating(modelBuilder, context);

        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<CogTag>().Property(c => c.IssueDate).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<City>().Property(g => g.Location).HasColumnType("varchar(100)");
    }

    public override ISetSource GetExpectedData()
    {
        if (_expectedData is null)
        {
            _expectedData = (GearsOfWarData)base.GetExpectedData();

            // GearsOfWarData contains DateTimeOffsets with various offsets, which we don't support. Change these to UTC.
            // Also chop sub-microsecond precision which GaussDB does not support.
            foreach (var mission in _expectedData.Missions)
            {
                mission.Timeline = new DateTimeOffset(
                    mission.Timeline.Ticks - (mission.Timeline.Ticks % (TimeSpan.TicksPerMillisecond / 1000)), TimeSpan.Zero);
                mission.Duration = new TimeSpan(
                    mission.Duration.Ticks - (mission.Duration.Ticks % (TimeSpan.TicksPerMillisecond / 1000)));
            }
        }

        return _expectedData;
    }

    protected override async Task SeedAsync(GearsOfWarContext context)
    {
        // GearsOfWarData contains DateTimeOffsets with various offsets, which we don't support. Change these to UTC.
        // Also chop sub-microsecond precision which GaussDB does not support.
        // See https://github.com/dotnet/efcore/issues/26068

        var squads = GearsOfWarData.CreateSquads();
        var missions = GearsOfWarData.CreateMissions();
        var squadMissions = GearsOfWarData.CreateSquadMissions();
        var cities = GearsOfWarData.CreateCities();
        var weapons = GearsOfWarData.CreateWeapons();
        var tags = GearsOfWarData.CreateTags();
        var gears = GearsOfWarData.CreateGears();
        var locustLeaders = GearsOfWarData.CreateLocustLeaders();
        var factions = GearsOfWarData.CreateFactions();
        var locustHighCommands = GearsOfWarData.CreateHighCommands();

        foreach (var mission in missions)
        {
            mission.Timeline = new DateTimeOffset(
                mission.Timeline.Ticks - (mission.Timeline.Ticks % (TimeSpan.TicksPerMillisecond / 1000)), TimeSpan.Zero);
            mission.Duration = new TimeSpan(
                mission.Duration.Ticks - (mission.Duration.Ticks % (TimeSpan.TicksPerMillisecond / 1000)));
        }

        GearsOfWarData.WireUp(
            squads, missions, squadMissions, cities, weapons, tags, gears, locustLeaders, factions, locustHighCommands);

        context.Squads.AddRange(squads);
        context.Missions.AddRange(missions);
        context.SquadMissions.AddRange(squadMissions);
        context.Cities.AddRange(cities);
        context.Weapons.AddRange(weapons);
        context.Tags.AddRange(tags);
        context.Gears.AddRange(gears);
        context.LocustLeaders.AddRange(locustLeaders);
        context.Factions.AddRange(factions);
        context.LocustHighCommands.AddRange(locustHighCommands);
        await context.SaveChangesAsync();

        GearsOfWarData.WireUp2(locustLeaders, factions);

        await context.SaveChangesAsync();
    }
}
