using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Converters
{
    // This is used in ProgressPage in list of entries.
    internal class WorkTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => v == null))
            {
                Trace.WriteLine("Null here >.<");
                return "Some nulls here...";
            }
            try
            {
                DateTime startTime = (DateTime)values[0];
                DateTime endTime = (DateTime)values[1];
                return $"{TimeOnly.FromDateTime(startTime).ToString("HH:mm:ss")} - {TimeOnly.FromDateTime(endTime).ToString("HH:mm:ss")}";
            }
            catch (Exception ex) 
            {
                    return ex.Message;
            }
;                
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
