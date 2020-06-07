
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using Newtonsoft.Json;
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
            var t = Task.Run(() => GetResponseFromURI(uri));
            t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
            {



                // act = this.Activity;
                //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))

                var Template = "CarppiDeliveryDashBoard.html";
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
                    WebInterfaceProfile.RetriveProfile();


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
                using (StreamReader sr = new StreamReader(assets.Open("FragmentGrocery_Map.html")))
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
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                try
                {
                    LoadAvailableProductsStartUp();
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
                catch(Exception)
                {

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
            public async void LoadAvailableProductsStartUp()
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
            // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
            /*
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                "CadenadelUsuarioRestaurant=" + FaceID


                ));
                 var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByTextTest?" +
                "ServiceAreaText_Test=" + 2
                + "&TextMeantToesearched_Test=" + texttosearch.ToLower()


                ));
            */

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
           
        }

        //SearchFoodByBox
        [JavascriptInterface]
        [Export("SearchFoodByBox")]
        public async void SearchFoodByBox(long SearchType)
        {
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
            // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
            /*
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                "CadenadelUsuarioRestaurant=" + FaceID


                ));
                  var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByTagTest?" +
                "ServiceAreaTest=" + 2
                +"&availableFoodListingTest=" + SearchType


                ));
            */

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
            /*
            var AvailableFoodListing = {
            Hamburgesas: 1,
            Guajolotes: 2,
            Postres: 4,
            Pollo: 8,
            Indio: 16,
            Americano: 32,
            Pizza: 64,
            Saludable: 128,
            Vegetariano: 256,
            Chino: 512,
            Continental: 1024,
            Pastes: 2048
        };
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

   


