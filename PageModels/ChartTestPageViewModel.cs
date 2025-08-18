using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.PageModels
{
    public partial class ChartTestPageViewModel : ObservableObject
    {
        // https://livecharts.dev/docs/maui/2.0.0-rc5.4/Overview.Automatic%20updates
        [ObservableProperty]
        private ObservableCollection<int> _ints =new ObservableCollection<int>() {1,2,3}; // with List<int> this won't work.
        Random random = new Random();


        public ISeries[] Series2 { get;}
        public ISeries[] MyIntsSeries { get; }

        [ObservableProperty]
        private ObservableCollection<DateTimePoint> _dateTimePoints = new ObservableCollection<DateTimePoint>();

        public ISeries[] MyWorkingHours { get; }

        // Do tego jakoś ciężko się dostać...
        //public ISeries[] DateSeries { get; set; } =
        //    [
        //        // Both ColumnSeries and DataTimePoint implement INotifyPropertyChanged
        //        new ColumnSeries<DateTimePoint>
        //            {
        //                Values = [
        //                    new() { DateTime = new(2025, 8, 12), Value = 3 },
        //                    new() { DateTime = new(2025, 8, 13), Value = 6 },
        //                    new() { DateTime = new(2025, 8, 14), Value = 5 }
        //                        ]
        //            }
        //    ];
         
        int kulfon = 15;

        DatabaseFun database;


        private void ChartTestPageViewModel_PointMeasured(LiveChartsCore.Kernel.ChartPoint<DateTimePoint, LiveChartsCore.SkiaSharpView.Drawing.Geometries.RoundedRectangleGeometry, LiveChartsCore.SkiaSharpView.Drawing.Geometries.LabelGeometry> obj)
        {

        }

        public ChartTestPageViewModel(DatabaseFun db)
        {
            database = db;

            var cc = new ColumnSeries<DateTimePoint>()
            {
                Values = DateTimePoints,
                Name = "Kokoszka",
             //   Fill = new SolidColorPaint(SKColors.Beige),
             //   Rx = 23,
            //    Ry = 23,
                 Stroke = new SolidColorPaint(SKColors.Transparent) { StrokeThickness = 0 },
            };
            cc.PointMeasured += (chartPoint) => {
                double y = chartPoint.Coordinate.PrimaryValue;
                Trace.WriteLine($"value y {y}");
                double t = Math.Clamp( y / 36.0, 0, 1);  // normalize 0..6 → 0..1
                byte r = (byte)(255 * (1 - t));
                byte g = (byte)(255 * t);
                var color = new SKColor(r, g, 0);

                chartPoint.Visual.Fill = new SolidColorPaint(color);
            
            };

            cc.ChartPointPointerHover += (sender, chartPoint) => {
                var color = SKColors.Beige;
                chartPoint.Visual.Stroke = new SolidColorPaint(color) { StrokeThickness = 4 };
                chartPoint.Context.Series.Name = chartPoint.Coordinate.PrimaryValue.ToString();
            };

            cc.ChartPointPointerHoverLost += (sender, chartPoint) =>
            {
                chartPoint.Visual.Stroke = new SolidColorPaint(SKColors.Transparent) { StrokeThickness = 0 };
            };

            MyWorkingHours = new ISeries[]{cc};

            for (int i = 0; i < 6; i++)
            {
                DateTimePoints.Add(new DateTimePoint() { DateTime = new(2025, 8, kulfon), Value = kulfon });
                kulfon++;
            }


            // since _ints is of type ObservableCollection 
            // LiveCharts will update when you add, remove, replace or clear the collection

            Series2 = new ISeries[] { new LineSeries<int>() { Values = Ints} };         // This results in line plot
            MyIntsSeries = new ISeries[] {new ColumnSeries<int>() { Values = Ints} };   // This results in columns

            // This results in THREE different types drawn values on sigle plot
            MyIntsSeries = new ISeries[]
            {
                new ColumnSeries<int>() { Values = Ints},
                new LineSeries<int>() { Values = Ints},
                new ScatterSeries<int>() {Values = Ints}
            };
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

            [RelayCommand]
            public async Task DodajDoDat()
            {
            await Task.Delay(1000);
                DateTimePoints.Add(new DateTimePoint() { DateTime = new DateTime(2025, 8, kulfon), Value = kulfon++ });
            }

            [RelayCommand]
            public async Task LoadTimeFromDb()
            {
                List<WorkTime> d = await database.GetWorkingEntriesForTimePeriodAsync(new DateTime(2025, 8, 1), new DateTime(2025, 8, 30));
                var group = d.GroupBy(wt => DateTime.Parse(wt.StartTime).ToString("yyyy MM dd"));
                DateTimePoints.Clear();
                foreach (var w in group)
                {
                    Trace.WriteLine($"{w.Key} {w.Count()}"); 
                    DateTimePoints.Add(new DateTimePoint() {DateTime = DateTime.Parse(w.Key) , Value = w.Aggregate(0, (sum, wt) => sum += (int)wt.Duration().TotalSeconds) });
                }
            }

        }
}
