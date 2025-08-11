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
        public string StartTime { get; set; }  
        public string EndTime { get; set; }
    }
}
