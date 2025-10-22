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
using TimeTracker.Helpers;
using TimeTracker.Models;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp.Views.Maui;
using TimeTracker.Models.Database;
using TimeTracker.Extensions;

namespace TimeTracker.PageModels
{
    public partial class TimeTrackingPageViewModel : ObservableObject
    {
        // Page registered in Shell with ShellContent ContentTemplate acts like singleton.
        public int a = 0;
        public static int sa = 0;
        DatabaseFun db;
        SupabaseClient _client;
        IDispatcherTimer timer;
        WorkTime currentWorkTime = null;
        TimeSpan todaysWorkingTime = TimeSpan.Zero;


        public TimeTrackingPageViewModel(DatabaseFun db, SupabaseClient client)
        {
            timer = Application.Current!.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.IsRepeating = true;
            timer.Tick += (s, e) =>
            {
                //    var startTime = DateTime.ParseExact(currentWorkTime!.StartTime,DbConsts.dbDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                currentWorkTime.EndTime = DateTime.Now;//.ToString();
                
                TicksMsg = $"Current working: {currentWorkTime.Duration.ToString(@"hh\:mm\:ss")}";
                UpdateTodaysWorkTimeText();
            };
            this.db = db;
            _client = client;
            UpdateTimeButtonText();
        }

        [ObservableProperty]
        private string _todayWorkTime= string.Empty;

        [ObservableProperty]
        private Color _textColor;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd");

        [ObservableProperty]
        private string _ticksMsg = $"";

        [ObservableProperty]
        private string _workTimeComment = "";

        [ObservableProperty]
        private string _countTimeButtonText;

        [RelayCommand]
        private async Task TimeCountingButtonPressed()
        {
            Trace.WriteLine("Time counting button pressed...");
            if (timer.IsRunning)
            {
              await StopWorkTimeCounting();
            }
            else
            {
                StartWorkTimeCounting();
            }
            UpdateTimeButtonText();
        }

        // This page is registered in Shell, so this page model behaves as singleton, but this event is triggered each time when page is appearing
        [RelayCommand]
        public async Task AppearingEvent()
        {
            Trace.WriteLine($"Appearing event in vm :) {++a}/{++sa}");
            await LoadTodayWorkTimesFromDb();
        }

        private async Task LoadTodayWorkTimesFromDb()
        {
            var todayWorkEntries = await db.GetTodayWorkingEntriesAsync();
            todaysWorkingTime = db.SumTimeSpans(todayWorkEntries);
            UpdateTodaysWorkTimeText();
        }

        private void UpdateTodaysWorkTimeText()
        {
            TimeSpan current = TimeSpan.Zero;
            if (currentWorkTime is not null)
            {
                current = currentWorkTime.Duration;
            }
            TodayWorkTime = "Today work time: " + (todaysWorkingTime + current).ToString(@"hh\:mm\:ss");
            TextColor = GradientSampler.GetWorkTimeColor(todaysWorkingTime).ToMauiColor();
        }
         
        

        [RelayCommand]
        private async Task GetDates()
        {
            Trace.WriteLine("Dzisiejsze wpisy...");
            var todayWorkEntries = await db.GetTodayWorkingEntriesAsync();
            var todayWorkTime = db.SumTimeSpans(todayWorkEntries);
            WorkTimeComment = $"You worked today: {todayWorkTime.ToString(@"hh\:mm\:ss")}";
        }
         
        private async Task StopWorkTimeCounting()
        {
            timer.Stop();
            currentWorkTime.EndTime = DateTime.Now.ToDBFormat();
            currentWorkTime.Title = WorkTimeComment;
            Trace.WriteLine($"Saving activity: '{currentWorkTime.StartTime}'to '{currentWorkTime.EndTime}'");
            var affectedRows = await db.AddWorkTimeAsync(currentWorkTime);
            Trace.WriteLine($"Affected rows: {affectedRows}");
            currentWorkTime = null;
        }

        private void StartWorkTimeCounting()
        {
            var st = DateTime.Now.ToDBFormat();
            currentWorkTime = new WorkTime() { 
                StartTime = st, 
                EndTime = st, 
                Title = WorkTimeComment,
                backupID = AppConsts.NoBackupID,
                User = "KKK",// _client.LoggedUser.UserName,
                ActivityType  = AppConsts.ActivityNotSet
            };
            Trace.WriteLine($"Starting new activity from: '{currentWorkTime.StartTime}'");
            timer.Start();
        }

        private void UpdateTimeButtonText()
        {
            CountTimeButtonText = (timer.IsRunning ? "Stop" : "Start") + " counting time";
        }
    }
}
