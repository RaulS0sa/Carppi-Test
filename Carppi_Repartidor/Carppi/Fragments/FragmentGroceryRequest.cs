
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
            var Template = "FragmentGrocery_Map.html";
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
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };

        public WebInterfaceGroceryRequest(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            webi_static = web;
            StaticContext = Act;
        }
        //ShowPopUpStateToDeliveryboy
        public enum WorkState { StopWork, StartWork };
        [JavascriptInterface]
        [Export("ShowPopUpStateToDeliveryboy")]
        public async void ShowPopUpStateToDeliveryboy()
        {
            //RequestID_ToFinish
            Action action = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                alert.SetTitle("Opciones");
                alert.SetMessage("Que deseas hacer?");



                alert.SetPositiveButton("Empezar Jornada", async (senderAlert, args) =>
                {
                    try
                    {
                        /*
                         *
                         *   public enum WorkState { StopWork, StartWork };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ChangeDeliverymanJourney(string FaceIDHash_Deliveryman, WorkState workState )

                         * */


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
                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRepartidorApi/ChangeDeliverymanJourney?" +
                            "FaceIDHash_Deliveryman=" + FaceID +
                            "&workState=" + (int)WorkState.StartWork

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
                     

                    }

                    catch (Exception Ex)
                    {

                    }

                });
                alert.SetNeutralButton("Terminar jornada", (senderAlert, args) =>
                {
                    try
                    {
                        /*
                         *
                         *   public enum WorkState { StopWork, StartWork };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ChangeDeliverymanJourney(string FaceIDHash_Deliveryman, WorkState workState )

                         * */


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
                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRepartidorApi/ChangeDeliverymanJourney?" +
                            "FaceIDHash_Deliveryman=" + FaceID +
                            "&workState=" + WorkState.StopWork

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


                    }

                    catch (Exception Ex)
                    {

                    }
                });

                alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                {
                
                });
               

                Dialog dialog = alert.Create();
                dialog.Show();
            };
            ((Activity)mContext).RunOnUiThread(action);

        }
        public static async void PendingGroceryRequest(WebView view)
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
                    "FaceIDHash_DeliveryBoy=" + FaceID


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
                    var ListaCoordenadas = new List<CoordenadasDeOrdenes>();
                    foreach (var Order in OrderList)
                    {
                        if (Order.Orden.Stat == Fragments.GroceryOrderState.RequestCreated)
                        {
                            Action action_WhowAlert = async () =>
                            {
                                /*
                                 *
                                 *
                                 
            document.getElementById('UserName_NewTripProposal').innerHTML ="Usuario: " + Data.NameOfRequester;
            document.getElementById('UserCost_NewTripProposal').innerHTML ="Costo: $" +  Data.Cost;
            document.getElementById('UserDistance_NewTripProposal').innerHTML = "Distancia: " + Data.Distance;
                                 */
                                var MyLatLong = await Clases.Location.GetCurrentPosition();
                                var newDarta = new NewOrderData();
                                double sumup = 0.0;
                                Order.Productos.ForEach(x => sumup += Convert.ToDouble(x.Costo));
                                newDarta.IdOfRequest = Order.Orden.ID.ToString();
                                newDarta.Cost = (sumup + 30).ToString();
                                newDarta.NameOfRequester = Order.Orden.NombreDelUsuario;
                                newDarta.tipodePago = Order.Orden.TipoDePago == enumTipoDePago.Efectivo ? "Efectivo" : "Tarjeta";
                                var placemarks = await Geocoding.GetPlacemarksAsync(Convert.ToDouble(Order.Orden.Latitud), Convert.ToDouble(Order.Orden.Longitud));
                                var placemark = placemarks?.FirstOrDefault();
                                newDarta.Direccion = "Colonia: " + placemark.SubLocality + "| Calle: " + placemark.Thoroughfare + "| Numero: " + placemark.SubThoroughfare;

                                var distance = Math.Sqrt(Math.Pow(Convert.ToDouble(Order.Orden.Latitud) - MyLatLong.Latitude, 2) + Math.Pow(Convert.ToDouble(Order.Orden.Longitud) - MyLatLong.Longitude, 2));

                                newDarta.Distance = (distance * (1 / 0.0090909)).ToString();
                                var script = "ShowRequestOptions(" + JsonConvert.SerializeObject(newDarta) + ");";
                                Action action = () =>
                                {
                                    view.EvaluateJavascript(script, null);

                                };
                                view.Post(action);



                            };

                            ((Activity)StaticContext).RunOnUiThread(action_WhowAlert);
                            break;
                        }
                        else
                        {
                            var Coordenada = new CoordenadasDeOrdenes();
                            Coordenada.Latitud = Convert.ToDouble(Order.Orden.Latitud);
                            Coordenada.Longitud = Convert.ToDouble(Order.Orden.Longitud);
                            Coordenada.ID = Order.Orden.ID;
                            ListaCoordenadas.Add(Coordenada);

                        }
                        /*
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

                        */

                    }
                    Action SetMarkerAcction = async () =>
                    {
                        var MyLatLong = await Clases.Location.GetCurrentPosition();
                        var script = "SetAllPlaceMarkers(" + JsonConvert.SerializeObject(ListaCoordenadas) + "," + JsonConvert.SerializeObject(MyLatLong) + ");";
                        Action action = () =>
                        {
                            view.EvaluateJavascript(script, null);

                        };
                        view.Post(action);



                    };

                    ((Activity)StaticContext).RunOnUiThread(SetMarkerAcction);



                    /*

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
                    */

                }
                else
                {

                }


            }

            catch (Exception Ex)
            {

            }
        }
        [JavascriptInterface]
        [Export("RejectBuyOrder")]
        public async void RejectBuyOrder(Int32 RequestID)
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
                "FaceIDHash_DeliveryBoy=" + FaceID +
                "&Estado=" + GroceryOrderState.RequestRejected +
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
                PendingGroceryRequest(webi);
            }
            else
            {

            }
        }

        [JavascriptInterface]
        [Export("AcceptBuyOrder")]
        public async void AcceptBuyOrder(Int32 RequestID)
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
                "FaceIDHash_DeliveryBoy=" + FaceID +
                "&Estado=" + GroceryOrderState.RequestAccepted +
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
                PendingGroceryRequest(webi);
            }
            else
            {

            }
        }
        //OrderExtraData
        [JavascriptInterface]
        [Export("OrderExtraData")]
        public async void OrderExtraData(Int32 RequestID)
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

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRepartidorApi/GetOrderProductList?" +
                    "FaceIDHash_DeliveryBoy=" + FaceID + 
                    "&OrderID=" + RequestID


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                //DeliveryContext
                var ContextualData = JsonConvert.DeserializeObject<DeliveryContext>(S_Ressult.Response);
                var OrderList = ContextualData.deliveryOrderQuery;//JsonConvert.DeserializeObject<List<DeliveryOrderQuery>>(S_Ressult.Response);
                    var Order = OrderList.FirstOrDefault();
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
                    var Viewww = new GroceryUtilityResponseJavascriptInterface(mContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("PassengerExtraData.html")))
                    {
                        content = sr.ReadToEnd();
                        content = content.Replace("SustituteTripID", RequestID.ToString());
                        //<h4>ListaDeMercancias</h4>
                        var DecodedJsonArray = Base64Decode(Order.Orden.ListaDeProductos);

                        var ListaDeserializada = JsonConvert.DeserializeObject<List<ShopItem>>(DecodedJsonArray);

                        List<ProducListOptions> ListaNombres_Productos = new List<ProducListOptions>();

                        var CadenadeMercancias = "";
                        var CostoParcial = 0.0;
                        foreach (var producto in ListaDeserializada)
                        {
                            if (producto.Quantity != 0)
                            {
                                ProducListOptions produc = new ProducListOptions();
                                produc.Cantidat = producto.Quantity;
                                produc.ProductName = Order.Productos.Where(x => x.ID == producto.ItemID).FirstOrDefault().Producto;
                                ListaNombres_Productos.Add(produc);
                                CadenadeMercancias += "<h5>"+ produc.ProductName  + ": " + produc.Cantidat  +"</h5>";
                                

                            }
                        }
                        Order.ListaNombres_Productos = ListaNombres_Productos;

                        content = content.Replace("<h4>ListaDeMercancias</h4>", CadenadeMercancias);
                        content = content.Replace("<h4>CostoDeLaOrden</h4>","Costo: " +  (ContextualData.CostoTotal + 30).ToString());
                        content = content.Replace("<h4>NombreDelCliente</h4>","Cliente: " +  ContextualData.cliente);
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());



                };

                ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;




            }

            catch (Exception Ex)
            {

            }
            // Console.WriteLine(RequestID);
          
        }
        class DeliveryContext
        {
            public List<DeliveryOrderQuery> deliveryOrderQuery;
            public double CostoTotal;
            public string cliente;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
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
                            "FaceIDHash_DeliveryBoy=" + FaceID + 
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
                            "FaceIDHash_DeliveryBoy=" + FaceID +
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
                            "FaceIDHash_DeliveryBoy=" + FaceID +
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
        private static System.Timers.Timer aTimer =  null;
        private static WebView sView;
        private static Context sContext;
        public GroceryRequestWebClient(Context contexto)
        {
            mContext = contexto;
            sContext = contexto;
        }

        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded };
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            sView = view;

            if (aTimer == null)
            {
                aTimer = new System.Timers.Timer(10000);

                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            }

            // UpdateRegion();
            //  UpdateLocation();
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            UpdateLocation();
            PendingGroceryRequest(sView);
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

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRepartidorApi/ActualizaLocacion?" +
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



        public static async void PendingGroceryRequest(WebView view)
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
                    "FaceIDHash_DeliveryBoy=" + FaceID


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
                    var ListaCoordenadas = new List<CoordenadasDeOrdenes>();
                    foreach (var Order in OrderList)
                    {
                        if(Order.Orden.Stat == Fragments.GroceryOrderState.RequestCreated)
                        {
                            Action action_WhowAlert = async () =>
                            {
                                /*
                                 *
                                 *
                                 
            document.getElementById('UserName_NewTripProposal').innerHTML ="Usuario: " + Data.NameOfRequester;
            document.getElementById('UserCost_NewTripProposal').innerHTML ="Costo: $" +  Data.Cost;
            document.getElementById('UserDistance_NewTripProposal').innerHTML = "Distancia: " + Data.Distance;
                                 */
                                var MyLatLong = await Clases.Location.GetCurrentPosition();
                                var newDarta = new NewOrderData();
                                double sumup = 0.0;
                                Order.Productos.ForEach(x => sumup+= Convert.ToDouble(x.Costo));
                                newDarta.IdOfRequest = Order.Orden.ID.ToString();
                                newDarta.Cost = (sumup + 30).ToString();
                                newDarta.NameOfRequester = Order.Orden.NombreDelUsuario;
                                newDarta.tipodePago = Order.Orden.TipoDePago == enumTipoDePago.Efectivo ? "Efectivo" : "Tarjeta";

                                var placemarks = await Geocoding.GetPlacemarksAsync(Convert.ToDouble(Order.Orden.Latitud), Convert.ToDouble(Order.Orden.Longitud));
                                var placemark = placemarks?.FirstOrDefault();
                                newDarta.Direccion = "Colonia: " + placemark.SubLocality + "| Calle: " + placemark.Thoroughfare + "| Numero: " + placemark.SubThoroughfare;

                                var distance = Math.Sqrt(Math.Pow(Convert.ToDouble(Order.Orden.Latitud )- MyLatLong.Latitude, 2) + Math.Pow(Convert.ToDouble(Order.Orden.Longitud )- MyLatLong.Longitude, 2));

                                newDarta.Distance = (distance * (1/0.0090909)).ToString();
                                var script = "ShowRequestOptions(" + JsonConvert.SerializeObject(newDarta) + ");";
                                Action action = () =>
                                {
                                    view.EvaluateJavascript(script, null);

                                };
                                view.Post(action);



                            };

                            ((Activity)sContext).RunOnUiThread(action_WhowAlert);
                            break;
                        }
                        else
                        {
                            var Coordenada = new CoordenadasDeOrdenes();
                            Coordenada.Latitud = Convert.ToDouble(Order.Orden.Latitud);
                            Coordenada.Longitud = Convert.ToDouble(Order.Orden.Longitud);
                            Coordenada.ID = Order.Orden.ID;
                            ListaCoordenadas.Add(Coordenada);

                        }
                        /*
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

                        */

                    }
                    Action SetMarkerAcction = async () =>
                    {
                        var MyLatLong = await Clases.Location.GetCurrentPosition();
                        var script = "SetAllPlaceMarkers(" + JsonConvert.SerializeObject(ListaCoordenadas) +"," + JsonConvert.SerializeObject(MyLatLong) +  ");";
                        Action action = () =>
                        {
                            view.EvaluateJavascript(script, null);

                        };
                        view.Post(action);



                    };

                    ((Activity)sContext).RunOnUiThread(SetMarkerAcction);



                    /*

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
                    */

                }
                else
                {
                    
                }
              

            }

            catch (Exception Ex)
            {

            }
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
    public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded };
    public enum enumTipoDePago { Efectivo, Tarjeta};
    public class CarppiGrocery_BuyOrders_AddType
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
        public List<ShopItem> ListaDeItems { get; set; }

        public string Direccion { get; set; }
        public string NombreDelUsuario { get; set; }
        public enumTipoDePago TipoDePago { get; set; }


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
    class CoordenadasDeOrdenes
    {
        public double Latitud;
        public double Longitud;
        public long ID;
    }
    class NewOrderData
    {
        public string NameOfRequester;
        public string Cost;
        public string Distance;
        public string IdOfRequest;
        public string Direccion;
        public string tipodePago;


    }
}
