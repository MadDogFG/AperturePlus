namespace AperturePlus.RatingService.Api.DTOs
{
    public class SubmitRatingRequest
    {
        public int Score { get; set; }
        public string? Comments { get; set; }

    }
}
