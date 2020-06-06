
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
//using static Carppi.Fragments.FragmentMain.WebInterfaceMenuCarppi;
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
using System.Text.RegularExpressions;

namespace Carppi.Fragments
{
    public class FragmentMenu : Fragment
    {
        // public static WebInterfaceMenuCarppi MainWebView;
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
            using (StreamReader sr = new StreamReader(assets.Open("RestaurantDetailedView.html")))
            {
                content = sr.ReadToEnd();
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new MenuWebView(this.Activity, webi);
                // MainWebView = wew;
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
                webi.SetWebViewClient(new MenuWebClient(view1, this.Activity));
                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView
                //WebInterfaceMenuCarppi.DriverStateDetermination();

                // DoWork_Driver();

            }

            return view1;



        }
        class MenuWebClient : WebViewClient
        {
            public View CurrentView;
            public Context P_Context;
            private static System.Timers.Timer aTimer;
            private static WebView s_Webview;
            public MenuWebClient(View v, Context c)
            {
                CurrentView = v;
                P_Context = c;
            }
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                s_Webview = view;

                try
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/RestaurantToDownloadItsOwnState?" +
                        "RestaurantHashDownloadOpenState=" + HAsh
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                    // t.Wait();
                    var S_Ressult = t.Result;
                    if(S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var Response = JsonConvert.DeserializeObject<bool?>(S_Ressult.Response);
                        Action action = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "UpdateOprionsButton(" + S_Ressult.Response + ")";
                            s_Webview.EvaluateJavascript(script, null);


                        };


                        s_Webview.Post(action);


