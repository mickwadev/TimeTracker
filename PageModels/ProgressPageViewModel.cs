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
            TimePeriod = p.start == p.end ? p.start.ToString(f) :  $"From {p.start.ToString(f)} to {p.end.ToString(f)}";
        }
        #endregion

        // https://livecharts.dev/docs/maui/2.0.0-rc5.4/Overview.Automatic%20updates
        [ObservableProperty]
        private ObservableCollection<int> _ints =new ObservableCollection<int>() {1,2,3}; // with List<int> this won't work.
        Random random = new Random();


        public ISeries[] Series2 { get;}
        public ISeries[] MyIntsSeries { get; }

        [ObservableProperty]
        private ObservableCollection<DateTimePoint> _dateTimePoints = new ObservableCollection<DateTimePoint>();

        public ISeries[] MyWorkingHours { get; }

        int kulfon = 15;

        [ObservableProperty]
        private string _timePeriod = "";

        DatabaseFun database;

        public Axis[] YAxes { get; } = new Axis[] {
            new Axis() {
                MinLimit = 0,
                MinStep = 1,
                Name = "Working hours",
                    NamePaint = new SolidColorPaint(SKColors.Beige),
                    
                    LabelsPaint = new SolidColorPaint(SKColors.Beige),
                    TextSize = 12,

                    SeparatorsPaint = new SolidColorPaint(SKColors.DarkGray) { StrokeThickness = 1 }
            }
        };

        public ICartesianAxis[] XAxes { get; set; } =
            [
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd MM")),
            ];





        public ProgressPageViewModel(DatabaseFun db)
        {
            database = db;

            var cc = new ColumnSeries<DateTimePoint>()
            {
                Values = DateTimePoints,
                //  Name = "Kokoszka",
                //   Fill = new SolidColorPaint(SKColors.Beige),
                //   Rx = 23,
                //    Ry = 23,
                Stroke = new SolidColorPaint(SKColors.Transparent) { StrokeThickness = 0 },
                YToolTipLabelFormatter = p => TimeSpan.FromSeconds((int)(p.Model.Value)).ToString(@"hh\:mm\:ss")
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
           //     chartPoint.Context.Series.Name = chartPoint.Coordinate.PrimaryValue.ToString();
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

        public async Task LoadTimeFromDb(DateTime start, DateTime end)
        {
            List<WorkTime> d = await database.GetWorkingEntriesForTimePeriodAsync(start, end);
            var group = d.GroupBy(wt => DateTime.Parse(wt.StartTime).ToString("yyyy MM dd"));
            DateTimePoints.Clear();
            foreach (var w in group)
            {
                Trace.WriteLine($"For '{w.Key}' found {w.Count()} activities");
                DateTimePoints.Add(new DateTimePoint() { DateTime = DateTime.Parse(w.Key), Value = w.Aggregate(0, (sum, wt) => 
                {
                    var v = (int)wt.Duration().TotalSeconds;
                    Trace.WriteLine($"Adding total seconds: {v}");
                    sum += v;
                    return sum;
                }) });
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
                    DateTimePoints.Add(new DateTimePoint() {DateTime = DateTime.Parse(w.Key) , Value = w.Aggregate(0, (sum, wt) => sum += (int)wt.Duration().TotalSeconds) });
                }
            }
        }
}
