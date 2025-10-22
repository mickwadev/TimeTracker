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
        const string TimeTable = "TimesTable";
        public async Task DropTableAsync()
        {
            Trace.WriteLine($"Using connection string: {DbConsts.connectionString}");
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var dropTableCommand = connection.CreateCommand();
            dropTableCommand.CommandText = @$"
                DROP TABLE IF EXISTS {TimeTable};";
            await dropTableCommand.ExecuteNonQueryAsync();
        }

        public async Task AddTimesTableAsync()
        {
            Trace.WriteLine($"Using connection string: {DbConsts.connectionString}");
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @$"
CREATE TABLE IF NOT EXISTS {TimeTable} (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT NOT NULL,
    ActivityType TEXT,
    User TEXT,
    BackupID INTEGER
);";
            await createTableCommand.ExecuteNonQueryAsync();
        }

        public async Task CheckIfTableExistsAsync()
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var checkIfTableExistCmd = connection.CreateCommand();
            checkIfTableExistCmd.CommandText = @$"
            SELECT name FROM sqlite_master WHERE type='table' AND name='{TimeTable}';";
            await using var reader = await checkIfTableExistCmd.ExecuteReaderAsync();
            bool tableExists = await reader.ReadAsync();
            Trace.WriteLine($"Table {TimeTable} exists: {tableExists}");
        }

        public WorkTime GetRandomWorkTime()
        {
            DateTime start = DateTime.Now;
            return new WorkTime() { Title = "Random 10 minute ;)", StartTime = start, EndTime = (start + TimeSpan.FromMinutes(10)) };
        }

        public async Task<int> ClearTableAsync()
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {TimeTable}";
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
                cmd.CommandText = $@"INSERT INTO {TimeTable} 
