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
    public class GetRatingStatsQueryHandler : IRequestHandler<GetRatingStatsQuery, RatingStatsDto>
    {
        private readonly IRatingRepository ratingRepository;
        public GetRatingStatsQueryHandler(IRatingRepository ratingRepository) 
        {
            ratingRepository = ratingRepository;
        } 

        public Task<RatingStatsDto> Handle(GetRatingStatsQuery request, CancellationToken cancellationToken)
        {
            return ratingRepository.GetRatingStatsAsync(request.UserId, cancellationToken);
        }
    }
}
