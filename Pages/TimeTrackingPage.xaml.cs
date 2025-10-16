using TimeTracker.PageModels;

namespace TimeTracker.Pages;

public partial class TimeTrackingPage : ContentPage
{
	public TimeTrackingPage(TimeTrackingPageViewModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}