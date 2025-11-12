using System.Diagnostics;
using TimeTracker.Data;

namespace TimeTracker.Pages;

public partial class SamplePage : ContentPage
{
	public SamplePage(WorkTimesManager wtm)
	{
		InitializeComponent();
		try
		{
			shellBkgLabel.Text = Shell.Current.BackgroundColor.ToString();
		}
		catch (Exception ex) 
		{
		 Trace.WriteLine(ex.Message);
		}
	}

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
		var d = AppTheme.Dark;
		Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        shellBkgLabel.Text = Shell.Current.BackgroundColor.ToString();
    }
}