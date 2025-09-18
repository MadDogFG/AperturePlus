using AperturePlus.RatingService.Application.Commands;
using AperturePlus.RatingService.Application.Interfaces;
using AperturePlus.RatingService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Application.Handlers
{
    public class SubmitRatingCommandHandler : IRequestHandler<SubmitRatingCommand, bool>
    {
        private readonly IRatingRepository ratingRepository;
        private readonly IUnitOfWork unitOfWork;

        public SubmitRatingCommandHandler(IRatingRepository ratingRepository, IUnitOfWork unitOfWork)
        {
            this.ratingRepository = ratingRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(SubmitRatingCommand request, CancellationToken cancellationToken)
        {
            var ratings = await ratingRepository.GetRatingstoUserAsync(request.RateToUserId, cancellationToken);
            var rating = ratings.Where(r=>r.RateByUserId == request.RateByUserId&&r.ActivityId==request.ActivityId).FirstOrDefault();
            if(rating != null)
            {
                return false;
            }
            rating = Rating.CreateRating(request.ActivityId, request.RateByUserId, request.RateToUserId, request.RatedUserRole, request.Score, request.Comments);
            await ratingRepository.AddAsync(rating,cancellationToken);
            int result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
