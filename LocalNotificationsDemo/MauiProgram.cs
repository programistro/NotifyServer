using LocalNotificationsDemo.Pages;
using LocalNotificationsDemo.Service;
using LocalNotificationsDemo.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using NotifyNet.Application.Interface;
using NotifyNet.Application.Service;
using NotifyNet.Core.Interface;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Infrastructure.Repository;

namespace LocalNotificationsDemo
{
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
            builder.Services.AddDbContextFactory<AppDbConetxt>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
        
            // Регистрация ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
        
            // Регистрация страниц
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MainPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID
            builder.Services.AddSingleton<INotificationManagerService, LocalNotificationsDemo.Platforms.Android.NotificationManagerService>();
#elif IOS
            builder.Services.AddSingleton<INotificationManagerService, LocalNotificationsDemo.Platforms.iOS.NotificationManagerService>();
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
