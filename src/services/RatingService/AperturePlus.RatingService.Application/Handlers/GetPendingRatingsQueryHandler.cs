using AperturePlus.RatingService.Application.DTOs;
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Application.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Handlers
{
    public class GetPendingRatingsQueryHandler : IRequestHandler<GetPendingRatingsQuery, IEnumerable<PendingRatingDto>>
    {
        private readonly IRatingRepository ratingRepository;
        public GetPendingRatingsQueryHandler(IRatingRepository ratingRepository) 
        {
            this.ratingRepository = ratingRepository;
        }

        public Task<IEnumerable<PendingRatingDto>> Handle(GetPendingRatingsQuery request, CancellationToken cancellationToken)
        {
            return ratingRepository.GetPendingRatingsAsync(request.UserId, cancellationToken);
        }
    }
}
