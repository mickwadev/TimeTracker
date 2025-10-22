using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using TimeTracker.Data;
using TimeTracker.Models.Database;
using TimeTracker.PageModels;
using TimeTracker.PageViewModels;
using TimeTracker.Pastebin;
using UraniumUI;

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
          //  builder.Services.AddTransient<WorkTimeModel>();
            builder.Services.AddTransient<WorkTimeDashboardPageModel>();
            builder.Services.AddTransient<ProgressPageViewModel>();
            builder.Services.AddTransient<PastebinPageViewModel>();
            builder.Services.AddTransient<SupabaseLoginPageViewModel>();
            builder.Services.AddTransient<DatabasePageViewModel>();
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            
            return builder.Build();
        }
    }
}
