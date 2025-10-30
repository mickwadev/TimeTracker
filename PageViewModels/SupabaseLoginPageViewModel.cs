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
using TimeTracker.Models.Database;

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
            await client.SignUpUser(UserName, ":3=", Password);
        }

        [RelayCommand]
        public async Task SignInUser()
        {
            InProgress = true;
            
            loggedUser = await client.SignIn(UserName, Password);
           
            LoggingInfo = $"Loggin {UserName} info: {loggedUser.Info}";
            
            InProgress = false;
        }

        [RelayCommand]
        public async Task PutTestData()
        { 
          await client.PutTestData();
        }

        [RelayCommand]
        public async Task GetDataForUser()
        {
            await client.GetData();
        }

        [RelayCommand]
        public async Task RestoreSession()
        {
            await client.RestoreSession();
        }

        [RelayCommand]
        public async Task SignOutUser()
        {
            await client.SignOutUser();
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
