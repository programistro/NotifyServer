﻿using System;

namespace LocalNotificationsDemo;

public interface INotificationManagerService
{
    event EventHandler NotificationReceived;
    void SendNotification(string title, string message, DateTime? notifyTime = null, string link = null);
    void ReceiveNotification(string title, string message);
}

