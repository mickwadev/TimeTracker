using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    public class WorkTimesManager
    {
        private DatabaseFun _db;
        private List<WorkTime> _workTimes;
        public WorkTimesManager(DatabaseFun db) 
        { 
            _db = db;
        }
        public async Task Initialize()
        {
            await _db.Initialize();
            _workTimes = await _db.GetAllWorkingEntriesAsync(); 
        }

        public void GetAllWorkingTime()
        {
            //DateOnly start = 
            DateTime start = _workTimes.Min(wt => wt.StartTime);
            DateTime end = _workTimes.Max(wt => wt.StartTime);
          //  Trace.WriteLine(A);
        }
    }
}
