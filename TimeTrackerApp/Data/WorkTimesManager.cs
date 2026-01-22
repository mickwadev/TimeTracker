using DateRangeHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Models;
using Database.SQLiteDB;

namespace TimeTracker.Data
{
    public class WorkTimesManager
    {
        private DatabaseFun _db;
        private List<WorkTime> _workTimes;
        private DateTime _startDate;
        private IGetDateRange _dataRangeSource;
        const string dateFormat = "yyyy-MM-dd";

        public WorkTimesManager(DatabaseFun db) 
        { 
            _db = db;
            _startDate = DateTime.Today;
        }

        public async Task Initialize()
        { 
            _workTimes = await _db.GetAllWorkingEntriesAsync();
        }

        public void GoToNextDataRange()
        { 
         _dataRangeSource.GoToNextDateRange();
        }

        public void GoToPreviousDataRange()
        {
            _dataRangeSource.GoToPreviousDateRange();
        }

        public void ChangeDataSource(string period)
        {
            _dataRangeSource = GetDataRangeFactory(Enum.Parse<PeriodType>(period));
        }

        

        public async Task<List<WorkTime>> GetUpdateWorkTimesForDateRange()
        {
            DateRange r = _dataRangeSource.GetCurrentDateRange();
            DateTime startDate = new DateTime(r.startDate.Year, r.startDate.Month, r.startDate.Day);
            DateTime endDate = new DateTime(r.endDate.Year, r.endDate.Month, r.endDate.Day, 23, 59,59);
            Trace.WriteLine($"[KKK] All dates are in range: {startDate} to {endDate}");
            // this does not have to be async, but lets test if this works
            return await Task.FromResult(_workTimes.Where(wt => wt.StartTime >= startDate && wt.EndTime <= endDate).ToList<WorkTime>());
        }
         
        public string GetTimePeriodInfo()
        { 
            DateRange dateRange = _dataRangeSource.GetCurrentDateRange();
            return $"From {dateRange.startDate.ToString(dateFormat)} to {dateRange.endDate.ToString(dateFormat)}";
        }

        public (long min, long max) GetTicksRange => _dataRangeSource.GetTicksRange();

        private IGetDateRange GetDataRangeFactory(PeriodType period)
        {
            IGetDateRange range = period switch
            {
                PeriodType.WEEK => new DateTimeWeekHelper(_startDate),
                PeriodType.MONTH => new DateTimeMonthHelpers(_startDate),
                PeriodType.YEAR => new DateTimeYearHelpers(_startDate),
                PeriodType.ALL => GetAllDataRangeHelper()
            };
            return range;
        }

        private IGetDateRange GetAllDataRangeHelper()
        {
            DateTime start = _workTimes.Min(wt => wt.StartTime);
            DateTime end = _workTimes.Max(wt => wt.StartTime);
            Trace.WriteLine($"All {_workTimes.Count} dates are in range: {start} to {end}");
            return new DateTimeAllHelper(start, end);
        }

        public IEnumerable<WorkTime> GetEntriesForDate(DateTime day)
        {
           return _workTimes.Where(wt =>  SameDay(wt.StartTime,day) );
        }

       private bool SameDay(DateTime d1, DateTime d2) => d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
    }
}
