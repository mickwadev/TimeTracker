using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Database;

[Table("TimeTracking")]
public class WorkTime : BaseModel
{
    [PrimaryKey("id", false)]
    public long ID { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;
    // TO DO: store this as DateTime to avoid this to string parsing
    [Column("start_time")]
    public DateTime StartTime { get; set; }

    [Column("end_time")]
    public DateTime EndTime { get; set; }

    // this will be same as 'id', but I'm not sure how to handle that yet
    [Column("backup_id")]
    public long backupID { get; set; } = -1;

    [Column("user")]
    public string User { get; set; } = "user_not_set";

    [Column("activity_type")]
    public string ActivityType { get; set; } = "at_not_set";

    //{"code":"PGRST204","details":null,"hint":null,"message":"Could not find the 'Duration' column of 'TimeTracking' in the schema cache"}
    [JsonIgnore]
    public TimeSpan Duration => EndTime - StartTime;

    public override string ToString()
    {
        return $"ID={ID} Title: {Title}, user: {User} start: {StartTime}, BackupID={backupID}";
    }

}
