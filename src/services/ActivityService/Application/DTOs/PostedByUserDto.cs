namespace AperturePlus.ActivityService.Application.DTOs
{
    public record class PostedByUserDto
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; }

        public PostedByUserDto(Guid userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
