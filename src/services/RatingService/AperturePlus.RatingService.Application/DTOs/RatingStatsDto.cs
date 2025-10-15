using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.DTOs
{
    public record RatingStatsDto(int TotalCount, double PositiveRate);
}
