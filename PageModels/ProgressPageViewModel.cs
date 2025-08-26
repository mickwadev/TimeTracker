using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using Syncfusion.Maui.Toolkit.Buttons;
using Syncfusion.Maui.Toolkit.SegmentedControl;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using TimeTracker.Data;
using TimeTracker.Helpers;
using TimeTracker.Models;

namespace TimeTracker.PageModels
{
    public partial class ProgressPageViewModel : ObservableObject
    {
        #region SEGMENT

        public List<SfSegmentItem> Segments { get; } = new List<SfSegmentItem>()
        {
            new SfSegmentItem(){Text = "Today"},
            new SfSegmentItem(){Text = "Week"},
            new SfSegmentItem(){Text = "Month"},
            new SfSegmentItem(){Text = "All"}
        };



        [RelayCommand]
        public async Task TimeRange_SelectionChanged(Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            Trace.WriteLine($"zmiana: {e.OldIndex} ==> {e.NewIndex}  {Segments[(int)e.NewIndex!].Text}");
            string f = "yyyy-MM-dd";
            var p = Segments[(int)e.NewIndex!].Text switch
            {
                "Today" => Dates.Today,
                "Week" => Dates.ThisWeek,
                "Month" => Dates.ThisMonth,
                "All" => Dates.All,
                _ => Dates.All
            };
            await LoadTimeFromDb(p.start, p.end);
            TimePeriod = p.start == p.end ? p.start.ToString(f) : $"From {p.start.ToString(f)} to {p.end.ToString(f)}";
        }
        #endregion
 
        Random random = new Random();
         
        [ObservableProperty]
        private ObservableCollection<DateTimePoint> _dateTimePoints = new ObservableCollection<DateTimePoint>();

        public ISeries[] MyWorkingHours { get; }

        

        [ObservableProperty]
        private string _timePeriod = "";

        DatabaseFun database;

        [ObservableProperty]
        private bool _hasAnyEntries = false;

        // This must be static to add it in Labeler while creating it
        private static string TimeLabeler(double seconds) => (seconds / AppConsts.SecondsInHours) + "h";

        // this function must match Func<double, string>
        private string MyCustomLabelFormatter(double value)
        {
            if (value > 1000)
                return $"{value / 1000:0.0}k";

            return value.ToString("0.##");
        }

        public Axis[] YAxes { get; } = new Axis[] {
            new Axis {
                MinLimit = 0,
                MinStep = AppConsts.SecondsInHours*2,
                Name = "Working hours",
                ForceStepToMin = true,
                Labeler = TimeLabeler,
                NamePaint = new SolidColorPaint(DeviceInfo.Current.Platform == DevicePlatform.Android ? SKColors.DarkGray :  SKColors.Beige),
                LabelsPaint = new SolidColorPaint(DeviceInfo.Current.Platform == DevicePlatform.Android? SKColors.DarkGray  : SKColors.Beige),
                TextSize = DeviceInfo.Current.Platform == DevicePlatform.Android? 7 : 12,
                SeparatorsPaint = new SolidColorPaint(SKColors.DarkGray) { StrokeThickness = 1 }
            }
        };

        public ICartesianAxis[] XAxes { get; set; } =
            [
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd MM")),
            ];

        public ProgressPageViewModel(DatabaseFun db)
        {
            var yAxesLabelColors = new OnPlatform<SKColor>
            {

            };

            database = db;

            var cc = new ColumnSeries<DateTimePoint>
            {
                Values = DateTimePoints,
                Stroke = new SolidColorPaint(SKColors.Transparent) { StrokeThickness = 0 },
                YToolTipLabelFormatter = p => TimeSpan.FromSeconds((int)(p.Model.Value)).ToString(@"hh\:mm\:ss")
            };
            cc.PointMeasured += (chartPoint) =>
            {
                double y = chartPoint.Coordinate.PrimaryValue;
                Trace.WriteLine($"value y {y}");
                chartPoint.Visual.Fill = new SolidColorPaint(GradientSampler.GetWorkTimeColor(y));

            };

            cc.ChartPointPointerHover += (sender, chartPoint) =>
            {
                var color = SKColors.Beige;
                chartPoint.Visual.Stroke = new SolidColorPaint(color) { StrokeThickness = 4 };
            };

            cc.ChartPointPointerHoverLost += (sender, chartPoint) =>
            {
                chartPoint.Visual.Stroke = new SolidColorPaint(SKColors.Transparent) { StrokeThickness = 0 };
            };

            MyWorkingHours = new ISeries[] { cc };
             
        }
         
        public async Task LoadTimeFromDb(DateTime start, DateTime end)
        {
            List<WorkTime> d = await database.GetWorkingEntriesForTimePeriodAsync(start, end);
            var group = d.GroupBy(wt => DateTime.Parse(wt.StartTime).ToString("yyyy MM dd"));
            DateTimePoints.Clear();
            foreach (var w in group)
            {
                Trace.WriteLine($"For '{w.Key}' found {w.Count()} activities");
                DateTimePoints.Add(new DateTimePoint()
                {
                    DateTime = DateTime.Parse(w.Key),
                    Value = w.Aggregate(0, (sum, wt) =>
                {
                    var v = (int)wt.Duration().TotalSeconds;
                    Trace.WriteLine($"Adding total seconds: {v}");
                    sum += v;
                    return sum;
                })
                });
            }
        }

        [RelayCommand]
        public async Task LoadTimeFromDb()
        {
            List<WorkTime> d = await database.GetWorkingEntriesForTimePeriodAsync(new DateTime(2025, 8, 1), new DateTime(2025, 8, 30));
            var group = d.GroupBy(wt => DateTime.Parse(wt.StartTime).ToString("yyyy MM dd"));
            DateTimePoints.Clear();
            foreach (var w in group)
            {
                //Trace.WriteLine($"{w.Key} {w.Count()}"); 
                DateTimePoints.Add(new DateTimePoint() { DateTime = DateTime.Parse(w.Key), Value = w.Aggregate(0, (sum, wt) => sum += (int)wt.Duration().TotalSeconds) });
            }
        }
    }
}
