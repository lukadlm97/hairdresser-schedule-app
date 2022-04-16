using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HairdresserScheduleApp.BusinessLogic.Models.Logging
{
    public class LogRequest
    {
        public string RequestId { get; set; }
        public string AutorizationData { get; set; }
        public string Message { get; set; }
    }
}
