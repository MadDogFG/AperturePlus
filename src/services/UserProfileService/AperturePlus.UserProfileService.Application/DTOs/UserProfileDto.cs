using AperturePlus.UserProfileService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.UserProfileService.Application.DTOs
{
    public record class UserProfileDto
    {
        public Guid? UserId { get; private set; }
        public string? UserName { get; private set; }
        public string? Bio { get; private set; }//个人简介
        public string? AvatarUrl { get; private set; }//头像URL
        public string? Error { get; private set; }
        public bool Successed { get; private set; }


        private UserProfileDto(Guid userId, string userName, string bio, string avatarUrl,string? error,bool successed)
        {
            UserId = userId;
            UserName = userName;
            Bio = bio;
            AvatarUrl = avatarUrl;
            Error = error;
            Successed = successed;
        }
        private UserProfileDto(string? error, bool successed) 
        {
            Error = error;
            Successed = successed;
        }
        public static UserProfileDto Success(UserProfile userProfile)
        {
            return new UserProfileDto(userProfile.UserId, userProfile.UserName, userProfile.Bio,userProfile.AvatarUrl,null,true);
        }
        public static UserProfileDto Failure()
        {
            return new UserProfileDto("获取用户档案失败", false);
        }
    }
}
