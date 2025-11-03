using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Date = System.DateOnly;

namespace DateRangeHelpers
{
    public class DateTimeYearHelpers : IGetDateRange
    {
        Date startDate;
        Date currentDate;
        public DateTimeYearHelpers(DateTime startTime)
        {
            startDate = new Date(startTime.Year, startTime.Month, startTime.Day);
            currentDate = startDate;
        }
         
        public DateRange GetCurrentDateRange() => new DateRange(
                  new Date(currentDate.Year, 1, 1),
                  new Date(currentDate.Year, 12, 31));
         
        public void GoToNextDateRange()
        {
            currentDate = currentDate.AddYears(1);
        }

        public void GoToPreviousDateRange()
        {
            currentDate = currentDate.AddYears(-1);
        }

        public void GoToStartRange()
        {
            currentDate = startDate;
        }

        public (long, long) GetTicksRange()
        {
            var dr = GetCurrentDateRange();
            return (dr.startDate.ToDateTime(new TimeOnly(0, 0)).Ticks, dr.endDate.ToDateTime(new TimeOnly(0, 0)).Ticks);
        }
    }
}
