using TimeTracker.Data;

namespace TimeTracker.Pages;

public partial class SamplePage : ContentPage
{
	public SamplePage(WorkTimesManager wtm)
	{
		InitializeComponent();
		Task.Run(async () => 
		{
			await wtm.Initialize();
			wtm.GetAllWorkingTime();
		});
	}
}