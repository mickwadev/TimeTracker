using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimeHelpers
{
    public interface IGetDateRange
    {
        DateRange GetCurrentDateRange();
        void GoToNextDateRange();
        void GoToPreviousDateRange();
        void GoToStartRange();
    }
}
