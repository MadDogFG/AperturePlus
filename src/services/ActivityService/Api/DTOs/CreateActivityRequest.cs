using AperturePlus.ActivityService.Domain.ValueObjects;

namespace AperturePlus.ActivityService.Api.DTOs
{
    public record class CreateActivityRequest
    {
        public string ActivityTitle { get; set; } //活动标题
        public string ActivityDescription { get; set; } //活动描述
        public Location ActivityLocation { get; set; } //活动地点
        public DateTime ActivityStartTime { get; set; } //活动开始时间
    }
}
