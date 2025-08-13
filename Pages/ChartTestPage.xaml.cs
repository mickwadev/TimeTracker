using TimeTracker.PageModels;

namespace TimeTracker.Pages;

public partial class ChartTestPage : ContentPage
{
	ChartTestPageViewModel model;
	public ChartTestPage(ChartTestPageViewModel m)
	{
		InitializeComponent();
		model = m;
		BindingContext = model;
	}


}