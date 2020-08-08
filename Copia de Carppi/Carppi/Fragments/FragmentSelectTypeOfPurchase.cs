
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android;
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
using Carppi.BackGroundWorkers;
using Carppi.DatabaseTypes;
using Java.Interop;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SQLite;
using Xamarin.Facebook;
using static Carppi.Fragments.LocalWebViewClient_RestaurantDetailedView;
using Fragment = Android.Support.V4.App.Fragment;

namespace Carppi.Fragments
{
    public class FragmentSelectTypeOfPurchase : Fragment
    {
        public static long OrderIDIfActive = 0;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            FragmentRestaurantDetailedView.ListaDeProductos = new List<CarppiGroceryProductos>();

            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);

            string content;
            AssetManager assets = this.Activity.Assets;
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
            // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
            /*
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                "CadenadelUsuarioRestaurant=" + FaceID


                ));
            */
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/UserUIStateDetermination?" +
              "UserChain=" + FaceID


              ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var S_Response = new UriResponse();
            S_Response.httpStatusCode = System.Net.HttpStatusCode.NotFound;
           
            try
            {

                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                
                S_Response = t.Result;
            }
            catch(Exception)
            {
                //no internet Exceptior
            }

            
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
            {



                // act = this.Activity;
                //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))


                var colorEnv =new Carppi.Clases.Environment_Android();
                var asss = colorEnv.GetOperatingSystemTheme();
                //var Template = asss == UiMode.NightNo? "CarppiDeliveryDashBoard.html" : "CarppiDeliveryDashBoardDarkMode.html"; 
                var Template = false? "CarppiDeliveryDashBoard.html" : "CarppiDeliveryDashBoardDarkMode.html"; 
                using (StreamReader sr = new StreamReader(assets.Open(Template)))
                {
                    content = sr.ReadToEnd();
                    var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                    var wew = new WebInterfaceRestaurantOptions(this.Activity, webi);
                    //MainWebView = wew;
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
                    webi.SetWebViewClient(new FragmentRestaurantView_WebClient(this.Activity, Resources, webi, stateOfRequest.ShowingRestaurants));
                   // WebInterfaceProfile.RetriveProfile();


                    //wew.Get10LastHomeworks();
                    //HTML String

                    //Load HTML Data in WebView

                }

            }
            else if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)

            {

                var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Response.Response);
                OrderIDIfActive = Order.ID;
                if (Order.Stat == GroceryOrderState.RequestEnded)
                {
                    ShowRateDeliverManRestaurant(this.Activity);
                }
                else
                { 
                //Map
                //An Order Is Pending


                var sss = view1.FindViewById<WebView>(Resource.Id.webView_);
                sss.Settings.JavaScriptEnabled = true;

                sss.Settings.DomStorageEnabled = true;
                sss.Settings.LoadWithOverviewMode = true;
                sss.Settings.UseWideViewPort = true;
                sss.Settings.BuiltInZoomControls = true;
                sss.Settings.DisplayZoomControls = false;
                sss.Settings.SetSupportZoom(true);
                sss.Settings.JavaScriptEnabled = true;

                // AssetManager assets = ((Activity)mContext).Assets;
                //string content;
                var Viewww = new WebInterfaceRestaurantOptions(this.Activity, sss);
                sss.AddJavascriptInterface(Viewww, "Android");

                    var colorEnv = new Carppi.Clases.Environment_Android();
                    var asss = colorEnv.GetOperatingSystemTheme();
                    //var Template = asss == UiMode.NightNo ? "FragmentGrocery_Map.html" : "FragmentGrocery_MapDarkMode.html";
                    var Template = false ? "FragmentGrocery_Map.html" : "FragmentGrocery_MapDarkMode.html";
                    using (StreamReader sr = new StreamReader(assets.Open(Template)))
                {
                    content = sr.ReadToEnd();
                    //ReplaceForDeliveryBoy_Searching
                    content = content.Replace("ReplaceForDeliveryBoy_Searching", Order.Stat == GroceryOrderState.RequestCreated ? "block" : "none");
                    content = content.Replace("ReplaceForDeliveryBoy_Acepted", Order.Stat == GroceryOrderState.RequestAccepted ? "block" : "none");
                    content = content.Replace("ReplaceForDeliveryBoy_BeingAttended", Order.Stat == GroceryOrderState.RequestBeingAttended ? "block" : "none");
                    content = content.Replace("ReplaceForDeliveryBoy_Ended", Order.Stat == GroceryOrderState.RequestEnded ? "block" : "none");
                    //ReplaceForDeliveryBoy_Rejected
                    content = content.Replace("ReplaceForDeliveryBoy_Rejected", Order.Stat == GroceryOrderState.RequestRejected ? "block" : "none");
                    sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                }
                sss.SetWebViewClient(new FragmentRestaurantView_WebClient(this.Activity, Resources, sss, stateOfRequest.ShowwingMap));

            }

            }

            return view1;
        }

        
        public static void ShowRateDeliverManRestaurant(Activity Esta)
        {
            try
            {
                Android.Support.V4.App.Fragment fragment = null;
                fragment = FragmentRateDElivery.NewInstance();
                MainActivity.static_FragmentMAnager.BeginTransaction()
                   .Replace(Resource.Id.content_frame, fragment)
                   .Commit();
            }
            catch(Exception)
            { }

          
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
        class FragmentRestaurantView_WebClient : WebViewClient
        {
            public Context mContext;

            public static Context Static_mContext;

            public Resources Resources;

            public WebView WebView_;

            public static WebView StaticWebView_;

            public static stateOfRequest stateOfRequest;

            private static System.Timers.Timer aTimer = null;



            public FragmentRestaurantView_WebClient(Context contexto, Resources res, WebView webi, stateOfRequest req)
            {
                mContext = contexto;
                Static_mContext = contexto;
                Resources = res;
                WebView_ = webi;
                stateOfRequest = req;
                StaticWebView_ = webi;
            }
            public static bool IsLocationAvailable()
            {
                if (!CrossGeolocator.IsSupported)
                    return false;

                return CrossGeolocator.Current.IsGeolocationAvailable;
            }

            public static bool LocationPermitionGranted()
            {
                //  Android.Support.V4.App.ActivityCompat.RequestPermissions((Activity)StaticContext, new System.String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.LocationHardware, Manifest.Permission.Internet, }, 1);
                var rt = (Android.Support.V4.App.ActivityCompat.CheckSelfPermission((Activity)Static_mContext, Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
               || (Android.Support.V4.App.ActivityCompat.CheckSelfPermission((Activity)Static_mContext, Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted);

                return rt;


            }


            public override async void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                try
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

                                Action TempAction = () =>
                                {
                                    //var jsr = new JavascriptResult();
                                    var script = "NoGpsActivated()";
                                    WebView_.EvaluateJavascript(script, null);


                                };


                                WebView_.Post(TempAction);

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
                                    Action TempAction = () =>
                                    {
                                        //var jsr = new JavascriptResult();
                                        var script = "NoGpsActivated()";
                                        WebView_.EvaluateJavascript(script, null);


                                    };


                                    WebView_.Post(TempAction);

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
                                        //RegionResponse = 15;
                                        var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                                        var db10 = new SQLiteConnection(databasePath10);

                                        db10.CreateTable<DatabaseTypes.Log_info>();
                                        RegionResponse = 15;
                                        if (query == null)
                                        {


                                            var s = db10.Insert(new DatabaseTypes.Log_info()
                                            {
                                                Region_Delivery = RegionResponse,

                                             

                                            });
                                        }
                                        else
                                        {
                                            query.Region_Delivery = RegionResponse;

                                         
                                            db10.RunInTransaction(() =>
                                            {
                                                db10.Update(query);
                                            });
                                        }

                                        await LoadAvailableProductsStartUp();
                                        var navigationView = ((Activity)mContext).FindViewById<NavigationView>(Resource.Id.nav_view);
                                        HideOptionsInMenu(navigationView);

                                        if (stateOfRequest == stateOfRequest.ShowwingMap)
                                        {
                                            aTimer = new System.Timers.Timer(2000);
                                            // Hook up the Elapsed event for the timer. 
                                            aTimer.Elapsed += OnTimedEvent;
                                            aTimer.AutoReset = true;
                                            aTimer.Enabled = true;
                                        }



                                    }



                                }
                                else
                                {

                                    Action TempAction = () =>
                                    {
                                        //var jsr = new JavascriptResult();
                                        var script = "NoGpsActivated()";
                                        WebView_.EvaluateJavascript(script, null);


                                    };


                                    WebView_.Post(TempAction);

                                }
                            }

                        }
                        else
                        {
                            Action TempAction = () =>
                            {
                                //var jsr = new JavascriptResult();
                                var script = "NoPermitionAlowed()";
                                WebView_.EvaluateJavascript(script, null);


                            };


                            WebView_.Post(TempAction);
                        }

                      




                    }
                    catch (System.Exception ex)
                    {
                        Action TempAction = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "NoPermitionAlowed()";
                            WebView_.EvaluateJavascript(script, null);


                        };


                        WebView_.Post(TempAction);
                    }

                    /*
                    await LoadAvailableProductsStartUp();
                    var navigationView = ((Activity)mContext).FindViewById<NavigationView>(Resource.Id.nav_view);
                    HideOptionsInMenu(navigationView);

                    if (stateOfRequest == stateOfRequest.ShowwingMap)
                    {
                        aTimer = new System.Timers.Timer(2000);
                        // Hook up the Elapsed event for the timer. 
                        aTimer.Elapsed += OnTimedEvent;
                        aTimer.AutoReset = true;
                        aTimer.Enabled = true;
                    }
                    */
                }
                catch(Exception)
                {
                    Action TempAction = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "NoPermitionAlowed()";
                        WebView_.EvaluateJavascript(script, null);


                    };


                    WebView_.Post(TempAction);
                }


            }
               public bool isLoggedIn()
            {

                AccessToken accessToken = AccessToken.CurrentAccessToken;//AccessToken.getCurrentAccessToken();
                return accessToken != null;
                //return false;
            }
            public async void HideOptionsInMenu(NavigationView mNavigationView)
            {
                try
                {
                    // var Mnu = FindViewById<IMenu>(Resource.Id.MuttaMenu);
                    var menuNav = mNavigationView.Menu;
                    menuNav.FindItem(Resource.Id.nav_GroceryRequest).SetVisible(false);
                    menuNav.FindItem(Resource.Id.nav_GroceryConversation).SetVisible(false);
                    menuNav.FindItem(Resource.Id.nav_home).SetVisible(false);
                    menuNav.FindItem(Resource.Id.nav_LogOutButton).SetVisible(false);
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
                        menuNav.FindItem(Resource.Id.nav_friends).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_discussion).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);
                        menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);
                       // menuNav.FindItem(Resource.Id.nav_LogOutButton).SetVisible(false);
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

                           // response = await client.GetAsync(uri);
                            menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);
                            menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);
                            menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);
                            menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);
                            /*

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
                            */
                        }
                        catch (System.Exception) { }

                    }
                
                }
                catch (Exception)
                { }
            }

            private static void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                try
                {
                    if (stateOfRequest == stateOfRequest.ShowwingMap)
                    {
                        string FaceID = null;
                        var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        var db5 = new SQLiteConnection(databasePath5);
                        HttpClient client = new HttpClient();


                        //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                        try
                        {

                            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                            FaceID = query.ProfileId;
                        }
                        catch (Exception ex)
                        {

                        }
                        // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                            "CadenadelUsuarioRestaurant=" + FaceID


                            ));
                        // HttpResponseMessage response;

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //  var  response =  client.GetAsync(uri).Result;
                        var t = Task.Run(() => GetResponseFromURI(uri));
                        //t.Wait();

                        var S_Response = t.Result;
                        if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)

                        {

                            var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Response.Response);
                            Action action2 = () =>
                            {
                        //var jsr = new JavascriptResult();
                        var script = "ShowStatusOfGroceryOrder(" + ((int)Order.Stat) + "," + S_Response.Response + ")";
                                StaticWebView_.EvaluateJavascript(script, null);


                            };


                            StaticWebView_.Post(action2);

                            if (Order.Stat == GroceryOrderState.RequestEnded)
                            {
                                aTimer.Enabled = false;
                                aTimer.AutoReset = false;
                                stateOfRequest = stateOfRequest.ShowingRestaurants;
                                aTimer.Stop();
                                aTimer = null;
                                ShowRateDeliverManRestaurant((Activity)Static_mContext);
                            }


                        }
                    }
                }
                catch(Exception)
                { }
            }

            public async Task LoadAvailableProductsStartUp()
            {
                try
                {
                    HttpClient client = new HttpClient();


                    string FaceID = null;
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    long RegionTemp = 0;
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    try
                    {

                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        FaceID = query.ProfileId;
                        RegionTemp =Convert.ToInt64( query.Region_Delivery);
                    }
                    catch (Exception ex)
                    {

                    }
                  
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDeterminationWithRealRegion?" +
                     "CadenadelUsuarioRestaurant=" + FaceID +
                     "&RealRegion=" + RegionTemp


                     ));

                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    //var t = Task.Run(() => GetResponseFromURI(uri));
                    //t.Wait();
                    var aca = await GetResponseFromURI(uri);

                    var S_Response = aca;//.Result;
                    if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // var ListOfRestaurants = JsonConvert.DeserializeObject<List<restaurantDelta>>(S_Response.Response);
                        var F_Response = JsonConvert.DeserializeObject<FirstResponseWithRegion>(S_Response.Response);
                        if (F_Response.RestaurantesDelta.Count > 0)
                        {
                            Action action = () =>
                            {
                                //var jsr = new JavascriptResult();
                                var script = "SetCategoriesStartup(" + F_Response.BitfieldForProducts + ")";
                                WebView_.EvaluateJavascript(script, null);


                            };


                            WebView_.Post(action);
                            foreach (var Rest in F_Response.RestaurantesDelta)
                            {
                                await QueryRestaurant(Rest, WebView_);
                                var t = await Task.Run(() => new UpdateProductList(Rest.RestaurantIndex));
                                /*
                                Task
                                .Factory
                                .StartNew(() => {
                                    new UpdateProductList(Rest.RestaurantIndex);
                                    Console.WriteLine(Thread.CurrentThread.IsBackground);
                                    }
                                )
                                .Wait();

                                */

                                /*
                                var bgw = new BackgroundWorker();
                                bgw.DoWork += (_, __) =>
                                {
                                    var t = Task.Run(() => new UpdateProductList(Rest.RestaurantIndex));
                                    // Thread.Sleep(1000);
                                };

                                bgw.RunWorkerAsync();
                                */
                                /*
                                new Thread(() =>
                                {
                                    Thread.CurrentThread.IsBackground = true;

                                    //  Console.WriteLine("Hello, world");
                                    new UpdateProductList(Rest.RestaurantIndex);
                                }).Start();
                            */
                                // new UpdateProductList(Rest.RestaurantIndex);
                            }
                        }
                        else
                        {
                            Action TempAction = () =>
                            {
                                //var jsr = new JavascriptResult();
                                var script = "NoRestaurantsInArea()";
                                WebView_.EvaluateJavascript(script, null);


                            };


                            WebView_.Post(TempAction);
                        }

                      
                       
                    }
                }
                catch (Exception)
                { }
            }
            public class FirstResponseWithRegion
            {
                public long BitfieldForProducts;
                public List<restaurantDelta> RestaurantesDelta;
            }
            public async Task QueryRestaurant(restaurantDelta RestaurantIndex, WebView LocalWebView)
            {

                var databasePath11 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Restaurantes.db");
                var db11 = new SQLiteConnection(databasePath11);
                var RestaurantQuery = new Carppi_IndicesdeRestaurantes();
                try
                {
                    RestaurantQuery = db11.Table<DatabaseTypes.Carppi_IndicesdeRestaurantes>().Where(v => v.ID == RestaurantIndex.RestaurantIndex).FirstOrDefault();
                }
                catch (Exception) { }
                if (RestaurantQuery == null)
                {
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
                    // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/DownloadRestaurant?" +
                        "RestaurantToDownload=" + RestaurantIndex.CarppiHash


                        ));
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                   // var t = Task.Run(() => GetResponseFromURI(uri));
                   // var S_Response = t.Result;


                    var aca = await GetResponseFromURI(uri);

                    //t.Wait();
                    if (aca.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {

                        //EraseContentGrid
                        var Respuesta = JsonConvert.DeserializeObject<Carppi_IndicesdeRestaurantes>(aca.Response);
                        if (Respuesta != null) {
                            var AddingProduct = db11.Insert(new DatabaseTypes.Carppi_IndicesdeRestaurantes()
                            {
                                ID = Respuesta.ID,
                                CarppiHash = Respuesta.CarppiHash,
                                Foto = Respuesta.Foto,
                                Nombre = Respuesta.Nombre,
                                EstaAbierto = Respuesta.EstaAbierto,
                                Categoriasbitfield = Respuesta.Categoriasbitfield,
                                UpdateTag = RestaurantIndex.UpdateTag,
                                AnyDeliveryMan = RestaurantIndex.AnyDeliveryMan,
                                calificacion = RestaurantIndex.calificacion

                            });

                            Action action = () =>
                        {
                           
                            var script = "UpdateREstaurantGrid(" + aca.Response + ")";
                            LocalWebView.EvaluateJavascript(script, null);


                        };

                        //UpdateProductGrid
                        //SetStartupMenu
                        LocalWebView.Post(action);
                    }



                    }
                }
                else
                {
                   // var Respuesta = JsonConvert.DeserializeObject<Carppi_IndicesdeRestaurantes>(S_Response.Response);
                    RestaurantQuery.EstaAbierto = RestaurantIndex.EstaAbierto;
                    RestaurantQuery.AnyDeliveryMan = RestaurantIndex.AnyDeliveryMan;
                    RestaurantQuery.calificacion = RestaurantIndex.calificacion;
                    db11.RunInTransaction(() =>
                    {
                        db11.Update(RestaurantQuery);
                    });
                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        // var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);

                        // var asas = new SevenZip.Compression.LZMA.Decoder();

                        // Productquery.Foto = Decompress(Productquery.Foto);
                        var script = "UpdateREstaurantGrid(" + JsonConvert.SerializeObject(RestaurantQuery) + ")";
                        LocalWebView.EvaluateJavascript(script, null);


                    };


                    LocalWebView.Post(action);
                }
            }


            public class restaurantDelta
            {
                public long RestaurantIndex;
                public bool EstaAbierto;
                public string CarppiHash;
                public string UpdateTag;
                public bool AnyDeliveryMan;
                public double calificacion;
            }
            public async void LoadAvailableProductsStartUpLegacy()
            {
                try
                {
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
                    // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
                    /*
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                        "CadenadelUsuarioRestaurant=" + FaceID


                        ));

                     var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDeterminationTest?" +
                        "CadenadelUsuarioRestaurant_TOTEst=" + FaceID


                        ));

                     var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                        "CadenadelUsuarioRestaurant=" + FaceID


                        ));

                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                      "CadenadelUsuarioRestaurant=" + FaceID


                      ));
                    */
                    /*
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                      "CadenadelUsuarioRestaurant=" + FaceID


                      ));
                    */

                    /*
                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiIOSRestaurantApi/CarppiRestaurantExistanceDeterminationTest?" +
                         "CadenadelUsuarioRestaurant_TOTEst=" + FaceID


                         ));
                    */
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                     "CadenadelUsuarioRestaurant=" + FaceID


                     ));
                  
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    //var t = Task.Run(() => GetResponseFromURI(uri));
                    //t.Wait();
                    var aca = await GetResponseFromURI(uri);

                    var S_Response = aca;//.Result;
                    if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {

                        Action action = () =>
                        {
                        //var jsr = new JavascriptResult();
                        var script = "SetStartupMenu(" + S_Response.Response + ")";
                            WebView_.EvaluateJavascript(script, null);


                        };


                        WebView_.Post(action);
                    }
                }
                catch(Exception)
                { }
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

        }



        public static FragmentSelectTypeOfPurchase NewInstance()
        {
            var frag1 = new FragmentSelectTypeOfPurchase { Arguments = new Bundle() };
            return frag1;
        }
    }
    public class WebInterfaceRestaurantOptions : Java.Lang.Object
    {
        Context mContext;
        WebView webi;
        public static WebView webi_static;
        public static Context StaticContext;
        //public static enumEstado_del_usuario EstadoPrevioDelUsuario = enumEstado_del_usuario.Sin_actividad;
        public static double Costo_Global;
        //public static SearchLocationObject Static_WhereToGo;
        public WebInterfaceRestaurantOptions(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            webi_static = web;
            StaticContext = Act;
        }

        [JavascriptInterface]
        [Export("RequestPermissions")]
        public async void RequestPermissions()
        {
            Android.Support.V4.App.ActivityCompat.RequestPermissions((Activity)mContext, new System.String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.LocationHardware, Manifest.Permission.Internet, }, 1);

        }

        //ShowExtraOptionsOnDeliverManAwait
        [JavascriptInterface]
        [Export("ShowExtraOptionsOnDeliverManAwait")]
        public async void ShowExtraOptionsOnDeliverManAwait()
        {
            try
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
                    var Viewww = new UtilityJavascriptInterface_RestaurantDetailedView((Activity)StaticContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                //using (StreamReader sr = new StreamReader(assets.Open("ShoppingKart.html")))
                using (StreamReader sr = new StreamReader(assets.Open("DelivermanExtraData.html")))
                    {
                        content = sr.ReadToEnd();
                    // var cadea = GenerateRowsForCheckOutModal(FragmentRestaurantDetailedView.ListaDeProductos);
                    sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new FragmentMain.LocalWebViewClient());

                //<span class="woocommerce-Price-currencySymbol">£</span>90.00</span>
                //CompleCostOFGroceryy


            };

                ((Activity)StaticContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
            }
            catch(Exception)
            { }


        }

        //ShowExtraOptionsOnGroceryAwait
        [JavascriptInterface]
        [Export("ShowOptionInOrderCreated")]
        public async void ShowOptionInOrderCreated()
        {
            try
            {
                Action action = () =>
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                    alert.SetTitle("Opciones");
                    alert.SetMessage("Que deseas hacer");

                    alert.SetPositiveButton("Cerrar Dialogo", (senderAlert, args) =>
                    {
                        //  count--;
                        //  button.Text = string.Format("{0} clicks!", count);
                    });


                    alert.SetNegativeButton("Cancelar Orden", async (senderAlert, args) =>
                    {
                        try
                        {
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
                        

                            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CancelOrder?" +
                                "FaceIDOfBuyer=" + FaceID
                                + "&IdfBuy=" + FragmentSelectTypeOfPurchase.OrderIDIfActive.ToString()


                                ));
                            // HttpResponseMessage response;

                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            //  var  response =  client.GetAsync(uri).Result;
                            var t = Task.Run(() => GetResponseFromURI(uri));
                            //t.Wait();

                            var S_Response = t.Result;
                            if(S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                            {
                                MainActivity.LoadFragment_Static(Resource.Id.menu_video);
                            }
                         
                        }
                        catch (Exception)
                        { }
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();

                    //CurrentDialogReference = dialog;
                };
                ((Activity)mContext).RunOnUiThread(action);
            }
            catch (Exception)
            { }


        }


        //ShowExtraOptionsOnGroceryAwait
        [JavascriptInterface]
        [Export("ShowExtraOptionsOnGroceryAwait")]
        public async void ShowExtraOptionsOnGroceryAwait()
        {
            try
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
                    var Viewww = new UtilityJavascriptInterface_RestaurantDetailedView((Activity)StaticContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                //using (StreamReader sr = new StreamReader(assets.Open("ShoppingKart.html")))
                using (StreamReader sr = new StreamReader(assets.Open("PassengerExtraData.html")))
                    {
                        content = sr.ReadToEnd();
                    // var cadea = GenerateRowsForCheckOutModal(FragmentRestaurantDetailedView.ListaDeProductos);
                    sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new FragmentMain.LocalWebViewClient());

                //<span class="woocommerce-Price-currencySymbol">£</span>90.00</span>
                //CompleCostOFGroceryy


            };

                ((Activity)StaticContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
            }
            catch(Exception)
            { }


        }

        //SearchByText
        [JavascriptInterface]
        [Export("SearchByText")]
        public async void SearchByText(string texttosearch)
        {
           // QueryRestaurant()
            try
            {
                HttpClient client = new HttpClient();


                string FaceID = null;
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                long RegionTemp = 0;
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                try
                {

                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                    FaceID = query.ProfileId;
                    RegionTemp = Convert.ToInt64(query.Region_Delivery);
                }
                catch (Exception ex)
                {

                }

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByTextAndIndex?" +
                    "ServiceAreaText=" + RegionTemp
                    + "&TextMeantToesearchedIndexed=" + texttosearch.ToLower()


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                //var t = Task.Run(() => GetResponseFromURI(uri));
                //t.Wait();
                var aca = await GetResponseFromURI(uri);

                var S_Response = aca;//.Result;
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // var ListOfRestaurants = JsonConvert.DeserializeObject<List<restaurantDelta>>(S_Response.Response);
                    var F_Response = JsonConvert.DeserializeObject<List<restaurantDelta>>(S_Response.Response);
                    foreach (var Rest in F_Response)
                    {
                        await QueryRestaurant(Rest, webi);
                        // var t = Task.Run(() => new UpdateProductList(Rest.RestaurantIndex));

                        // new UpdateProductList(Rest.RestaurantIndex);
                    }
                  

                }
            }
            catch (Exception)
            { }
            /*
            try { 
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
          

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByText?" +
                "ServiceAreaText=" + 2
                + "&TextMeantToesearched=" + texttosearch.ToLower()


                ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
            //t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
            {

                Action action = () =>
                {
                    //var jsr = new JavascriptResult();
                    var script = "PrintTagResponse(" + S_Response.Response + ")";
                    webi.EvaluateJavascript(script, null);


                };


                webi.Post(action);
            }
        }
            catch(Exception)
            { }
            */

        }
        public class restaurantDelta
        {
            public long RestaurantIndex;
            public bool EstaAbierto;
            public string CarppiHash;
            public string UpdateTag;
            public double calificacion;
            public bool AnyDeliveryMan;
        }
        public class FirstResponseWithRegion
        {
            public long BitfieldForProducts;
            public bool AnyDeliveryMan;
            public List<restaurantDelta> RestaurantesDelta;
        }
        public async Task QueryRestaurant(restaurantDelta RestaurantIndex, WebView LocalWebView)
        {

            var databasePath11 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Restaurantes.db");
            var db11 = new SQLiteConnection(databasePath11);
            var RestaurantQuery = new Carppi_IndicesdeRestaurantes();
            try
            {
                RestaurantQuery = db11.Table<DatabaseTypes.Carppi_IndicesdeRestaurantes>().Where(v => v.ID == RestaurantIndex.RestaurantIndex).FirstOrDefault();
            }
            catch (Exception) { }
            if (RestaurantQuery == null)
            {
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
                // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/DownloadRestaurant?" +
                    "RestaurantToDownload=" + RestaurantIndex.CarppiHash


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                var S_Response = t.Result;
                //t.Wait();
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                {

                    //EraseContentGrid
                    var Respuesta = JsonConvert.DeserializeObject<Carppi_IndicesdeRestaurantes>(S_Response.Response);
                    if (Respuesta != null)
                    {
                        var AddingProduct = db11.Insert(new DatabaseTypes.Carppi_IndicesdeRestaurantes()
                        {
                            ID = Respuesta.ID,
                            CarppiHash = Respuesta.CarppiHash,
                            Foto = Respuesta.Foto,
                            Nombre = Respuesta.Nombre,
                            EstaAbierto = Respuesta.EstaAbierto,
                            Categoriasbitfield = Respuesta.Categoriasbitfield,
                            UpdateTag = RestaurantIndex.UpdateTag,
                            AnyDeliveryMan = RestaurantIndex.AnyDeliveryMan,
                            calificacion = RestaurantIndex.calificacion

                        });

                        Action action = () =>
                        {
                            //var jsr = new JavascriptResult();

                            /* button.ContextualID = StartUpData.PromotedProducts[i].ID;
                    //button.id = "Restaurant_"

                    button.RestaurantHash = StartUpData.PromotedProducts[i].CarppiHash;
                    //CarppiHash

                    button.ImagenDelNegocio = "data:image/png;base64," + StartUpData.PromotedProducts[i].Foto;
                    button.NombreDelREstauratnte = StartUpData.PromotedProducts[i].Nombre;
                    button.EstaAbierto = StartUpData.PromotedProducts[i].EstaAbierto;
                    //YCategoriabitfield*/

                            // var asas = new SevenZip.Compression.LZMA.Decoder();

                            //Respuesta.Foto = Decompress(Respuesta.Foto);
                            var script = "UpdateREstaurantGrid(" + S_Response.Response + ")";
                            LocalWebView.EvaluateJavascript(script, null);


                        };

                        //UpdateProductGrid
                        //SetStartupMenu
                        LocalWebView.Post(action);
                    }



                }
            }
            else
            {
                // var Respuesta = JsonConvert.DeserializeObject<Carppi_IndicesdeRestaurantes>(S_Response.Response);
                RestaurantQuery.EstaAbierto = RestaurantIndex.EstaAbierto;
                RestaurantQuery.AnyDeliveryMan = RestaurantIndex.AnyDeliveryMan;
                RestaurantQuery.calificacion = RestaurantIndex.calificacion;
                db11.RunInTransaction(() =>
                {
                    db11.Update(RestaurantQuery);
                });
                Action action = () =>
                {
                    //var jsr = new JavascriptResult();
                    // var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);

                    // var asas = new SevenZip.Compression.LZMA.Decoder();

                    // Productquery.Foto = Decompress(Productquery.Foto);
                    var script = "UpdateREstaurantGrid(" + JsonConvert.SerializeObject(RestaurantQuery) + ")";
                    LocalWebView.EvaluateJavascript(script, null);


                };


                LocalWebView.Post(action);
            }
        }

        //SearchBackAllRestaurants

        //SearchFoodByBox
        [JavascriptInterface]
        [Export("SearchBackAllRestaurants")]
        public async void SearchBackAllRestaurants()
        {
            try
            {
                HttpClient client = new HttpClient();


                string FaceID = null;
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                long RegionTemp = 0;
                try
                {

                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                    FaceID = query.ProfileId;
                    RegionTemp = Convert.ToInt64(query.Region_Delivery);
                }
                catch (Exception ex)
                {

                }

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDeterminationWithRealRegion?" +
                 "CadenadelUsuarioRestaurant=" + FaceID +
                 "&RealRegion=" + RegionTemp


                 ));

                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                //var t = Task.Run(() => GetResponseFromURI(uri));
                //t.Wait();
                var aca = await GetResponseFromURI(uri);

                var S_Response = aca;//.Result;
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // var ListOfRestaurants = JsonConvert.DeserializeObject<List<restaurantDelta>>(S_Response.Response);
                    var F_Response = JsonConvert.DeserializeObject<FirstResponseWithRegion>(S_Response.Response);
                    foreach (var Rest in F_Response.RestaurantesDelta)
                    {
                        await QueryRestaurant(Rest, webi);
                        var t = await Task.Run(() => new UpdateProductList(Rest.RestaurantIndex));
                        /*
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;

                            //  Console.WriteLine("Hello, world");
                            new UpdateProductList(Rest.RestaurantIndex);
                        }).Start();
                    */
                        // new UpdateProductList(Rest.RestaurantIndex);
                    }
                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "SetCategoriesStartup(" + F_Response.BitfieldForProducts + ")";
                        webi.EvaluateJavascript(script, null);


                    };


                    webi.Post(action);

                }
            }
            catch (Exception)
            { }
            /*
            try
            {
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
               
                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                  "CadenadelUsuarioRestaurant=" + FaceID


                  ));


                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                //var t = Task.Run(() => GetResponseFromURI(uri));
                //t.Wait();
                var aca = await GetResponseFromURI(uri);

                var S_Response = aca;//.Result;
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                {

                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "SetStartupMenu(" + S_Response.Response + ")";
                        webi.EvaluateJavascript(script, null);


                    };


                    webi.Post(action);
                }
            }
            catch (Exception)
            { }
            */
        }
        

        //SearchFoodByBox
        [JavascriptInterface]
        [Export("SearchFoodByBox")]
        public async void SearchFoodByBox(long SearchType)
        {
            try
            {
                HttpClient client = new HttpClient();


                string FaceID = null;
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                long RegionTemp = 0;
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                try
                {

                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                    FaceID = query.ProfileId;
                    RegionTemp = Convert.ToInt64(query.Region_Delivery);
                }
                catch (Exception ex)
                {

                }

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByTagIndex?" +
                 "ServiceArea=" + RegionTemp
                 + "&availableFoodListingIndex=" + SearchType


                 ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                //var t = Task.Run(() => GetResponseFromURI(uri));
                //t.Wait();
                var aca = await GetResponseFromURI(uri);

                var S_Response = aca;//.Result;
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // var ListOfRestaurants = JsonConvert.DeserializeObject<List<restaurantDelta>>(S_Response.Response);
                    var F_Response = JsonConvert.DeserializeObject<List<restaurantDelta>>(S_Response.Response);
                    foreach (var Rest in F_Response)
                    {
                        await QueryRestaurant(Rest, webi);
                        // var t = Task.Run(() => new UpdateProductList(Rest.RestaurantIndex));

                        // new UpdateProductList(Rest.RestaurantIndex);
                    }


                }
            }
            catch (Exception)
            { }
            /*
            try { 
            //PrintTagResponse
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
           

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByTag?" +
                "ServiceArea=" + 2
                + "&availableFoodListing=" + SearchType


                ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
            //t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
            {

                Action action = async () =>
                {
                    //var jsr = new JavascriptResult();
                    var script = "PrintTagResponse(" + S_Response.Response + ")";
                    webi.EvaluateJavascript(script, null);


                };


                webi.Post(action);
            }
        }
            catch(Exception)
            { }
           */
        }
        //CenterThemap(lat, lng)

        [JavascriptInterface]
        [Export("RestaurantDetailedView")]
        public async void RestaurantDetailedView(long ContextualID, string RestaurantHash, string Imagen, string NombreDelRestaurante, bool EstaAbierto)
        {
            try
            {
                Android.Support.V4.App.Fragment fragment = null;
                fragment = FragmentRestaurantDetailedView.NewInstance(ContextualID, RestaurantHash, Imagen, NombreDelRestaurante, EstaAbierto);
                MainActivity.static_FragmentMAnager.BeginTransaction()
                   .Replace(Resource.Id.content_frame, fragment)
                   .Commit();
            }
            catch(Exception)
            { }

            /*
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
            // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantDetailedView?" +
                "RestaurantDetailID=" + ContextualID


                ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
            t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
            {
                var Restaurante = JsonConvert.DeserializeObject<DetailedProductViewFromRestauran>(S_Response.Response);
                //---REstaurantName-----

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
                    var Viewww = new UtilityJavascriptInterface_RestaurantDetailedView((Activity)mContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("CarppiDeliveryRestaurantDetailedView.html")))
                    {
                        content = sr.ReadToEnd();
                        content = content.Replace("---REstaurantName-----", Restaurante.RestaurantData.Nombre);
                        //mall-dedicated-banner-replace----
                        content = content.Replace("mall-dedicated-banner-replace----", "data:image/png;base64," + Restaurante.RestaurantData.Foto);
                        //---OpenTag---
                        content = content.Replace("---OpenTag---", (Restaurante.RestaurantData.EstaAbierto == null || Restaurante.RestaurantData.EstaAbierto == false )? "Cerrado" : "Abierto");
                        content = content.Replace("---OpenTagColor---", (Restaurante.RestaurantData.EstaAbierto == null || Restaurante.RestaurantData.EstaAbierto == false) ? "danger" : "success");
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient_RestaurantDetailedView(sss, Restaurante));



                };

                ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                */

            /*
            Action action = () =>
            {

                var script = "SetStartupMenu(" + S_Response.Response + ")";
                webi.EvaluateJavascript(script, null);


            };


            webi.Post(action);

*/
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


    }

      


    }

   


