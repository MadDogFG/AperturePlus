using AperturePlus.RatingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Infrastructure.Configurations
{
    public class PendingRatingConfig : IEntityTypeConfiguration<PendingRating>
    {
        public void Configure(EntityTypeBuilder<PendingRating> builder)
        {
            builder.HasKey(p => p.PendingRatingId);
            builder.Property(p => p.RatedUserRole).HasConversion<string>();
        }
    }
}
