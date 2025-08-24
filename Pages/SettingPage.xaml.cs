using System.Diagnostics;
using TimeTracker.Helpers;
using TimeTracker.ModelMasla;

namespace TimeTracker.Pages;



public partial class SettingPage : ContentPage
{
    public static int u=0;
    public SettingPage()
    {
        Appearing += SettingPage_Appearing;
        Loaded += SettingPage_Loaded;
        InitializeComponent();
        BindingContext = new MasloModel(); 
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Trace.WriteLine($"OnNavigatedTo...{u++}");

        // eee... dziwny ten sposób SetBinding...
        Kolekcja.SetBinding(ItemsView.ItemsSourceProperty, static (MasloModel mm) => mm.masla);
    }

    private void SettingPage_Loaded(object? sender, EventArgs e)
    {
        Trace.WriteLine($"Loaded...{u++}");
    }

    private void SettingPage_Appearing(object? sender, EventArgs e)
    {
        Trace.WriteLine($"Appearing...{u++}");
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        
    }
}