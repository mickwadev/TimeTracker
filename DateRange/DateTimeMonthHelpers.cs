using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Date = System.DateOnly;

namespace DateTimeHelpers
{
    public class DateTimeMonthHelpers : IGetDateRange
    {
        Date startDate;
        Date currentDate;
        public DateTimeMonthHelpers(DateTime startTime)
        {
            startDate = new Date(startTime.Year, startTime.Month, startTime.Day);
            currentDate = startDate;
        }

        public DateRange GetCurrentDateRange()
        {
            var year = currentDate.Year;
            var month = currentDate.Month;
            Date sd = new Date(year, month, 1);
            Date ed = new Date(year, month, DateTime.DaysInMonth(year, month));
            return new DateRange(sd, ed);
        }
         
        public void GoToNextDateRange()
        {
            currentDate = currentDate.AddMonths(1);
        }

        public void GoToPreviousDateRange()
        {
            currentDate = currentDate.AddMonths(-1);
        }

        public void GoToStartRange()
        {
            currentDate = startDate;
        }
    }
}
