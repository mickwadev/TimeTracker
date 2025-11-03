using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Date = System.DateOnly;

namespace DateRangeHelpers;

public class DateTimeAllHelper : IGetDateRange
{
    DateTime _start, _end;
    public DateTimeAllHelper(DateTime start, DateTime end)
    {
        Trace.WriteLine("Using DataTimeAllHelper...");
        _start = start;
        _end = end;
    }

    public DateRange GetCurrentDateRange()
    {
        Date sd = new Date(_start.Year, _start.Month, _start.Day);
        Date ed = new Date(_end.Year, _end.Month, _end.Day);
        return new DateRange(sd, ed);
    }

    public (long min, long max) GetTicksRange()
    {
        var dr = GetCurrentDateRange();
        return (dr.startDate.ToDateTime(new TimeOnly(0, 0)).Ticks, dr.endDate.ToDateTime(new TimeOnly(0, 0)).Ticks);
    }

    public void GoToNextDateRange()
    {
        
    }

    public void GoToPreviousDateRange()
    {
        
    }

    public void GoToStartRange()
    {
        
    }
}
