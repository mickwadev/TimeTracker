using System.Diagnostics;
using TimeTracker.PageModels;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Defaults;
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