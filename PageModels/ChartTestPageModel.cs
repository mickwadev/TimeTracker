using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace TimeTracker.PageModels
{
    // https://livecharts.dev/docs/Maui/2.0.0-rc5.4/Overview.Installation
    class ChartTestPageModel
    {
        public ISeries[] Series { get; set; } = [
         new ColumnSeries<DateTimePoint>
        {
            Values = [
                new() { DateTime = new(2025, 8, 12), Value = 3 },
                new() { DateTime = new(2025, 8, 13), Value = 6 },
                new() { DateTime = new(2025, 8, 14), Value = 5 },
                //new() { DateTime = new(2021, 1, 4), Value = 3 },
                //new() { DateTime = new(2021, 1, 5), Value = 5 },
                //new() { DateTime = new(2021, 1, 6), Value = 8 },
                //new() { DateTime = new(2021, 1, 7), Value = 6 }
            ]
        }
     ];

        public ICartesianAxis[] XAxes { get; set; } =
            [
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd MM")),
             
            ];

    }
}
