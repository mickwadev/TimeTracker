using TimeTracker.Pastebin;

namespace TimeTracker.Pages;

public partial class PastebinPage : ContentPage
{
	public PastebinPage(PastebinPageViewModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}