using Microsoft.EntityFrameworkCore;
using Tradibit.Common.Entities;

namespace Tradibit.DataAccess;

public class TradibitDb : DbContext
{
    public TradibitDb(DbContextOptions<TradibitDb> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Scenario> Scenarios { get; set; }
    public DbSet<Strategy> Strategies { get; set; }
}