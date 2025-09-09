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
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public string Bio { get; private set; }//个人简介
        public string AvatarUrl { get; private set; }//头像URL


        public UserProfileDto(Guid userId, string userName, string bio, string avatarUrl)
        {
            UserId = userId;
            UserName = userName;
            Bio = bio;
            AvatarUrl = avatarUrl;
        }
    }
}
