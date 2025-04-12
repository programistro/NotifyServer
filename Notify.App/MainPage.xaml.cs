using Notify.App.Platforms.Android;

namespace Notify.App;

//todo add https://github.com/dotnet/maui-samples/tree/main/9.0/PlatformIntegration/LocalNotificationsDemo
public partial class MainPage : ContentPage
{
    int count = 0;

    INotificationManagerService notificationManager;
    int notificationNumber = 0;

    public MainPage(INotificationManagerService manager)
    {
        InitializeComponent();

        notificationManager = manager;
        notificationManager.NotificationReceived += (sender, eventArgs) =>
        {
            var eventData = (NotificationEventArgs)eventArgs;
            ShowNotification(eventData.Title, eventData.Message);
        };

        // Send
        notificationManager.SendNotification("w", "w");

        // Scheduled send
        notificationManager.SendNotification("Notification title goes here", "Notification messages goes here.", DateTime.Now.AddSeconds(10));

        // Receive
        notificationManager.NotificationReceived += (sender, eventArgs) =>
        {
            var eventData = (NotificationEventArgs)eventArgs;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Take required action in the app once the notification has been received.
            });
        };
    }

#if ANDROID
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PermissionStatus status = await Permissions.RequestAsync<NotificationPermission>();
    }
#endif

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        string title = $"Local Notification #{notificationNumber}";
        string message = $"You have now received {notificationNumber} notifications!";
        notificationManager.SendNotification(title, message);
    }

    private void Button_Clicked1(object sender, EventArgs e)
    {
        string title = $"Local Notification #{notificationNumber}";
        string message = $"You have now received {notificationNumber} notifications!";
        notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));
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
}