using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using TimeTracker.Data;
using TimeTracker.PageModels;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseFun>();
            builder.Services.AddSingleton<TimeTrackingPageModel>();
            builder.Services.AddSingleton<WorkTimeModel>();
            builder.Services.AddSingleton<WorkTimeDashboardPageModel>();
            builder.Services.AddSingleton<ProgressPageViewModel>();
            
            return builder.Build();
        }
    }
}
