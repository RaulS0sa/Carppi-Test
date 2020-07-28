using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Carppi;
using Firebase.Messaging;
using Newtonsoft.Json;
using static Carppi.Fragments.FragmentMain;
using static Carppi.Fragments.FragmentMain.WebInterfaceMenuCarppi;

namespace App6
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            var body = message.GetNotification().Body;
            var Title = message.GetNotification().Title;
            Log.Debug(TAG, "Notification Message Body: " + body);
          
            var Request = new CarppiRequestForDrive();
            try
            {
                var Data = message.GetNotification().Sound;

                Request = JsonConvert.DeserializeObject<CarppiRequestForDrive>(Base64Decode(Data));
            }
            catch(Exception)
            {

            }
            SendNotification(body, message.Data, Request, Title);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
      
        void SendNotification(string messageBody, IDictionary<string, string> data, CarppiRequestForDrive Request, string title)
        {
            var intent = new Intent(this, typeof(MainActivity));
            if(Request.ID != 0)
            {
                //ShowTripOptionToRideSharer.Data = Request;
                //Data_CarppiRequestForDrive = Request;
                //WebInterfaceMenuCarppi.DisplayAceptRejectTripModal();

                //intent = new Intent(this,typeof( ShowTripOptionToRideSharer));
            }
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(this,
                                                          MainActivity.NOTIFICATION_ID,
                                                          intent,
                                                          PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.rocket)
                                      .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.IconForPushNotifsJpeg))
                                      .SetContentTitle(title)
                                      .SetContentText(messageBody)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(this);
            //notificationManager.Notify(Activity1.NOTIFICATION_ID, notificationBuilder.Build());
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                
              //  notification.setSmallIcon(R.drawable.icon_transperent);
              //  notification.setColor(getResources().getColor(R.color.notification_color));

                notificationBuilder.SetSmallIcon(Resource.Drawable.rocket);
                notificationBuilder.SetColor(Android.Graphics.Color.White);
            }
            else
            {
                notificationBuilder.SetSmallIcon(Resource.Drawable.rocket);
                // notification.setSmallIcon(R.drawable.icon);
            }
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
    }

}