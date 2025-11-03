using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.Pastebin
{
    public partial class PastebinPageViewModel : ObservableObject
    {
        private const string DevKey = "RonoRqC9RYzZiR9AGaC0R4RRGFwj8RLeTSrR6fERdD";
        private const string Username = "RmiRckwaR";
        private const string Password = "PastebRinPassRwordR";

        private DatabaseFun _db;
        public PastebinPageViewModel(DatabaseFun db)
        { 
         _db = db;
        }
         
        [ObservableProperty]
        private string _poem = "Gentle panda in the morning mist,  \r\nChewing bamboo with a sleepy twist.  \r\nBlack and white, a peaceful sight,  \r\nSoft as clouds, yet strong in might.  \r\nThey wander forests calm and deep,  \r\nGuarding secrets trees still keep.  \r\nWith every step the mountains ring,  \r\nA quiet hymn the breezes sing.  \r\nPandas dream where rivers flow,  \r\nIn quiet groves where blossoms grow.  \r\nTheir gentle hearts remind us all,  \r\nEven giants may be small.  \r\nA tender soul in fur so grand,  \r\nA living poem of the land.";

        [ObservableProperty]
        private string _lastPasteURL = "";

        [RelayCommand]
        public async Task CreatePastebinPaste()
        {
            Trace.WriteLine("Create new paste...");
            string serializedJson = string.Empty;
           
            try
            { 
                var allEntries = await _db.GetAllWorkingEntriesAsync();
                JsonSerializerSettings settings = new()
                {
                    DateFormatString = DbConsts.dbDateFormat
                };
                serializedJson = JsonConvert.SerializeObject(allEntries, Formatting.Indented, settings);
                using var pb = new PastebinClient(DevKey.Replace("R", ""));
                var userKey = await pb.GetUserKeyAsync(Username.Replace("R", ""), Password.Replace("R", ""));
                var latest = await pb.CreatePasteAsync(serializedJson, $"Enties backup from {DateTime.Now.ToString("G")}",userKey);
                LastPasteURL = $"Backup done: {latest}";
            }
            catch (Exception ex) 
            {
                Trace.WriteLine($"Serialization failed... {ex.Message}");
                return;
            }
           
        }

        [RelayCommand]
        public async Task LoadFromLastPaste()
        {
            var dataFromBackup = await GetLatestPaste();
            int added = await _db.CreateFromBackupAsync(JsonConvert.DeserializeObject<List<WorkTime>>(dataFromBackup));
        }

        [RelayCommand]
        public async Task<string> GetLatestPaste()
        {
            Trace.WriteLine("Get latest paste...");
            using var pb = new PastebinClient(DevKey.Replace("R", ""));

            // 1) login -> user key
            var userKey = await pb.GetUserKeyAsync(Username.Replace("R", ""), Password.Replace("R", ""));

            // 2) latest paste metadata (newest first; we limited to 1)
            var latest = await pb.GetLatestPasteMetaAsync(userKey);
            if (latest == null || string.IsNullOrWhiteSpace(latest.Key))
            {
                Trace.WriteLine("No pastes found.");
                return ":(";
            }

            Trace.WriteLine($"Latest paste: {latest.Title} ({latest.Key})");
            Trace.WriteLine(latest.Url);

            // 3) (optional) fetch its contents
            var raw = await pb.GetPasteRawAsync(latest.Key!, userKey); // include userKey for private pastes
            Trace.WriteLine("---- RAW CONTENT ----");
            Trace.WriteLine(raw);
            Poem = raw;
            return raw;
        }
    }
}
