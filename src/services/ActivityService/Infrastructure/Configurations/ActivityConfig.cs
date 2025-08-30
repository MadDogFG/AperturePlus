using AperturePlus.ActivityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Infrastructure.Configurations
{
    public class ActivityConfig : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.OwnsOne(a => a.ActivityLocation);

            builder.OwnsMany(a => a.RoleRequirements, rb =>//rb是配置RoleRequirement，因为这个配置是Activity的，但是没必要单独开个文件配置RoleRequirement，所以就用OwnedNavigationBuilder
            {
                rb.WithOwner().HasForeignKey("ActivityId");
                rb.Property(r => r.Role).HasConversion<string>();
                rb.HasKey("ActivityId", "Role");
            });

            builder.OwnsMany(a => a.Participants, pb =>
            {
                pb.WithOwner().HasForeignKey("ActivityId");
                pb.Property(p => p.Role).HasConversion<string>();
                pb.Property(p => p.Status).HasConversion<string>();
                pb.HasKey("ActivityId", "UserId");
            });
        }
    }
}