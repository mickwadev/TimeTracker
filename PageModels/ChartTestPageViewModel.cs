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
        ObservableCollection<int> _ints =new ObservableCollection<int>() {1,2,3}; // with List<int> this won't work.
        Random random = new Random();


        public ISeries[] Series2 { get;}
        public ISeries[] ColumnSeries { get; }

        public ISeries[] DateSeries { get; set; } =
            [
                // Both ColumnSeries and DataTimePoint implement INotifyPropertyChanged
                new ColumnSeries<DateTimePoint>
                    {
                        Values = [
                            new() { DateTime = new(2025, 8, 12), Value = 3 },
                            new() { DateTime = new(2025, 8, 13), Value = 6 },
                            new() { DateTime = new(2025, 8, 14), Value = 5 }
                                ]
                    }
            ];

        public void AddToDateSeries()
        {
            ISeries? d =DateSeries[0];
            
        
        }

        public ChartTestPageViewModel()
        {
            // since _ints is of type ObservableCollection 
            // LiveCharts will update when you add, remove, replace or clear the collection
             
            Series2 = new ISeries[] { new LineSeries<int>() { Values = Ints} };
            ColumnSeries = new ISeries[] {new ColumnSeries<int>() { Values = Ints} };
            AddToDateSeries();
        }

        

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
            }

        }
}
