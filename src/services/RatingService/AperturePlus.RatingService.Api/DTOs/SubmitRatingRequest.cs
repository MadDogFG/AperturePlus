namespace AperturePlus.RatingService.Api.DTOs
{
    public class SubmitRatingRequest
    {
        public Guid ActivityId { get; set; }
        public Guid RateToUserId { get; set; }
        public string RatedUserRoleString { get; set; }
        public int Score { get; set; }
        public string? Comments { get; set; }

    }
}
