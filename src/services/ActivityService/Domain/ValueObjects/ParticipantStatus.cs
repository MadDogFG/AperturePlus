using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Domain.ValueObjects
{
    public enum ParticipantStatus
    {
        Pending,    // 待处理
        Approved,   // 已批准
        Rejected    // 已拒绝
    }
}
