
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
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;
using Fragment = Android.Support.V4.App.Fragment;

namespace Carppi.Fragments
{
    public class FragmentGroceryRequest : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static FragmentGroceryRequest NewInstance()
        {
            var frag1 = new FragmentGroceryRequest { Arguments = new Bundle() };
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var scale = Resources.DisplayMetrics.Density;//density i.e., pixels per inch or cms

            var widthPixels = Resources.DisplayMetrics.WidthPixels;////getting the height in pixels  
            var width = (double)((widthPixels - 0.5f) / scale);//width in units //411 //359
            var heightPixels = Resources.DisplayMetrics.HeightPixels;////getting the height in pixels  
            var height = (double)((heightPixels - 0.5f) / scale);//731 //6


            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);

            string content;
            AssetManager assets = this.Activity.Assets;
            // act = this.Activity;
            //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))
            var Template = "GroceryRequestList.html";
            //var Template = "ShoppingOptions.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            //Template = "ShoppingCatalog.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            using (StreamReader sr = new StreamReader(assets.Open(Template)))
            {
                content = sr.ReadToEnd();
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new WebInterfaceGroceryRequest(this.Activity, webi);
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

                
               webi.SetWebViewClient(new GroceryRequestWebClient(this.Activity));
                // WebInterfaceProfile.RetriveProfile();

                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView

            }

