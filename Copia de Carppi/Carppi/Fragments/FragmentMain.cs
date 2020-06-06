
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using SQLite;
using Fragment = Android.Support.V4.App.Fragment;
using Newtonsoft.Json;
using Carppi.Clases;
using System.Threading.Tasks;
using static Carppi.Fragments.FragmentMain.WebInterfaceMenuCarppi;
using Xamarin.Essentials;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Plugin.Media;
using Plugin.Permissions;
using static Android.Manifest;
using Android;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Carppi.DatabaseTypes;
using Android.Support.V4.Widget;
using System.Timers;
using Xamarin.Facebook;
using static Carppi.Fragments.WebInterfaceProfile;

namespace Carppi.Fragments
{
    public class FragmentMain : Fragment
    {
        public static WebInterfaceMenuCarppi MainWebView;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);

            //var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            //return inflater.Inflate(Resource.Layout.fragment1, null);
            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);

            string content;
            AssetManager assets = this.Activity.Assets;
           // act = this.Activity;
            //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))
            using (StreamReader sr = new StreamReader(assets.Open("Fragment1_NewIcons.html")))
            {
                content = sr.ReadToEnd();
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new WebInterfaceMenuCarppi(this.Activity, webi);
                MainWebView = wew;
               // ;
                webi.AddJavascriptInterface(wew, "Android");

                webi.Settings.JavaScriptEnabled = true;

                webi.Settings.DomStorageEnabled = true;
                webi.Settings.LoadWithOverviewMode = true;
                webi.Settings.UseWideViewPort = true;
                webi.Settings.BuiltInZoomControls = true;
                webi.Settings.DisplayZoomControls = false;
                webi.Settings.SetSupportZoom(true);

                webi.Settings.JavaScriptEnabled = true;
                // webi.LoadUrl("http://geolocale.azurewebsites.net/geoloc");
                webi.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                // webi.LoadData(content, "text/html", null);
                webi.SetWebViewClient(new NewWebClient(view1, this.Activity) );
                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView
                WebInterfaceMenuCarppi.DriverStateDetermination();

               // DoWork_Driver();

            }

            return view1;



        }
        public override void OnStop()
        {
            try
            {
                aTimer.Enabled = false;

            }
            catch (Exception)
            { }
            base.OnStop();
        }
        public override void OnPause()
        {
            try
            {
                aTimer.Enabled = false;

            }
            catch (Exception)
            { }
            base.OnPause();
        }
        public override void OnResume()
        {
            try
            {
                aTimer.Enabled = true;

            }
            catch (Exception)
            { }
            base.OnResume();
        }
        class NewWebClient : WebViewClient
        {
            public View CurrentView;
            public Context P_Context;
            public NewWebClient(View v, Context c)
            {
                CurrentView = v;
                P_Context = c;
            }
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                WebInterfaceMenuCarppi.CenterThemap();
                FragmentMain.DoWork_Driver();
                GetLocationByState();
                var navigationView = ((Activity)P_Context).FindViewById<NavigationView>(Resource.Id.nav_view);
                HideOptionsInMenu(navigationView);
                // UpdateRegion();
                //  UpdateLocation();
            }
            public bool isLoggedIn()
            {

                AccessToken accessToken = AccessToken.CurrentAccessToken;//AccessToken.getCurrentAccessToken();
                return accessToken != null;
                //return false;
            }
            async void HideOptionsInMenu(NavigationView mNavigationView)
            {
                try
                {
                    // var Mnu = FindViewById<IMenu>(Resource.Id.MuttaMenu);
                    var menuNav = mNavigationView.Menu;
                    menuNav.FindItem(Resource.Id.nav_GroceryRequest).SetVisible(false);
                    menuNav.FindItem(Resource.Id.nav_GroceryConversation).SetVisible(false);
                    if (isLoggedIn() == false)
                    {
                        var aca = menuNav.FindItem(Resource.Id.nav_messages).SetVisible(false);

                        menuNav.FindItem(Resource.Id.nav_friends).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_discussion).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_LogOutButton).SetVisible(false);
                        //Resource.Id.nav_GroceryRequest:
                        

                    }
                    else
                    {
                        var aca = menuNav.FindItem(Resource.Id.nav_messages).SetVisible(false);

                        menuNav.FindItem(Resource.Id.nav_friends).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_LoginButton).SetVisible(false);
                        try
                        {
                            HttpClient client = new HttpClient();
                            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                            var db5 = new SQLiteConnection(databasePath5);
                            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();
                            if(query.ProfileId == "849994702134646")
                            {
                                menuNav.FindItem(Resource.Id.nav_GroceryRequest).SetVisible(true);
                                menuNav.FindItem(Resource.Id.nav_GroceryConversation).SetVisible(true);
                            }

                            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/GEetOwnProfile?" +
                                "user10_Hijo=" + query.ProfileId
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
                                var Profile = JsonConvert.DeserializeObject<Traveler_Perfil>(errorMessage1);
                                var RealRole = Profile.IsUserADriver;
                                if (RealRole == true)//UserIsADriver
                                {
                                    menuNav.FindItem(Resource.Id.nav_discussion).SetVisible(false);
                                    if (Profile.StripeDriverID == null)
                                    {
                                        menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);

                                    }
                                    else
                                    {
                                        menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);

                                    }
                                }
                                else
                                {
                                    menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);
                                    menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);
                                    // menuNav.FindItem(Resource.Id.nav_LogOutButton).SetVisible(false);

                                }

                                //  MainActivity.LoadFragmentStatic(Resource.Id.menu_video);


                            }
                        }
                        catch (System.Exception) { }

                    }
                    /*
                        < item
              android: id = "@+id/nav_home"
              android: icon = "@drawable/ic_dashboard"
              android: title = "Pantalla Principal" />

             < item
              android: id = "@+id/nav_messages"
              android: icon = "@drawable/ic_event"
              android: title = "Cambiar Foto" />

             < item
              android: id = "@+id/nav_friends"
              android: icon = "@drawable/ic_headset"
              android: title = "Mis Estadisticas" />

             < item
              android: id = "@+id/nav_discussion"
              android: icon = "@drawable/ic_forum"
              android: title = "Quiero Ser Conductor!" />

             < item
              android: id = "@+id/nav_Clabe"
              android: icon = "@drawable/ic_forum"
              android: title = "Ingresa Tu Clabe" />

             < item
              android: id = "@+id/nav_Balance"
              android: icon = "@drawable/ic_forum"
              android: title = "Mi Balance" />

             < item
              android: id = "@+id/nav_LoginButton"
              android: icon = "@drawable/ic_forum"
              android: title = "Log In" />

              < item
              android: id = "@+id/nav_LogOutButton"
              android: icon = "@drawable/ic_forum"
              android: title = "Log Out" />
              */
                }
                catch (Exception)
                { }
            }

            public async void GetLocationByState()
            {
                try
                {
                    //http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerArea?LatitudUser=20&LongitudUser=-100
                    var MyLatLong = await Clases.Location.GetCurrentPosition();


                    var placemarks = await Geocoding.GetPlacemarksAsync(MyLatLong.Latitude, MyLatLong.Longitude);

                    var placemark = placemarks?.FirstOrDefault();



                    HttpClient client = new HttpClient();

                    string FaceID = null;
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    try
                    {

                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        FaceID = query.ProfileId;
                    }
                    catch (Exception ex)
                    {

                    }

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerAreaByStateAndCountry?" +
                        "Country=" + placemark.CountryName
                        + "&State=" + placemark.AdminArea
                        + "&FacebookID_UpdateArea=" + FaceID


                        ));
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                    try
                    {
                        if (FaceID == null)
                        {
                            db5.CreateTable<DatabaseTypes.Log_info>();


                            var s = db5.Insert(new DatabaseTypes.Log_info()
                            {
                                Region = rrr.ID,
                                ServiciosRegionales = rrr.ServicioRegional



                            });
                        }
                        else
                        {
                            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                            query.Region = rrr.ID;
                            query.ServiciosRegionales = rrr.ServicioRegional;
                            db5.RunInTransaction(() =>
                            {
                                db5.Update(query);
                            });

                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }

                catch (Exception Ex)
                {

                }
            }

            public async void UpdateRegion()
            {
                try
                {
                    //http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerArea?LatitudUser=20&LongitudUser=-100
                    var MyLatLong = await Clases.Location.GetCurrentPosition();


                    var placemarks = await Geocoding.GetPlacemarksAsync(MyLatLong.Latitude, MyLatLong.Longitude);

                    var placemark = placemarks?.FirstOrDefault();



                    HttpClient client = new HttpClient();
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerArea?" +
                        "LatitudUser=" + MyLatLong.Latitude
                        + "&LongitudUser=" + MyLatLong.Longitude
                        + "&FacebookID_UpdateArea=" + query.ProfileId


                        ));
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                    try
                    {
                      //  query.Region = rrr.ID;
                      //  query.ServiciosRegionales = rrr.ServicioRegional;
                      //  db5.RunInTransaction(() =>
                      //  {
                      //      db5.Update(query);
                      //  });
                    }
                    catch (Exception ex)
                    {

                    }
                }

                catch(Exception Ex)
                {

                }
            }
           
        }


        public static System.Timers.Timer aTimer = null;
        public static async void DoWork_Driver()
        {
            // Fragments.Fragment1.Vista.UpdateImage1(result);
            //window.Android_driver.RequestDriverState(deviceSessionId);
            // Create a timer with a two second interval.
            if (aTimer == null)
            {
                aTimer = new System.Timers.Timer(4000);
                // Hook up the Elapsed event for the timer. 
                //aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            }

            /*
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(1.8);

            var timer = new System.Threading.Timer((e) =>
            {
                try
                {
                    WebInterfaceMenuCarppi.UpdateLocation();
                    WebInterfaceMenuCarppi.DriverStateDetermination();
                   // UpdateLocation()



                }
                catch (System.Exception) { }

            }, null, startTimeSpan, periodTimeSpan);

            */


        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            WebInterfaceMenuCarppi.UpdateLocation();
            WebInterfaceMenuCarppi.DriverStateDetermination();
            Console.WriteLine("DriverTimer  at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

        public static FragmentMain NewInstance()
        {
            var frag1 = new FragmentMain { Arguments = new Bundle() };
            return frag1;
        }



        public class WebInterfaceMenuCarppi : Java.Lang.Object
        {
            Context mContext;
            WebView webi;
           public static WebView webi_static;
            public static Context StaticContext;
            public static enumEstado_del_usuario EstadoPrevioDelUsuario = enumEstado_del_usuario.Sin_actividad;
            public static double Costo_Global;
            public static SearchLocationObject Static_WhereToGo;
           public WebInterfaceMenuCarppi(Activity Act, WebView web)
            {
                mContext = Act;
                webi = web;
                webi_static = web;
                StaticContext = Act;
            }
            //CenterThemap(lat, lng)

            public static async void GetLocationByState()
            {
                try
                {
                    //http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerArea?LatitudUser=20&LongitudUser=-100
                    var MyLatLong = await Clases.Location.GetCurrentPosition();


                    var placemarks = await Geocoding.GetPlacemarksAsync(MyLatLong.Latitude, MyLatLong.Longitude);

                    var placemark = placemarks?.FirstOrDefault();



                    HttpClient client = new HttpClient();

                    string FaceID = null;
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    try
                    {
                       
                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        FaceID = query.ProfileId;
                    }
                    catch (Exception ex)
                    {

                    }

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerAreaByStateAndCountry?" +
                        "Country=" + placemark.CountryName
                        + "&State=" + placemark.AdminArea
                        + "&FacebookID_UpdateArea=" + FaceID


                        ));
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                    try
                    {
                        if(FaceID == null)
                        {
                            db5.CreateTable<DatabaseTypes.Log_info>();


                            var s = db5.Insert(new DatabaseTypes.Log_info()
                            {
                              Region = rrr.ID,
                              ServiciosRegionales = rrr.ServicioRegional



                            });
                        }
                        else
                        {
                            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                            query.Region = rrr.ID;
                                 query.ServiciosRegionales = rrr.ServicioRegional;
                                 db5.RunInTransaction(() =>
                                 {
                                     db5.Update(query);
                                 });

                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }

                catch (Exception Ex)
                {

                }
            }

            public static async void CenterThemap()
            {
                //DriverStateDetermination();
                try
                {
                    var MyLatLong = await Clases.Location.GetCurrentPosition();

                    var script = "CenterThemap("+ MyLatLong.Latitude + "," + MyLatLong.Longitude + ");";
                    Action action = () =>
                    {
                        webi_static.EvaluateJavascript(script, null);

                    };
                    webi_static.Post(action);
                }
                catch(Exception)
                { }
            }


            public static void UpdateTripState(UtilityJavascriptInterface.enumEstado_de_Solicitud estado, UtilityJavascriptInterface.Traveler_SolicitudDeViajeTemporal viaje)
            {
                DriverStateDetermination();
                var script = "ChangeToShowTripData();";
                Action action = () =>
                {
                    webi_static.EvaluateJavascript(script, null);

                };
                webi_static.Post(action);
            }
            public enum enumEstado_del_usuario {
                Sin_actividad,
                PidiendoViajePasajero,
                ViajeEncontradoPasajero,
                ViajandoPasajero,
                EntregadoPasajero,
                PidiendoViajeConductor,
                ViajandoConductor,
                ViajeAceptadoNoRecogido_Conductor,
                PasajeroSinArribar,
                PasajeroDejoPlantado,
                ViajandoPasajeroRideShare,
                ConductorEsperando,
                EsperandoCalificar,
                BusquedaEnProceso,
                DisponibleParaRideShare,
                EsperandoCalificarRideShare,
                Sin_actividadRideShare,
                ViajeNoEncontradoUsuario,
                PasajeroEsperandoconductorRideshare,
                BusquedaEnProcesoRideShare

            };
            public static async void DriverStateDetermination()
            {
                try
                {

                    // var cadena = Base64Decode(Base64_Obj);
                    // var aca = JsonConvert.DeserializeObject<Obj_publicacion>(cadena);

                    HttpClient client = new HttpClient();
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/DriverStateDetermination?" +
                        "FaceBookIdentifierDriverState=" + query.ProfileId
                        +""
                       

                        ));
                   // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;

                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        var errorMessage1 = S_Ressult.Response;

                        var aca = JsonConvert.DeserializeObject<DriverStateDeterminationResponse>(errorMessage1);
                         switch (aca.pidiendoViaje)
                        //switch (enumEstado_del_usuario.PasajeroDejoPlantado)
                        {
                            case enumEstado_del_usuario.EsperandoCalificar:
                                {
                                    if (EstadoPrevioDelUsuario != enumEstado_del_usuario.EsperandoCalificar)
                                    {
                                        DisplayRateDriver();
                                        
                                    }
                                    ClearAllMarkers();
                                }
                                break;
                            case enumEstado_del_usuario.Sin_actividad:
                                {
                                    try
                                    {
                                        var MyLatLong = await Clases.Location.GetCurrentPosition();
                                        //CenterThemap(lat, lng)

                                        DisplaySearchButton();
                                        ClearAllMarkers();
                                        WebInterfaceMenuCarppi.CenterThemap();
                                        GetLocationByState();
                                    }
                                    catch(Exception)
                                    {

                                    }
                                }
                                break;
                            case enumEstado_del_usuario.BusquedaEnProceso:
                                {
                                    if (EstadoPrevioDelUsuario != enumEstado_del_usuario.BusquedaEnProceso)
                                    {
                                      //  GetLAstTripData();
                                    }
                                    SetSearchBar();

                                }
                                break;
                            case enumEstado_del_usuario.ViajandoPasajero:
                                {
                                    // TripMap(aca);
                                    //MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;


                                    try
                                    {
                                        RequestID_ToFinish = Convert.ToInt32(aca.TripID);
                                        var MyLatLong = await Clases.Location.GetCurrentPosition();

                                        // var Myloc = new Geolocation_Service();
                                        //  var MyLatLong = await Myloc.GetLocationAsync();



                                        var GoingData = new GoingForPassengerData();
                                        GoingData.MyLatitud = MyLatLong.Latitude;
                                        GoingData.MyLongitud = MyLatLong.Longitude;
                                        GoingData.LatitudConductor = MyLatLong.Latitude;
                                        GoingData.LongitudConductor = MyLatLong.Longitude;
                                        GoingData.PassengerLatitude = Convert.ToDouble(aca.LatitudObjetivo);
                                        GoingData.PassengerLongitude = Convert.ToDouble(aca.LongitudObjetivo);
                                        GoingData.Name_Of_Passenger = "Josefo";


                                        Action action = async () =>
                                        {
                                            var script = "ShowRouteToGoal_Passenger(" + JsonConvert.SerializeObject(GoingData) + ")";

                                            webi_static.EvaluateJavascript(script, null);


                                        };
                                        webi_static.Post(action);
                                    }
                                    catch (Exception)
                                    { }

                                }
                                break;
                            case enumEstado_del_usuario.PasajeroDejoPlantado:
                                {

                                }
                                break;
                            case enumEstado_del_usuario.ViajandoPasajeroRideShare:
                                {
                                    //Aqui ya esta viajando , agregar un estasdo para la espera
                                    goto case enumEstado_del_usuario.ViajandoPasajero;
                                }
                                break;
                            case enumEstado_del_usuario.EsperandoCalificarRideShare:
                                {
                                    if (EstadoPrevioDelUsuario != enumEstado_del_usuario.EsperandoCalificarRideShare)
                                    {
                                        goto case enumEstado_del_usuario.EsperandoCalificar;
                                    }
                                }
                                break;
                            case enumEstado_del_usuario.DisponibleParaRideShare:
                                {
                                    //   await Task.Run(GetTripStateForDriver);
                                    //  await GetTripStateForDriver();
                                    //((Activity)StaticContext).RunOnUiThread(GetTripStateForDriver);
                                   // var RRR = JsonConvert.DeserializeObject<RideShareDriverStateResponse>(aca.rideShareDriverStateResponse);
                                    GetTripStateForDriver(aca.rideShareDriverStateResponse);
                                }
                                break;
                            case enumEstado_del_usuario.Sin_actividadRideShare:
                                {
                                    goto case enumEstado_del_usuario.DisponibleParaRideShare;
                                }
                            case enumEstado_del_usuario.ViajeNoEncontradoUsuario:
                                {
                                    ShowNotFoundTrip();
                                    //Console.WriteLine("Viaje No Encontrado");
                                }
                                break;
                            case enumEstado_del_usuario.PasajeroEsperandoconductorRideshare:
                                {
                                    try
                                    {

                                       RequestID_ToFinish = Convert.ToInt32(aca.TripID);
                                        var MyLatLong = await Clases.Location.GetCurrentPosition();

                                        // var Myloc = new Geolocation_Service();
                                        //  var MyLatLong = await Myloc.GetLocationAsync();


                                        double lat = Convert.ToDouble(MyLatLong.Latitude);
                                        double log = Convert.ToDouble(MyLatLong.Longitude);

                                        var GoingData = new GoingForPassengerData();
                                        GoingData.MyLatitud = Convert.ToDouble(aca.LatitudObjetivo); ;
                                        GoingData.MyLongitud = Convert.ToDouble(aca.LongitudObjetivo); ;

                                        GoingData.LatitudConductor = Convert.ToDouble(aca.LatitudConductor);
                                        GoingData.LongitudConductor = Convert.ToDouble(aca.LongitudConductor);

                                        GoingData.PassengerLatitude = Convert.ToDouble(aca.LatitudObjetivo); ;
                                        GoingData.PassengerLongitude =  Convert.ToDouble(aca.LongitudObjetivo); ;

                                        GoingData.Marca_Vehiculo= aca.Marca_Vehiculo;
                                        GoingData.Modelo_Vehiculo = aca.Modelo_Vehiculo;
                                        GoingData.Placa_Vehiculo = aca.Placa_Vehiculo ;
                                        GoingData.Color_Vehiculo = aca.Color_Vehiculo;
            //GoingData.Name_Of_Passenger = aca.;
            // var placemarks = await Geocoding.GetPlacemarksAsync(lat, log);
            var Meta = "";//placemarks.FirstOrDefault().Thoroughfare + " " + placemarks.FirstOrDefault().SubThoroughfare;

                                        Action action = async () =>
                                        {
                                            var script = "ShowRouteFromDriver(" + JsonConvert.SerializeObject(GoingData) + ",'" + Meta + "')";

                                            webi_static.EvaluateJavascript(script, null);


                                        };
                                        webi_static.Post(action);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }

                                }
                                break;
                            case enumEstado_del_usuario.BusquedaEnProcesoRideShare:
                                {
                                    goto case enumEstado_del_usuario.BusquedaEnProceso;
                                }
                                break;




                        }
                        await Clases.Location.StartListening();
                        // UpdateLocation();

                        // DisplayAceptRejectTripModal();

                        EstadoPrevioDelUsuario = aca.pidiendoViaje;
                      


                    }
                }
                catch (System.Exception ex)
                {

                    Console.WriteLine(ex);
                }
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
            public enum StatesOfDriver { isInDeliverRegion, TripRequestAvailable, IsGoingForThePasenger, HasNoRequest, IsNotAvailable, isInPickUpPassengerRegion, IsGoingToTheGoal };

            public class RideShareDriverStateResponse
            {
                public StatesOfDriver statesOfDriver;
                public CarppiRequestForDrive carppiRequest;
                public double LatitudPasajero;
                public double LongitudPasajero;
            }
            public class GoingForPassengerData
            {
                public double MyLatitud;
                public double MyLongitud;
                public double PassengerLatitude;
                public double PassengerLongitude;
                public double LatitudConductor;
                public double LongitudConductor;
                public string Name_Of_Passenger;
                public string Marca_Vehiculo;
                public string Modelo_Vehiculo;
                public string Placa_Vehiculo;
                public string Color_Vehiculo;
            }

            public static int RequestID_ToFinish { get; set; }
            public static async void GetTripStateForDriver(RideShareDriverStateResponse Data)
            {
                try
                {


                    switch (Data.statesOfDriver)
                   // switch (StatesOfDriver.TripRequestAvailable)
                    {
                        case StatesOfDriver.isInDeliverRegion:
                            {
                                //var ContextData = S_Ressult.Response;
                                //var ContextObject = JsonConvert.DeserializeObject<CarppiRequestForDrive>(ContextData);
                                RequestID_ToFinish = Convert.ToInt32(Data.carppiRequest.ID);
                                ShowFinishButton();

                            }
                            break;
                        case StatesOfDriver.IsGoingForThePasenger:
                            {
                                try
                                {
                                    RequestID_ToFinish = Convert.ToInt32(Data.carppiRequest.ID);
                                    var MyLatLong = await Clases.Location.GetCurrentPosition();

                                    // var Myloc = new Geolocation_Service();
                                    //  var MyLatLong = await Myloc.GetLocationAsync();


                                    double lat = Convert.ToDouble(MyLatLong.Latitude);
                                    double log = Convert.ToDouble(MyLatLong.Longitude);

                                    var GoingData = new GoingForPassengerData();
                                    GoingData.MyLatitud = MyLatLong.Latitude;
                                    GoingData.MyLongitud = MyLatLong.Longitude;
                                    GoingData.LatitudConductor = MyLatLong.Latitude;
                                    GoingData.LongitudConductor = MyLatLong.Longitude;
                                    GoingData.PassengerLatitude = Data.LatitudPasajero;
                                    GoingData.PassengerLongitude = Data.LongitudPasajero;
                                    GoingData.Name_Of_Passenger = Data.carppiRequest.NameOfRequester;

                                    var placemarks = await Geocoding.GetPlacemarksAsync(Data.LatitudPasajero, Data.LongitudPasajero);
                                    var Meta = placemarks.FirstOrDefault().Thoroughfare + " " + placemarks.FirstOrDefault().SubThoroughfare;

                                    Action action = async () =>
                                    {
                                        var script = "ShowRouteToPassenger(" + JsonConvert.SerializeObject(GoingData) +",'" + Meta + "')";

                                        webi_static.EvaluateJavascript(script, null);


                                    };
                                    webi_static.Post(action);
                                }
                                catch(Exception)
                                {

                                }


                            }
                            break;
                        case StatesOfDriver.isInPickUpPassengerRegion:
                            {
                                RequestID_ToFinish = Convert.ToInt32(Data.carppiRequest.ID);
                                Show_StartTrip_Button();
                            }
                            break;
                        case StatesOfDriver.IsGoingToTheGoal:
                            {
                                try
                                {
                                    RequestID_ToFinish = Convert.ToInt32(Data.carppiRequest.ID);
                                    var MyLatLong = await Clases.Location.GetCurrentPosition();

                                    // var Myloc = new Geolocation_Service();
                                    //  var MyLatLong = await Myloc.GetLocationAsync();



                                    var GoingData = new GoingForPassengerData();
                                    GoingData.MyLatitud = MyLatLong.Latitude;
                                    GoingData.MyLongitud = MyLatLong.Longitude;
                                    GoingData.LatitudConductor = MyLatLong.Latitude;
                                    GoingData.LongitudConductor = MyLatLong.Longitude;
                                    GoingData.PassengerLatitude = Convert.ToDouble(Data.carppiRequest.LatitudViajePendiente);
                                    GoingData.PassengerLongitude = Convert.ToDouble(Data.carppiRequest.LongitudViajePendiente);
                                    GoingData.Name_Of_Passenger = "Josefo";


                                    Action action = async () =>
                                    {
                                        var script = "ShowRouteToGoal_Driver(" + JsonConvert.SerializeObject(GoingData) + ")";

                                        webi_static.EvaluateJavascript(script, null);


                                    };
                                    webi_static.Post(action);
                                }
                                catch(Exception)
                                { }


                              

                            }
                            break;
                        //case StatesOfDriver.IsAttendingRequest:
                        //        {
                        //            try
                        //            {
                        //                var Resp = new DriverStateDeterminationResponse();
                        //            var Response_Server = Data.carppiRequest;//JsonConvert.DeserializeObject<CarppiRequestForDrive>(S_Ressult.Response);
                        //                //CarppiRequestForDrive
                        //                var MyLatLong = await Clases.Location.GetCurrentPosition();

                        //                Resp.MyLatitud = MyLatLong.Latitude;
                        //                Resp.MyLongitud = MyLatLong.Longitude;
                        //                Resp.LatitudConductor = MyLatLong.Latitude.ToString();
                        //                Resp.LongitudConductor = MyLatLong.Longitude.ToString();
                        //                Resp.LatitudObjetivo = Response_Server.LatitudViajePendiente.ToString();
                        //                Resp.LongitudObjetivo = Response_Server.LongitudViajePendiente.ToString();
                        //                Resp.distane_Driver_Objective = 15;
                        //                Resp.distane_Me_Driver = 19;
                        //                Resp.TripID = Convert.ToInt32(Response_Server.ID);
                        //                TripMap(Resp);
                        //            }
                        //            catch (Exception)
                        //            {
                        //            }
                        //        }
                        //        break;
                        case StatesOfDriver.TripRequestAvailable:
                            {
                                try
                                {
                                    RequestID_ToFinish = Convert.ToInt32(Data.carppiRequest.ID);
                                    var MyLatLong = await Clases.Location.GetCurrentPosition();

                                    Data_CarppiRequestForDrive = Data.carppiRequest;
                                    //Data.carppiRequest.Cost = 15;
                                    //Data.carppiRequest.NameOfRequester = "Jaime";
                                    Data.carppiRequest.Distance = ((Math.Abs((MyLatLong.Latitude - Data.LatitudPasajero)) + Math.Abs((MyLatLong.Longitude - Data.LongitudPasajero))) * (1 / 0.009090)).ToString() + "km";


                                    ShowRequestOptions(JsonConvert.SerializeObject(Data.carppiRequest));
                                }
                                catch(Exception)
                                {

                                }
                            }
                            break;
                        case StatesOfDriver.IsNotAvailable:
                            {
                                //Console.WriteLine("No disponible");
                                ShowNonActiveUserOption();
                                ClearAllMarkers();
                            }
                            break;
                        case StatesOfDriver.HasNoRequest:
                            {
                                GetLocationByState();
                                //Console.WriteLine("Hola");
                                ShowActiveUserOption();
                                ClearAllMarkers();
                            }
                            break;
                    }
                   // UpdateLocation();
                }

                catch (Exception) { }

            }

            public static async void UpdateLocation()
            {
                try
                {
                    var MyLatLong = await Clases.Location.GetCurrentPosition();
                    //WhereoGo.LatitudeOrigen = Loc.Latitude;
                    //WhereoGo.LongitudOrigen = Loc.Longitude;

                    HttpClient client = new HttpClient();
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/ActualizaLocacion?" +
                        "user5=" + query.ProfileId
                        + "&Latitud=" + MyLatLong.Latitude
                         + "&Longitud=" + MyLatLong.Longitude

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
                      //  Toast.MakeText(mContext, "Sesion Fiinalizada", ToastLength.Long).Show();
                    }
                }
                catch(Exception)
                {

                }

            }



            public static void Show_StartTrip_Button()
            {
                Action action = async () =>
                {
                    var script = "Show_StartTrip_Button()";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }

            public static void ShowNotFoundTrip()
            {
                Action action = async () =>
                {
                    var script = "ShowNotFoundTrip()";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }

            public static void ShowNonActiveUserOption()
            {
                Action action = async () =>
                {
                    var script = "ShowNonActiveUserOption()";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }
            public static void ShowActiveUserOption()
            {
                Action action = async () =>
                {
                    var script = "ShowActiveUserOption()";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }

            public static void ShowRequestOptions(string RequestTrip)
            {
                Action action = async () =>
                {
                    var script = "ShowRequestOptions("+ RequestTrip + ")";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }

            public static void TripMap(DriverStateDeterminationResponse aca)
            {
                if (aca.TripID > 0)
                {
                    Action action = async () =>
                    {
                        try
                        {
                            var MyLatLong = await Clases.Location.GetCurrentPosition();

                            // var Myloc = new Geolocation_Service();
                            //  var MyLatLong = await Myloc.GetLocationAsync();


                            double lat = Convert.ToDouble(MyLatLong.Latitude);
                            double log = Convert.ToDouble(MyLatLong.Longitude);
                            //
                            aca.MyLatitud = lat;
                            aca.MyLongitud = log;
                            // Fragment1.newTripID = aca.TripID;
                            // Fragment1.IsInTrip = aca.pidiendoViaje;
                            //var jsr = new JavascriptResult();
                            var script = "UpdateUIDriverStateVariable(" + JsonConvert.SerializeObject(aca) + ")";

                            webi_static.EvaluateJavascript(script, null);





                            var script2 = "ChangeToShowTripData();";

                            webi_static.EvaluateJavascript(script2, null);


                            //  webi_static.Post(action);
                        }
                        catch (System.Exception)
                        {

                        }

                    };

                    // Create a task but do not start it.
                    // Task t1 = new Task(action, "alpha");

                    webi_static.Post(action);



                }

            }

            public static void ShowFinishButton()
            {
                Action action = async () =>
                {
                    var script = "ShowFinishButton()";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }

            //ClearAllMarkers
            public static void ClearAllMarkers()
            {
                Action action = async () =>
                {
                    var script = "ClearAllMarkers(0)";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }
            //SwitchSearchProgressBarState
            public static void SetSearchBar()
            {
                Action action = async () =>
                {
                    var script = "SwitchSearchProgressBarState()";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }

            public static void DisplaySearchButton()
            {
                Action action = async () =>
                {
                    var script = "SwitchSToButtonState()";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);

            }

            public static async void GetLAstTripData()
            {
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/GEtLastSearchOfUser?" +
                    "user7_Hijo=" + query.ProfileId 

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
                    try
                    {
                        var WhereoGo = JsonConvert.DeserializeObject<SearchLocationObject>(errorMessage1);
                        if (WhereoGo.AtentionToDate != true)
                        {
                            var Ahora = DateTime.Now;
                            WhereoGo.Year = Ahora.Year;
                            WhereoGo.Month = Ahora.Month;
                            WhereoGo.Day = Ahora.Day;
                        }
                        if (WhereoGo.AtentionToOrigin != true)
                        {
                            var Loc = await Clases.Location.GetCurrentPosition();
                            WhereoGo.LatitudeOrigen = Loc.Latitude;
                            WhereoGo.LongitudOrigen = Loc.Longitude;
                        }
                        Look_For_Ride_AidMethod(JsonConvert.SerializeObject(WhereoGo));
                    }
                    catch (Exception) { }


                }


            }
            public static async void Look_For_Ride_AidMethod(string Data)
            {
                Console.WriteLine(Data);
                FragmentMain.MainWebView.SwitchSearchProgressBarState();
                var WhereoGo = JsonConvert.DeserializeObject<SearchLocationObject>(Data);
                if (WhereoGo.AtentionToDate != true)
                {
                    var Ahora = DateTime.Now;
                    WhereoGo.Year = Ahora.Year;
                    WhereoGo.Month = Ahora.Month;
                    WhereoGo.Day = Ahora.Day;
                }
                if (WhereoGo.AtentionToOrigin != true)
                {
                    var Loc = await Clases.Location.GetCurrentPosition();
                    WhereoGo.LatitudeOrigen = Loc.Latitude;
                    WhereoGo.LongitudOrigen = Loc.Longitude;
                }
                FragmentMain.MainWebView.Look_For_Ride(WhereoGo.LatitudeOrigen, WhereoGo.LongitudOrigen, WhereoGo.LatitudDestino, WhereoGo.LongitudDestino, Convert.ToInt32(WhereoGo.Day), Convert.ToInt32(WhereoGo.Month), Convert.ToInt32(WhereoGo.Year));
            }


            public static void DisplayRateDriver()
            {
                Action action_WhowAlert = () =>
                {

                    var sss = ((Activity)StaticContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                    sss.Settings.JavaScriptEnabled = true;

                    sss.Settings.DomStorageEnabled = true;
                    sss.Settings.LoadWithOverviewMode = true;
                    sss.Settings.UseWideViewPort = true;
                    sss.Settings.BuiltInZoomControls = true;
                    sss.Settings.DisplayZoomControls = false;
                    sss.Settings.SetSupportZoom(true);
                    sss.Settings.JavaScriptEnabled = true;

                    AssetManager assets = ((Activity)StaticContext).Assets;
                    string content;
                    var Viewww = new UtilityJavascriptInterface(StaticContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("RateUser.html")))
                    {
                        content = sr.ReadToEnd();
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());




                };

                ((Activity)StaticContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateHalfExpanded;


            }

            public class DriverStateDeterminationResponse
            {
                public enumEstado_del_usuario pidiendoViaje;
                public int TripID;
                public string LatitudConductor;
                public string LongitudConductor;
                public string LatitudObjetivo;
                public string LongitudObjetivo;
                public double MyLatitud;
                public double MyLongitud;
                public double distane_Me_Driver;
                public double distane_Driver_Objective;
                public enumEstado_del_usuario EstadoDelUsuario;
                public RideShareDriverStateResponse rideShareDriverStateResponse;
                public string Marca_Vehiculo;
                public string Modelo_Vehiculo;
                public string Placa_Vehiculo;
                public string Color_Vehiculo;
            }
            public class CarppiRequestForDrive
            {
                public int? ID { get; set; }
                public string FaceIDDriver { get; set; }
                public string FaceIDPassenger { get; set; }
                public double? Cost { get; set; }
                public int? Stat { get; set; }
                public double? LatitudViajePendiente { get; set; }
                public double? LongitudViajePendiente { get; set; }

                public string Distance { get; set; }
                public string NameOfRequester { get; set; }
            }


            [JavascriptInterface]
            [Export("ShowExtraOptionsOnDriverSearchingForPassenger")]
            public async void ShowExtraOptionsOnDriverSearchingForPassenger()
            {
               // This Method Calls Te interface for the driver to see the data and
               //  Comunicat With the passenger

                Action action_WhowAlert = () =>
                {
                    var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
            sss.Settings.JavaScriptEnabled = true;

                    sss.Settings.DomStorageEnabled = true;
                    sss.Settings.LoadWithOverviewMode = true;
                    sss.Settings.UseWideViewPort = true;
                    sss.Settings.BuiltInZoomControls = true;
                    sss.Settings.DisplayZoomControls = false;
                    sss.Settings.SetSupportZoom(true);
                    sss.Settings.JavaScriptEnabled = true;
                    
                    AssetManager assets = ((Activity)mContext).Assets;
            string content;
            var Viewww = new UtilityJavascriptInterface(mContext, sss);
            sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("PassengerExtraData.html")))
                    {
                        content = sr.ReadToEnd();
                        content = content.Replace("SustituteTripID", RequestID_ToFinish.ToString());
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
        sss.SetWebViewClient(new LocalWebViewClient());



                };

    ((Activity) mContext).RunOnUiThread(action_WhowAlert);

    MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;



            }
//OpenDrawer
[JavascriptInterface]
            [Export("OpenDrawer")]
            public async void OpenDrawer()
            {
                Action action = () =>
                {

                    var drawerLayout = ((Activity)mContext).FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
                    drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    
                };
                ((Activity)mContext).RunOnUiThread(action);
            }



            //ShowExtraOptionsOnPassengerWaitToDriver
            [JavascriptInterface]
            [Export("ShowExtraOptionsOnPassengerWaitToDriver")]
            public async void ShowExtraOptionsOnPassengerWaitToDriver()
            {

                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/GetExtraDataFromTheTrip?" +
                    "PendingTrip_ToExtractData=" + RequestID_ToFinish

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

                    var Dat = JsonConvert.DeserializeObject<TripExtraDatacaCommentUtility>(errorMessage1);


                    Action action_WhowAlert = () =>
                    {

                        var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                        sss.Settings.JavaScriptEnabled = true;

                        sss.Settings.DomStorageEnabled = true;
                        sss.Settings.LoadWithOverviewMode = true;
                        sss.Settings.UseWideViewPort = true;
                        sss.Settings.BuiltInZoomControls = true;
                        sss.Settings.DisplayZoomControls = false;
                        sss.Settings.SetSupportZoom(true);
                        sss.Settings.JavaScriptEnabled = true;
                        /// sss.AddJavascriptInterface(webi, "Android");
                        AssetManager assets = ((Activity)mContext).Assets;
                        string content;
                        var Viewww = new UtilityJavascriptInterface(mContext, sss);
                        sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                        using (StreamReader sr = new StreamReader(assets.Open("DriverExtraData.html")))
                        {
                            content = sr.ReadToEnd();

                            content = content.Replace("SustituteTripID", RequestID_ToFinish.ToString());
                            //
                            content = content.Replace("DriversNameID", Dat.DriverName);
                            //InnerRateID
                            content = content.Replace("InnerRateID", "Calificacion:" + ((int)(Dat.Rate * 100) / 100));
                            //TradeMark_andModelID
                            content = content.Replace("TradeMark_andModelID", Dat.Marca_Vehiculo + " " + Dat.Modelo_Vehiculo);
                            content = content.Replace("LicensePlate", "Placa: " + Dat.Placa_Vehiculo);
                            //CarSourceImage
                            content = content.Replace("CarSourceImage", "data:image/png;base64, " + Dat.Vehiclephoto);
                            //DriverPhotograpg
                            content = content.Replace("DriverPhotograpg", "data:image/png;base64, " + Dat.Driverphoto);
                            content = content.Replace("No hay comentarios por el momento", Dat.ListOfComents.Count > 0 ? GenerateCommentSeccion(Dat) : "Sin comentarios por el momento");


                            sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                        }
                        sss.SetWebViewClient(new LocalWebViewClient());




                    };

                    ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                    MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;

                }

            }

            public String GenerateCommentSeccion(TripExtraDatacaCommentUtility Data)
            {
                var BigSTR = "";
                foreach(var coment in Data.ListOfComents)
                {
                    BigSTR += "<img src='data:image/png;base64, " + coment.PhotoOfComenter + "' alt='Avatar' class='w3-left w3-circle w3-margin-right' style='width:80px'>";
                    BigSTR += "<p><span class='w3-large w3-text-black w3-margin-right'>Calificacion: "+ coment.ComentData.Rate+ "</span></p>";
                    BigSTR += "<p>" + coment.ComentData.Comentario + "</p>";
                }
                return BigSTR;
               
            }
        public class TripExtraDatacaCommentUtility
            {

                public string DriverName;
                public double? Rate;
                public string Marca_Vehiculo;
                public string Modelo_Vehiculo;
                public string Placa_Vehiculo;
                public string Color_Vehiculo;
                public string Vehiclephoto;
                public string Driverphoto;
                public List<CarppiComentUtility> ListOfComents;

            }
            public class CarppiComentUtility
            {
                public Carppi_ComentariosHaciaElPerfil ComentData;
                public string PhotoOfComenter;

            }


            public partial class Carppi_ComentariosHaciaElPerfil
            {
                public long ID { get; set; }
                public string Comentario { get; set; }
                public string FaceID_OfComentedFolk { get; set; }
                public string FaceID_OfRater { get; set; }
                public double? Rate { get; set; }
            }




            //OptiosDuringTripPickUp
            [JavascriptInterface]
            [Export("OptiosDuringTripPickUp")]
            public async void OptiosDuringTripPickUp()
            {
                //RequestID_ToFinish
                Action action = () =>
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                    alert.SetTitle("Opciones");
                    alert.SetMessage("Que deseas hacer?");

                   

                    alert.SetNegativeButton("Cancelar Viaje", async (senderAlert, args) =>
                    {
                        

                    });

                    alert.SetNeutralButton("Cerrar ventana", (senderAlert, args) =>
                    {
                        //  count--;
                        //  button.Text = string.Format("{0} clicks!", count);
                    });


                    Dialog dialog = alert.Create();
                    dialog.Show();
                };
                ((Activity)mContext).RunOnUiThread(action);

            }


            //ShowNonActiveOptionsToDrivers
            [JavascriptInterface]
            [Export("ShowNonActiveOptionsToDrivers")]
            public async void ShowNonActiveOptionsToDrivers()
            {
                //RequestID_ToFinish
                Action action = () =>
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                    alert.SetTitle("Opciones");
                    alert.SetMessage("Que deseas hacer?");

                    alert.SetPositiveButton("Postear Viaje en carpool", (senderAlert, args) =>
                    {

                    });

                    alert.SetNegativeButton("Empezar Jornada", async (senderAlert, args) =>
                    {
                        //RequestID_ToFinish
                        HttpClient client = new HttpClient();
                        //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                        var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        var db5 = new SQLiteConnection(databasePath5);
                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/StartSession?" +
                            "FaceIDOfDriver_StartSession=" + query.ProfileId

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
                            Toast.MakeText(mContext, "Sesion Iniciada", ToastLength.Long).Show();
                        }

                    });

                    alert.SetNeutralButton("Cerrar ventana", (senderAlert, args) =>
                    {
                        //  count--;
                        //  button.Text = string.Format("{0} clicks!", count);
                    });


                    Dialog dialog = alert.Create();
                    dialog.Show();
                };
                ((Activity)mContext).RunOnUiThread(action);

            }



            //
            //ShowOptionsToDrivers
            //FinishPendingTrip
            [JavascriptInterface]
            [Export("ShowOptionsToDrivers")]
            public async void ShowOptionsToDrivers()
            {
                //RequestID_ToFinish
                Action action = () =>
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                    alert.SetTitle("Opciones");
                    alert.SetMessage("Que deseas hacer?");

                    alert.SetPositiveButton("Postear Viaje en carpool", (senderAlert, args) =>
                    {
                      
                    });

                    alert.SetNegativeButton("Finalizar jornada",async (senderAlert, args) =>
                    {
                        //RequestID_ToFinish
                        HttpClient client = new HttpClient();
                        //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                        var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        var db5 = new SQLiteConnection(databasePath5);
                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/FinishSession?" +
                            "FaceIDOfDriver_FinishSession=" + query.ProfileId

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
                            Toast.MakeText(mContext, "Sesion Fiinalizada", ToastLength.Long).Show();
                        }

                    });

                    alert.SetNeutralButton("Cerrar ventana", (senderAlert, args) =>
                    {
                        //  count--;
                        //  button.Text = string.Format("{0} clicks!", count);
                    });


                    Dialog dialog = alert.Create();
                    dialog.Show();
                };
                ((Activity)mContext).RunOnUiThread(action);

            }


            //StartPendingTrip
            [JavascriptInterface]
            [Export("StartPendingTrip")]
            public static async void StartPendingTrip()
            {
                //RequestID_ToFinish
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/StartPendingTrip?" +
                    "FaceIDOfDriver_StartTrip=" + query.ProfileId
                    + "&TripRequestToStart=" + RequestID_ToFinish

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
            }


            [JavascriptInterface]
            [Export("FinishPendingTrip")]
            public static async void FinishPendingTrip()
            {
                //RequestID_ToFinish
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/FinishRide?" +
                    "FaceIDOfDriver_2=" + query.ProfileId
                    + "&TripRequestToFinish=" + RequestID_ToFinish

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
            }


            public static CarppiRequestForDrive Data_CarppiRequestForDrive;

            [JavascriptInterface]
            [Export("DisplayAceptRejectTripModal")]
            public static void DisplayAceptRejectTripModal()
            {
                Action action = async () =>
                {
                    var script = "ShowRequestOptions(" + JsonConvert.SerializeObject(Data_CarppiRequestForDrive) + ")";

                    webi_static.EvaluateJavascript(script, null);


                };
                webi_static.Post(action);
                Action action_WhowAlert = () =>
                {
                    //AlertDialog.Builder alert = new AlertDialog.Builder(mContext);

                    //LocalWebView sss = new LocalWebView(mContext);
                    var sss = ((Activity)(StaticContext)).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                    sss.Settings.JavaScriptEnabled = true;

                    sss.Settings.DomStorageEnabled = true;
                    sss.Settings.LoadWithOverviewMode = true;
                    sss.Settings.UseWideViewPort = true;
                    sss.Settings.BuiltInZoomControls = true;
                    sss.Settings.DisplayZoomControls = false;
                    sss.Settings.SetSupportZoom(true);
                    sss.Settings.JavaScriptEnabled = true;
                    /// sss.AddJavascriptInterface(webi, "Android");
                    AssetManager assets = ((StaticContext)).Assets;
                    string content;
                    var Viewww = new UtilityJavascriptInterface(StaticContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("RequestOption.html")))
                    {
                        content = sr.ReadToEnd();
                        var Name = Data_CarppiRequestForDrive.NameOfRequester == null ? "Sin Nombre" : Data_CarppiRequestForDrive.NameOfRequester.ToString();
                        var Costo = Data_CarppiRequestForDrive.Cost == null ? "$ 0.0" : "$" + Data_CarppiRequestForDrive.Cost.ToString();
                        var Distance = Data_CarppiRequestForDrive.Distance == null ? "20km" : Data_CarppiRequestForDrive.Distance.ToString() + "km";

                        content = content.Replace("UserName", Name);
                        content = content.Replace("UserCost", Costo);
                        content = content.Replace("UserDistance", Distance);
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());


                    //   sss.LoadUrl("https://connect.stripe.com/express/oauth/authorize?client_id=ca_Ggzg0AhS6oJ8zpin4CEKmHgTW4pHFJRW&state=read_write");
                    ///
                    //  dialog.Window.fla

                };
                // Device.BeginInvokeOnMainThread(action_WhowAlert);
                ((Activity)(StaticContext)).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;




            }

            [JavascriptInterface]
            [Export("DisplayTripTypeSelector")]
            public void DisplayTripTypeSelector(double CostoRideShare, double CostoPool)
            {

                Action action_WhowAlert = () =>
                {

                    var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                    sss.Settings.JavaScriptEnabled = true;

                    sss.Settings.DomStorageEnabled = true;
                    sss.Settings.LoadWithOverviewMode = true;
                    sss.Settings.UseWideViewPort = true;
                    sss.Settings.BuiltInZoomControls = true;
                    sss.Settings.DisplayZoomControls = false;
                    sss.Settings.SetSupportZoom(true);
                    sss.Settings.JavaScriptEnabled = true;

                    AssetManager assets = ((Activity)mContext).Assets;
                    string content;
                    var Viewww = new UtilityJavascriptInterface(mContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("TripTypeSelect.html")))
                    {
                        content = sr.ReadToEnd();
                        content = content.Replace("RideShareCost", "$" + CostoRideShare.ToString());
                        content = content.Replace("CarpoolCost", "$" + CostoPool.ToString());
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());


                };

                ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;




            }

            //DisplayBottomModal
            [JavascriptInterface]
            [Export("DisplayDestinySearchBottomModal")]
            public void DisplayDestinySearchBottomModal()
            {

                Action action_WhowAlert = () =>
                {
                    var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                    sss.Settings.JavaScriptEnabled = true;

                    sss.Settings.DomStorageEnabled = true;
                    sss.Settings.LoadWithOverviewMode = true;
                    sss.Settings.UseWideViewPort = true;
                    sss.Settings.BuiltInZoomControls = true;
                    sss.Settings.DisplayZoomControls = false;
                    sss.Settings.SetSupportZoom(true);
                    sss.Settings.JavaScriptEnabled = true;

                    AssetManager assets = ((Activity)mContext).Assets;
                    string content;
                    var Viewww = new UtilityJavascriptInterface(mContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    //using (StreamReader sr = new StreamReader(assets.Open("SearchForPlace.html")))
                    using (StreamReader sr = new StreamReader(assets.Open("DrivingNotFound.html")))
                    {
                        content = sr.ReadToEnd();
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());



                };

                ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateHalfExpanded;




            }

            //DissmissBottomModal

            [JavascriptInterface]
            [Export("DissmissBottomModal")]
            public void DissmissBottomModal()
            {
                Action action_WhowAlert = () =>
                {
                    var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                    sss.Settings.JavaScriptEnabled = true;

                    sss.Settings.DomStorageEnabled = true;
                    sss.Settings.LoadWithOverviewMode = true;
                    sss.Settings.UseWideViewPort = true;
                    sss.Settings.BuiltInZoomControls = true;
                    sss.Settings.DisplayZoomControls = false;
                    sss.Settings.SetSupportZoom(true);
                    sss.Settings.JavaScriptEnabled = true;

                    AssetManager assets = ((Activity)mContext).Assets;
                    string content;
                    var Viewww = new UtilityJavascriptInterface(mContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("EmptyPage.html")))
                    {
                        content = sr.ReadToEnd();
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());


                };

                ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

            }

            [JavascriptInterface]
            [Export("Look_For_Ride")]
            public async void Look_For_Ride(double LatOrigen, double LongOrigen, double LatDestino, double longDestino, int day, int month, int year)
            {
                try
                {
                    // var cadena = Base64Decode(Base64_Obj);
                    // var aca = JsonConvert.DeserializeObject<Obj_publicacion>(cadena);
                    var Client_Id = "none";
                    HttpClient client = new HttpClient();
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    try
                    {
                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        Client_Id = query.ProfileId;
                    }
                    catch (Exception)
                    {

                    }

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/ClientCalculateProximityLeaves?" +
                        "Latitud_salida=" + LatOrigen +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                        "&Longitud_salida=" + LongOrigen +
                        "&Latitud_Llegada=" + LatDestino +
                        "&Longitud_Llegada=" + longDestino
                       + "&FID_Solicitante=" + Client_Id
                       + "&day=" + day.ToString()
                       + "&month=" + month.ToString()
                       + "&year=" + year.ToString()

                        ));
                    HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.GetAsync(uri);

                    Console.WriteLine(response.StatusCode);
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
                  {
                '"'
                  });
                        Console.WriteLine(errorMessage1);
                        if (errorMessage1 != "[]")
                        {
                            var aca = JsonConvert.DeserializeObject<List<RewtunrDataOfSearch>>(errorMessage1);



                            Action action_WhowAlert = async () =>
                            {
                                var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                                sss.Settings.JavaScriptEnabled = true;

                                sss.Settings.DomStorageEnabled = true;
                                sss.Settings.LoadWithOverviewMode = true;
                                sss.Settings.UseWideViewPort = true;
                                sss.Settings.BuiltInZoomControls = true;
                                sss.Settings.DisplayZoomControls = false;
                                sss.Settings.SetSupportZoom(true);
                                sss.Settings.JavaScriptEnabled = true;

                                AssetManager assets = ((Activity)mContext).Assets;
                                string content;
                                var Viewww = new UtilityJavascriptInterface(mContext, sss);
                                sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                                using (StreamReader sr = new StreamReader(assets.Open("TripProposal.html")))
                                {
                                    content = sr.ReadToEnd();
                                    var Costo = ValidaViaje_y_Calcula_costo(((Math.Abs((LatOrigen - LatDestino)) + Math.Abs((LongOrigen - longDestino))) * (1 / 0.009090)).ToString());
                                    Costo_Global = (Costo);
                                    var nuevoCosto = Costo + ((Costo * 0.09) + 4.5);
                                    content = content.Replace("Viajes Encontrados", "Costo: " + (((int)(nuevoCosto * 100.00)) / 100.00).ToString());
                                    content = content.Replace("TableInsides", await GenerateTableToRequestTrip(aca));

                                    content = content.Replace("ArrayContent", ArrayOfTrips(aca));

                                    sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                                }
                                sss.SetWebViewClient(new LocalWebViewClient());



                            };

                            ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                            MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateHalfExpanded;


                        }
                        else
                        {
                            Action action = () =>
                            {
                                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                alert.SetTitle("Error");
                                alert.SetMessage("No se a encontrado ningun viaje");
                                //alert.SetPositiveButton("Añadir", (senderAlert, args) =>
                                //{

                                //});

                                alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                                {
                                    //  count--;
                                    //  button.Text = string.Format("{0} clicks!", count);
                                });

                                Dialog dialog = alert.Create();
                                dialog.Show();
                            };
                            ((Activity)mContext).RunOnUiThread(action);



                            Action action2 = () =>
                            {
                                //var jsr = new JavascriptResult();
                                var script = "SwitchSToButtonState()";
                                webi.EvaluateJavascript(script, null);


                            };


                            webi.Post(action2);
                          


                        }


                    }
                    else
                    {
                        Action Toatsaction = () =>
                        {
                            Toast.MakeText(mContext, "Error en el servidor", ToastLength.Short).Show();
                        };
                        ((Activity)mContext).RunOnUiThread(Toatsaction);
                            Action action = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "SwitchSToButtonState()";
                            webi.EvaluateJavascript(script, null);


                        };

                        // Create a task but do not start it.
                        // Task t1 = new Task(action, "alpha");

                        webi.Post(action);
                    }
                }
                catch (SQLiteException)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(mContext);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Error");
                    alert.SetMessage("Para Buscar un viaje tienes que loguear primero");
                    alert.SetButton("OK", (c, ev) =>
                    {
                        // Ok button click task  
                    });
                    alert.Show();
                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "SwitchSToButtonState()";
                        webi.EvaluateJavascript(script, null);


                    };

                    // Create a task but do not start it.
                    // Task t1 = new Task(action, "alpha");

                    webi.Post(action);
                }
                catch (System.Exception) {

                    Action Toatsaction = () =>
                    {
                        Toast.MakeText(mContext, "Error en el servidor", ToastLength.Short).Show();
                    };
                    ((Activity)mContext).RunOnUiThread(Toatsaction);
                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "SwitchSToButtonState()";
                        webi.EvaluateJavascript(script, null);


                    };

                    // Create a task but do not start it.
                    // Task t1 = new Task(action, "alpha");

                    webi.Post(action);



                }
            }

         

            public async Task<string> GenerateTableToRequestTrip(List<RewtunrDataOfSearch> Tacs)
            {
                //                 < tr >
                //                  < td > Adam </ td >
                //                  < td >
                //                      < div >
                //                          < h6 > Ditancia 47m de ti</ h6 >

                //                             < h6 > Costo: $912 </ h6 >

                //                            </ div >

                //                        </ td >

                //                        < td >

                //                            < div >

                //                                < button style = "  background-color: #529; /* Green */
                //border: none;
                //          color: white;
                //          padding: 15px 32px;
                //              text - align: center;
                //              text - decoration: none;
                //          display: inline - block;
                //              font - size: 16px; ">
                //                              Solicitar
                //                          </ button >
                //                      </ div >
                //                  </ td >

                //              </ tr >

                string rrr = "";
                var Loc = await Clases.Location.GetCurrentPosition();
                foreach (var trip in Tacs)
                {

                    var Distance = (Math.Sqrt(Math.Pow((Loc.Latitude - trip.DriverLat), 2) + Math.Pow((Loc.Longitude - trip.driverLong), 2)) * (1 / 0.009090));
                    var d2 = Distance > 1 ? "<h6> Distancia: " + Distance.ToString() + "km de ti</h6>" : "<h6> Distancia: " + ((int)(Distance * 1000)).ToString() + "m de ti</h6>";

                    rrr += "<tr>";
                    rrr += "<td> Adam </td>";
                    rrr += "<td>";
                    rrr += "<div>";
                    rrr += d2;
                    // rrr += "<h6> Costo: $912 </h6>";
                    rrr += "<h6> Calificacion: " + trip.Rate.ToString() + "&#11088;</h6>";
                    rrr += "</div>";
                    rrr += "</td>";
                    rrr += "<td>";

                    rrr += "<div>";


                    rrr += "<button style = 'background-color: #529; /* Green */";
                    rrr += "border: none;";
                    rrr += "color: white;";
                    rrr += "padding: 15px 32px;";
                    rrr += "text-align: center;";
                    rrr += "text-decoration: none;";
                    rrr += "display: inline - block;";
                    rrr += "font-size: 16px;' id='Tag_" + trip.TripId.ToString() + "' value='" + trip.TripId.ToString() + "' onclick='window.Android_BottomModal.SolicitarViaje(" + trip.TripId.ToString() + "); window.Android_BottomModal.LookForRequestOfTrip(" + trip.TripId.ToString() + ")';>";
                    rrr += "Solicitar";
                    rrr += "</button>";
                    rrr += "</div>";
                    rrr +="</td>";

                    rrr += "</tr>";
                }

                return rrr;
            }
            public string ArrayOfTrips(List<RewtunrDataOfSearch> Tacs)
            {
                List<string> fsf = new List<string>();
                foreach (var trip in Tacs)
                {
                    fsf.Add(trip.TripId.ToString());
                }
                return String.Join(",", fsf);

            }

           // [JavascriptInterface]
           // [Export("ValidaViaje_y_Calcula_costo")]
            public double ValidaViaje_y_Calcula_costo(string distancia)
            {
                Interpolate_Points P1 = new Interpolate_Points();
                P1.X0 = 0; P1.X1 = 20; P1.Y0 = 2; P1.Y1 = 1.5;// = {0,0,0,0 };
                Interpolate_Points P2 = new Interpolate_Points();
                P2.X0 = 20; P2.X1 = 60; P2.Y0 = 1.5; P2.Y1 = 1.05;// = {0,0,0,0 };
                Interpolate_Points P3 = new Interpolate_Points();
                P3.X0 = 60; P3.X1 = 120; P3.Y0 = 1.05; P3.Y1 = 0.97;// = {0,0,0,0 };
                Interpolate_Points P4 = new Interpolate_Points();
                P4.X0 = 120; P4.X1 = 290; P4.Y0 = 0.97; P4.Y1 = 0.765;// = {0,0,0,0 };
                Interpolate_Points P5 = new Interpolate_Points();
                P5.X0 = 290; P5.X1 = 500; P5.Y0 = 0.765; P5.Y1 = 0.735;// = {0,0,0,0 };
                List<Interpolate_Points> puntos = new List<Interpolate_Points>();
                puntos.Add(P1);
                puntos.Add(P2);
                puntos.Add(P3);
                puntos.Add(P4);
                puntos.Add(P5);
                double price = calculate_interpolation(puntos, Convert.ToDouble(distancia));
                AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
                alertDialog.SetTitle("Alert");
                alertDialog.SetMessage((price * Convert.ToDouble(distancia)).ToString());
                alertDialog.SetPositiveButton("Delete", (senderAlert, args) =>
                {
                    Toast.MakeText(mContext, "Deleted!", ToastLength.Short).Show();
                });
                Dialog dialog = alertDialog.Create();
                //dialog.Show();
                //Cost_Adapter
                var InterpolatedCost = (((price * Convert.ToDouble(distancia)) * 1.0745) + 3.0);
                // Fragment1.Cost = (InterpolatedCost).ToString();

                //Action action = () =>
                //{
                //    //var jsr = new JavascriptResult();
                //    var script = "Cost_Adapter(" + (InterpolatedCost).ToString() + ")";
                //    webi.EvaluateJavascript(script, null);

                //};

                // Create a task but do not start it.
                // Task t1 = new Task(action, "alpha");

                // webi.Post(action);

                return InterpolatedCost;


            }



            public class Interpolate_Points
            {
                public double X0;
                public double X1;
                public double Y0;
                public double Y1;
            }

           public double calculate_interpolation(List<Interpolate_Points> List_of_points, double x_arg)
            {
                double result = 0;
                if (x_arg >= 500)
                {
                    result = 0.74;
                    //var point = List_of_points[4];
                    //result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                }
                else
                {
                    foreach (var point in List_of_points)
                    {
                        if (x_arg >= point.X0 && x_arg < point.X1)
                        {
                            result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                        }

                    }
                }

                return result;
            }



            public class RewtunrDataOfSearch
            {
                public Int32 TripId;
                public double DriverLat;
                public double driverLong;
                public double Rate;
                public int Cost;
                public string Photo;

            }
            //SwitchSearchProgressBarState
            [JavascriptInterface]
            [Export("SwitchSearchProgressBarState")]
            public void SwitchSearchProgressBarState()
            {
                var script = "SwitchSearchProgressBarState();";
                Action action = () =>
                {
                    webi.EvaluateJavascript(script, null);

                };
                webi.Post(action);
                DissmissBottomModal();
            }

            [JavascriptInterface]
            [Export("DisplayDestinySelector")]
            public void DisplayDestinySelector()
            {

                Action action_WhowAlert = () =>
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);

                    LocalWebView sss = new LocalWebView(mContext);
                    sss.Settings.JavaScriptEnabled = true;

                    sss.Settings.DomStorageEnabled = true;
                    sss.Settings.LoadWithOverviewMode = true;
                    sss.Settings.UseWideViewPort = true;
                    sss.Settings.BuiltInZoomControls = true;
                    sss.Settings.DisplayZoomControls = false;
                    sss.Settings.SetSupportZoom(true);
                    sss.Settings.JavaScriptEnabled = true;
                    /// sss.AddJavascriptInterface(webi, "Android");
                    AssetManager assets = ((Activity)mContext).Assets;
                    string content;
                    var Viewww = new UtilityJavascriptInterface((Activity)mContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android");
                    using (StreamReader sr = new StreamReader(assets.Open("SearchForPlace.html")))
                    {
                        content = sr.ReadToEnd();
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());


                    //   sss.LoadUrl("https://connect.stripe.com/express/oauth/authorize?client_id=ca_Ggzg0AhS6oJ8zpin4CEKmHgTW4pHFJRW&state=read_write");
                    ///
                    alert.SetView(sss);
                    alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                    {
                        //count++;
                        // button.Text = string.Format("{0} clicks!", count);
                    });

                    alert.SetPositiveButton("Buscar", (senderAlert, args) =>
                    {
                        //SwitchTaskBarState
                        //count++;
                        // button.Text = string.Format("{0} clicks!", count);
                        var script = "SwitchTaskBarState();";
                        Action action = () =>
                        {
                            webi.EvaluateJavascript(script, null);

                        };
                        webi.Post(action);


                    
                    });

                    Dialog dialog = alert.Create();
                    //  dialog.Window.fla
                    dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                    dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                    dialog.Show();

                };
                // Device.BeginInvokeOnMainThread(action_WhowAlert);
                ((Activity)mContext).RunOnUiThread(action_WhowAlert);






            }

            public enum StatesOfRideShare { RequestCrated = 0, ReuqestAccepted = 1, RequestCanceled = 2 };
            //SetStateOfRideShareRequest
            [JavascriptInterface]
            [Export("SetStateOfRideShareRequest")]
            public async void SetStateOfRideShareRequest(StatesOfRideShare State)
            {
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/AcceptTripRequest?" +
                    "RequestID=" + Data_CarppiRequestForDrive.ID +
                    "&Desicion=" + (int)State

                    ));
                HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(uri);

                DissmissBottomModal();


            }

        }

        public class LocalWebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return false; // then it is not handled by default action

                // return base.ShouldOverrideUrlLoading(view, url);
            }
        }

        public class LocalWebView : WebView
        {
            //public LocalWebView(Context context)
            //{
            //    super(context);
            //}

            //public LocalWebView(Context context, AttributeSet attrs)
            //{
            //    super(context, attrs);
            //}

            //public LocalWebView(Context context, AttributeSet attrs, int defStyleAttr)
            //{
            //    super(context, attrs, defStyleAttr);
            //}

            //@TargetApi(Build.VERSION_CODES.LOLLIPOP)
            //public LocalWebView(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes)
            //{
            //    super(context, attrs, defStyleAttr, defStyleRes);
            //}

            //public LocalWebView(Context context, AttributeSet attrs, int defStyleAttr, boolean privateBrowsing)
            //{
            //    super(context, attrs, defStyleAttr, privateBrowsing);
            //}

            //@Override
            //public boolean onCheckIsTextEditor()
            //{
            //    return true;
            //}
            public LocalWebView(Context context) : base(context)
            {
            }

            public override bool OnCheckIsTextEditor()
            {
                return true;
            }
        }


        public class UtilityJavascriptInterface : Java.Lang.Object
        {
            Context mContext;
            WebView webi;
            static Dialog CurrentDialogReference;
            public UtilityJavascriptInterface(Context Act, WebView web)
            {
                mContext = Act;
                webi = web;
            }

            [JavascriptInterface]
            [Export("GetAllMessagesFromConversation")]
            public async void GetAllMessagesFromConversation(Int64 TripID)
            {

                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppi_MessagesApi/GetAllMessagesFromTheConversation?" +
                    "FaceID_Interest=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                    "&TripRequest=" + TripID 

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

                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "UpdateConversationLayout(" + response.Content.ReadAsStringAsync().Result + ")";
                        webi.EvaluateJavascript(script, null);


                    };

                    // Create a task but do not start it.
                    // Task t1 = new Task(action, "alpha");

                    webi.Post(action);
                }
                // Action Toatsaction = () =>
                // {
                //     Toast.MakeText(mContext, ss, ToastLength.Short).Show();
                // };
                //  ((Activity)mContext).RunOnUiThread(Toatsaction);
            }

            //SendMessageInRideshare
            [JavascriptInterface]
            [Export("DriverIsCancelingTrip")]
            public async void DriverIsCancelingTrip(Int64 TripID)
            {

                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/DriverISCancelingTrip?" +
                    "FaceID_CancelerDriver=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                    "&TripRequest_ToCancel=" + TripID

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

                // Action Toatsaction = () =>
                // {
                //     Toast.MakeText(mContext, ss, ToastLength.Short).Show();
                // };
                //  ((Activity)mContext).RunOnUiThread(Toatsaction);
            }



            //SendMessageInRideshare
            [JavascriptInterface]
            [Export("SendMessageInRideshare")]
            public async void SendMessageInRideshare(string message, Int64 TripID)
            {

                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppi_MessagesApi/PostMessageInRideShare?" +
                    "FaceID_Interlocutor=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                    "&TripRequest=" + TripID + 
                    "&Message=" + message

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

                    // Action Toatsaction = () =>
                    // {
                    //     Toast.MakeText(mContext, ss, ToastLength.Short).Show();
                    // };
                    //  ((Activity)mContext).RunOnUiThread(Toatsaction);
            }


            [JavascriptInterface]
            [Export("DismissPaymentModal")]
            public async void DismissPaymentModal()
            {
                
                Console.WriteLine("DismissModal");
                try
                {
                    //http://localhost:56436/api/TCarppiRideshare/IsUserAGirl?FaceIDOfUserMeantTOBeAgirl=102986257996595
                    HttpClient client = new HttpClient();
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/IsUserAGirl?" +
                        "FaceIDOfUserMeantTOBeAgirl=" + query.ProfileId

                        ));
                    Console.WriteLine(uri.AbsoluteUri);
                    HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.GetAsync(uri);
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
                 {
                    '"'
                 });
                        enumGender gender = (enumGender)Convert.ToInt32(errorMessage1);
                        if (gender != enumGender.Female)
                        {
                            SearchRegularTrip();
                           // VerifyIfIsAGirl();
                        }
                        else
                        {
                            SearchRegularGirlTrip(gender);
                        }
                    }

                }
                catch (Exception)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(mContext);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Error!");
                    alert.SetMessage("Tienes que loguear para poder pedir validar tu identidad");
                    alert.SetButton("OK", (c, ev) =>
                    {
                        // Ok button click task  
                    });
                    alert.Show();

                }


                //if(CurrentDialogReference != null)
                //{
                //    CurrentDialogReference.Dismiss();
                //}
                // Action Toatsaction = () =>
                // {
                //     Toast.MakeText(mContext, ss, ToastLength.Short).Show();
                // };
                //  ((Activity)mContext).RunOnUiThread(Toatsaction);
            }

            [JavascriptInterface]
            [Export("SSToast")]
            public void SSToast(int ss)
            {
                Console.WriteLine(ss);
                Action Toatsaction = () =>
                {
               //     Toast.MakeText(mContext, ss, ToastLength.Short).Show();
                };
              //  ((Activity)mContext).RunOnUiThread(Toatsaction);
            }

            //LookForRequestOfTrip
            [JavascriptInterface]
            [Export("LookForRequestOfTrip")]
            public async void LookForRequestOfTrip(Int32 TripState)
            {
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/GetStateOfRequestUnitarian?" +
                    "Face_Id_Hitchhicker=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                    "&TripID=" + TripState

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
                    var caca = JsonConvert.DeserializeObject<Traveler_SolicitudDeViajeTemporal>(errorMessage1);


                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "SetStateOfOpitonButton("+ errorMessage1 + ")";
                        webi.EvaluateJavascript(script, null);


                    };

                    // Create a task but do not start it.
                    // Task t1 = new Task(action, "alpha");

                    webi.Post(action);

                    if(caca.Estado_De_solicitud == (int)enumEstado_de_Solicitud.Aceptado)
                    {
                        DissmissBottomModal();
                        WebInterfaceMenuCarppi.UpdateTripState(enumEstado_de_Solicitud.Aceptado, caca);
                    }

                }


            }

            [JavascriptInterface]
            [Export("SwitchToTripState")]
            public void SwitchToTripState()
            {
              //  WebInterfaceMenuCarppi.UpdateTripState();
               // MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

            }

            public enum enumEstado_de_Solicitud { EnEspera, Aceptado, Rechazado, Conduciendo, NoLlego };
            public class Traveler_SolicitudDeViajeTemporal
            {
                public int ID { get; set; }
                public string Face_id_solicitante { get; set; }
                public int? Estado_De_solicitud { get; set; }
                public int? Id_del_viaje { get; set; }
                public int? TipoDePago { get; set; }
            }

            public enum enumTipoDePAgoPreferido { Tarjeta, EnEfectivo, Ambos };
            [JavascriptInterface]
            [Export("SolicitarViaje")]
            public async void SolicitarViaje(int TripId)
            {
                try
                {
                    // var cadena = Base64Decode(Base64_Obj);
                    // var aca = JsonConvert.DeserializeObject<Obj_publicacion>(cadena);

                    HttpClient client = new HttpClient();
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/RequestSpaceInTrip?" +
                        "Face_Identifier_3=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                        "&TripID=" + TripId +
                        "&TipoDePAgo=" + (int)enumTipoDePAgoPreferido.Tarjeta
                        + "&Amount=" + WebInterfaceMenuCarppi.Costo_Global

                        ));
                    HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.GetAsync(uri);
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.Accepted:
                            {
                                var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
        {
                '"'
        });
                            }
                            break;
                        case System.Net.HttpStatusCode.BadRequest:
                            {
                                Action action = () =>
                                {
                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("No Puedes reservar ningun viaje si no has añadido un metodo de pago, deseas añadirlo ahora?");

                                    alert.SetPositiveButton("Añadir", (senderAlert, args) =>
                                    {
                                        AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                        LocalWebView sss = new LocalWebView(mContext);
                                        sss.Settings.JavaScriptEnabled = true;

                                        sss.Settings.DomStorageEnabled = true;
                                        sss.Settings.LoadWithOverviewMode = true;
                                        sss.Settings.UseWideViewPort = true;
                                        sss.Settings.BuiltInZoomControls = true;
                                        sss.Settings.DisplayZoomControls = false;
                                        sss.Settings.SetSupportZoom(true);
                                        sss.Settings.JavaScriptEnabled = true;
                                        var wew = new UtilityJavascriptInterface(mContext, webi);

                                        // ;
                                        sss.AddJavascriptInterface(wew, "Android_BottomModal");
                                        //    int TutorHalfAnHourCalculator = (int)(((double)16 * 100) / 2);
                                        sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiPayment/index?Amount=" + WebInterfaceMenuCarppi.Costo_Global + "&User=" + query.ProfileId + "&TripID=" + TripId);

                                        alert.SetView(sss);
                                      

                                        alert.SetPositiveButton("Cerrar", (senderAlert, args) =>
                                        {
                                            //count++;
                                            // button.Text = string.Format("{0} clicks!", count);
                                        });

                                        Dialog dialog = alert.Create();
                                        //  dialog.Window.fla
                                        dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                                        dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                                        dialog.Show();
                                        // count++;
                                        // button.Text = string.Format("{0} clicks!", count);
                                    });

                                    alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                    {
                                    //  count--;
                                    //  button.Text = string.Format("{0} clicks!", count);
                                });

                                    Dialog dialog = alert.Create();
                                    dialog.Show();
                                };
                                ((Activity)mContext).RunOnUiThread(action);

                            }
                            break;
                    }

                
                }
                catch (System.Exception)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(mContext);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Error");
                    alert.SetMessage("Para Solicitar un viaje tienes que loguear primero");
                    alert.SetButton("OK", (c, ev) =>
                    {
                        // Ok button click task  
                    });
                    alert.Show();
                }
            }

            //RateUSer
            [JavascriptInterface]
            [Export("RateUser")]
            public async void RateUser(Int32 Stars)
            {

                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/RateTrip?" +
                    "IDOfRater=" + query.ProfileId +
                    "&Rate=" + Stars

                    ));

                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                {

                    DissmissBottomModal();
                }

                Console.WriteLine(uri.AbsoluteUri);
             
            }

            //DisplayBottomModal
            [JavascriptInterface]
            [Export("DissmissBottomModal")]
            public void DissmissBottomModal()
            {

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

            }
            [JavascriptInterface]
            [Export("Look_For_Ride_AidMethod")]
            public async void Look_For_Ride_AidMethod(string Data)
            {
                Console.WriteLine(Data);
                
                var WhereoGo = JsonConvert.DeserializeObject<SearchLocationObject>(Data);
                if(WhereoGo.AtentionToDate != true)
                {
                    var Ahora = DateTime.Now;
                    WhereoGo.Year = Ahora.Year;
                    WhereoGo.Month = Ahora.Month;
                    WhereoGo.Day = Ahora.Day;
                }
                if(WhereoGo.AtentionToOrigin != true)
                {
                    var Loc = await Clases.Location.GetCurrentPosition();
                    WhereoGo.LatitudeOrigen = Loc.Latitude;
                    WhereoGo.LongitudOrigen = Loc.Longitude;
                }
                try
                {
                    //http://localhost:56436/api/TCarppiRideshare/IsUserAGirl?FaceIDOfUserMeantTOBeAgirl=102986257996595
                    HttpClient client = new HttpClient();
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                    //(double LatOrigen_RequestCost, double LongOrigen_RequestCost, double LatDestino_RequestCost, double LongDestino_RequestCost, Int32 Region_RequestCost)

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/RequestCostOfTraveling?" +
                            "LatOrigen_RequestCost=" + WhereoGo.LatitudeOrigen +
                            "&LongOrigen_RequestCost=" + WhereoGo.LongitudOrigen +
                            "&LatDestino_RequestCost=" + WhereoGo.LatitudDestino +
                            "&LongDestino_RequestCost=" + WhereoGo.LongitudDestino
                            + "&Region_RequestCost=" + query.Region

                            ));
                    Console.WriteLine(uri.AbsoluteUri);
                    HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.GetAsync(uri);
                    Console.WriteLine(response.StatusCode);
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
                 {
                    '"'
                 });
                        FragmentMain.MainWebView.SwitchSearchProgressBarState();
                        Console.WriteLine(errorMessage1);
                        var response_Kwaii = JsonConvert.DeserializeObject<CostCalculatedReturnValue>(errorMessage1);
                        WhereoGo.RegularTripCost = response_Kwaii.RideShareCost;
                        Costo_Global = WhereoGo.RegularTripCost;
                        WhereoGo.SharedTripCost = response_Kwaii.CarpoolCost;
                        Static_WhereToGo = WhereoGo;
                        FragmentMain.MainWebView.DisplayTripTypeSelector((((int)(response_Kwaii.RideShareCost * 100.0)) / 100.00), (((int)(response_Kwaii.CarpoolCost * 100.0)) / 100.0));

                    }
                }

                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                //var Costo = ValidaViaje_y_Calcula_costo_RideShare(((Math.Abs((WhereoGo.LatitudeOrigen - WhereoGo.LatitudDestino)) + Math.Abs((WhereoGo.LongitudOrigen - WhereoGo.LongitudDestino))) * (1 / 0.009090)).ToString());
                //WhereoGo.RegularTripCost = Costo;
                //Costo_Global = (Costo);
                //var nuevoCosto = (Costo + ((Costo * 0.1) + 5)) > 10 ? (Costo + ((Costo * 0.1) + 5)) : 10 ;


                //var Costo_Pool = ValidaViaje_y_Calcula_costo(((Math.Abs((WhereoGo.LatitudeOrigen - WhereoGo.LatitudDestino)) + Math.Abs((WhereoGo.LongitudOrigen - WhereoGo.LongitudDestino))) * (1 / 0.009090)).ToString());
                ////Costo_Global = (Costo);
                //var nuevoCosto_Pool =( Costo_Pool + ((Costo_Pool * 0.1) + 5)) > 10 ? (Costo_Pool + ((Costo_Pool * 0.1) + 5)) : 10;
                //WhereoGo.SharedTripCost = Costo_Pool;
                //Static_WhereToGo = WhereoGo;

                //FragmentMain.MainWebView.DisplayTripTypeSelector((((int)(nuevoCosto*100.0))/100.00), (((int)(nuevoCosto_Pool *100.0)) /100.0));

                // FragmentMain.MainWebView.Look_For_Ride(WhereoGo.LatitudeOrigen, WhereoGo.LongitudOrigen, WhereoGo.LatitudDestino, WhereoGo.LongitudDestino ,Convert.ToInt32( WhereoGo.Day), Convert.ToInt32(WhereoGo.Month), Convert.ToInt32(WhereoGo.Year));
            }
            public class CostCalculatedReturnValue
            {
                public double RideShareCost;
                public double CarpoolCost;
                public CostCalculatedReturnValue(double rideshare, double pool)
                {
                    RideShareCost = rideshare;
                    CarpoolCost = pool;
                }
            }

            //SearchCarpoolTrip
            [JavascriptInterface]
            [Export("SearchCarpoolTrip")]
            public void SearchCarpoolTrip()
            {

                FragmentMain.MainWebView.Look_For_Ride(Static_WhereToGo.LatitudeOrigen, Static_WhereToGo.LongitudOrigen, Static_WhereToGo.LatitudDestino, Static_WhereToGo.LongitudDestino, Convert.ToInt32(Static_WhereToGo.Day), Convert.ToInt32(Static_WhereToGo.Month), Convert.ToInt32(Static_WhereToGo.Year));

            }

            public enum enumGender { Male, Female, Genderless };

            //SearchGirlTrip
            [JavascriptInterface]
            [Export("SearchGirlTrip")]
            public async void SearchGirlTrip()
            {
                try
                {
                    //http://localhost:56436/api/TCarppiRideshare/IsUserAGirl?FaceIDOfUserMeantTOBeAgirl=102986257996595
                    HttpClient client = new HttpClient();
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/IsUserAGirl?" +
                        "FaceIDOfUserMeantTOBeAgirl=" + query.ProfileId

                        ));
                    Console.WriteLine(uri.AbsoluteUri);
                    HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.GetAsync(uri);
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
                 {
                    '"'
                 });
                        enumGender gender = (enumGender)Convert.ToInt32(errorMessage1);
                        if(gender != enumGender.Female)
                        {
                            VerifyIfIsAGirl();
                        }
                        else
                        {
                            SearchRegularGirlTrip(gender);
                        }
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                            {
                        //Console.WriteLine(ex.ToString());
                        Action action = () =>
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                            alert.SetTitle("Error");
                            alert.SetMessage("No Puedes reservar ningun viaje si no has Logueado, Da click en el boton 'comparte tu viaje con carppi!' y despues oprime 'login' ");



                            alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                            {
                                //  count--;
                                //  button.Text = string.Format("{0} clicks!", count);
                            });

                            Dialog dialog = alert.Create();
                            dialog.Show();
                        };
                        ((Activity)mContext).RunOnUiThread(action);
                    }

                }
                catch (Exception)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(mContext);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Error!");
                    alert.SetMessage("Tienes que loguear para poder pedir validar tu identidad");
                    alert.SetButton("OK", (c, ev) =>
                    {
                            // Ok button click task  
                        });
                    alert.Show();

                }









            }
            public async void SearchRegularGirlTrip(enumGender gender)
            {
               
                    try
                    {

                        HttpClient client = new HttpClient();
                        //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                        var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        var db5 = new SQLiteConnection(databasePath5);
                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/SearchForRide?" +
                            "User1=" + query.ProfileId +
                            "&AreaOfService=" + query.Region +
                            "&Cost=" + Static_WhereToGo.RegularTripCost
                            + "&Latitud_Arg=" + Static_WhereToGo.LatitudDestino
                             + "&LongitudARG=" + Static_WhereToGo.LongitudDestino
                             + "&n_destino=" + Static_WhereToGo.Arrival
                              + "&Latitud_Origen=" + Static_WhereToGo.LatitudeOrigen
                             + "&Longitud_Origen=" + Static_WhereToGo.LongitudOrigen
                             + "&Gender=" + (int)gender



                            ));
                        Console.WriteLine(uri.AbsoluteUri);
                        HttpResponseMessage response;

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        response = await client.GetAsync(uri);

                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.BadRequest:
                                {

                                    Action action = () =>
                                    {
                                        AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                        alert.SetTitle("Error");
                                        alert.SetMessage("No Puedes reservar ningun viaje si no has añadido un metodo de pago, deseas añadirlo ahora?");

                                        alert.SetPositiveButton("Añadir", (senderAlert, args) =>
                                        {
                                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                            //alertToShow.getWindow().setSoftInputMode(
                                            //WindowManager.LayoutParams.SOFT_INPUT_STATE_ALWAYS_VISIBLE);
                                            //alert.
                                            //alert.SetTitle("Login");
                                            // alert.SetMessage("Do you want to add or substract?");
                                            // var c_view = (Fragment1.act).LayoutInflater.Inflate(Resource.Layout.OportinisticWebViiew, null);
                                            // WebView sss = new WebView(mContext);
                                            LocalWebView sss = new LocalWebView(mContext);
                                            sss.Settings.JavaScriptEnabled = true;

                                            sss.Settings.DomStorageEnabled = true;
                                            sss.Settings.LoadWithOverviewMode = true;
                                            sss.Settings.UseWideViewPort = true;
                                            sss.Settings.BuiltInZoomControls = true;
                                            sss.Settings.DisplayZoomControls = false;
                                            sss.Settings.SetSupportZoom(true);
                                            sss.Settings.JavaScriptEnabled = true;
                                            var wew = new UtilityJavascriptInterface(mContext, webi);

                                            // ;
                                            sss.AddJavascriptInterface(wew, "Android_BottomModal");
                                            //    int TutorHalfAnHourCalculator = (int)(((double)16 * 100) / 2);
                                            //public ActionResult Index(double Amount, string User, Int32 ServiceArea, double LatitudObjectivo, double LongitudObjetivo, string NombreDestino, enumGender Gender, double LatitudOrigen, double LongitudOrigen)
                                            Static_WhereToGo.RegularTripCost = 10;
                                            sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiAddCard/Index?Amount=" + Static_WhereToGo.RegularTripCost + "&User=" + query.ProfileId + "&ServiceArea=" + 1 + "&LatitudObjectivo=" + Static_WhereToGo.LatitudDestino + "&LongitudObjetivo=" + Static_WhereToGo.LongitudDestino + "&NombreDestino=" + Static_WhereToGo.Arrival + "&Gender=" + (int)Gender.Female + "&LatitudOrigen="  + Static_WhereToGo.LatitudeOrigen +  "&LongitudOrigen=" + Static_WhereToGo.LongitudOrigen);

                                            alert.SetView(sss);


                                            alert.SetPositiveButton("Cerrar", (senderAlert, args) =>
                                            {
                                                //count++;
                                                // button.Text = string.Format("{0} clicks!", count);
                                            });

                                            // alert.SetNegativeButton("Substract", (senderAlert, args) =>
                                            //  {
                                            // count--;
                                            // button.Text = string.Format("{0} clicks!", count);
                                            //  });



                                            //LoginManager.Instance.RegisterCallback(mFBCallManager, this);
                                            Dialog dialog = alert.Create();
                                            //  dialog.Window.fla
                                            dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                                            dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                                            dialog.Show();
                                            // count++;
                                            // button.Text = string.Format("{0} clicks!", count);
                                        });

                                        alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                        {
                                            //  count--;
                                            //  button.Text = string.Format("{0} clicks!", count);
                                        });

                                        Dialog dialog = alert.Create();
                                        dialog.Show();

                                        CurrentDialogReference = dialog;
                                    };
                                    ((Activity)mContext).RunOnUiThread(action);
                                }
                                break;
                            case System.Net.HttpStatusCode.Conflict:
                                {

                                    Action action = () =>
                                    {
                                        AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                        alert.SetTitle("Error");
                                        alert.SetMessage("No hay conductores en tu area");

                                        alert.SetPositiveButton("Aceptaar", (senderAlert, args) =>
                                        {

                                            // count++;
                                            // button.Text = string.Format("{0} clicks!", count);
                                        });


                                        Dialog dialog = alert.Create();
                                        dialog.Show();

                                        CurrentDialogReference = dialog;
                                    };
                                    ((Activity)mContext).RunOnUiThread(action);
                                }

                                break;

                            case System.Net.HttpStatusCode.Accepted:
                                {
                                    DissmissBottomModal();
                                }
                                break;
                        case System.Net.HttpStatusCode.InternalServerError:
                            {
                                //Console.WriteLine(ex.ToString());
                                Action action = () =>
                                {
                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("No Puedes reservar ningun viaje si no has Logueado, Da click en el boton 'comparte tu viaje con carppi!' y despues oprime 'login' ");



                                    alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                                    {
                                        //  count--;
                                        //  button.Text = string.Format("{0} clicks!", count);
                                    });

                                    Dialog dialog = alert.Create();
                                    dialog.Show();
                                };
                                ((Activity)mContext).RunOnUiThread(action);
                            }
                            break;
                        }
                    }

                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.ToString());
                        Action action = () =>
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                            alert.SetTitle("Error");
                            alert.SetMessage("No Puedes reservar ningun viaje si no has Logueado, Da click en el boton 'comparte tu viaje con carppi!' y despues oprime 'login' ");



                            alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                            {
                                //  count--;
                                //  button.Text = string.Format("{0} clicks!", count);
                            });

                            Dialog dialog = alert.Create();
                            dialog.Show();
                        };
                        ((Activity)mContext).RunOnUiThread(action);
                    }


                    // MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                    // FragmentMain.MainWebView.Look_For_Ride(Static_WhereToGo.LatitudeOrigen, Static_WhereToGo.LongitudOrigen, Static_WhereToGo.LatitudDestino, Static_WhereToGo.LongitudDestino, Convert.ToInt32(Static_WhereToGo.Day), Convert.ToInt32(Static_WhereToGo.Month), Convert.ToInt32(Static_WhereToGo.Year));

                
            }

            public async void UpdateGenderOfTraveler(Gender? gender)
            {
                try
                {
                    //http://localhost:56436/api/TCarppiRideshare/IsUserAGirl?FaceIDOfUserMeantTOBeAgirl=102986257996595
                    HttpClient client = new HttpClient();
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/UpdateGenderOfTraveler?" +
                        "FaceIDOfUserToUpdateGenderOfTraveler=" + query.ProfileId + 
                        "&GenderOfTraveler=" + (int)gender

                        ));
                    Console.WriteLine(uri.AbsoluteUri);
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

                }
                catch (Exception)
                {
                  

                }



            }
            public void VerifyIfIsAGirl()
            {
                Action action = () =>
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                    alert.SetTitle("Alerta!");
                    alert.SetMessage("Antes de pedir un viaje para mujeres, tienes que verificar que lo eres, otorga los permisos y despues tomate una selfie");

                    alert.SetPositiveButton("Aceptar", async (senderAlert, args) =>
                    {

                        Android.Support.V4.App.ActivityCompat.RequestPermissions((Activity)mContext, new System.String[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage }, 1);

                        try
                        {
                            await CrossMedia.Current.Initialize();

                            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                            {
                                // DisplayAlert("No Camera", ":( No camera available.", "OK");
                                return;
                            }
                            Uri uri2 = new Uri(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
                            string relativePath = uri2.MakeRelativeUri(uri2).ToString();

                            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                            {

                                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
                                Directory = relativePath,
                                Name = "test.jpg"
                            });


                            string Title = "Detecting...";
                            AlertDialog.Builder AlertLoading = new AlertDialog.Builder(mContext);

                            LocalWebView sss = new LocalWebView(mContext);
                            sss.Settings.JavaScriptEnabled = true;

                            sss.Settings.DomStorageEnabled = true;
                            sss.Settings.LoadWithOverviewMode = true;
                            sss.Settings.UseWideViewPort = true;
                            sss.Settings.BuiltInZoomControls = true;
                            sss.Settings.DisplayZoomControls = false;
                            sss.Settings.SetSupportZoom(true);
                            sss.Settings.JavaScriptEnabled = true;
                            //var wew = new UtilityJavascriptInterface(mContext, webi);

                            // ;
                            //sss.AddJavascriptInterface(wew, "Android_BottomModal");
                            //    int TutorHalfAnHourCalculator = (int)(((double)16 * 100) / 2);
                            AssetManager assets = ((Activity)mContext).Assets;
                            using (StreamReader sr = new StreamReader(assets.Open("LoadingBar.html")))
                            {
                                var Readocontent = sr.ReadToEnd();

                                sss.LoadDataWithBaseURL(null, Readocontent, "text/html", "utf-8", null);

                                // webi.LoadData(content, "text/html", null);
                                sss.SetWebViewClient(new NewWebClient(sss, mContext));
                                //wew.Get10LastHomeworks();


                            }
                            //sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiPayment/index?Amount=" + WebInterfaceMenuCarppi.Costo_Global + "&User=" + query.ProfileId + "&TripID=" + TripId);

                            AlertLoading.SetView(sss);

                            Dialog dialog_Loading = AlertLoading.Create();
                            dialog_Loading.SetCancelable(false);
                            dialog_Loading.SetCanceledOnTouchOutside(false);
                            //  dialog.Window.fla
                            dialog_Loading.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                            dialog_Loading.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                            dialog_Loading.Show();



                            if (file == null)
                                return;


                            var stream = File.Open(file.Path, FileMode.Open, System.IO.FileAccess.ReadWrite);

                            byte[] PhotoBytes = new byte[stream.Length];
                            stream.Read(PhotoBytes, 0, PhotoBytes.Length);

                            if (System.Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
                            {
                                faceClient.Endpoint = faceEndpoint;
                                //  faceClient.
                            }


                            faceList = await UploadAndDetectFaces(file.Path, PhotoBytes);
                            Title = String.Format(
                                "Detection Finished. {0} face(s) detected", faceList.Count);

                            if (faceList.Count == 1)
                            {
                                var Face = faceList.FirstOrDefault();
                                if (Face.FaceAttributes.Gender == Gender.Male || Face.FaceAttributes.Gender == Gender.Genderless)
                                {
                                    dialog_Loading.Dismiss();
                                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(mContext);
                                    AlertDialog alert = dialog.Create();
                                    alert.SetTitle("Error!");
                                    alert.SetMessage("No Puedes pedir este viaje");
                                    alert.SetButton("OK", (c, ev) =>
                                    {
                                        // Ok button click task  
                                    });
                                    alert.Show();

                                }
                                else
                                {
                                    UpdateGenderOfTraveler(Face.FaceAttributes.Gender);
                                    SearchRegularGirlTrip(enumGender.Female);
                                    dialog_Loading.Dismiss();
                                }

                            }
                            else
                            {
                                dialog_Loading.Dismiss();
                                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(mContext);
                                AlertDialog alert = dialog.Create();
                                alert.SetTitle("Error!");
                                alert.SetMessage("Debe de haber una persona en la foto (solo una)");
                                alert.SetButton("OK", (c, ev) =>
                                {
                                    // Ok button click task  
                                });
                                alert.Show();
                            }


                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }


                    });
                    alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                    {


                    });


                    Dialog dialog = alert.Create();
                    dialog.Show();

                    CurrentDialogReference = dialog;
                };
                ((Activity)mContext).RunOnUiThread(action);


            }


            private const string subscriptionKey = "5edb414bfe3a422fa04a289ab91c93e1";

         
            private const string faceEndpoint =
                "https://southcentralus.api.cognitive.microsoft.com";

            private readonly IFaceClient faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // The list of detected faces.
            private IList<DetectedFace> faceList;
         
            private string[] faceDescriptions;
            // The resize factor for the displayed image.
            private double resizeFactor;

            private const string defaultStatusBarText =
                "Place the mouse pointer over a face to see the face description.";


            const string APP_NAME = "SimpleCameraApp";
            private async Task<IList<DetectedFace>> UploadAndDetectFaces(string imageFilePath, byte[] os)
            {
                // The list of Face attributes to return.
                IList<FaceAttributeType> faceAttributes =
                    new FaceAttributeType[]
                    {
            FaceAttributeType.Gender, FaceAttributeType.Age,
            FaceAttributeType.Smile, FaceAttributeType.Emotion,
            FaceAttributeType.Glasses, FaceAttributeType.Hair
                    };

                // Call the Face API.
                try
                {
                    MemoryStream imageFileStream = new MemoryStream(os);
                    //Stream imageFileStream = null;
                    // os.CopyTo(imageFileStream);
                    // using (Stream imageFileStream = os.req)
                    // {
                    // The second argument specifies to return the faceId, while
                    // the third argument specifies not to return face landmarks.
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, faceAttributes);
                    return faceList;
                    // }
                }
                // Catch and display Face API errors.
                catch (APIErrorException f)
                {
                    //MessageBox.Show(f.Message);
                    return new List<DetectedFace>();
                }
                // Catch and display all other errors.
                catch (Exception e)
                {
                    //  MessageBox.Show(e.Message, "Error");
                    return new List<DetectedFace>();
                }
            }

            private string FaceDescription(DetectedFace face)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Face: ");

                // Add the gender, age, and smile.
                sb.Append(face.FaceAttributes.Gender);
                sb.Append(", ");
                sb.Append(face.FaceAttributes.Age);
                sb.Append(", ");
                sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));

                // Add the emotions. Display all emotions over 10%.
                sb.Append("Emotion: ");
                Emotion emotionScores = face.FaceAttributes.Emotion;
                if (emotionScores.Anger >= 0.1f) sb.Append(
                    String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));
                if (emotionScores.Contempt >= 0.1f) sb.Append(
                    String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));
                if (emotionScores.Disgust >= 0.1f) sb.Append(
                    String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));
                if (emotionScores.Fear >= 0.1f) sb.Append(
                    String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));
                if (emotionScores.Happiness >= 0.1f) sb.Append(
                    String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));
                if (emotionScores.Neutral >= 0.1f) sb.Append(
                    String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));
                if (emotionScores.Sadness >= 0.1f) sb.Append(
                    String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));
                if (emotionScores.Surprise >= 0.1f) sb.Append(
                    String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));

                // Add glasses.
                sb.Append(face.FaceAttributes.Glasses);
                sb.Append(", ");

                // Add hair.
                sb.Append("Hair: ");

                // Display baldness confidence if over 1%.
                if (face.FaceAttributes.Hair.Bald >= 0.01f)
                    sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

                // Display all hair color attributes over 10%.
                IList<HairColor> hairColors = face.FaceAttributes.Hair.HairColor;
                foreach (HairColor hairColor in hairColors)
                {
                    if (hairColor.Confidence >= 0.1f)
                    {
                        sb.Append(hairColor.Color.ToString());
                        sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                    }
                }

                // Return the built string.
                return sb.ToString();
            }


            //SearchRegularTrip
            [JavascriptInterface]
            [Export("SearchRegularTrip")]
            public async void SearchRegularTrip()
            {
                try
                {

                    HttpClient client = new HttpClient();
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TCarppiRideshare/SearchForRide?" +
                        "User1=" + query.ProfileId +
                        "&AreaOfService=" + query.Region +
                        "&Cost=" + Static_WhereToGo.RegularTripCost
                        +"&Latitud_Arg=" + Static_WhereToGo.LatitudDestino
                         + "&LongitudARG=" + Static_WhereToGo.LongitudDestino
                         + "&n_destino=" + Static_WhereToGo.Arrival
                          + "&Latitud_Origen=" + Static_WhereToGo.LatitudeOrigen
                         + "&Longitud_Origen=" + Static_WhereToGo.LongitudOrigen
                         + "&Gender=" + (int)enumGender.Genderless



                        ));
                    Console.WriteLine(uri.AbsoluteUri);
                    HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.GetAsync(uri);
                    Console.WriteLine(response.StatusCode);

                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            {

                                Action action = () =>
                                {
                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("No Puedes reservar ningun viaje si no has añadido un metodo de pago, deseas añadirlo ahora?");

                                    alert.SetPositiveButton("Añadir", (senderAlert, args) =>
                                    {
                                        AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                    //alertToShow.getWindow().setSoftInputMode(
                                    //WindowManager.LayoutParams.SOFT_INPUT_STATE_ALWAYS_VISIBLE);
                                    //alert.
                                    //alert.SetTitle("Login");
                                    // alert.SetMessage("Do you want to add or substract?");
                                    // var c_view = (Fragment1.act).LayoutInflater.Inflate(Resource.Layout.OportinisticWebViiew, null);
                                    // WebView sss = new WebView(mContext);
                                    LocalWebView sss = new LocalWebView(mContext);
                                        sss.Settings.JavaScriptEnabled = true;

                                        sss.Settings.DomStorageEnabled = true;
                                        sss.Settings.LoadWithOverviewMode = true;
                                        sss.Settings.UseWideViewPort = true;
                                        sss.Settings.BuiltInZoomControls = true;
                                        sss.Settings.DisplayZoomControls = false;
                                        sss.Settings.SetSupportZoom(true);
                                        sss.Settings.JavaScriptEnabled = true;
                                        var wew = new UtilityJavascriptInterface(mContext, webi);

                                    // ;
                                    sss.AddJavascriptInterface(wew, "Android_BottomModal");
                                        //    int TutorHalfAnHourCalculator = (int)(((double)16 * 100) / 2);
                                        // sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiAddCard/Index?Amount=" + Static_WhereToGo.RegularTripCost + "&User=" + query.ProfileId + "&ServiceArea=" + 1 + "&LatitudObjectivo=" + Static_WhereToGo.LatitudDestino + "&LongitudObjetivo=" + Static_WhereToGo.LongitudDestino + "&NombreDestino=" + Static_WhereToGo.Arrival );
                                        sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiAddCard/Index?Amount=" + Static_WhereToGo.RegularTripCost + "&User=" + query.ProfileId + "&ServiceArea=" + 1 + "&LatitudObjectivo=" + Static_WhereToGo.LatitudDestino + "&LongitudObjetivo=" + Static_WhereToGo.LongitudDestino + "&NombreDestino=" + Static_WhereToGo.Arrival + "&Gender=" + (int)Gender.Male + "&LatitudOrigen=" + Static_WhereToGo.LatitudeOrigen + "&LongitudOrigen=" + Static_WhereToGo.LongitudOrigen);

                                        alert.SetView(sss);


                                        alert.SetPositiveButton("Cerrar", (senderAlert, args) =>
                                        {
                                        //count++;
                                        // button.Text = string.Format("{0} clicks!", count);
                                    });

                                    // alert.SetNegativeButton("Substract", (senderAlert, args) =>
                                    //  {
                                    // count--;
                                    // button.Text = string.Format("{0} clicks!", count);
                                    //  });



                                    //LoginManager.Instance.RegisterCallback(mFBCallManager, this);
                                    Dialog dialog = alert.Create();
                                    //  dialog.Window.fla
                                    dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                                        dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                                        dialog.Show();
                                    // count++;
                                    // button.Text = string.Format("{0} clicks!", count);
                                });

                                    alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                    {
                                    //  count--;
                                    //  button.Text = string.Format("{0} clicks!", count);
                                });

                                    Dialog dialog = alert.Create();
                                    dialog.Show();

                                    CurrentDialogReference = dialog;
                                };
                                ((Activity)mContext).RunOnUiThread(action);
                            }
                            break;
                        case System.Net.HttpStatusCode.Conflict:
                            {

                                Action action = () =>
                                {
                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("No hay conductores en tu area");

                                    alert.SetPositiveButton("Aceptaar", (senderAlert, args) =>
                                    {
                                     
                                        // count++;
                                        // button.Text = string.Format("{0} clicks!", count);
                                    });

                                 
                                    Dialog dialog = alert.Create();
                                    dialog.Show();

                                    CurrentDialogReference = dialog;
                                };
                                ((Activity)mContext).RunOnUiThread(action);
                            }
                    
                            break;

                        case System.Net.HttpStatusCode.Accepted:
                            {
                                DissmissBottomModal();
                            }
                            break;
                        case System.Net.HttpStatusCode.InternalServerError:
                            {
                                //Console.WriteLine(ex.ToString());
                                Action action = () =>
                                {
                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("No Puedes reservar ningun viaje si no has Logueado, Da click en el boton 'comparte tu viaje con carppi!' y despues oprime 'login' ");



                                    alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                                    {
                                        //  count--;
                                        //  button.Text = string.Format("{0} clicks!", count);
                                    });

                                    Dialog dialog = alert.Create();
                                    dialog.Show();
                                };
                                ((Activity)mContext).RunOnUiThread(action);
                            }
                            break;
                    }
                }

                catch(Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                    Action action = () =>
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                        alert.SetTitle("Error");
                        alert.SetMessage("No Puedes reservar ningun viaje si no has Logueado, Da click en el boton 'comparte tu viaje con carppi!' y despues oprime 'login' ");

                       

                        alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                        {
                            //  count--;
                            //  button.Text = string.Format("{0} clicks!", count);
                        });

                        Dialog dialog = alert.Create();
                        dialog.Show();
                    };
                    ((Activity)mContext).RunOnUiThread(action);
                }


                // MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                // FragmentMain.MainWebView.Look_For_Ride(Static_WhereToGo.LatitudeOrigen, Static_WhereToGo.LongitudOrigen, Static_WhereToGo.LatitudDestino, Static_WhereToGo.LongitudDestino, Convert.ToInt32(Static_WhereToGo.Day), Convert.ToInt32(Static_WhereToGo.Month), Convert.ToInt32(Static_WhereToGo.Year));

            }


            public double ValidaViaje_y_Calcula_costo_RideShare(string distancia)
            {
                Interpolate_Points P1 = new Interpolate_Points();
                P1.X0 = 0; P1.X1 = 1.8; P1.Y0 = 6; P1.Y1 = 9.8;// = {0,0,0,0 };
                Interpolate_Points P2 = new Interpolate_Points();
                P2.X0 = 1.8; P2.X1 = 2.4; P2.Y0 = 9.8; P2.Y1 = 9.1;// = {0,0,0,0 };
                Interpolate_Points P3 = new Interpolate_Points();
                P3.X0 = 2.4; P3.X1 = 3.8; P3.Y0 = 9.1; P3.Y1 = 9.3;// = {0,0,0,0 };
                Interpolate_Points P4 = new Interpolate_Points();
                P4.X0 = 3.8; P4.X1 = 4.5; P4.Y0 = 9.3; P4.Y1 = 8.4;// = {0,0,0,0 };
                Interpolate_Points P5 = new Interpolate_Points();
                P5.X0 = 4.5; P5.X1 = 8; P5.Y0 = 8.4; P5.Y1 = 5;// = {0,0,0,0 };
                List<Interpolate_Points> puntos = new List<Interpolate_Points>();
                puntos.Add(P1);
                puntos.Add(P2);
                puntos.Add(P3);
                puntos.Add(P4);
                puntos.Add(P5);
                double price = calculate_interpolation_RideShare(puntos, Convert.ToDouble(distancia));
                AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
                alertDialog.SetTitle("Alert");
                alertDialog.SetMessage((price * Convert.ToDouble(distancia)).ToString());
                alertDialog.SetPositiveButton("Delete", (senderAlert, args) =>
                {
                    Toast.MakeText(mContext, "Deleted!", ToastLength.Short).Show();
                });
                Dialog dialog = alertDialog.Create();
                //dialog.Show();
                //Cost_Adapter
                var InterpolatedCost = (((price * Convert.ToDouble(distancia)) ));
                // Fragment1.Cost = (InterpolatedCost).ToString();

                //Action action = () =>
                //{
                //    //var jsr = new JavascriptResult();
                //    var script = "Cost_Adapter(" + (InterpolatedCost).ToString() + ")";
                //    webi.EvaluateJavascript(script, null);

                //};

                // Create a task but do not start it.
                // Task t1 = new Task(action, "alpha");

                // webi.Post(action);

                return InterpolatedCost;


            }
            public double calculate_interpolation_RideShare(List<Interpolate_Points> List_of_points, double x_arg)
            {
                double result = 0;
                if (x_arg >= 8)
                {
                    result = 5;
                    //var point = List_of_points[4];
                    //result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                }
                else
                {
                    foreach (var point in List_of_points)
                    {
                        if (x_arg >= point.X0 && x_arg < point.X1)
                        {
                            result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                        }

                    }
                }

                return result;
            }

            public double ValidaViaje_y_Calcula_costo(string distancia)
            {
                Interpolate_Points P1 = new Interpolate_Points();
                P1.X0 = 0; P1.X1 = 20; P1.Y0 = 2; P1.Y1 = 1.5;// = {0,0,0,0 };
                Interpolate_Points P2 = new Interpolate_Points();
                P2.X0 = 20; P2.X1 = 60; P2.Y0 = 1.5; P2.Y1 = 1.05;// = {0,0,0,0 };
                Interpolate_Points P3 = new Interpolate_Points();
                P3.X0 = 60; P3.X1 = 120; P3.Y0 = 1.05; P3.Y1 = 0.97;// = {0,0,0,0 };
                Interpolate_Points P4 = new Interpolate_Points();
                P4.X0 = 120; P4.X1 = 290; P4.Y0 = 0.97; P4.Y1 = 0.765;// = {0,0,0,0 };
                Interpolate_Points P5 = new Interpolate_Points();
                P5.X0 = 290; P5.X1 = 500; P5.Y0 = 0.765; P5.Y1 = 0.735;// = {0,0,0,0 };
                List<Interpolate_Points> puntos = new List<Interpolate_Points>();
                puntos.Add(P1);
                puntos.Add(P2);
                puntos.Add(P3);
                puntos.Add(P4);
                puntos.Add(P5);
                double price = calculate_interpolation(puntos, Convert.ToDouble(distancia));
                AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
                alertDialog.SetTitle("Alert");
                alertDialog.SetMessage((price * Convert.ToDouble(distancia)).ToString());
                alertDialog.SetPositiveButton("Delete", (senderAlert, args) =>
                {
                    Toast.MakeText(mContext, "Deleted!", ToastLength.Short).Show();
                });
                Dialog dialog = alertDialog.Create();
                //dialog.Show();
                //Cost_Adapter
                var InterpolatedCost = (((price * Convert.ToDouble(distancia)) * 1.0745) + 3.0);
                // Fragment1.Cost = (InterpolatedCost).ToString();

                //Action action = () =>
                //{
                //    //var jsr = new JavascriptResult();
                //    var script = "Cost_Adapter(" + (InterpolatedCost).ToString() + ")";
                //    webi.EvaluateJavascript(script, null);

                //};

                // Create a task but do not start it.
                // Task t1 = new Task(action, "alpha");

                // webi.Post(action);

                return InterpolatedCost;


            }


            public double calculate_interpolation(List<Interpolate_Points> List_of_points, double x_arg)
            {
                double result = 0;
                if (x_arg >= 500)
                {
                    result = 0.74;
                    //var point = List_of_points[4];
                    //result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                }
                else
                {
                    foreach (var point in List_of_points)
                    {
                        if (x_arg >= point.X0 && x_arg < point.X1)
                        {
                            result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                        }

                    }
                }

                return result;
            }



        }

        public class SearchLocationObject
        {
            public double LatitudeOrigen;
            public double LongitudOrigen;


            public double LatitudDestino;
            public double LongitudDestino;
            public int? Day;
            public int? Month;
            public int? Year;
            public bool AtentionToOrigin;
            public bool AtentionToDate;
            public string Departure;
            public string Arrival;
            public double SharedTripCost;
            public double RegularTripCost;



        }


}
}
