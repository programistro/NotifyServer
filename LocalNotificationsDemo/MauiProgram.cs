using System.Net.Http;
using LocalNotificationsDemo.Interfaces;
using LocalNotificationsDemo.Pages;
using LocalNotificationsDemo.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using NotifyNet.Application.Interface;
using NotifyNet.Application.Service;
using NotifyNet.Core.Interface;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Infrastructure.Repository;
using Plugin.FirebasePushNotifications;
using Plugin.FirebasePushNotifications.Model.Queues;
#if ANDROID
using LocalNotificationsDemo.Platforms.Android;
using Android.App;
using Plugin.FirebasePushNotifications;
using Firebase;
#endif

namespace LocalNotificationsDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseFirebasePushNotifications(o =>
                {
                    o.AutoInitEnabled = false;
                    o.QueueFactory = new PersistentQueueFactory();
#if ANDROID
                    // You can configure Android-specific options under o.Android:
                    // o.Android.NotificationActivityType = typeof(MainActivity);
                    // o.Android.DefaultNotificationImportance = NotificationImportance.High;
                    // o.Android.NotificationChannelGroups = NotificationChannelGroupSamples.GetAll().ToArray();
                    o.Android.NotificationChannels = NotificationChannelSamples.GetAll().ToArray();
                    // o.Android.NotificationChannels = new [] { NotificationChannelSamples.Default };
                    // o.Android.NotificationCategories = NotificationCategorySamples.GetAll().ToArray();

                    // If you don't want to use the google-services.json file,
                    // you can configure Firebase programmatically
                    // o.Android.FirebaseOptions = new FirebaseOptions.Builder()
                    //     .SetApplicationId("appId")
                    //     .SetProjectId("projectId")
                    //     .SetApiKey("apiKey")
                    //     .SetGcmSenderId("senderId")
                    //     .Build();
#elif IOS
                    // You can configure iOS-specific options under o.iOS:
                    // o.iOS.FirebaseOptions = new Firebase.Core.Options("appId", "senderId");

                    // o.iOS.PresentationOptions = UNNotificationPresentationOptions.Banner;
                    // o.iOS.iOS18Workaround.Enable = true;
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            builder.Services.AddDbContextFactory<AppDbConetxt>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddLogging();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();
// #if DEBUG
            builder.Logging.AddDebug();
// #endif

#if ANDROID
            builder.Services.AddSingleton<INotificationManagerService, LocalNotificationsDemo.Platforms.Android.NotificationManagerService>();
            builder.Services.AddSingleton<LocalNotificationsDemo.Platforms.Android.ForegroundServiceDemo>();
#elif IOS
            builder.Services
                .AddSingleton<INotificationManagerService,
                    LocalNotificationsDemo.Platforms.iOS.NotificationManagerService>();
#elif MACCATALYST
            builder.Services.AddSingleton<INotificationManagerService, LocalNotificationsDemo.Platforms.MacCatalyst.NotificationManagerService>();
#endif
            builder.Services.AddSingleton<OrderBackgroundService>();
            builder.Services.AddHostedService<OrderBackgroundService>();
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}