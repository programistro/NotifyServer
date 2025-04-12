using Microsoft.Extensions.Logging;
using Notify.App;

namespace Notify.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#if DEBUG
        builder.Logging.AddDebug();
#endif
#if ANDROID
            builder.Services.AddSingleton<INotificationManagerService, Notify.App.Platforms.Android.NotificationManagerService>();
#elif IOS
        // builder.Services.AddSingleton<INotificationManagerService, Notify.App.Platforms.IOS.NotificationManagerService>();
#elif MACCATALYST
            // builder.Services.AddSingleton<INotificationManagerService, Notify.App.Platforms.MacCatalyst.NotificationManagerService>();
#endif
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}