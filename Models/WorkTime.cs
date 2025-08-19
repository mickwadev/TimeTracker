using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Models
{
    public class WorkTime
    {
        public int ID { get; set; }
        public string Title { get; set; }
        // TO DO: store this as DateTime to avoid this to string parsing
        public string StartTime { get; set; }  
        public string EndTime { get; set; }

        public TimeSpan Duration() => DateTime.Parse(EndTime) - DateTime.Parse(StartTime);
         
    }
}