(Title, StartTime, EndTime, ActivityType, User, BackupID) VALUES 
(@Title, @StartTime, @EndTime, @ActivityType, @User, @BackupID);";

                // Define parameters once (faster when reusing)
                var pTitle = cmd.Parameters.Add("@Title", SqliteType.Text);
                var pStart = cmd.Parameters.Add("@StartTime", SqliteType.Text);   // or SqliteType.Integer if ticks/epoch
                var pEnd = cmd.Parameters.Add("@EndTime", SqliteType.Text);
                var activityType = cmd.Parameters.Add("@ActivityType", SqliteType.Text);
                var user = cmd.Parameters.Add("@User", SqliteType.Text);
                var backupID = cmd.Parameters.Add("@BackupID", SqliteType.Integer);

                // Optional: prepare for better perf
                cmd.Prepare();
                var affected = 0;
                foreach (WorkTime r in workEntries)
                {
                    pTitle.Value = r.Title ?? (object)DBNull.Value;
                    pStart.Value = r.StartTime;  // ensure this matches your column type
                    pEnd.Value = r.EndTime;
                    activityType.Value = r.ActivityType;
                    user.Value = r.User;
                    backupID.Value = r.backupID;
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
            INSERT INTO {TimeTable} (Title, StartTime, EndTime, ActivityType, User, BackupID) VALUES (@Title, @StartTime, @EndTime, @ActivityType, @User, @BackupID)";
            addWorkTimeCmd.Parameters.AddWithValue("@Title", r.Title);
            addWorkTimeCmd.Parameters.AddWithValue("@StartTime", r.StartTime);
            addWorkTimeCmd.Parameters.AddWithValue("@EndTime", r.EndTime);
            addWorkTimeCmd.Parameters.AddWithValue("@ActivityType", r.ActivityType);
            addWorkTimeCmd.Parameters.AddWithValue("@User", r.User);
            addWorkTimeCmd.Parameters.AddWithValue("@BackupID", r.backupID);

            return await addWorkTimeCmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateRowsWithNoUser(string user)
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var updateMissingUsers = connection.CreateCommand();
            updateMissingUsers.CommandText = $@"
            UPDATE {TimeTable} SET User = @NewUser WHERE User IS NULL OR User IS @NotSetUser";
            updateMissingUsers.Parameters.AddWithValue("@NewUser", user);
            updateMissingUsers.Parameters.AddWithValue("@NotSetUser", DbConsts.NotSetUser);
            return await updateMissingUsers.ExecuteNonQueryAsync();
        }


        public async Task<List<int>> SelectRowsWithNoUser()
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var getMissingUsers = connection.CreateCommand();
            getMissingUsers.CommandText = $@"
            SELECT ID FROM {TimeTable} WHERE User IS NULL";
            List<int> missingUsers = new();
            await using (var reader = await getMissingUsers.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                { 
                    missingUsers.Add(reader.GetInt32(0));
                }
            }
            return missingUsers;
        }

        public async Task<int> UpdateBackupRow(long rowID, long backupID)
        {
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var updateMissingUsers = connection.CreateCommand();
            updateMissingUsers.CommandText = $@"
            UPDATE {TimeTable} SET backupID = @backupID WHERE ID = @rowID";
            updateMissingUsers.Parameters.AddWithValue("@backupID", backupID);
            updateMissingUsers.Parameters.AddWithValue("@rowID", rowID);
            return await updateMissingUsers.ExecuteNonQueryAsync();
        }

        public async Task<List<WorkTime>> SelectRowsWithNoBackup()
        {
            string onlyDateFormat = "yyyy-MM-dd ";

            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var addWorkTimeCmd = connection.CreateCommand();
            
            addWorkTimeCmd.CommandText = @$"SELECT ID, Title, StartTime, EndTime, ActivityType, User, BackupID FROM {TimeTable} WHERE backupID = -1 ;";
            
            await using var reader = await addWorkTimeCmd.ExecuteReaderAsync();
            List<WorkTime> times = new List<WorkTime>();

            while (await reader.ReadAsync())
            {
                bool startDateParsuSuccess =
                    DateTime.TryParseExact(reader.GetString(2), DbConsts.dbDateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTimeFromDb);
                bool endDateParsuSuccess =
                    DateTime.TryParseExact(reader.GetString(3), DbConsts.dbDateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTimeFromDb);


                var wt = new WorkTime()
                {
                    // tu trzeba sie upewnić, że nigdzie nie ma nulla:
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    StartTime = startTimeFromDb,
                    EndTime = endTimeFromDb,
                    ActivityType = reader.GetString(4),
                    User = reader.GetString(5),
                    backupID = reader.GetInt32(6),
                };
                Trace.WriteLine($"Parsed times from db: '{wt.StartTime}' '{wt.EndTime}'");
                times.Add(wt);
                // Trace.WriteLine($"Time span from activity: {wt.Duration} ({startTime.ToString(DbConsts.dbDateFormat)} - {endTime.ToString(DbConsts.dbDateFormat)})");
            }
            return times;
        }


        public async Task<int> AddRandomWorkTimeAsync() => await AddWorkTimeAsync(GetRandomWorkTime());

        public async Task<List<WorkTime>> GetTodayWorkingEntriesAsync() => await GetWorkingEntriesForTimePeriodAsync(DateTime.Now,DateTime.Now);
        public async Task<List<WorkTime>> GetAllWorkingEntriesAsync() => await GetWorkingEntriesForTimePeriodAsync(new DateTime(1410,7,15), DateTime.Now);
 
        public async Task<List<WorkTime>> GetWorkingEntriesForTimePeriodAsync(DateTime startDate, DateTime endDate)
        {
            string onlyDateFormat = "yyyy-MM-dd ";
           
            await using var connection = new SqliteConnection(DbConsts.connectionString);
            await connection.OpenAsync();
            var addWorkTimeCmd = connection.CreateCommand();
            var start = startDate.ToString(onlyDateFormat);
            var end = endDate.ToString(onlyDateFormat);
            Trace.WriteLine($"Time period days: {start} to {end}");
            addWorkTimeCmd.CommandText = @$"SELECT ID, Title, StartTime, EndTime, ActivityType, User, BackupID FROM {TimeTable} WHERE date(StartTime) >= date(@start) AND date(StartTime) <= date(@end);";
            addWorkTimeCmd.Parameters.AddWithValue("@start", start);
            addWorkTimeCmd.Parameters.AddWithValue("@end", end);
            await using var reader = await addWorkTimeCmd.ExecuteReaderAsync();
            List<WorkTime> times = new List<WorkTime>();
             
            while (await reader.ReadAsync())
            {
                var rawStartTimeFromDb = reader.GetString(2);
                var rawEndTimeFromDb = reader.GetString(3);
                bool startDateParsuSuccess = 
                    DateTime.TryParseExact(rawStartTimeFromDb, DbConsts.dbDateFormat, 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTimeFromDb);
                bool endDateParsuSuccess = 
                    DateTime.TryParseExact(rawEndTimeFromDb, DbConsts.dbDateFormat, 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTimeFromDb);

                if (startDateParsuSuccess == false)
                {
                    throw new Exception($"Failed to parse: {rawStartTimeFromDb}");
                }

                if (endDateParsuSuccess == false)
                {
                    throw new Exception($"Failed to parse: {rawEndTimeFromDb}");
                }

                var wt = new WorkTime()
                {
                    // tu trzeba sie upewnić, że nigdzie nie ma nulla:
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    StartTime = startTimeFromDb,
                    EndTime = endTimeFromDb,
                    ActivityType = reader.GetString(4),
                    User = reader.GetString(5),
                    backupID = reader.GetInt32(6),
                };
                Trace.WriteLine($"Parsed times from db: '{wt.StartTime}' '{wt.EndTime}'");
                times.Add(wt); 
               // Trace.WriteLine($"Time span from activity: {wt.Duration} ({startTime.ToString(DbConsts.dbDateFormat)} - {endTime.ToString(DbConsts.dbDateFormat)})");
            }
            return times;
        }
        public TimeSpan SumTimeSpans(List<WorkTime> todayWorkEntries)
        {
            TimeSpan todayWorkTime = TimeSpan.FromSeconds(0);
            foreach (var todayWorkEntry in todayWorkEntries)
            {
                todayWorkTime += todayWorkEntry.Duration;
            }
            return todayWorkTime;
        }


    }
}
