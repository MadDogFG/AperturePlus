using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Domain.Entities
{
    public class UserProfile
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public string Bio { get; private set; }//个人简介
        public string AvatarUrl { get; private set; }//头像URL

        public UserProfile() { }

        private UserProfile(Guid userId, string userName, string bio, string avatarUrl)
        {
            UserId = userId;
            UserName = userName;
            Bio = bio;
            AvatarUrl = avatarUrl;
        }

        public static UserProfile CreateUserProfile(Guid userId, string userName, string bio, string avatarUrl)
        {
            return new UserProfile(userId, userName, bio,avatarUrl);
        }

        public void UpdateUserProfile(string? bio, string? avatarUrl)
        {
            if (bio != null)
            {
                Bio = bio;
            }
            if (avatarUrl != null)
            {
                AvatarUrl = avatarUrl;
            }
        }
    }
}
