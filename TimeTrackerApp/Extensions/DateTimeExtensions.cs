using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDBFormat(this DateTime dt)  => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
    }
}
