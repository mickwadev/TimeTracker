using TimeTracker.Data;

namespace TimeTracker.Pages;

public partial class SamplePage : ContentPage
{
	public SamplePage(WorkTimesManager wtm)
	{
		InitializeComponent();
	}

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
		var d = AppTheme.Dark;
		Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }
}