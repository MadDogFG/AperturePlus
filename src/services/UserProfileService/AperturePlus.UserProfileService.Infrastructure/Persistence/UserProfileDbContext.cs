using AperturePlus.UserProfileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Infrastructure.Persistence
{
    public class UserProfileDbContext:DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }

        public UserProfileDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
