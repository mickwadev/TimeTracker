using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;
 
namespace TimeTracker.PageModels
{
    public partial class TimeTrackingPageModel : ObservableObject
    {
        DatabaseFun db;
        private DateTime _startTime, _endTime;
        private bool learningInProgress = false;
        IDispatcherTimer timer;
        int ticks = 0;

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

        [RelayCommand]
        private void TimeCountingButtonPressed()
        {
            Trace.WriteLine("Time counting...");
            StartWorkTimeCounting();
        }

        private void StartWorkTimeCounting()
        {
            if(!timer.IsRunning)
            timer.Start(); 
        }
    }
}
