using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int TokenValidityInDays { get; set; }
    }
}
