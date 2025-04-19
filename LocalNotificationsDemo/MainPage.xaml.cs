using System.Windows.Input;
using LocalNotificationsDemo.Service;
#if ANDROID
using LocalNotificationsDemo.Platforms.Android;
#endif

namespace LocalNotificationsDemo;

public partial class MainPage : ContentPage
{
    INotificationManagerService notificationManager;
    int notificationNumber = 0;
    private readonly IAuthService _authService;
    
    public string WelcomeMessage => $"Hello, {_authService.Username}!";
    
    public ICommand LogoutCommand { get; }

    public MainPage()
    {
        InitializeComponent();

        notificationManager = IPlatformApplication.Current.Services.GetRequiredService<INotificationManagerService>();
        _authService = IPlatformApplication.Current.Services.GetRequiredService<IAuthService>();
        LogoutCommand = new Command(Logout);
        notificationManager.NotificationReceived += (sender, eventArgs) =>
        {
            var eventData = (NotificationEventArgs)eventArgs;
            ShowNotification(eventData.Title, eventData.Message);
        };
    }
    
    private void Logout()
    {
        _authService.Logout();
        Shell.Current.GoToAsync("//Login");
    }

#if ANDROID
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PermissionStatus status = await Permissions.RequestAsync<NotificationPermission>();
    }
#endif

    void OnSendClick(object sender, EventArgs e)
    {
        notificationNumber++;
        string title = $"Local Notification #{notificationNumber}";
        string message = $"You have now received {notificationNumber} notifications!";
        notificationManager.SendNotification(title, message);
    }

    void OnScheduleClick(object sender, EventArgs e)
    {
        notificationNumber++;
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
            verticalStackLayout.Children.Add(msg);
        });
    }
}
