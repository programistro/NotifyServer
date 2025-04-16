using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using LocalNotificationsDemo.Platforms.Android;

namespace LocalNotificationsDemo
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private Intent? _startServiceIntent;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _startServiceIntent = new Intent(this, typeof(ForegroundServiceDemo));
            _startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);
            StartService(_startServiceIntent);
            CreateNotificationFromIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            CreateNotificationFromIntent(intent);
        }

        static void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(LocalNotificationsDemo.Platforms.Android.NotificationManagerService.TitleKey);
                string message = intent.GetStringExtra(LocalNotificationsDemo.Platforms.Android.NotificationManagerService.MessageKey);

                var service = IPlatformApplication.Current?.Services.GetService<INotificationManagerService>();
                service?.ReceiveNotification(title, message);
            }
        }
    }
    public static class Constants
    {
        public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const string BROADCAST_MESSAGE_KEY = "broadcast_message";
        public const string NOTIFICATION_BROADCAST_ACTION = "ServicesDemo3.Notification.Action";

        public const string ACTION_START_SERVICE = "ServicesDemo3.action.START_SERVICE";
        public const string ACTION_STOP_SERVICE = "ServicesDemo3.action.STOP_SERVICE";
        public const string ACTION_RESTART_TIMER = "ServicesDemo3.action.RESTART_TIMER";
        public const string ACTION_MAIN_ACTIVITY = "ServicesDemo3.action.MAIN_ACTIVITY";
    }
}
