using System.Diagnostics;
using TimeTracker.PageModels;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
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

    private void Button_Clicked(object sender, EventArgs e)
    {
		model.HasAnyEntries = !model.HasAnyEntries;
    }

    
	// This is not used, but I will leave this here, for learning purpouse
	// here events have 3 parameters.
	// Command has 1 parameter HoverCommandArgs.
	// EventToCommand behaviour is not needed.
    private void WorkTimeChart_HoveredPointsChanged(
		IChartView chart, 
		IEnumerable<ChartPoint> newItems, 
		IEnumerable<ChartPoint> oldItems)
    {
		 
		if (newItems is null)
		{
			Trace.WriteLine("Looks like you are not on chart anymore...");
			return;
		}
		if (newItems.Count() == 0)
        {
            Trace.WriteLine("No DateTimePoint selected...");
			return;
        }
        Trace.WriteLine(newItems.Count());
		foreach (var item in newItems) 
		{
			DateTimePoint currentSelectePoint = item.Context.Entity as DateTimePoint;

			if (currentSelectePoint is not null)
			{
				Trace.WriteLine($"{currentSelectePoint.DateTime}");
			}
		}
    }
	 
}