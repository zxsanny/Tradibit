﻿using Microsoft.EntityFrameworkCore;
using Tradibit.Common.Entities;
using Tradibit.DataAccess.Configuration;

namespace Tradibit.DataAccess;

public class TradibitDb : DbContext
{
    public TradibitDb(DbContextOptions<TradibitDb> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Scenario> Scenarios { get; set; }
    public DbSet<Strategy> Strategies { get; set; }
    public DbSet<UserFund> UserFunds { get; set; }
    public DbSet<ScenarioOperation> ScenarioHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}