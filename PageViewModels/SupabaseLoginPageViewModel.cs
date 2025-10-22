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
        LoggedUser loggedUser;

        [ObservableProperty]
        private string userName = "Paputek";

        [ObservableProperty]
        private string password = "password";

        [ObservableProperty]
        private string currentLoggedUser =string.Empty;

        public SupabaseLoginPageViewModel(SupabaseClient client)
        {
            this.client = client;
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
           loggedUser = await client.SignIn(UserName, Password);
            CurrentLoggedUser = loggedUser.UserName;
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

        private void UpdateCurrentLoggedUser()
        {
            if (client is null)
            {
                CurrentLoggedUser = "Init client first...";
                return;
            }

            
        }
    }
}
