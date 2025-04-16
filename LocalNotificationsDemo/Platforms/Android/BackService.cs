using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
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


    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        return StartCommandResult.NotSticky;
    }
}