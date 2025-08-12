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
    public partial class TimeTrackingPageModel : ObservableObject
    {
        DatabaseFun db;
        IDispatcherTimer timer;
        int ticks = 0;
        DateTime startTime;

        public TimeTrackingPageModel(DatabaseFun db)
        {
            timer = Application.Current!.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsRepeating = true;
            timer.Tick += (s, e) =>
            {
                ticks++;
                TicksMsg = $"Ticks: {ticks}";
            };
            this.db = db;
        }

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd");

        [ObservableProperty]
        private string _ticksMsg = $"Ticks: -1";

        [ObservableProperty]
        private string _workTimeComment = "";

        [RelayCommand]
        private void TimeCountingButtonPressed()
        {
            Trace.WriteLine("Time counting button pressed...");
            if (timer.IsRunning)
            {
                StopWorkTimeCounting();
            }
            else
            {
                StartWorkTimeCounting();
            }
        }
         
        [RelayCommand]
        private async Task CreateDatabase()
        {
            await db.AddTimesTableAsync();
        }

        [RelayCommand]
        private async Task GetDates()
        {
            Trace.WriteLine("Dzisiejsze wpisy...");
            var todayWorkEntries = await db.GetTodayWorkingEntriesAsync();
            TimeSpan todayWorkTime = TimeSpan.FromSeconds(0);
            foreach (var todayWorkEntry in todayWorkEntries)
            {
                todayWorkTime += todayWorkEntry.Duration();
            }
            WorkTimeComment = $"You worked today: {todayWorkTime.ToString(@"hh\:mm\:ss")}";
        }

        private void StopWorkTimeCounting()
        {
            timer.Stop();
            var format = "yyyy-MM-dd HH:MM:ss"; // this format is required for sqlite date functions to work.
            var wt = new WorkTime() { StartTime = startTime.ToString(format) , EndTime = DateTime.Now.ToString(format), Title = WorkTimeComment };
            Task.Run(async () => await db.AddWorkTimeAsync(wt));
        }

        private void StartWorkTimeCounting()
        {
            timer.Start();
            startTime = DateTime.Now;
        }
    }
}
