using AperturePlus.RatingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Infrastructure.Persistence
{
    public class RatingServiceDbContext:DbContext
    {
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UserSummary> UserSummaries { get; set; }
        public DbSet<ActivitySummary> ActivitySummaries { get; set; }

        public RatingServiceDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
