using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using TimeTracker.Data;
using TimeTracker.Models;
using TimeTracker.PageModels;
using TimeTracker.PageViewModels;
using TimeTracker.Pastebin;
using UraniumUI;
using Database;

namespace TimeTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMarkup()
                .ConfigureSyncfusionToolkit()
                .UseSkiaSharp()
                .UseLiveCharts()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                 
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddMaterialSymbolsFonts();
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseFun>();
            builder.Services.AddSingleton<SupabaseClient>();
            builder.Services.AddTransient<TimeTrackingPageViewModel>();
            builder.Services.AddTransient<ProgressPageViewModel>();
            builder.Services.AddTransient<PastebinPageViewModel>();
            builder.Services.AddTransient<SupabaseLoginPageViewModel>();
            builder.Services.AddTransient<DatabasePageViewModel>();
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddTransient<PomodoroControlViewModel>();
            builder.Services.AddSingleton<WorkTimesManager>();
            return builder.Build();
        }
    }
}
