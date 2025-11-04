using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.PageViewModels
{
    public partial class SupabaseLoginPageViewModel : ObservableObject
    {
        SupabaseClient client;
        DatabaseFun _sqliteDb;
        UserSignInStatus loggedUser;

        [ObservableProperty]
        private string userName = "Paputek";

        [ObservableProperty]
        private string password = "password";

        [ObservableProperty]
        private string loggingInfo = string.Empty;

        [ObservableProperty]
        private string dbFun = string.Empty;

        [ObservableProperty]
        private UserState userState = UserState.CLIENT_NOT_INITIALIZED;

        public SupabaseLoginPageViewModel(SupabaseClient client, DatabaseFun sqliteDb)
        {
            this.client = client;
            _sqliteDb = sqliteDb;
        }

        [ObservableProperty]
        private bool _inProgress = false;

        [RelayCommand]
        public async Task InitClient()
        {
            await client.InitSupabaseClient();
            UserState = UserState.CLIENT_INITIALIZED;
            LoggingInfo = "Client initialized. Sign up or sign in";
        }

        [RelayCommand]
        public async Task SignUpUser()
        {
            InProgress = true;
            loggedUser = await client.SignUpUser(UserName, ":3=", Password);
            await SecureStorage.SetAsync(DbConsts.secureJsonSupabaseSessionKey, loggedUser.SecureJsonSupabaseSessionKey);
            LoggingInfo = $"Loggin {UserName} info: {loggedUser.Info}";
            InProgress = false;

            if (loggedUser.OK)
            {
                UserState = UserState.USER_SIGNEDIN;
            }
        }

        [RelayCommand]
        public async Task SignInUser()
        {
            InProgress = true;
            
            loggedUser = await client.SignIn(UserName, Password);
            await SecureStorage.SetAsync(DbConsts.secureJsonSupabaseSessionKey, loggedUser.SecureJsonSupabaseSessionKey);
            LoggingInfo = $"Loggin {UserName} info: {loggedUser.Info}";
            
            InProgress = false;
            if (loggedUser.OK) 
            {
                UserState = UserState.USER_SIGNEDIN;
            }
        }

        [RelayCommand]
        public async Task PutTestData()
        {
            InProgress = true;
            await client.PutTestData();
            DbFun = "Adding some test data to db...";
            InProgress = false;
        }

        [RelayCommand]
        public async Task GetDataForUser()
        {
            InProgress = true;
            var models = await client.GetData();
            DbFun = $"Received models from db: {models.Count}"; 
            InProgress= false;
        }

        [RelayCommand]
        public async Task RestoreSession()
        {
            InProgress = true;
             string sessionJson = await SecureStorage.GetAsync(DbConsts.secureJsonSupabaseSessionKey);
            loggedUser = await client.RestoreSession(sessionJson);
            LoggingInfo = $"Loggin from previous session info: {loggedUser.Info}";
            InProgress = false;
            if (loggedUser.OK)
            {
                UserState = UserState.USER_SIGNEDIN;
            }
        }

        [RelayCommand]
        public async Task SignOutUser()
        {
            bool successfulyRemoved =SecureStorage.Remove(DbConsts.secureJsonSupabaseSessionKey);
            await client.SignOutUser();
            UserState = UserState.CLIENT_INITIALIZED;
        }

        [RelayCommand]
        private async Task BackupToSupabaseFromSQLite()
        {
            List<WorkTime> notBackuped = await _sqliteDb.SelectRowsWithNoBackup();
            foreach (WorkTime workTime in notBackuped)
            {
                Trace.WriteLine("Przed backupem: "+workTime);
                WorkTime backuped = await client.BackupSingleData(workTime);
                Trace.WriteLine("Po backupie: "+backuped);
                await _sqliteDb.UpdateBackupRow(workTime.ID,backuped.ID);
            }
        }
    }
}
