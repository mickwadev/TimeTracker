using TimeTracker.PageViewModels;

namespace TimeTracker.Pages;

public partial class DatabasePage : ContentPage
{
	public DatabasePage(DatabasePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}