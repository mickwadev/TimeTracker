using TimeTracker.PageModels;

namespace TimeTracker.Pages;

public partial class ProgressPage : ContentPage
{
	ProgressPageViewModel model;
	public ProgressPage(ProgressPageViewModel m)
	{
		InitializeComponent();
		model = m;
		BindingContext = model;
	}


}