using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using SQLite;

namespace App6
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        [Obsolete]
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
           SendRegistrationToServerAsync(refreshedToken);
        }
        async void SendRegistrationToServerAsync(string token)
        {

            try
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                var db5 = new SQLiteConnection(databasePath5);
                string FaceID = null;
                var query = new Carppi.DatabaseTypes.RestauratLoginTypes();
                try
                {

                     query = db5.Table<Carppi.DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    FaceID = query.FacebookId;
                }
                catch (Exception ex)
                {

                }
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)


                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/UpdateFirebaseToken?" +
                    "FaceID=" + query.FacebookId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
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
                    db5.CreateTable<Carppi.DatabaseTypes.RestauratLoginTypes>();


                    var s = db5.Insert(new Carppi.DatabaseTypes.RestauratLoginTypes()
                    {
                       FirebaseID= token


                    });
                }
                else
                {
                   query = db5.Table<Carppi.DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    query.FirebaseID = token;
                  //  query.ServiciosRegionales = rrr.ServicioRegional;
                    db5.RunInTransaction(() =>
                    {
                        db5.Update(query);
                    });

                }
            }
            catch (Exception ex) {

                Console.WriteLine(ex.ToString());
            }
            // Add custom implementation, as needed.
        }
    }
}