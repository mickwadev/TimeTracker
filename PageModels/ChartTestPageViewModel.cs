using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TimeTracker.PageModels
{
    public partial class ChartTestPageViewModel : ObservableObject
    {
        // https://livecharts.dev/docs/maui/2.0.0-rc5.4/Overview.Automatic%20updates
        [ObservableProperty]
        ObservableCollection<int> _ints =new ObservableCollection<int>() {1,2,3};
        Random random = new Random();

        public ISeries[] Series2 { get;}

        public ChartTestPageViewModel()
        {
            // since _ints is of type ObservableCollection 
            // LiveCharts will update when you add, remove, replace or clear the collection
            LineSeries<int> lineSeries = new LineSeries<int>();
            lineSeries.Values = Ints;


            Series2 = new ISeries[] { new LineSeries<int>() { Values = Ints} };
        }

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
        public async Task AddSomeValuesAsync()
        {
            for (int i = 0; i < 5; i++)
            { 
                await Task.Delay(1000);
                Ints.Add(random.Next(1, 10));
            }
        }

        [RelayCommand]
        public void AddValueButton()
        {
            Trace.WriteLine("Add value to chart...");
            Ints.Add(random.Next(0,10));
             
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
