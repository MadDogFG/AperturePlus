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
        public string Bio { get; private set; } = "还没有任何个人简介哦";//个人简介
        public string AvatarUrl { get; private set; } = "默认头像URL";//头像URL
        public List<string> Roles { get; private set; } = new List<string>();//用户角色列表

        private UserProfile() : base(){}

        private UserProfile(Guid userId, string userName, List<string> roles)
        {
            UserId = userId;
            UserName = userName;
            Roles = roles ?? new List<string>();
        }

        public static UserProfile CreateUserProfile(Guid userId, string userName, List<string> roles = null)
        {
            return new UserProfile(userId, userName, roles ?? new List<string>());
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

        public void UpdateRoles(List<string> roles)
        {
            Roles = roles ?? new List<string>();
        }
    }
}
