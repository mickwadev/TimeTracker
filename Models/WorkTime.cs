using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest.Attributes;

namespace TimeTracker.Models
{
    [Table("TimeTracking")]
    public class WorkTime
    {
        [PrimaryKey("id",false)]
        public long ID { get; set; }

        [Column("title")]
        public string Title { get; set; }
        // TO DO: store this as DateTime to avoid this to string parsing
        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        public TimeSpan Duration => EndTime - StartTime;
         
    }
}
