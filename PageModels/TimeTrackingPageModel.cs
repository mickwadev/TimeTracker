using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        WorkTime currentWorkTime = null;

        public TimeTrackingPageModel(DatabaseFun db)
        {
            timer = Application.Current!.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsRepeating = true;
            timer.Tick += (s, e) =>
            {
                var startTime = DateTime.ParseExact(currentWorkTime!.StartTime,DbConsts.dbDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                TimeSpan sp = (DateTime.Now -  startTime); 
                TicksMsg = $"Ticks: {(int)sp.TotalSeconds}";
            };
            this.db = db;
            UpdateTimeButtonText();
        }

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd");

        [ObservableProperty]
        private string _ticksMsg = $"Ticks: -1";

        [ObservableProperty]
        private string _workTimeComment = "";

        [ObservableProperty]
        private string _countTimeButtonText;

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
            UpdateTimeButtonText();
        }
         
        [RelayCommand]
        private async Task CreateDatabase()
        {
            await db.AddTimesTableAsync();
        }

        [RelayCommand]
        private async Task DropTable()
        { 
           await db.DropTableAsync();
        }

        [RelayCommand]
        private async Task GetDates()
        {
            Trace.WriteLine("Dzisiejsze wpisy...");
            var todayWorkEntries = await db.GetTodayWorkingEntriesAsync();
            WorkTimeComment = db.SumTimeSpans(todayWorkEntries);
        }
         
        private void StopWorkTimeCounting()
        {
            timer.Stop();
            var et = DateTime.Now.ToString(DbConsts.dbDateFormat);
            currentWorkTime.EndTime = et;
            Trace.WriteLine($"Saving activity: '{currentWorkTime.StartTime}'to '{currentWorkTime.EndTime}'");
            Task.Run(async () => await db.AddWorkTimeAsync(currentWorkTime));
        }

        private void StartWorkTimeCounting()
        {
            var st = DateTime.Now.ToString(DbConsts.dbDateFormat);
            currentWorkTime = new WorkTime() { StartTime = st, EndTime = null, Title = WorkTimeComment };
            Trace.WriteLine($"Starting new activity from: '{currentWorkTime.StartTime}'");
            timer.Start();
        }

        private void UpdateTimeButtonText()
        {
            CountTimeButtonText = (timer.IsRunning ? "Stop" : "Start") + " counting time";
        }
    }
}
