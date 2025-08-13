using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using System.Diagnostics;

namespace TimeTracker.PageModels
{
    public partial class ChartTestPageViewModel : ObservableObject
    {
        public ISeries[] Series { get; set; } = 
            [
                new ColumnSeries<DateTimePoint>
                    {
                        Values = [
                            new() { DateTime = new(2025, 8, 12), Value = 3 },
                            new() { DateTime = new(2025, 8, 13), Value = 6 },
                            new() { DateTime = new(2025, 8, 14), Value = 5 }
                                ]
                    }
                    ];

        public ICartesianAxis[] XAxes { get; set; } =
        [
            new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd MM")),
        ];

        [RelayCommand]
        public void AddValueButton()
        {
            Trace.WriteLine("Add value to chart...");
            var d = new ColumnSeries<DateTimePoint>
            {
                Values = [
                            new() { DateTime = new(2025, 8, 12), Value = 1 },
                            new() { DateTime = new(2025, 8, 13), Value = 1 },
                            new() { DateTime = new(2025, 8, 14), Value = 1 }
                                ]
            };
                   
            Series.SetValue(d, 0);
        }

        }
}
