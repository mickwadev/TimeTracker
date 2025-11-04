using DateRangeHelpers;
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
            await _db.Initialize(AppDbConsts.connectionString);
            _workTimes = await _db.GetAllWorkingEntriesAsync();
            if (_workTimes is null) 
            {
                Trace.WriteLine("XXXXXXXXXXX _workTimes null!!!!");    
            }
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
            DateTime endDate = new DateTime(r.endDate.Year, r.endDate.Month, r.endDate.Day);
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
            return new DateTimeAllHelper(start, end);
        }
    }
}