            return view1;
            // return base.OnCreateView(inflater, container, savedInstanceState);
        }

    }
    public class WebInterfaceGroceryRequest : Java.Lang.Object
    {
        Context mContext;
        WebView webi;
        public static WebView webi_static;
        public static Context StaticContext;
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded };

        public WebInterfaceGroceryRequest(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            webi_static = web;
            StaticContext = Act;
        }

        //OptiosDuringTripPickUp
        [JavascriptInterface]
        [Export("DisplayOptionsToGroceryBuyOrder")]
        public async void DisplayOptionsToGroceryBuyOrder(Int32 RequestID)
        {
            //RequestID_ToFinish
            Action action = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                alert.SetTitle("Opciones");
                alert.SetMessage("Que deseas hacer?");



                alert.SetPositiveButton("Aceptar Solicitud", async (senderAlert, args) =>
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
                        //
                        //public HttpResponseMessage SetOrderStatus(string FaceIDHash_DeliveryBoy, GroceryOrderState Estado, Int32 OrderID)
                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/SetOrderStatus?" +
                            "FaceIDHash_DeliveryBoy=" + "849994702134646" + 
                            "&Estado=" + GroceryOrderState.RequestAccepted+
                            "&OrderID=" + RequestID


                            ));
                        // HttpResponseMessage response;

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //  var  response =  client.GetAsync(uri).Result;
                        var t = Task.Run(() => GetResponseFromURI(uri));
                        t.Wait();
                        var S_Ressult = t.Result;
                        if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                        {

                        }
                        else
                        {

                        }
                        /*
                        var script = "UpdateProductGrid(" + t.Result.Response + ");";
                        Action action = () =>
                        {
                            view.EvaluateJavascript(script, null);

                        };
                        view.Post(action);
                        */
                        // var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                        //UpdateProductGrid(Data)

                    }

                    catch (Exception Ex)
                    {

                    }

                });

                alert.SetNegativeButton("Ir al Cliente", (senderAlert, args) =>
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
                        //
                        //public HttpResponseMessage SetOrderStatus(string FaceIDHash_DeliveryBoy, GroceryOrderState Estado, Int32 OrderID)
                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/SetOrderStatus?" +
                            "FaceIDHash_DeliveryBoy=" + "849994702134646" +
                            "&Estado=" + GroceryOrderState.RequestGoingToClient +
                            "&OrderID=" + RequestID


                            ));
                        // HttpResponseMessage response;

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //  var  response =  client.GetAsync(uri).Result;
                        var t = Task.Run(() => GetResponseFromURI(uri));
                        t.Wait();
                        var S_Ressult = t.Result;
                        if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                        {

                        }
                        else
                        {

                        }
                        /*
                        var script = "UpdateProductGrid(" + t.Result.Response + ");";
                        Action action = () =>
                        {
                            view.EvaluateJavascript(script, null);

                        };
                        view.Post(action);
                        */
                        // var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                        //UpdateProductGrid(Data)

                    }

                    catch (Exception Ex)
                    {

                    }
                });
                alert.SetNeutralButton("finalizar Orden", (senderAlert, args) =>
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
                        //
                        //public HttpResponseMessage SetOrderStatus(string FaceIDHash_DeliveryBoy, GroceryOrderState Estado, Int32 OrderID)
                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/SetOrderStatus?" +
                            "FaceIDHash_DeliveryBoy=" + "849994702134646" +
                            "&Estado=" + GroceryOrderState.RequestEnded +
                            "&OrderID=" + RequestID


                            ));
                        // HttpResponseMessage response;

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //  var  response =  client.GetAsync(uri).Result;
                        var t = Task.Run(() => GetResponseFromURI(uri));
                        t.Wait();
                        var S_Ressult = t.Result;
                        if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                        {

                        }
                        else
                        {

                        }
                        /*
                        var script = "UpdateProductGrid(" + t.Result.Response + ");";
                        Action action = () =>
                        {
                            view.EvaluateJavascript(script, null);

                        };
                        view.Post(action);
                        */
                        // var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                        //UpdateProductGrid(Data)

                    }

                    catch (Exception Ex)
                    {

                    }
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            };
            ((Activity)mContext).RunOnUiThread(action);

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




    class GroceryRequestWebClient : WebViewClient
    {
        public Context mContext;
        private static System.Timers.Timer aTimer;
        public GroceryRequestWebClient(Context contexto)
        {
            mContext = contexto;
        }

        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded };
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            PendingGroceryRequest(view);
            aTimer = new System.Timers.Timer(12000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            // UpdateRegion();
            //  UpdateLocation();
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            UpdateLocation();
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
            catch (Exception)
            {

            }

        }



        public async void PendingGroceryRequest(WebView view)
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

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetListOfDeliveryBoyOrder?" +
                    "FaceIDHash_DeliveryBoy=" + "849994702134646"


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                {

                    var OrderList = JsonConvert.DeserializeObject<List<DeliveryOrderQuery>>(S_Ressult.Response);
                    foreach(var Order in OrderList)
                    {
                        var placemarks = await Geocoding.GetPlacemarksAsync(Convert.ToDouble(Order.Orden.Latitud), Convert.ToDouble(Order.Orden.Longitud));

                        var placemark = placemarks?.FirstOrDefault();

                        var DecodedJsonArray = Base64Decode(Order.Orden.ListaDeProductos);

                        var ListaDeserializada = JsonConvert.DeserializeObject<List<ShopItem>>(DecodedJsonArray);

                        List<ProducListOptions> ListaNombres_Productos = new List<ProducListOptions>();
                        foreach(var producto in ListaDeserializada)
                        {
                            if(producto.Quantity != 0)
                            {
                                ProducListOptions produc = new ProducListOptions();
                                produc.Cantidat = producto.Quantity;
                                produc.ProductName = Order.Productos.Where(x => x.ID == producto.ItemID).FirstOrDefault().Producto;
                                ListaNombres_Productos.Add(produc);
                            }
                        }
                        Order.ListaNombres_Productos = ListaNombres_Productos;


                       Order.Orden.Direccion ="Colonia: " +  placemark.SubLocality + "| Calle: " + placemark.Thoroughfare + "| Numero: " + placemark.SubThoroughfare;

                    }
                  

                    // var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Ressult.Response);
                    //Map
                    //An Order Is Pending

                    Action action_WhowAlert = () =>
                    {
                        var script = "UpdateRequestList(" + JsonConvert.SerializeObject(OrderList) +");";
                        Action action = () =>
                        {
                            view.EvaluateJavascript(script, null);

                        };
                        view.Post(action);



                    };

                    ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                }
                else
                {
                    
                }
                /*
                var script = "UpdateProductGrid(" + t.Result.Response + ");";
                Action action = () =>
                {
                    view.EvaluateJavascript(script, null);

                };
                view.Post(action);
                */
                // var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                //UpdateProductGrid(Data)

            }

            catch (Exception Ex)
            {

            }
        }

        class DeliveryOrderQuery
        {
            public CarppiGrocery_BuyOrders_AddType Orden;
            public List<CarppiGrocery_Productos> Productos;
            public List<ProducListOptions> ListaNombres_Productos;


        }
        class ProducListOptions
        {
            public string ProductName;
            public int Cantidat;
        }


        public partial class CarppiGrocery_Productos
        {
            public long ID { get; set; }
            public long? RegionID { get; set; }
            public string Producto { get; set; }
            public double? Costo { get; set; }
            public byte[] Foto { get; set; }
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public class CarppiGrocery_BuyOrders
        {
            public long ID { get; set; }
            public long? RegionID { get; set; }
            public string UserID { get; set; }
            public string paymentIntent { get; set; }
            public double? Latitud { get; set; }
            public double? Longitud { get; set; }
            public GroceryOrderState Stat { get; set; }
            public string ListaDeProductos { get; set; }
            public double? Latitud_Repartidor { get; set; }
            public double? Longitud_Repartidor { get; set; }
            public double? FaceIDRepartidor_Repartidor { get; set; }
            public string FaceIDRepartidor_RepartidorCadena { get; set; }
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

    public class ShopItem
    {
        public int ItemID;
        public int Quantity;
    }
    //public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded };
    public class CarppiGrocery_BuyOrders_AddType
    {
        public long ID { get; set; }
        public long? RegionID { get; set; }
        public string UserID { get; set; }
        public string paymentIntent { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public FragmentRestaurantDetailedView.GroceryOrderState Stat { get; set; }
        public string ListaDeProductos { get; set; }
        public double? Latitud_Repartidor { get; set; }
        public double? Longitud_Repartidor { get; set; }
        public double? FaceIDRepartidor_Repartidor { get; set; }
        public string FaceIDRepartidor_RepartidorCadena { get; set; }
        public List<ShopItem> ListaDeItems { get; set; }

        public string Direccion { get; set; }


    }
}
