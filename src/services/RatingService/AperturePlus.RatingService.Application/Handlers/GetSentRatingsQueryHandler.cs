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
    public class GetSentRatingsQueryHandler : IRequestHandler<GetSentRatingsQuery, IEnumerable<SentRatingDto>>
    {
        private readonly IRatingRepository ratingRepository;
        public GetSentRatingsQueryHandler(IRatingRepository ratingRepository)
        {
            this.ratingRepository = ratingRepository;
        }

        public Task<IEnumerable<SentRatingDto>> Handle(GetSentRatingsQuery request, CancellationToken cancellationToken)
        {
            return ratingRepository.GetSentRatingsAsync(request.UserId, cancellationToken);
        }
    }
}
