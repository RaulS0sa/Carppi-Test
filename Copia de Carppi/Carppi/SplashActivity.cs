
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Carppi.DatabaseTypes;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SQLite;

namespace Carppi
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        public enum PagoPreferido { Efectivo, Tarjeta };
        public enum UbicacionPreferida { Actual, Custom };

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
            SetRegion();
            await Task.Delay(500); // Simulate a bit of startup work.
            Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
        async void SetRegion()
        {

            try
            {

                if (IsLocationAvailable() && LocationPermitionGranted())
                {
                    //
                    //0000000000
                    //CalculateCostOfTrip(Int32 Region_costo, double LatitudPedido, double LongitudPedido)
                    var MyLatLong = await Clases.Location.GetCurrentPosition();

                    if (MyLatLong == null)
                    {

                     

                        //Sin Log En la base de datos
                    }
                    else
                    {
                        try
                        {
                            Clases.Location.StartListening();
                        }
                        catch (Exception)
                        {

                        }

                        HttpClient client = new HttpClient();
                        MyLatLong = await Clases.Location.GetCurrentPosition();
                        //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)

                        var query = new Log_info();
                        var Region = "";
                        var faceID = "";
                        var NewLat = "";
                        var NewLong = "";
                        var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        try
                        {
                            var db5 = new SQLiteConnection(databasePath5);
                            query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                            Region = (query.Region_Delivery == null ? 0.ToString() : (query.Region_Delivery).ToString());
                            faceID = (query.ProfileId == null ? "" : (query.ProfileId).ToString());
                            NewLat = MyLatLong.Latitude.ToString().Replace(",", ".");
                            NewLong = MyLatLong.Longitude.ToString().Replace(",", ".");
                        }
                        catch (Exception ex)
                        {
                    
                        }
                       
                        if (!String.IsNullOrEmpty(NewLat) || !String.IsNullOrEmpty(NewLong))
                        {
                            var new_uri = "http://geolocale.azurewebsites.net/api/CarppiGroceryApi/CarppiDeliveryGetArea?" +
                             "lat=" + NewLat +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                             "&log=" + NewLong;


                            var uri = new Uri(new_uri);

                            HttpResponseMessage response;

                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            // response = await client.GetAsync(uri);
                            var tak = Task.Run(() => GetResponseFromURI(uri));
                            tak.Wait();
                            var SRes = tak.Result;
                            //var SRes = GetResponseFromURI(uri).Result;
                            if (SRes.httpStatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var RegionResponse = JsonConvert.DeserializeObject<long>(SRes.Response);
                                var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                                var db10 = new SQLiteConnection(databasePath10);

                                db10.CreateTable<DatabaseTypes.Log_info>();





                                //var query = db10.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                                if (query == null)
                                {
                                    /*

                                    var s = db10.Insert(new DatabaseTypes.Log_info()
                                    {
                                        Region_Delivery = RegionResponse,

                                   

                                    });
                                    */
                                }
                                else
                                {
                                    /*
                                    query.Region_Delivery = RegionResponse;

                                 
                                    db10.RunInTransaction(() =>
                                    {
                                        db10.Update(query);
                                    });
                                    */
                                }



                            }



                        }
                        else
                        {
                          

                        }
                    }

                }
                else
                {
                
                }

                /*

                var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db10 = new SQLiteConnection(databasePath10);

                db10.CreateTable<DatabaseTypes.Log_info>();



              

                var query = db10.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                if (query == null)
                {


                    var s = db10.Insert(new DatabaseTypes.Log_info()
                    {
                        Region_Delivery = 0,

                        PagoPreferido = (int)PagoPreferido.Efectivo,
                        UbicacionPreferida = (int)UbicacionPreferida.Actual,



                    });
                }
                else
                {
                    query.Region_Delivery = 0;

                    query.PagoPreferido = (int)PagoPreferido.Efectivo;
                    query.UbicacionPreferida = (int)UbicacionPreferida.Actual;
                    db10.RunInTransaction(() =>
                    {
                        db10.Update(query);
                    });
                }
                */





            }
            catch (System.Exception ex)
            {

            }
        }

        public bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        public bool LocationPermitionGranted()
        {
            //  Android.Support.V4.App.ActivityCompat.RequestPermissions((Activity)StaticContext, new System.String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.LocationHardware, Manifest.Permission.Internet, }, 1);
            var rt = (Android.Support.V4.App.ActivityCompat.CheckSelfPermission((Activity)this, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
           || (Android.Support.V4.App.ActivityCompat.CheckSelfPermission((Activity)this, Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted);

            return rt;


        }
        public static async Task<UriResponse> GetResponseFromURI(Uri u)
        {
            var response = "";
            var Respuesta = new UriResponse();
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.GetAsync(u);
                // var result = client.GetAsync(u).Result;
                Respuesta.httpStatusCode = result.StatusCode;
                //if (result.IsSuccessStatusCode)
                //{
                Respuesta.Response = await result.Content.ReadAsStringAsync();
                //}
            }
            return Respuesta;
        }
        public class UriResponse
        {
            public String Response;
            public System.Net.HttpStatusCode httpStatusCode;
        }

        public override void OnBackPressed() { }
    }
}
