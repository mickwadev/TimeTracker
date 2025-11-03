using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateRangeHelpers
{
    public record DateRange(DateOnly startDate, DateOnly endDate)
    {
        public int Duration => endDate.DayNumber - startDate.DayNumber + 1;
        public override string ToString() => $"From {startDate} to {endDate}, duration: {Duration}";
    }
}
