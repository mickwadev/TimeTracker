using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