                        RestaurantCatalogSequentialLoad();
                    }
                    
                   
                   
                }
                catch (Exception)
                { }



              
               // ((RestaurantCatalogLoad();
                //aTimer = new System.Timers.Timer(2000);
                // Hook up the Elapsed event for the timer. 
                //aTimer.Elapsed += OnTimedEvent;
                //aTimer.AutoReset = true;
                //aTimer.Enabled = true;
            }

            public void RestaurantCatalogSequentialLoad()
            {
                try
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/RestaurantToDownloadItsOwnMenuIDs?" +
                        "RestaurantHashDownloadMenu_IDList=" + HAsh
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                    // t.Wait();
                    var S_Ressult = t.Result;
                    var Response = JsonConvert.DeserializeObject<List<long>>(S_Ressult.Response);
                    foreach (var index in Response)
                    {
                        var t_k = Task.Run(() => QueryAllProducts(index));
                        t.Wait();

                    }
                    /*
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //var Myob = JsonConvert.DeserializeObject<List<CarppiRestaurant_BuyOrders>>(S_Ressult.Response);
                        Action action2 = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "UpdateProductGrid_Iterative(" + S_Ressult.Response + ")";
                            s_Webview.EvaluateJavascript(script, null);


                        };


                        s_Webview.Post(action2);
                    }
                    */
                }
                catch (Exception)
                { }

            }
            public void QueryAllProducts(long Index)
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
                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiProductDetailedView?" +
                    "ProductDetailID=" + Index


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();

                var S_Response = t.Result;

                Action action = () =>
                {
                    //var jsr = new JavascriptResult();
                    var script = "UpdateProductGrid(" + S_Response.Response + ")";
                    s_Webview.EvaluateJavascript(script, null);


                };


                s_Webview.Post(action);
            }

            public void RestaurantCatalogLoad()
            {
                try
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/RestaurantToDownloadItsOwnMenu?" +
                        "RestaurantHashDownloadMenu=" + HAsh
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                   // t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //var Myob = JsonConvert.DeserializeObject<List<CarppiRestaurant_BuyOrders>>(S_Ressult.Response);
                        Action action2 = () =>
                        {
                        //var jsr = new JavascriptResult();
                        var script = "UpdateProductGrid_Iterative(" + S_Ressult.Response + ")";
                            s_Webview.EvaluateJavascript(script, null);


                        };


                        s_Webview.Post(action2);
                    }
                }
                catch(Exception)
                { }

            }
            public bool isLoggedIn()
            {

                AccessToken accessToken = AccessToken.CurrentAccessToken;//AccessToken.getCurrentAccessToken();
                return accessToken != null;
                //return false;
            }
            private static async void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                //Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                //                  e.SignalTime);

                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                HttpClient client = new HttpClient();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/CarppiPendingRequest?" +
                    "RestaurantHash=" + HAsh
                    ));


                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    //var Myob = JsonConvert.DeserializeObject<List<CarppiRestaurant_BuyOrders>>(S_Ressult.Response);
                    Action action2 = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "UpdateListOfOrders(" + S_Ressult.Response + ")";
                        s_Webview.EvaluateJavascript(script, null);


                    };


                    s_Webview.Post(action2);
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


        }




        public static FragmentMenu NewInstance()
        {
            var frag1 = new FragmentMenu { Arguments = new Bundle() };
            return frag1;
        }




    }
    class MenuWebView : WebViewClient
    {
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
        public Context mContext;
        public static Context RequestBeingAttended_StaticContext;
        public WebView Resources;
        public MenuWebView(Context contexto, WebView res)
        {
            mContext = contexto;
            Resources = res;
            RequestBeingAttended_StaticContext = contexto;
        }
        [JavascriptInterface]
        [Export("ShowExtraOptionsOnOrderAwait")]
        public static async void ShowExtraOptionsOnOrderAwait(int OrderID)
        {
            try
            {
                Action action_WhowAlert = () =>
            {

                var sss = ((Activity)RequestBeingAttended_StaticContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                sss.Settings.JavaScriptEnabled = true;

                sss.Settings.DomStorageEnabled = true;
                sss.Settings.LoadWithOverviewMode = true;
                sss.Settings.UseWideViewPort = true;
                sss.Settings.BuiltInZoomControls = true;
                sss.Settings.DisplayZoomControls = false;
                sss.Settings.SetSupportZoom(true);
                sss.Settings.JavaScriptEnabled = true;
                /// sss.AddJavascriptInterface(webi, "Android");
                AssetManager assets = ((Activity)RequestBeingAttended_StaticContext).Assets;
                string content;
                var Viewww = new UtilityJavascriptInterfaceGrocery(RequestBeingAttended_StaticContext, sss);
                sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                using (StreamReader sr = new StreamReader(assets.Open("GroceryRequestExtra.html")))
                {
                    content = sr.ReadToEnd();

                    content = content.Replace("0000000000", OrderID.ToString());

                    sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                }
                //  sss.SetWebViewClient(new LocalWebViewClient());




            };

            ((Activity)RequestBeingAttended_StaticContext).RunOnUiThread(action_WhowAlert);
            
                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
            }
            catch(Exception ex)
            {


            }

            /* HttpClient client = new HttpClient();
             //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
             var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
             var db5 = new SQLiteConnection(databasePath5);
             var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

             var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetExtraDataFromTheBuyOrder?" +
                 "PendingOrder_ToExtractData=" + OrderID

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

                 var Dat = JsonConvert.DeserializeObject<GroceryExtraExtraDatacaCommentUtility>(errorMessage1);


                 Action action_WhowAlert = () =>
                 {

                     var sss = ((Activity)Static_mContext).FindViewById<WebView>(Resource.Id.webView_Bottomsheet);
                     sss.Settings.JavaScriptEnabled = true;

                     sss.Settings.DomStorageEnabled = true;
                     sss.Settings.LoadWithOverviewMode = true;
                     sss.Settings.UseWideViewPort = true;
                     sss.Settings.BuiltInZoomControls = true;
                     sss.Settings.DisplayZoomControls = false;
                     sss.Settings.SetSupportZoom(true);
                     sss.Settings.JavaScriptEnabled = true;
                     /// sss.AddJavascriptInterface(webi, "Android");
                     AssetManager assets = ((Activity)Static_mContext).Assets;
                     string content;
                     var Viewww = new UtilityJavascriptInterfaceGrocery(Static_mContext, sss);
                     sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                     using (StreamReader sr = new StreamReader(assets.Open("GroceryRequestExtra.html")))
                     {
                         content = sr.ReadToEnd();

                         content = content.Replace("SustituteTripID", OrderID.ToString());
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
                     //  sss.SetWebViewClient(new LocalWebViewClient());




                 };

                 ((Activity)Static_mContext).RunOnUiThread(action_WhowAlert);

                 MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;

             }
             */
        }

        public enum ItemAvailability { SetUnavailable, SetAvailable };
        [JavascriptInterface]
        [Export("DisplayOptionsOfProduct")]
        public async void DisplayOptionsOfProduct(long IdOfMyProduct)
        {


            Action action = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                alert.SetTitle("Opciones");
                alert.SetMessage("Sellecciona el estado de tu Producto, asi este no se mostrara al cliente");



                alert.SetPositiveButton("Producto Disponible", (senderAlert, args) =>
                {
                    
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/RestaurantToSetItsItemAvailable?" +
                        "RestaurantHashDownloadMenu=" + HAsh+
                        "&itemID=" + IdOfMyProduct +
                        "&Available=" + (int)ItemAvailability.SetAvailable
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                   // t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        /*SetTagState(NumberID, State)*/
                        //MainActivity.LoadFragment_Static(Resource.Id.menu_Menu);
                        //var Myob = JsonConvert.DeserializeObject<List<CarppiRestaurant_BuyOrders>>(S_Ressult.Response);

                        Action action2 = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "SetTagState(" + IdOfMyProduct + "," +"1" +  ")";
                            Resources.EvaluateJavascript(script, null);


                        };


                        Resources.Post(action2);

                    }

                });
                alert.SetNegativeButton("Producto No Disponible", (senderAlert, args) =>
                {
                    
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/RestaurantToSetItsItemAvailable?" +
                       "RestaurantHashDownloadMenu=" + HAsh +
                       "&itemID=" + IdOfMyProduct +
                       "&Available=" + (int)ItemAvailability.SetUnavailable
                       ));



                    var t = Task.Run(() => GetResponseFromURI(uri));
                 //   t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {/*SetTagState(NumberID, State)*/
                        // MainActivity.LoadFragment_Static(Resource.Id.menu_Menu);

                        Action action2 = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "SetTagState(" + IdOfMyProduct + "," + "0" + ")";
                            Resources.EvaluateJavascript(script, null);


                        };


                        Resources.Post(action2);
                    }
                 
                });
                alert.SetNeutralButton("Cerrar dialogo", (senderAlert, args) =>
                {
                    //  count--;
                    //  button.Text = string.Format("{0} clicks!", count);
                });

                Dialog dialog = alert.Create();
                dialog.Show();

                //CurrentDialogReference = dialog;
            };
            ((Activity)mContext).RunOnUiThread(action);

        }

        [JavascriptInterface]
        [Export("RestaurantOpenOptions")]
        public async void RestaurantOpenOptions()
        {


            Action action = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                alert.SetTitle("Opciones");
                alert.SetMessage("Sellecciona el estado de tu comercio");



                alert.SetPositiveButton("Comercio Abierto", (senderAlert, args) =>
                {
                    
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/RestaurantToSetItsAvailability?" +
                        "RestaurantHashToopen=" + HAsh +
                        "&Available=" + (int)ItemAvailability.SetAvailable
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                 //   t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                    {

                        var Response = JsonConvert.DeserializeObject<bool?>(S_Ressult.Response);
                        Action action = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "UpdateOprionsButton(" + "1"+ ")";
                            Resources.EvaluateJavascript(script, null);


                        };


                        Resources.Post(action);
                        //var Myob = JsonConvert.DeserializeObject<List<CarppiRestaurant_BuyOrders>>(S_Ressult.Response);

                    }
                    
                });
                alert.SetNegativeButton("Comercio Cerrado", (senderAlert, args) =>
                {
                    
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/RestaurantToSetItsAvailability?" +
                        "RestaurantHashToopen=" + HAsh +
                        "&Available=" + (int)ItemAvailability.SetUnavailable
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                  //  t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Action action = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "UpdateOprionsButton(" + "0" + ")";
                            Resources.EvaluateJavascript(script, null);


                        };


                        Resources.Post(action);

                    }
                 
                });
                alert.SetNeutralButton("Cerrar dialogo", (senderAlert, args) =>
                {
                    //  count--;
                    //  button.Text = string.Format("{0} clicks!", count);
                });

                Dialog dialog = alert.Create();
                dialog.Show();

                //CurrentDialogReference = dialog;
            };
            ((Activity)mContext).RunOnUiThread(action);

        }
        /*
        [JavascriptInterface]
        [Export("AcceptedOrderOptions")]
        public async void AcceptedOrderOptions(long ID)
        {


            Action action = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                alert.SetTitle("Opciones");
                alert.SetMessage("Que desesas hacer?");



                alert.SetPositiveButton("Aceptar Orden", (senderAlert, args) =>
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/SetOrderStatus?" +
                        "Estado=" + (int)GroceryOrderState.RequestAccepted +
                        "&OrderID=" + ID
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        //var Myob = JsonConvert.DeserializeObject<List<CarppiRestaurant_BuyOrders>>(S_Ressult.Response);

                    }
                    //  count--;
                    //  button.Text = string.Format("{0} clicks!", count);
                });
                alert.SetNegativeButton("Rechazar Orden", (senderAlert, args) =>
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/SetOrderStatus?" +
                        "Estado=" + (int)GroceryOrderState.RequestRejected +
                        "&OrderID=" + ID
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        //var Myob = JsonConvert.DeserializeObject<List<CarppiRestaurant_BuyOrders>>(S_Ressult.Response);

                    }
                    //  count--;
                    //  button.Text = string.Format("{0} clicks!", count);
                });
                alert.SetNeutralButton("Cerrar", (senderAlert, args) =>
                {
                    //  count--;
                    //  button.Text = string.Format("{0} clicks!", count);
                });

                Dialog dialog = alert.Create();
                dialog.Show();

                //CurrentDialogReference = dialog;
            };
            ((Activity)mContext).RunOnUiThread(action);

        }

        */
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


    





    public static FragmentMain NewInstance()
    {
        var frag1 = new FragmentMain { Arguments = new Bundle() };
        return frag1;
    }


}
    /*
public partial class CarppiRestaurant_BuyOrders
    {
        public long ID { get; set; }
        public long? RegionID { get; set; }
        public string UserID { get; set; }
        public string paymentIntent { get; set; }
        public double? LatitudPeticion { get; set; }
        public double? LongitudPeticion { get; set; }
        public double? LatitudRestaurante { get; set; }
        public double? LongitudRestaurante { get; set; }
        public double? LatitudRepartidor { get; set; }
        public double? LongitudRepartidor { get; set; }
        public double? Precio { get; set; }
        public int? Stat { get; set; }
        public string RestaurantHash { get; set; }
        public string NombreDelRestaurante { get; set; }
        public string NombreDelUsuario { get; set; }
        public string ListaDeProductos { get; set; }
        public string FaceIDRepartidor_RepartidorCadena { get; set; }
        public int? TipoDePago { get; set; }
        public bool RestaurantToDisplayOrders { get; set; }
    }
    */
}
