using System;

namespace Notify.App;

public class NotificationEventArgs : EventArgs
{
    public string Title { get; set; }

    public string Message { get; set; }
}
