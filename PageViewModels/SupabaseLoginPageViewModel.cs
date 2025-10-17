using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Models.Database;

namespace TimeTracker.PageViewModels
{
    public partial class SupabaseLoginPageViewModel : ObservableObject
    {
        SupabaseClient client;

        [ObservableProperty]
        private string userName = "Paputek";

        [ObservableProperty]
        private string password = "password";

        public SupabaseLoginPageViewModel()
        {
            client = new SupabaseClient();
        }

        [RelayCommand]
        public async Task InitClient()
        {
            await client.InitSupabaseClient();
        }

        [RelayCommand]
        public async Task SignUpUser()
        {
            await client.SignUpUser(UserName, ":3=", Password);
        }

        [RelayCommand]
        public async Task SignInUser()
        {
            await client.SignIn(UserName, Password);
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
    }
}
