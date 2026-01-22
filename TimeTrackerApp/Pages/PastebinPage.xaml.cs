using TimeTracker.Pastebin;

namespace TimeTracker.Pages;

public partial class PastebinPage : ContentPage
{
	PastebinPageViewModel _vm;
	public PastebinPage(PastebinPageViewModel model)
	{
		InitializeComponent();
		BindingContext = model;
		_vm = model;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _vm.OnAppearing();
    }
}