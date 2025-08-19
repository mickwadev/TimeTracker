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
        DateTime startTime;

        public TimeTrackingPageModel(DatabaseFun db)
        {
            timer = Application.Current!.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsRepeating = true;
            timer.Tick += (s, e) =>
            {
                TimeSpan sp = (DateTime.Now - startTime); 
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
        private async Task GetDates()
        {
            Trace.WriteLine("Dzisiejsze wpisy...");
            var todayWorkEntries = await db.GetTodayWorkingEntriesAsync();
            WorkTimeComment = db.SumTimeSpans(todayWorkEntries);
        }
         
        private void StopWorkTimeCounting()
        {
            timer.Stop();
            var format = "yyyy-MM-dd HH:MM:ss"; // this format is required for sqlite date functions to work.
            var st = startTime.ToString(format);
            var et = DateTime.Now.ToString(format);
            Trace.WriteLine($"Saving activity: '{st}'to '{et}'");
            var wt = new WorkTime() { StartTime = st , EndTime = et , Title = WorkTimeComment };
            Task.Run(async () => await db.AddWorkTimeAsync(wt));
             
        }

        private void StartWorkTimeCounting()
        {
            startTime = DateTime.Now;
            timer.Start();
        }

        private void UpdateTimeButtonText()
        {
            CountTimeButtonText = (timer.IsRunning ? "Stop" : "Start") + " counting time";
        }
    }
}
