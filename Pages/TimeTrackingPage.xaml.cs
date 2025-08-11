using TimeTracker.PageModels;

namespace TimeTracker.Pages;

public partial class TimeTrackingPage : ContentPage
{
	public TimeTrackingPage(TimeTrackingPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}