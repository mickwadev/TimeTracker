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
        WorkTimesManager _workTimeManager;
        public ProgressPageViewModel(DatabaseFun db, WorkTimesManager workTimesManager)
        {
            database = db;
            _workTimeManager = workTimesManager;
            SetChart();
        }

        #region SEGMENT

        public List<SfSegmentItem> Segments { get; } = new List<SfSegmentItem>()
        {
            new SfSegmentItem(){Text = PeriodType.WEEK.ToString()},
            new SfSegmentItem(){Text = PeriodType.MONTH.ToString()},
            new SfSegmentItem(){Text = PeriodType.YEAR.ToString()},
            new SfSegmentItem(){Text = PeriodType.ALL.ToString() }
        };

        [RelayCommand]
        public async Task TimeRange_SelectionChanged(Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            Trace.WriteLine($"Change: {e.OldIndex} ==> {e.NewIndex}  {Segments[(int)e.NewIndex!].Text}");
            string f = "yyyy-MM-dd";
            _workTimeManager.ChangeDataSource(Segments[(int)e.NewIndex!].Text);
            
             await LoadTimeFromDb();
         // await Task.Delay(1000);
            // Update label text:
            TimePeriod = _workTimeManager.GetTimePeriodInfo();
        }
        #endregion

        //DateTimePoint to jest klasa z tych charts.
        [ObservableProperty]
        private ObservableCollection<DateTimePoint> _dateTimePoints = new ObservableCollection<DateTimePoint>();

        // Do tego jest zrobiony binding: Series="{Binding MyWorkingHours}"
        [ObservableProperty]
        private ISeries[] _MyWorkingHours;

        [ObservableProperty]
        private string _timePeriod = "";

        DatabaseFun database;

        [ObservableProperty]
        private bool _hasAnyEntries = true;

        // This must be static to add it in Labeler while creating it
        private static string TimeLabeler(double seconds) => (seconds / AppConsts.SecondsInHours) + "h";

        #region AXIS_DEFINITIONS
        public ICartesianAxis[] YAxes { get; } = {
            new Axis {
                MinLimit = 0,
                MaxLimit= AppConsts.SecondsInHours*10,
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
                 new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd MM yyyy"))
                 {
                     MinStep = TimeSpan.FromDays(1).Ticks,
                     MinLimit = DateTime.Now.AddDays(-10).Ticks,
                     MaxLimit= DateTime.Now.AddDays(5).Ticks,
                     ForceStepToMin = true,
                     Name="Dates",
                     NamePaint = new SolidColorPaint(DeviceInfo.Current.Platform == DevicePlatform.Android ? SKColors.DarkGray :  SKColors.Beige),
                    // LabelsPaint = new SolidColorPaint(DeviceInfo.Current.Platform == DevicePlatform.Android? SKColors.DarkGray  : SKColors.Beige),
                 }
            ];

        #endregion
        private void SetChart()
        {
            // Add some fake DateTimePoints:
            DateTimePoints.Add(new DateTimePoint(new DateTime(2025, 7, 2), 7200));
            DateTimePoints.Add(new DateTimePoint(new DateTime(2025, 8, 12), 7200));
            DateTimePoints.Add(new DateTimePoint(new DateTime(2025, 8, 13), 7200));

            Trace.WriteLine($"Points here {DateTimePoints.Count}");
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

            // Do tego jest zrobiony binding w xaml.
            MyWorkingHours = new ISeries[] { cc };
        }

        // Tu są aktualizowane te DateTimePoints z bazy danych:
        public async Task LoadTimeFromDb()
        {
            List<WorkTime> d = await _workTimeManager.GetUpdateWorkTimesForDateRange();// database.GetWorkingEntriesForTimePeriodAsync(start, end);
            var group = d.GroupBy(wt => wt.StartTime.ToString("yyyy MM dd"));
            DateTimePoints.Clear();
            foreach (var w in group)
            {
                Trace.WriteLine($"For '{w.Key}' found {w.Count()} activities");
                DateTimePoints.Add(new DateTimePoint()
                {
                    DateTime = DateTime.Parse(w.Key),
                    // Sum seconds of all activities
                    Value = w.Aggregate(0, (sum, wt) =>
                    {
                        var v = (int)wt.Duration.TotalSeconds;
                        Trace.WriteLine($"Adding total seconds: {v}");
                        sum += v;
                        return sum;
                    })
                });
            }
            (long min, long max) ticks = _workTimeManager.GetTicksRange;
            foreach (var x in XAxes)
            {
                x.MinLimit = ticks.min;// start.Ticks;
                x.MaxLimit = ticks.max;// end.Ticks;
            }

            //foreach (var y in YAxes)
            //{
            //    y.MinLimit = null;
            //    y.MaxLimit = null;
            //}
        }
    }
}
