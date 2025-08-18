using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Models
{
    public static class Dates
    {
        static DayOfWeek firstWeekDay = DayOfWeek.Monday;
        static DateTime today = DateTime.Today;
        static DateTime thisMonthStart = new DateTime(today.Year, today.Month, 1);

        static int diff => (7 + today.DayOfWeek - firstWeekDay) % 7;

        public static (DateTime start, DateTime end) Today => (today, today);
        public static (DateTime start, DateTime end) ThisWeek => (today.AddDays(-diff).Date, today);
        public static (DateTime start, DateTime end) ThisMonth => (thisMonthStart, today);
        public static (DateTime start, DateTime end) All => (new DateTime(1999,9,9), today);

        public static void Test()
        {
            string f = "yyyy MM dd";
            Trace.WriteLine($"Today: from {Today.start.ToString(f)} to {Today.end.ToString(f)}");
            Trace.WriteLine($"Week: from {ThisWeek.start.ToString(f)} to {ThisWeek.end.ToString(f)}");
            Trace.WriteLine($"Month: from {ThisMonth.start.ToString(f)} to {ThisMonth.end.ToString(f)}");
            Trace.WriteLine($"All: from {All.start.ToString(f)} to {All.end.ToString(f)}");
        }
    }
}
