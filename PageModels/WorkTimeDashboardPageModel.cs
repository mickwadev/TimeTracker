using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        }
    }
}
