using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Database.SQLiteDB;
using Database.SupabaseDB;
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
         private const string PASTEBIN_API_DEV_KEY = "PASTEBIN_API_DEV_KEY";
         private const string PASTEBIN_USER_NAME = "PASTEBIN_USER_NAME";
         private const string PASTEBIN_PASSWORD = "PASTEBIN_PASSWORD";
        private const string REMEMBER_ME_TOGGLE = "REMEMBER_ME_TOGGLE";

        private DatabaseFun _db;
        public PastebinPageViewModel(DatabaseFun db)
        { 
         _db = db;
        }

        [ObservableProperty]
        private string _pastebinUserName = "mickwa";

        [ObservableProperty]
        private string _pastebinPassword = "PastebinPassword";

        [ObservableProperty]
        private string _lastPasteURL = "";

        [ObservableProperty]
        private string _PastebinApiDevKey = "onoqC9YzZi9AGaC04GFwj8LeTSr6fEdD";

        [ObservableProperty]
        private UserState userState = UserState.CLIENT_NOT_INITIALIZED;

        [ObservableProperty]
        private string _message = "";

        [ObservableProperty]
        private bool _isErrorMessage = false;

        [ObservableProperty]
        private bool _storeCredentials = false;

        private string _userKey = "";
        private PastebinClient _pbClient;

        [RelayCommand]
        private void ToggleChanged(ToggledEventArgs e)
        {
            Trace.WriteLine($"Toggle changed to: {e.Value}");
            Preferences.Set(REMEMBER_ME_TOGGLE, e.Value);
        }

        public async Task OnAppearing()
        { 
            StoreCredentials = Preferences.Get(REMEMBER_ME_TOGGLE, false);

            if (StoreCredentials == false) return;

            var storedUser = await SecureStorage.GetAsync(PASTEBIN_USER_NAME);
            if (string.IsNullOrWhiteSpace(storedUser) == false)
            { 
                PastebinUserName = storedUser;
            }
            var storedPassword = await SecureStorage.GetAsync(PASTEBIN_PASSWORD);
            if (string.IsNullOrWhiteSpace(storedPassword) == false)
            {
                PastebinPassword = storedPassword;
            }

            var storedApiKey = await SecureStorage.GetAsync(PASTEBIN_API_DEV_KEY);
            if (string.IsNullOrWhiteSpace(storedApiKey) == false)
            {
                PastebinApiDevKey = storedApiKey;
            }

        }

        [RelayCommand]
        private async Task LoginToPastebin()
        {
            Trace.WriteLine("Testing Pastebin login...");
            Preferences.Set(REMEMBER_ME_TOGGLE, StoreCredentials);

            if (StoreCredentials)
            {
                await SecureStorage.SetAsync(PASTEBIN_USER_NAME, PastebinUserName);
                await SecureStorage.SetAsync(PASTEBIN_API_DEV_KEY, PastebinApiDevKey);
                await SecureStorage.SetAsync(PASTEBIN_PASSWORD, PastebinPassword);
            }
            else
            {
                SecureStorage.Remove(PASTEBIN_USER_NAME);
                SecureStorage.Remove(PASTEBIN_API_DEV_KEY);
                SecureStorage.Remove(PASTEBIN_PASSWORD);
            }
                try
                {
                    _pbClient = new PastebinClient(PastebinApiDevKey);
                    try
                    {
                        _userKey = await _pbClient.GetUserKeyAsync(PastebinUserName, PastebinPassword);
                        Trace.WriteLine($"Login successful. User key: {_userKey}");
                        UserState = UserState.USER_SIGNEDIN;
                        Message = "Logged in to Pastebin successful ^_^";
                        IsErrorMessage = false;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Login failed: {ex.Message}");
                        Message = $"Login failed: {ex.Message}";
                        IsErrorMessage = true;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Pastebin client error: {ex.Message}");
                    Message = $"Login failed: {ex.Message}";
                    IsErrorMessage = true;
                }
        }

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
                 
                
                var latest = await _pbClient.CreatePasteAsync(serializedJson, $"Enties backup from {DateTime.Now.ToString("G")}", _userKey);
                LastPasteURL = $"Backup done: {latest}";
            }
            catch (Exception ex) 
            {
                Trace.WriteLine($"Serialization failed... {ex.Message}");
                Message = $"Creating backup failed: {ex.Message} (Solution: login to PasteBin and remove some older entries)";
                IsErrorMessage = true;
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
             
            PasteMeta? latest = await _pbClient.GetLatestPasteMetaAsync(_userKey);
            if (latest == null || string.IsNullOrWhiteSpace(latest.Key))
            {
                Trace.WriteLine("No pastes found.");
                return ":(";
            }

            Trace.WriteLine($"Latest paste: {latest.Title} ({latest.Key})");
            Trace.WriteLine(latest.Url);

            // 3) (optional) fetch its contents
            var raw = await _pbClient.GetPasteRawAsync(latest.Key!, _userKey); // include userKey for private pastes
            Trace.WriteLine("---- RAW CONTENT ----");
            Trace.WriteLine(raw);
             
            return raw;
        }
    }
}
