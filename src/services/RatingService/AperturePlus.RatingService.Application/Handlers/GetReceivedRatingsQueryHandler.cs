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
    public class GetReceivedRatingsQueryHandler:IRequestHandler<GetReceivedRatingsQuery,IEnumerable<ReceivedRatingDto>>
    {
        private readonly IRatingRepository ratingRepository;
        public GetReceivedRatingsQueryHandler(IRatingRepository ratingRepository) 
        {
            this.ratingRepository = ratingRepository;
        } 

        public Task<IEnumerable<ReceivedRatingDto>> Handle(GetReceivedRatingsQuery request, CancellationToken cancellationToken)
        {
            return ratingRepository.GetReceivedRatingsAsync(request.UserId, cancellationToken);
        }
    }
}
