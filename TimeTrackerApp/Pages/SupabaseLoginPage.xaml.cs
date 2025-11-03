using TimeTracker.PageViewModels;

namespace TimeTracker.Pages;

public partial class SupabaseLoginPage : ContentPage
{
	public SupabaseLoginPage(SupabaseLoginPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}