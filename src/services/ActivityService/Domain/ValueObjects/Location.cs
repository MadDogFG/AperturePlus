using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Domain.ValueObjects
{
    public class Location
    {
        public double Latitude { get; private set; }//纬度
        public double Longitude { get; private set; }//经度
        public Location(double latitude, double longitude)
        {
            //校验经纬度合法性
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "纬度必须在-90和90之间");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "经度必须在-180和180之间");

            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
