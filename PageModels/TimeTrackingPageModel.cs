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

        public TimeTrackingPageModel(DatabaseFun db) 
        {
            this.db = db;
        }

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd");

        [RelayCommand]
        private void TimeCountingButtonPressed()
        {
            Trace.WriteLine("Time counting...");
        }
    }
}
