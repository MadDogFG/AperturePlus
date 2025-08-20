using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Application.DTOs
{
    public record class CreateActivityResult
    {
        public bool Successed { get; init; } //是否创建成功
        public string? Error { get; init; } //错误信息
        public Guid? ActivityId { get; init; } //新活动ID，方便前端直接得到新创建的活动ID，而不是重新查询所有活动列表
        public CreateActivityResult(bool successed, string error,Guid? activityId)
        {
            Successed = successed;
            Error = error;
            ActivityId = activityId;
        }
        public static CreateActivityResult Success(Guid activityId)
        {
            return new CreateActivityResult(true, null,activityId);
        }
        public static CreateActivityResult Failure()
        {
            return new CreateActivityResult(false, "创建活动失败",null);
        }
    }
}
