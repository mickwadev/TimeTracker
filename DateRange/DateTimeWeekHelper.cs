using DateTimeHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Date = System.DateOnly;
namespace DateHelpers;

// Weeks do not start when year or months start. Do not user years, months at all.
public class DateTimeWeekHelper : IGetDateRange
{
    const DayOfWeek firstWeekDay = DayOfWeek.Monday;
    const int daysInWeek = 7;
    int weekOffset = 0;

    Date startDate;
    
    public int DaysToMonday { get; }
     
    public DateTimeWeekHelper(DateTime startTime)
    {
        startDate = new Date(startTime.Year, startTime.Month, startTime.Day);
        
        // how far are we from monday?
        DaysToMonday = (daysInWeek + startTime.DayOfWeek - firstWeekDay) % daysInWeek;
    }

    public DateRange GetCurrentDateRange()
    {
        int startDay = startDate.DayNumber - DaysToMonday + (daysInWeek * weekOffset);
   //     Console.WriteLine($"Week of year: {GetWeekOfYear(Date.FromDayNumber(startDay))} ({weekOffset})");
        return new DateRange(Date.FromDayNumber(startDay),Date.FromDayNumber(startDay + daysInWeek));
    }

 //   private int GetWeekOfYear(Date date) => ISOWeek.GetWeekOfYear(new DateTime(date.Year, date.Month, date.Day));

    public void GoToNextDateRange() => weekOffset++;
    public void GoToPreviousDateRange() => weekOffset--;
    public void GoToStartRange() => weekOffset = 0;


}