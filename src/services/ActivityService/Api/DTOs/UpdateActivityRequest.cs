using AperturePlus.ActivityService.Domain.ValueObjects;

namespace AperturePlus.ActivityService.Api.DTOs
{
    public record class UpdateActivityRequest
    {
        public string? ActivityTitle { get; set; }
        public string? ActivityDescription { get; set; }
        public Location? ActivityLocation { get; set; }
        public DateTime? ActivityStartTime { get; set; }
        public Decimal? Fee { get; set; }//活动费用
        public List<RoleRequirement>? RoleRequirements { get; set; }//角色需求列表
    }
}
