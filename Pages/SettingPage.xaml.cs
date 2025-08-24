using System.Diagnostics;
using TimeTracker.Helpers;

namespace TimeTracker.Pages;

public partial class SettingPage : ContentPage
{
    public SettingPage()
    {
        InitializeComponent();
        Appearing += SettingPage_Appearing;
        Loaded += SettingPage_Loaded;
    }

    private void SettingPage_Loaded(object? sender, EventArgs e)
    {
        Trace.WriteLine("Loaded...");
    }

    private void SettingPage_Appearing(object? sender, EventArgs e)
    {
        Trace.WriteLine("Appearing...");
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var v = e.NewValue * 3600;
      //  Boxik.BackgroundColor = GradientSampler.GetWorkTimeColor(v);
        Labelka.Text = "Working hours: "+ e.NewValue.ToString("F2");
    }
}