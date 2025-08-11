using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;

namespace TimeTracker.PageModels
{
    public partial class TimeTrackingPageModel : ObservableObject
    {
        DatabaseFun db;
        public TimeTrackingPageModel(DatabaseFun db) 
        {
            this.db = db;
        }

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd");
    }
}
