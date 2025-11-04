using System;

namespace Contracts
{
    public record class UserProfileUpdatedIntegrationEvent
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; }
        public string AvatarUrl { get; init; }

        public UserProfileUpdatedIntegrationEvent(Guid userId, string userName, string avatarUrl)
        {
            UserId = userId;
            UserName = userName;
            AvatarUrl = avatarUrl;
        }
    }
}