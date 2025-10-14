using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.RatingService.Domain.Entities
{
    public class ActivitySummary
    {
        public Guid ActivityId { get; private set; }
        public string ActivityTitle { get; private set; }

        private ActivitySummary() { }

        public static ActivitySummary Create(Guid activityId, string activityTitle)
        {
            return new ActivitySummary { ActivityId = activityId, ActivityTitle = activityTitle };
        }

        public void Update(string activityTitle)
        {
            ActivityTitle = activityTitle;
        }
    }
}
