using AperturePlus.RatingService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Queries
{
    public record GetReceivedRatingsQuery(Guid UserId) : IRequest<IEnumerable<ReceivedRatingDto>>;

}
