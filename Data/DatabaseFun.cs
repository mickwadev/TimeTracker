using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
       


        public async Task<List<WorkTime>> GetWorkingEntriesForTimePeriodAsync(DateTime startDate, DateTime endDate)
        {
            string format = "yyyy-MM-dd";
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var addWorkTimeCmd = connection.CreateCommand();
            var start = startDate.ToString(format);
            var end = endDate.ToString(format);
            Trace.WriteLine($"Time period: {start} to {end}");
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
                times.Add(wt);
                var time = DateTime.Parse(wt.EndTime) - DateTime.Parse(wt.StartTime);
                Trace.WriteLine($"Time span: {time}");
            }
            return times;
        }

    }
}
