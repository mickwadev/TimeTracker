using TimeTracker.Data;
using TimeTracker.PageViewModels;

namespace TimeTracker.Pages;

public partial class DatabasePage : ContentPage
{
	DatabasePageViewModel viewModel;
	public DatabasePage(DatabasePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		viewModel = vm;
	}

    protected override void OnAppearing()
    {
      //  viewModel.InitializeDatabase(DbConsts.connectionString);
    }
}