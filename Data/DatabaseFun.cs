using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Models;

namespace TimeTracker.Data
{
    // to jest dodane w builderze do Sevices
    public class DatabaseFun
    {
        const string CurrentTable = "TimesTable";
        public async Task DropTableAsync()
        {
            Trace.WriteLine($"Using connection string: {DbConsts.connectionString}");
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var dropTableCommand = connection.CreateCommand();
            dropTableCommand.CommandText = @$"
                DROP TABLE IF EXISTS {CurrentTable};";
            await dropTableCommand.ExecuteNonQueryAsync();
        }

        public async Task AddTimesTableAsync()
        {
            Trace.WriteLine($"Using connection string: {DbConsts.connectionString}");
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @$"
CREATE TABLE IF NOT EXISTS {CurrentTable} (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT NOT NULL
);";
            await createTableCommand.ExecuteNonQueryAsync();
        }

        public async Task CheckIfTableExistsAsync()
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var checkIfTableExistCmd = connection.CreateCommand();
            checkIfTableExistCmd.CommandText = @$"
            SELECT name FROM sqlite_master WHERE type='table' AND name='{CurrentTable}';";
            await using var reader = await checkIfTableExistCmd.ExecuteReaderAsync();
            bool tableExists = await reader.ReadAsync();
            Trace.WriteLine($"Table {CurrentTable} exists: {tableExists}");
        }

        public WorkTime GetRandomWorkTime()
        {
            DateTime start = DateTime.Now;
            return new WorkTime() { Title = "Random 10 minute ;)", StartTime = start.ToString(), EndTime = (start + TimeSpan.FromMinutes(10)).ToString() };
        }

        public async Task<int> ClearTableAsync()
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {CurrentTable}";
            return await cmd.ExecuteNonQueryAsync(); // returns number of rows deleted
        }

        public async Task<int> CreateFromBackupAsync(List<WorkTime> workEntries)
        {
            int removedRows = await ClearTableAsync();
            Trace.WriteLine($"Removed rows: {removedRows}");
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            // co to za dziwny syntax XD
            await using var tx = await connection.BeginTransactionAsync();
            try
            {
                await using var cmd = connection.CreateCommand();
                cmd.Transaction = (SqliteTransaction) tx;
                cmd.CommandText = $@"INSERT INTO {CurrentTable} (Title, StartTime, EndTime) VALUES (@Title, @StartTime, @EndTime);";

                // Define parameters once (faster when reusing)
                var pTitle = cmd.Parameters.Add("@Title", SqliteType.Text);
                var pStart = cmd.Parameters.Add("@StartTime", SqliteType.Text);   // or SqliteType.Integer if ticks/epoch
                var pEnd = cmd.Parameters.Add("@EndTime", SqliteType.Text);

                // Optional: prepare for better perf
                cmd.Prepare();
                var affected = 0;
                foreach (var r in workEntries)
                {
                    pTitle.Value = r.Title ?? (object)DBNull.Value;
                    pStart.Value = r.StartTime;  // ensure this matches your column type
                    pEnd.Value = r.EndTime;

                    affected += await cmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
                return affected;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Something failed... ;( {ex.Message}");
            }
            return 42;
        }

        public async Task<int> AddWorkTimeAsync(WorkTime r)
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var addWorkTimeCmd = connection.CreateCommand();
            addWorkTimeCmd.CommandText = @$"
            INSERT INTO {CurrentTable} (Title, StartTime, EndTime) VALUES (@Title, @StartTime, @EndTime)";
            addWorkTimeCmd.Parameters.AddWithValue("@Title", r.Title);
            addWorkTimeCmd.Parameters.AddWithValue("@StartTime", r.StartTime);
            addWorkTimeCmd.Parameters.AddWithValue("@EndTime", r.EndTime);
            return await addWorkTimeCmd.ExecuteNonQueryAsync();
        }

        public async Task<int> AddRandomWorkTimeAsync() => await AddWorkTimeAsync(GetRandomWorkTime());

        public async Task<List<WorkTime>> GetTodayWorkingEntriesAsync() => await GetWorkingEntriesForTimePeriodAsync(DateTime.Now,DateTime.Now);
        public async Task<List<WorkTime>> GetAllWorkingEntriesAsync() => await GetWorkingEntriesForTimePeriodAsync(new DateTime(1410,7,15), DateTime.Now);



        public async Task<List<WorkTime>> GetWorkingEntriesForTimePeriodAsync(DateTime startDate, DateTime endDate)
        {
            string format = "yyyy-MM-dd ";
           
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var addWorkTimeCmd = connection.CreateCommand();
            var start = startDate.ToString(format);
            var end = endDate.ToString(format);
            Trace.WriteLine($"Time period days: {start} to {end}");
            addWorkTimeCmd.CommandText = @$"SELECT ID, Title, StartTime, EndTime FROM {CurrentTable} WHERE date(StartTime) >= date(@start) AND date(StartTime) <= date(@end);";
            addWorkTimeCmd.Parameters.AddWithValue("@start", start);
            addWorkTimeCmd.Parameters.AddWithValue("@end", end);
            await using var reader = await addWorkTimeCmd.ExecuteReaderAsync();
            List<WorkTime> times = new List<WorkTime>();
             
            while (await reader.ReadAsync())
            {
                var wt = new WorkTime()
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    StartTime = reader.GetString(2),
                    EndTime = reader.GetString(3),
                };
                Trace.WriteLine($"Pure data: '{wt.StartTime}' '{wt.EndTime}'");
                times.Add(wt);
                var startTime = DateTime.ParseExact(wt.StartTime,  DbConsts.dbDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                var endTime = DateTime.ParseExact(wt.EndTime, DbConsts.dbDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                var time = endTime - startTime;
                
                Trace.WriteLine($"Time span from activity: {time} ({startTime.ToString(DbConsts.dbDateFormat)} - {endTime.ToString(DbConsts.dbDateFormat)})");
            }
            return times;
        }
        public string SumTimeSpans(List<WorkTime> todayWorkEntries)
        {
            TimeSpan todayWorkTime = TimeSpan.FromSeconds(0);
            foreach (var todayWorkEntry in todayWorkEntries)
            {
                todayWorkTime += todayWorkEntry.Duration();
            }
            return $"You worked today: {todayWorkTime.ToString(@"hh\:mm\:ss")}";
        }


    }
}
