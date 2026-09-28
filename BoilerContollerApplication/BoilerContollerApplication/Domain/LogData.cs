using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerContollerApplication.Domain
{
    public class LogData
    {
        public DateTime TimeStamp { get; set; }
        public string Event { get; set; } = string.Empty;
        public string EventData { get; set; } = string.Empty;
    }
}
