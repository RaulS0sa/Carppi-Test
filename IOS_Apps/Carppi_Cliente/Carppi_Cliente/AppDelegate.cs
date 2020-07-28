using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Firebase.CloudMessaging;
using Foundation;
using SQLite;
using UIKit;
using UserNotifications;

namespace Carppi_Cliente
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {

        [Export("window")]
        public UIWindow Window { get; set; }
        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            System.Diagnostics.Debug.WriteLine($"FCM Token: {fcmToken}");
        }

        [Export("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            Firebase.Core.App.Configure();
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                UNUserNotificationCenter.Current.Delegate = this;
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine(granted);
                });

                // For iOS 10 display notification (sent via APNS)
                

                // For iOS 10 data message (sent via FCM)
               // Messaging.SharedInstance.RemoteMessageDelegate = this;
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            // Firebase.Analytics.UserPropertyNamesConstants.

            // Firebase.Analytics.App.Configure();
            // var sad = Firebase.InstanceID.InstanceId. += new CompleteLestener();
            //FirebaseInstanceId.Instance.GetInstanceId().AddOnSuccessListener(new CompleteLestener());
            /*
            InstanceID.instanceID().instanceID {
                (result, error) in
  if let error = error {
                    print("Error fetching remote instance ID: \(error)")
  } else if let result = result {
                    print("Remote instance ID token: \(result.token)")
    self.instanceIDTokenMessage.text = "Remote InstanceID token: \(result.token)"
  }
            }

            Firebase.InstanceID.InstanceId.SharedInstance
            */
            //  var asdd = Firebase.InstanceID.ApnsTokenType.Prod;
            Messaging.SharedInstance.Delegate = this;

            //string token = Firebase.InstanceID.InstanceId.SharedInstance.token;


            //------------------------------
            var token = Messaging.SharedInstance.FcmToken ?? "";
          //  Console.WriteLine($"FCM token: {token}");

            //------------------------------

            // var sam = Firebase.InstanceID.InstanceId.TokenRefreshNotification.;

            /*
            Firebase.InstanceID.InstanceId.Notifications.ObserveTokenRefresh((sender, e) => {
                //var aca = e.
                
                var sad = Firebase.InstanceID.InstanceId.TokenRefreshNotification;
               // var newToken = Firebase.InstanceID.InstanceId.SharedInstance.;
                // if you want to send notification per user, use this token
                System.Diagnostics.Debug.WriteLine(sad);

                connectFCM();
            });
            */


            return true;
        }

        class CompleteLestener
        {


          
        }
        private void connectFCM()
        {
            
        }
     
        // UISceneSession Lifecycle

        [Export("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create("Default Configuration", connectingSceneSession.Role);
        }
        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
           // Console.WriteLine($"Firebase registration token: {fcmToken}");
            SendRegistrationToServerAsync(fcmToken);

            // TODO: If necessary send token to application server.
            // Note: This callback is fired at each app startup and whenever a new token is generated.
        }

        async void SendRegistrationToServerAsync(string token)
        {

            try
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                string FaceID = null;
                var query = new DatabaseTypes.Log_info();
                try
                {

                    query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                    FaceID = query.ProfileId;
                }
                catch (Exception ex)
                {

                }
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)


                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/UpdateFirebaseToken?" +
                    "FaceID=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                    "&FirebaseID=" + token

                    ));
                HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(uri);


                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
              {
                                '"'
              });
                }
                if (FaceID == null)
                {
                    db5.CreateTable<DatabaseTypes.Log_info>();


                    var s = db5.Insert(new DatabaseTypes.Log_info()
                    {
                        FirebaseID = token


                    });
                }
                else
                {
                    query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                    query.FirebaseID = token;
                    //  query.ServiciosRegionales = rrr.ServicioRegional;
                    db5.RunInTransaction(() =>
                    {
                        db5.Update(query);
                    });

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
            // Add custom implementation, as needed.
        }




        [Export("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions(UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
        /*
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Messaging.SharedInstance.AppDidReceiveMessage(userInfo);

            // Generate custom event
            NSString[] keys = { new NSString("Event_type") };
            NSObject[] values = { new NSString("Recieve_Notification") };
            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(keys, values, keys.Length);

            // Send custom event
            Firebase.Analytics.Analytics.LogEvent("CustomEvent", parameters);

            if (application.ApplicationState == UIApplicationState.Active)
            {
                System.Diagnostics.Debug.WriteLine(userInfo);
                var aps_d = userInfo["aps"] as NSDictionary;
                var alert_d = aps_d["alert"] as NSDictionary;
                var body = alert_d["body"] as NSString;
                var title = alert_d["title"] as NSString;
               // debugAlert(title, body);
            }
        }
        */

        // iOS 10, fire when recieve notification foreground
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var title = notification.Request.Content.Title;
            var body = notification.Request.Content.Body;
          //  debugAlert(title, body);
        }
    }
}

