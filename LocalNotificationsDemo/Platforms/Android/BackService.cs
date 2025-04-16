using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using Java.Lang;
using Java.Util.Logging;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalNotificationsDemo.Platforms.Android;

[Service]
public class ForegroundServiceDemo : global::Android.App.Service
{
    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    private readonly INotificationManagerService _notificationManagerService;

    private static bool IsStarted { get; set; }

    public ForegroundServiceDemo()
    {
        _notificationManagerService = MauiApplication.Current.Services.GetService<INotificationManagerService>();
        _notificationManagerService.NotificationReceived += (sender, eventArgs) =>
        {
            var eventData = (NotificationEventArgs)eventArgs;
            ShowNotification(eventData.Title, eventData.Message);
        };
    }

    void ShowNotification(string title, string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var msg = new Label()
            {
                Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
            };
        });
    }

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        _notificationManagerService.SendNotification("adwad", "dawdwad");

        if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
        {
            if (IsStarted)
            {
                Log.Info("wdwa", "OnStartCommand: Restarting the timer.");
                _notificationManagerService.SendNotification("adwad", "dawdwad");
            }
            else
            {
                Log.Info("wdwa", "OnStartCommand: Restarting the timer.");
                IsStarted = true;
            }
        }
        else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
        {
            Log.Info("wdwa", "OnStartCommand: Restarting the timer.");
            StopForeground(StopForegroundFlags.Remove);
            StopSelf();

        }
        else if (intent.Action.Equals(Constants.ACTION_RESTART_TIMER))
        {
            Log.Info("wdwa", "OnStartCommand: Restarting the timer.");

        }

        return StartCommandResult.Sticky;
    }
}