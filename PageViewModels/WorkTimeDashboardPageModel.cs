using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.PageModels
{
    public partial class WorkTimeDashboardPageModel: ObservableObject
    {
        DatabaseFun db;
        public WorkTimeDashboardPageModel(DatabaseFun db) 
        {
            this.db = db;
            Day = DateTime.Today.ToString("dd");
            Month = DateTime.Today.ToString("MM");
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ButtonText))]
        string _day;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ButtonText))]
        string _month;

        // Cool! This updates when Day or Month changes ^_^
        public string ButtonText => $"Get learning time for {Day} {Month}";


        [ObservableProperty]
        private string _timeLabel = "";

        [RelayCommand]
        public async Task GetDatabaseEntriesForDate()
        {
            Trace.WriteLine($"Check db for:{Day} {Month}");
            DateTime d = new DateTime(2025, int.Parse(Month), int.Parse(Day));
            var times = await db.GetWorkingEntriesForTimePeriodAsync(d, d);
            var todayWorkTime = db.SumTimeSpans(times);
            TimeLabel = $"In {Day}.{Month}.2025 you worked: {todayWorkTime.ToString(@"hh\:mm\:ss")}";
        }

        [RelayCommand]
        public async Task LoadAllWorkingHistory()
        {
            List<WorkTime> times = await db.GetWorkingEntriesForTimePeriodAsync(new DateTime(2025, 1, 1), DateTime.Today);
            var grouped = times.GroupBy(wt => (wt.StartTime).ToString("yyyy MM dd"));
            var d = new ColumnSeries<DateTimePoint>
            {
                Values = [
                new() { DateTime = new(2025, 8, 12), Value = 1 },
                new() { DateTime = new(2025, 8, 13), Value = 2 },
                new() { DateTime = new(2025, 8, 14), Value = 3 },
            ]
            };
            Trace.WriteLine("...");
            DateSeries = [d];
        }

        [ObservableProperty]
        private ISeries[] _dateSeries; 

        //public ISeries[] Series { get; set; } = [
        // new ColumnSeries<DateTimePoint>
        //{
        //    Values = [
        //        new() { DateTime = new(2025, 8, 12), Value = 3 },
        //        new() { DateTime = new(2025, 8, 13), Value = 6 },
        //        new() { DateTime = new(2025, 8, 14), Value = 5 },
        //        //new() { DateTime = new(2021, 1, 4), Value = 3 },
        //        //new() { DateTime = new(2021, 1, 5), Value = 5 },
        //        //new() { DateTime = new(2021, 1, 6), Value = 8 },
        //        //new() { DateTime = new(2021, 1, 7), Value = 6 }
        //    ]
        //}
     //];

        public ICartesianAxis[] XAxes { get; set; } =
            [
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd MM")),

            ];

    }
}
