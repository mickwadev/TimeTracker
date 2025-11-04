using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data; 
using Database;
namespace TimeTracker.PageViewModels
{
    public partial class DatabasePageViewModel : ObservableObject
    {
        [ObservableProperty]
        bool _hasInternet;

        IConnectivity _connectivity;
        DatabaseFun _db;
        SupabaseClient _supabaseClient;
        public DatabasePageViewModel(IConnectivity connectivity, DatabaseFun db, SupabaseClient supabaseClient) 
        { 
            _connectivity = connectivity;
            HasInternet = connectivity.NetworkAccess == NetworkAccess.Internet;
            _db = db;
            _supabaseClient = supabaseClient;
        }
         
        [RelayCommand]
        private async Task CreateDatabase()
        {
            await _db.AddTimesTableAsync();
        }

        [RelayCommand]
        private async Task DropTable()
        {
            await _db.DropTableAsync();
        }

        [RelayCommand]
        private async Task SelectMissingUsersRows()
        {
            List<int> users = await _db.SelectRowsWithNoUser();
            foreach (var user in users) 
            {
                Trace.WriteLine(user);
            }
        }

        [RelayCommand]
        private async Task UpdateMissingUsersRows()
        {
            await _db.UpdateRowsWithNoUser(_supabaseClient.GetLoggedUserName);
        }
    }
}
