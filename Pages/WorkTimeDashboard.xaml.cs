using TimeTracker.PageModels;

namespace TimeTracker.Pages;

public partial class WorkTimeDashboard : ContentPage
{
	public WorkTimeDashboard(WorkTimeDashboardPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}