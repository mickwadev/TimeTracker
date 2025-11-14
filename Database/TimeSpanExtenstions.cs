using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database;

public static class TimeSpanExtenstions
{
    public static string ToTrimmedFormat(this TimeSpan ts)
    {
        if (ts.Hours == 0)
        {
            //if (ts.Minutes == 0)
            //{
            //    return ts.ToString(@"ss");
            //}
            return ts.ToString(@"m\:ss");

        }
        return ts.ToString(@"h\:mm\:ss");
    }
}
