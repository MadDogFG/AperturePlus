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
    public class UserSummaryConfig : IEntityTypeConfiguration<UserSummary>
    {
        public void Configure(EntityTypeBuilder<UserSummary> builder)
        {
            builder.HasKey(u=>u.UserId);
        }
    }
}
