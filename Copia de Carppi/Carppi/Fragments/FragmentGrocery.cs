
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
using Carppi.DatabaseTypes;
using Java.Interop;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using static Carppi.Fragments.FragmentMain;
using static Carppi.Fragments.WebInterfaceFragmentGrocery;
using Fragment = Android.Support.V4.App.Fragment;
namespace Carppi.Fragments
{
    public class FragmentGrocery : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);

            string content;
            AssetManager assets = this.Activity.Assets;
            // act = this.Activity;
            //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))
            var Template = "EmptyPage.html";
            //var Template = "ShoppingOptions.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            //Template = "ShoppingCatalog.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            using (StreamReader sr = new StreamReader(assets.Open(Template)))
            {
                content = sr.ReadToEnd();
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new WebInterfaceFragmentGrocery(this.Activity, webi);
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
                webi.SetWebViewClient(new GroceryWebClient(this.Activity, Resources));
               // WebInterfaceProfile.RetriveProfile();

                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView

            }

            return view1;
            // return base.OnCreateView(inflater, container, savedInstanceState);
        }
        public override void OnStop()
        {
            try
            {
                
                aTimer.Enabled = false;
                aTimer.Stop();

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
            catch(Exception)
            { }
            base.OnPause();
        }
        public override void OnResume()
        {
            try
            {
                aTimer.Enabled = true;
              
            }
            catch(Exception)
            { }
            base.OnResume();
        }
        public static System.Timers.Timer aTimer = null;
        public static async void DoWork_Driver()
        {
            if (aTimer == null)
            {

                aTimer = new System.Timers.Timer(7000);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            }
            // Fragments.Fragment1.Vista.UpdateImage1(result);
            //window.Android_driver.RequestDriverState(deviceSessionId);
            /*
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(13);

            var timer = new System.Threading.Timer((e) =>
            {
                try
                {
                    WebInterfaceFragmentGrocery.UpdateGroceryMapState();
                    // WebInterfaceMenuCarppi.DriverStateDetermination();
                    // UpdateLocation()



                }
                catch (System.Exception) { }

            }, null, startTimeSpan, periodTimeSpan);

            */


        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            WebInterfaceFragmentGrocery.UpdateGroceryMapState();
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

        class GroceryWebClient : WebViewClient
        {
            public Context mContext;

            public Resources Resources;

            public GroceryWebClient(Context contexto, Resources res)
            {
                mContext = contexto;
                Resources = res;
            }

            public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                // WebInterfaceMenuCarppi.CenterThemap();
                LoadMyView();

                var t = Task.Run(() => InitialWork(view));
                // UpdateRegion();
                //  UpdateLocation();
            }
            public async void InitialWork(WebView view)
            {
               
                FragmentGrocery.DoWork_Driver();
                PendingGroceryOrder(view);
                GetLocationByState();
            }
            public async void LoadMyView()
            {
                Action action_WhowAlert = () =>
                {
                    var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_);
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
                    var scale = Resources.DisplayMetrics.Density;//density i.e., pixels per inch or cms

                    var widthPixels = Resources.DisplayMetrics.WidthPixels;////getting the height in pixels  
                    var width = (double)((widthPixels - 0.5f) / scale);//width in units //411 //359
                    var heightPixels = Resources.DisplayMetrics.HeightPixels;////getting the height in pixels  
                    var height = (double)((heightPixels - 0.5f) / scale);//731 //680

                    using (StreamReader sr = new StreamReader(assets.Open("GroceryCatalog_4.html")))
                    {
                        content = sr.ReadToEnd();
                        /*
                        content = content.Replace("<!--ScrollableVegtableList-->", productview.Vegetales);
                        content = content.Replace("<!--ScrollableFruitList-->", productview.Frutas);
                        content = content.Replace("<!--ScrollableMeatList-->", productview.Carne_Embutidos_Atun);
                        content = content.Replace("<!--ScrollableSodaList-->", productview.BotanasRefresco);
                        content = content.Replace("<!--ScrollableChesseList-->", productview.Queso);
                        content = content.Replace("<!--ScrollableTortillaList-->", productview.Tortillas_Pan);
                        content = content.Replace("<!--ScrollableAlcoholList-->", productview.Alcohol);
                        content = content.Replace("<!--ScrollableProductosDeLimpiezaList-->", productview.ProductosDeLimpieza);
                        content = content.Replace("<!--ScrollableGalletasList-->", productview.Galletas);
                        */


                        content = content.Replace("MAximunWidthReplace", (width * 1.2).ToString() + "px");

                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());


                };

                ((Activity)mContext).RunOnUiThread(action_WhowAlert);

            }
            public bool IsLocationAvailable()
            {
                if (!CrossGeolocator.IsSupported)
                    return false;

                return CrossGeolocator.Current.IsGeolocationAvailable;
            }
            public async void GetLocationByState()
            {
                try
                {
                    if (IsLocationAvailable()) { 
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
                    // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/SearchForPassengerAreaByStateAndCountry?" +
                        "Town=" + placemark.Locality +
                        "&Country=" + placemark.CountryName
                        + "&State=" + placemark.AdminArea
                        + "&FacebookID_UpdateArea=" + FaceID
                        + "&lat=" + MyLatLong.Latitude
                        + "&log=" + MyLatLong.Longitude


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
                                Region_Delivery = rrr.ID,
                                //ServiciosRegionales = rrr.ServicioRegional



                            });
                        }
                        else
                        {
                            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                            query.Region_Delivery = rrr.ID;
                            //query.ServiciosRegionales = rrr.ServicioRegional;
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
                }

                catch (Exception Ex)
                {

                }
            }

            public async void PendingGroceryOrder(WebView view)
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

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetPendingOrder?" +
                        "FaceIDHash=" + FaceID


                        ));
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    if(S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        
                        var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Ressult.Response);
                        //Map
                        //An Order Is Pending

                        Action action_WhowAlert = () =>
                        {
                            var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_);
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
                            sss.SetWebViewClient(new LocalWebViewClient());


                        };

                        ((Activity)mContext).RunOnUiThread(action_WhowAlert);

                    }
                    else
                    {

                        GetGroceryList(view);
                        //< !--ProductReplaceSuperTag-- >
                         
                    }
                   

                }

                catch (Exception Ex)
                {

                }
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
            public async void RequestProductSubSetList(ProductSubset product_index)
            {
                try
                {


                    HttpClient client = new HttpClient();

                    string FaceID = null;
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    try
                    {

                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        FaceID = query.ProfileId;
                    }
                    catch (Exception ex)
                    {

                    }

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetCompleteListOfProductsByProductSubset?" +
                        "Region=" + 1 +
                        "&Subset=" + (int)product_index


                        ));


                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    var Unserialized = JsonConvert.DeserializeObject<List<CarppiGrocery_Productos>>(S_Ressult.Response);
                    // var SubstituteString = GenerateProductBySubset(Unserialized);
                    //UpdateGrossProductListLaytou(Data, IndexOfpage)
                    //  var productview = GenerateProductRow(Unserialized);

                    //var productLsit = GenerateProducList(Unserialized);
                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        //GenerateProductBySubset
                        var script = "UpdateGrossProductListLaytou(" + JsonConvert.SerializeObject(Unserialized) + "," + ((int)product_index) + ")";
                        Static_Webi.EvaluateJavascript(script, null);


                    };

                    // Create a task but do not start it.
                    // Task t1 = new Task(action, "alpha");

                    Static_Webi.Post(action);





                }

                catch (Exception Ex)
                {

                }
            }

            public async void GetGroceryList(WebView view)
            {
                try
                {
                    for (var i = (int)ProductSubset.Verduras_hortalizas; i <= (int)ProductSubset.Galletas; i++)
                    {
                        var t = Task.Run(() => RequestProductSubSetList((ProductSubset)i));
                        t.Wait();
                    }
                }
                catch(Exception)
                {

                }
            
                /*
                try
                {
                   


                    HttpClient client = new HttpClient();

                    string FaceID = null;
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    try
                    {

                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        FaceID = query.ProfileId;
                    }
                    catch (Exception ex)
                    {

                    }

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetCompleteListOfProducts?" +
                        "Region=" + 1


                        ));
                    

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var S_Ressult = t.Result;
                    var Unserialized = JsonConvert.DeserializeObject<List<CarppiGrocery_Productos>>(S_Ressult.Response);
                     var productview =GenerateProductRow(Unserialized);

                    var productLsit = GenerateProducList(Unserialized);

                    var scale = Resources.DisplayMetrics.Density;//density i.e., pixels per inch or cms

                    var widthPixels = Resources.DisplayMetrics.WidthPixels;////getting the height in pixels  
                    var width = (double)((widthPixels - 0.5f) / scale);//width in units //411 //359
                    var heightPixels = Resources.DisplayMetrics.HeightPixels;////getting the height in pixels  
                    var height = (double)((heightPixels - 0.5f) / scale);//731 //680

                    Action action_WhowAlert = () =>
                    {
                        var sss = ((Activity)mContext).FindViewById<WebView>(Resource.Id.webView_);
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
                        
                        using (StreamReader sr = new StreamReader(assets.Open("GroceryCatalog_4.html")))
                        {
                            content = sr.ReadToEnd();
                            content = content.Replace("<!--ScrollableVegtableList-->", productview.Vegetales);
                            content = content.Replace("<!--ScrollableFruitList-->", productview.Frutas);
                            content = content.Replace("<!--ScrollableMeatList-->", productview.Carne_Embutidos_Atun);
                            content = content.Replace("<!--ScrollableSodaList-->", productview.BotanasRefresco);
                            content = content.Replace("<!--ScrollableChesseList-->", productview.Queso);
                            content = content.Replace("<!--ScrollableTortillaList-->", productview.Tortillas_Pan);
                            content = content.Replace("<!--ScrollableAlcoholList-->", productview.Alcohol);
                            content = content.Replace("<!--ScrollableProductosDeLimpiezaList-->", productview.ProductosDeLimpieza);
                            content = content.Replace("<!--ScrollableGalletasList-->", productview.Galletas);

                            
                            content = content.Replace("MAximunWidthReplace", (width*1.2).ToString() + "px");
                           
                            sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                        }
                        sss.SetWebViewClient(new LocalWebViewClient());


                    };

                    ((Activity)mContext).RunOnUiThread(action_WhowAlert);


                }

                catch (Exception Ex)
                {

                }
                */
            }
            public class CarppiGrocery_Productos
            {
                public long ID { get; set; }
                public long? RegionID { get; set; }
                public string Producto { get; set; }
                public double? Costo { get; set; }
                public byte[] Foto { get; set; }
                public ProductSubset Categoria { get; set; }
            }
            public string GenerateProducList(List<CarppiGrocery_Productos> ListadeProductos_arg)
            {
                var GenerativeString = "";
                var WidtInitial = 0.0;
                var HeigthInitial = 0.0;
                for (var i = 0; i < ListadeProductos_arg.Count; i++)
                {
                    var product = ListadeProductos_arg[i];
                    //AddItemToShopingKart + ProductJson
                    try
                    {
                        GenerativeString += "<li class='woocommerce-mini-cart-item mini_cart_item'>";
                       // GenerativeString += "<a href='#' class='remove remove_from_cart_button' aria-label='Remove this item' data-product_id='31' data-cart_item_key='975bddd97c5167b253173d5ef97b7ec9' data-product_sku=''><i class='flaticon-cross37'></i></a>";
                        GenerativeString += "<img width='550' height='700' src='data:image/png;base64," + Convert.ToBase64String(product.Foto) + "' class='attachment-woocommerce_thumbnail size-woocommerce_thumbnail' alt=''>" + product.Producto + "";
                        GenerativeString += "</a>";
                        GenerativeString += "<span class='quantity'><span class='woocommerce-Price-amount amount'><span class='woocommerce-Price-currencySymbol'>$</span>"+ product.Costo + "</span></span>";
                        GenerativeString += "<span><button class='Superbutton' id='Item_" + product.ID + "' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(product) + ")'>Añadir</button></span>";
                        GenerativeString += "</li>";
                      



                    }
                    catch (Exception)
                    { }



                   
                }
                return GenerativeString;
                /*
                 *
                 *
                  <li class='woocommerce-mini-cart-item mini_cart_item'>
                                                <a href="https://demo.kaliumtheme.com/shop/cart/?remove_item=975bddd97c5167b253173d5ef97b7ec9&amp;_wpnonce=bb47631bd3" class="remove remove_from_cart_button" aria-label="Remove this item" data-product_id="31" data-cart_item_key="975bddd97c5167b253173d5ef97b7ec9" data-product_sku=""><i class="flaticon-cross37"></i></a>											<a href="https://demo.kaliumtheme.com/shop/product/ninja-silhouette/?attribute_pa_color=white">
                                                    <img width="550" height="700" src="https://demo.kaliumtheme.com/shop/wp-content/uploads/2015/05/yes_030_braun_travel_alarm_clock_3-1340x7851-550x700.jpg" class="attachment-woocommerce_thumbnail size-woocommerce_thumbnail" alt="">Digital Clock - White
                                                </a>
                                                <span class="quantity">2 × <span class="woocommerce-Price-amount amount"><span class="woocommerce-Price-currencySymbol">£</span>28.00</span></span>
                                                <span><a href="https://demo.kaliumtheme.com/shop/checkout/" class="button checkout wc-forward">Aggregar al carrito</a></span>
                                            </li>
                 */
            }

            public enum ProductSubset { Verduras_hortalizas,
                Frutas,
                Carne_Embutidos_Atun,
                Queso,
                Tortillas_Pan,
                BotanasRefresco,
                Alcohol,
                ProductosDeLimpieza,
                Galletas
            };
            public class ProductList
            {
                public string Vegetales = "";
                public string Frutas = "";
                public string Carne_Embutidos_Atun = "";
                public string Queso = "";
                public string Tortillas_Pan = "";
                public string BotanasRefresco = "";
                public string Alcohol = "";
                public string ProductosDeLimpieza = "";
                public string Galletas = "";
            }
            public string GenerateProductBySubset(List<CarppiGrocery_Productos> ListadeProductos_arg)
            {
                var Coment = "";
               // var P_List = new ProductList();
                //GenerativeString += "<img width='550' height='700' src='data:image/png;base64," + Convert.ToBase64String(product.Foto) + "' class='attachment-woocommerce_thumbnail size-woocommerce_thumbnail' alt=''>" + product.Producto + "";
                foreach (var articulo in ListadeProductos_arg)
                {

                    Coment += "<div class='ScrollElement'>";
                    Coment += "<div class='owl-item' style='width: 237px;'>";
                    Coment += "<div class='item'>";
                    Coment += "<div class='product'>";
                    Coment += "<div>";
                    Coment += "<div class='product-header'>";
                    Coment += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                    Coment += "<span class='veg text-success mdi mdi-circle'></span>";
                    Coment += "</div>";
                    Coment += "<div class='product-body'>";
                    Coment += "<h5>" + articulo.Producto + "</h5>";
                    // Coment += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                    Coment += "</div>";
                    Coment += "<div class='product-footer'>";
                    Coment += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                    Coment += "</div>";
                    Coment += "</div>";
                    Coment += "</div>";
                    Coment += "</div>";
                    Coment += "</div>";
                    Coment += "</div>";
                    /*
                    Coment += "<div class='card'>";
                    Coment += "<div style='background-color:white;'>";
                    Coment += "<img src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "' alt='Denim Jeans' style='width:100%'>";
                    Coment += "<h1>Tailored Jeans</h1>";
                    Coment += "<p class='price'>$19.99</p>";

                    Coment += "<p><button>Add to Cart</button></p>";
                    Coment += "</div>";
                    Coment += "</div>";
                    Coment += "</div>";
                    */


                }
                return Coment;



            }
            public ProductList GenerateProductRow(List<CarppiGrocery_Productos> ListadeProductos_arg)
            {
                var Coment = "";
                var P_List = new ProductList();
                //GenerativeString += "<img width='550' height='700' src='data:image/png;base64," + Convert.ToBase64String(product.Foto) + "' class='attachment-woocommerce_thumbnail size-woocommerce_thumbnail' alt=''>" + product.Producto + "";
                foreach (var articulo in ListadeProductos_arg)
                {
                    if (articulo.Categoria == ProductSubset.Verduras_hortalizas)
                    {
                        P_List.Vegetales += "<div class='ScrollElement'>";
                        P_List.Vegetales += "<div class='owl-item' style='width: 237px;'>";
                        P_List.Vegetales += "<div class='item'>";
                        P_List.Vegetales += "<div class='product'>";
                        P_List.Vegetales += "<div>";
                        P_List.Vegetales += "<div class='product-header'>";
                        P_List.Vegetales += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.Vegetales += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "<div class='product-body'>";
                        P_List.Vegetales += "<h5>"+ articulo.Producto + "</h5>";
                       // P_List.Vegetales += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "<div class='product-footer'>";
                        P_List.Vegetales += "<p class='offer-price mb-0'>$" +articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";
                        
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "</div>";
                        /*
                        P_List.Vegetales += "<div class='card'>";
                        P_List.Vegetales += "<div style='background-color:white;'>";
                        P_List.Vegetales += "<img src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "' alt='Denim Jeans' style='width:100%'>";
                        P_List.Vegetales += "<h1>Tailored Jeans</h1>";
                        P_List.Vegetales += "<p class='price'>$19.99</p>";

                        P_List.Vegetales += "<p><button>Add to Cart</button></p>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "</div>";
                        P_List.Vegetales += "</div>";
                        */

                    }
                    else if (articulo.Categoria == ProductSubset.Frutas)
                    {
                        P_List.Frutas += "<div class='ScrollElement'>";
                        P_List.Frutas += "<div class='owl-item' style='width: 237px;'>";
                        P_List.Frutas += "<div class='item'>";
                        P_List.Frutas += "<div class='product'>";
                        P_List.Frutas += "<div>";
                        P_List.Frutas += "<div class='product-header'>";
                        P_List.Frutas += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.Frutas += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.Frutas += "</div>";
                        P_List.Frutas += "<div class='product-body'>";
                        P_List.Frutas += "<h5>" + articulo.Producto + "</h5>";
                     //   P_List.Frutas += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.Frutas += "</div>";
                        P_List.Frutas += "<div class='product-footer'>";
                        P_List.Frutas += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.Frutas += "</div>";
                        P_List.Frutas += "</div>";
                        P_List.Frutas += "</div>";
                        P_List.Frutas += "</div>";
                        P_List.Frutas += "</div>";
                        P_List.Frutas += "</div>";
                    }
                    else if (articulo.Categoria == ProductSubset.Carne_Embutidos_Atun)
                    {
                        P_List.Carne_Embutidos_Atun += "<div class='ScrollElement'>";
                        P_List.Carne_Embutidos_Atun += "<div class='owl-item' style='width: 237px;'>";
                        P_List.Carne_Embutidos_Atun += "<div class='item'>";
                        P_List.Carne_Embutidos_Atun += "<div class='product'>";
                        P_List.Carne_Embutidos_Atun += "<div>";
                        P_List.Carne_Embutidos_Atun += "<div class='product-header'>";
                        P_List.Carne_Embutidos_Atun += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.Carne_Embutidos_Atun += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.Carne_Embutidos_Atun += "</div>";
                        P_List.Carne_Embutidos_Atun += "<div class='product-body'>";
                        P_List.Carne_Embutidos_Atun += "<h5>" + articulo.Producto + "</h5>";
                    //    P_List.Carne_Embutidos_Atun += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.Carne_Embutidos_Atun += "</div>";
                        P_List.Carne_Embutidos_Atun += "<div class='product-footer'>";
                        P_List.Carne_Embutidos_Atun += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.Carne_Embutidos_Atun += "</div>";
                        P_List.Carne_Embutidos_Atun += "</div>";
                        P_List.Carne_Embutidos_Atun += "</div>";
                        P_List.Carne_Embutidos_Atun += "</div>";
                        P_List.Carne_Embutidos_Atun += "</div>";
                        P_List.Carne_Embutidos_Atun += "</div>";
                    }
                    else if (articulo.Categoria == ProductSubset.Queso)
                    {
                        P_List.Queso += "<div class='ScrollElement'>";
                        P_List.Queso += "<div class='owl-item' style='width: 237px;'>";
                        P_List.Queso += "<div class='item'>";
                        P_List.Queso += "<div class='product'>";
                        P_List.Queso += "<div>";
                        P_List.Queso += "<div class='product-header'>";
                        P_List.Queso += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.Queso += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.Queso += "</div>";
                        P_List.Queso += "<div class='product-body'>";
                        P_List.Queso += "<h5>" + articulo.Producto + "</h5>";
                     //   P_List.Queso += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.Queso += "</div>";
                        P_List.Queso += "<div class='product-footer'>";
                        P_List.Queso += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.Queso += "</div>";
                        P_List.Queso += "</div>";
                        P_List.Queso += "</div>";
                        P_List.Queso += "</div>";
                        P_List.Queso += "</div>";
                        P_List.Queso += "</div>";
                    }
                    else if (articulo.Categoria == ProductSubset.Tortillas_Pan)
                    {
                        P_List.Tortillas_Pan += "<div class='ScrollElement'>";
                        P_List.Tortillas_Pan += "<div class='owl-item' style='width: 237px;'>";
                        P_List.Tortillas_Pan += "<div class='item'>";
                        P_List.Tortillas_Pan += "<div class='product'>";
                        P_List.Tortillas_Pan += "<div>";
                        P_List.Tortillas_Pan += "<div class='product-header'>";
                        P_List.Tortillas_Pan += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.Tortillas_Pan += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.Tortillas_Pan += "</div>";
                        P_List.Tortillas_Pan += "<div class='product-body'>";
                        P_List.Tortillas_Pan += "<h5>" + articulo.Producto + "</h5>";
                   //     P_List.Tortillas_Pan += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.Tortillas_Pan += "</div>";
                        P_List.Tortillas_Pan += "<div class='product-footer'>";
                        P_List.Tortillas_Pan += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.Tortillas_Pan += "</div>";
                        P_List.Tortillas_Pan += "</div>";
                        P_List.Tortillas_Pan += "</div>";
                        P_List.Tortillas_Pan += "</div>";
                        P_List.Tortillas_Pan += "</div>";
                        P_List.Tortillas_Pan += "</div>";
                    }
                    else if (articulo.Categoria == ProductSubset.BotanasRefresco)
                    {
                        P_List.BotanasRefresco += "<div class='ScrollElement'>";
                        P_List.BotanasRefresco += "<div class='owl-item' style='width: 237px;'>";
                        P_List.BotanasRefresco += "<div class='item'>";
                        P_List.BotanasRefresco += "<div class='product'>";
                        P_List.BotanasRefresco += "<div>";
                        P_List.BotanasRefresco += "<div class='product-header'>";
                        P_List.BotanasRefresco += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.BotanasRefresco += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.BotanasRefresco += "</div>";
                        P_List.BotanasRefresco += "<div class='product-body'>";
                        P_List.BotanasRefresco += "<h5>" + articulo.Producto + "</h5>";
                     //   P_List.BotanasRefresco += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.BotanasRefresco += "</div>";
                        P_List.BotanasRefresco += "<div class='product-footer'>";
                        P_List.BotanasRefresco += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.BotanasRefresco += "</div>";
                        P_List.BotanasRefresco += "</div>";
                        P_List.BotanasRefresco += "</div>";
                        P_List.BotanasRefresco += "</div>";
                        P_List.BotanasRefresco += "</div>";
                        P_List.BotanasRefresco += "</div>";
                    }
                    else if (articulo.Categoria == ProductSubset.Alcohol)
                    {
                        P_List.Alcohol += "<div class='ScrollElement'>";
                        P_List.Alcohol += "<div class='owl-item' style='width: 237px;'>";
                        P_List.Alcohol += "<div class='item'>";
                        P_List.Alcohol += "<div class='product'>";
                        P_List.Alcohol += "<div>";
                        P_List.Alcohol += "<div class='product-header'>";
                        P_List.Alcohol += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.Alcohol += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.Alcohol += "</div>";
                        P_List.Alcohol += "<div class='product-body'>";
                        P_List.Alcohol += "<h5>" + articulo.Producto + "</h5>";
                        //   P_List.BotanasRefresco += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.Alcohol += "</div>";
                        P_List.Alcohol += "<div class='product-footer'>";
                        P_List.Alcohol += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.Alcohol += "</div>";
                        P_List.Alcohol += "</div>";
                        P_List.Alcohol += "</div>";
                        P_List.Alcohol += "</div>";
                        P_List.Alcohol += "</div>";
                        P_List.Alcohol += "</div>";
                    }
                    else if (articulo.Categoria == ProductSubset.ProductosDeLimpieza)
                    {
                        P_List.ProductosDeLimpieza += "<div class='ScrollElement'>";
                        P_List.ProductosDeLimpieza += "<div class='owl-item' style='width: 237px;'>";
                        P_List.ProductosDeLimpieza += "<div class='item'>";
                        P_List.ProductosDeLimpieza += "<div class='product'>";
                        P_List.ProductosDeLimpieza += "<div>";
                        P_List.ProductosDeLimpieza += "<div class='product-header'>";
                        P_List.ProductosDeLimpieza += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.ProductosDeLimpieza += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.ProductosDeLimpieza += "</div>";
                        P_List.ProductosDeLimpieza += "<div class='product-body'>";
                        P_List.ProductosDeLimpieza += "<h5>" + articulo.Producto + "</h5>";
                        //   P_List.BotanasRefresco += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.ProductosDeLimpieza += "</div>";
                        P_List.ProductosDeLimpieza += "<div class='product-footer'>";
                        P_List.ProductosDeLimpieza += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.ProductosDeLimpieza += "</div>";
                        P_List.ProductosDeLimpieza += "</div>";
                        P_List.ProductosDeLimpieza += "</div>";
                        P_List.ProductosDeLimpieza += "</div>";
                        P_List.ProductosDeLimpieza += "</div>";
                        P_List.ProductosDeLimpieza += "</div>";
                    }
                    else if (articulo.Categoria == ProductSubset.Galletas)
                    {
                        P_List.Galletas += "<div class='ScrollElement'>";
                        P_List.Galletas += "<div class='owl-item' style='width: 237px;'>";
                        P_List.Galletas += "<div class='item'>";
                        P_List.Galletas += "<div class='product'>";
                        P_List.Galletas += "<div>";
                        P_List.Galletas += "<div class='product-header'>";
                        P_List.Galletas += "<img class='img-fluid' src='data:image/png;base64," + Convert.ToBase64String(articulo.Foto) + "'' alt=''>";
                        P_List.Galletas += "<span class='veg text-success mdi mdi-circle'></span>";
                        P_List.Galletas += "</div>";
                        P_List.Galletas += "<div class='product-body'>";
                        P_List.Galletas += "<h5>" + articulo.Producto + "</h5>";
                        //   P_List.BotanasRefresco += "<h6><strong><span class='mdi mdi-approval'></span> Available in</strong> - 500 gm</h6>";
                        P_List.Galletas += "</div>";
                        P_List.Galletas += "<div class='product-footer'>";
                        P_List.Galletas += "<p class='offer-price mb-0'>$" + articulo.Costo + "</p><button id='Item_" + articulo.ID + "' type='button' class='btn btn-secondary btn-sm' onclick='AddItemToShopingKart(" + JsonConvert.SerializeObject(articulo) + ")'>Añadir</button>";

                        P_List.Galletas += "</div>";
                        P_List.Galletas += "</div>";
                        P_List.Galletas += "</div>";
                        P_List.Galletas += "</div>";
                        P_List.Galletas += "</div>";
                        P_List.Galletas += "</div>";
                    }
                }
                return P_List;
             
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



        public static FragmentGrocery NewInstance()
        {
            var frag1 = new FragmentGrocery { Arguments = new Bundle() };
            return frag1;
        }
        public bool isLoggedIn()
        {
           
              AccessToken accessToken = AccessToken.CurrentAccessToken;//AccessToken.getCurrentAccessToken();
              return accessToken != null;
            //return false;
        }
    }
   
    public class WebInterfaceFragmentGrocery : Java.Lang.Object
    {
        Context mContext;
        public static Context Static_mContext;
        WebView webi;
        public static WebView Static_Webi;
        //private ICallbackManager mFBCallManager;
        public static ICallbackManager mFBCallManager;
        private static MyProfileTracker mprofileTracker;
        private ProfilePictureView mprofile;
        public static facebookCallback facebookCallback_Static;
        public static Int64 OrderID;

        public WebInterfaceFragmentGrocery(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            Static_Webi = web;
            Static_mContext = Act;
            OrderID = 0;


        }
        //ShowExtraOptionsOnGroceryAwait
        [JavascriptInterface]
        [Export("ShowExtraOptionsOnGroceryAwait")]
        public static async void ShowExtraOptionsOnGroceryAwait()
        {
            try
            {
                HttpClient client = new HttpClient();
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
                        sss.SetWebViewClient(new LocalWebViewClient());




                    };

                    ((Activity)Static_mContext).RunOnUiThread(action_WhowAlert);

                    MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateHalfExpanded;

                }
            }
            catch(Exception)
            {

            }
        }
        public class GroceryExtraExtraDatacaCommentUtility
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

        public static String GenerateCommentSeccion(GroceryExtraExtraDatacaCommentUtility Data)
        {
            var BigSTR = "";
            foreach (var coment in Data.ListOfComents)
            {
                BigSTR += "<img src='data:image/png;base64, " + coment.PhotoOfComenter + "' alt='Avatar' class='w3-left w3-circle w3-margin-right' style='width:80px'>";
                BigSTR += "<p><span class='w3-large w3-text-black w3-margin-right'>Calificacion: " + coment.ComentData.Rate + "</span></p>";
                BigSTR += "<p>" + coment.ComentData.Comentario + "</p>";
            }
            return BigSTR;

        }
        public enum enumGroceryStates { none, regulargrocery, map};
        public static enumGroceryStates State = enumGroceryStates.none;
        [JavascriptInterface]
        [Export("UpdateGroceryMapState")]
        public static async void UpdateGroceryMapState()
        {
            // var casde = Base64Decode(ProductHash);
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

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetPendingOrder?" +
                    "FaceIDHash=" + FaceID


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    State = enumGroceryStates.map;
                    var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Ressult.Response);
                    //ShowStatusOfGroceryOrder
                    OrderID = Order.ID;
                    Action action2 = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "ShowStatusOfGroceryOrder(" + ((int)Order.Stat) + "," + S_Ressult.Response + ")";
                        Static_Webi.EvaluateJavascript(script, null);


                    };


                    Static_Webi.Post(action2);
                    //Map
                    //An Order Is Pending
                    if (Order.Stat == FragmentRestaurantDetailedView.GroceryOrderState.RequestEnded)
                    {
                        await Task.Delay(5000).ContinueWith(lol =>
                         { UdapteLayoutOnrequestFinish(); });
                        //LoadFragment_Static
                        //Resource.Id.menu_video:
                    }

                }
                else
                {
                    if (State == enumGroceryStates.map)
                    {
                        State = enumGroceryStates.none;
                        Action action2 = () =>
                        {
                        //var jsr = new JavascriptResult();
                        var script = "ShowStatusOfGroceryOrder(" + ((int)FragmentRestaurantDetailedView.GroceryOrderState.RequestEnded) + "," + S_Ressult.Response + ")";
                            Static_Webi.EvaluateJavascript(script, null);


                        };


                        Static_Webi.Post(action2);
                        //Map
                        //An Order Is Pending

                        await Task.Delay(5000).ContinueWith(lol =>
                        { UdapteLayoutOnrequestFinish(); });
                        //LoadFragment_Static
                        //Resource.Id.menu_video:


                    }
                }


            }

            catch (Exception Ex)
            {

            }
        }
        static void UdapteLayoutOnrequestFinish()
        {
           MainActivity.LoadFragment_Static(Resource.Id.menu_video);
        }
        public class CarppiGrocery_BuyOrders
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


        public static List<CarppiGroceryProductos> ListaDeProductos = new List<CarppiGroceryProductos>();
        [JavascriptInterface]
        [Export("UpdateProductList")]
        public static void UpdateProductList(string ProductHash, int direction)
        {
           // var casde = Base64Decode(ProductHash);
            var Cadena =JsonConvert.DeserializeObject< CarppiGroceryProductos>((ProductHash));
          var sss = ListaDeProductos.Where(X => X.ID == Cadena.ID).FirstOrDefault();
            if(sss == null)
            {

                Cadena.Cantidad = 1;
                ListaDeProductos.Add(Cadena);
            }
            else
            {
               if( sss.Cantidad == 1 && direction == -1)
                {
                    ListaDeProductos.Remove(sss);
                }
                else
                {
                    sss.Cantidad = sss.Cantidad + direction;
                }
            }
        }


        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public class CarppiGroceryProductos
        {
            public int ID;
            public int RegionID;
            public int Cantidad;
            public string Producto;
            public double Costo;
            public byte[] Foto;




        }

        //DisplayShopingKart
        [JavascriptInterface]
        [Export("DisplayShopingKart")]
        public async static void DisplayShopingKart()
        {
            //0000000000
            //CalculateCostOfTrip(Int32 Region_costo, double LatitudPedido, double LongitudPedido)
            var MyLatLong = await Clases.Location.GetCurrentPosition();



            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
          
            var query = new Log_info();
            var Region = "";
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            try
            {
                var db5 = new SQLiteConnection(databasePath5);
                query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();
                Region = (query.Region_Delivery == null ? 0.ToString() : (query.Region_Delivery).ToString());
            }
            catch (Exception)
            {

            }

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/CalculateCostOfTrip?" +
                "Region_costo=" + Region +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&LatitudPedido=" + MyLatLong.Latitude +
                "&LongitudPedido=" + MyLatLong.Longitude

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
                var TransportCost = Convert.ToInt32(errorMessage1);
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

                    AssetManager assets = ((Activity)Static_mContext).Assets;
                    string content;
                    var Viewww = new UtilityJavascriptInterfaceGrocery(Static_mContext, sss);
                    sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                    using (StreamReader sr = new StreamReader(assets.Open("ShoppingKart.html")))
                    {
                        content = sr.ReadToEnd();
                        var cadea = GenerateRowsForCheckOutModal(ListaDeProductos);
                        content = content.Replace("<!--ReplaceProductFromTheList-->", cadea);
                        content = content.Replace("0000000000", TransportCost.ToString());
                        content = content.Replace("CompleCostOFGroceryy", CompleteCostOFGrocery(ListaDeProductos));
                        //GroceryCostPlusFee
                        content = content.Replace("GroceryCostPlusFee", CompleteCostOFGroceryPlusFee(ListaDeProductos, TransportCost));
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new LocalWebViewClient());

                    //<span class="woocommerce-Price-currencySymbol">£</span>90.00</span>
                    //CompleCostOFGroceryy


                };

                ((Activity)Static_mContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;



            }
         

        }
        public static string CompleteCostOFGrocery(List<CarppiGroceryProductos> carppiGroceryProductos)
        {
            var Cost = 0.0;
            foreach (var element in carppiGroceryProductos)
            {
                Cost += element.Costo;

            }
            return "<span class='woocommerce-Price-currencySymbol'>$</span>"+ Cost.ToString() + "</span>";
        }
        public static string CompleteCostOFGroceryPlusFee(List<CarppiGroceryProductos> carppiGroceryProductos, int CostOfTravel)
        {
            var Cost = 0.0;
            foreach (var element in carppiGroceryProductos)
            {
                Cost += element.Costo;

            }
            return "<span class='woocommerce-Price-currencySymbol'>$</span>" + (Cost+ CostOfTravel).ToString() + "</span>";
        }
        
        public static string GenerateRowsForCheckOutModal(List<CarppiGroceryProductos> carppiGroceryProductos)
        {
            //UpdateProductList(string ProductHash, int direction)
            //window.Android_BottomModal.DissmissBottomModal();
            try
            {
                var CharArrr = "";
                foreach (var element in carppiGroceryProductos)
                {
                    if (element.Foto != null)
                    {
                        CharArrr += "<tr class='woocommerce-cart-form__cart-item cart_item' id='TR_Element_" + element.ID + "'>";
                        CharArrr += "<td class='product-remove'>";
                        CharArrr += "<a onclick='RemoveFromList(" + element.ID + ")' href='#' class='remove' aria-label='Remove this item' data-product_id='31' data-product_sku=''><i class='flaticon-cross37'></i></a>";
                        CharArrr += "</td>";
                        CharArrr += "<td class='product-thumbnail'>";
                        CharArrr += "<a href='#' class=''><img width = '550' height='700' src='data:image/png;base64," + Convert.ToBase64String(element.Foto) + "' class='attachment-woocommerce_thumbnail size-woocommerce_thumbnail' alt=''></a>'";
                        CharArrr += "</td>";

                        CharArrr += "<td class='product-name' data-title='Product'>";
                        CharArrr += "<a href='#' class=''>" + element.Producto.Replace("'", "") + "</a>'";
                        CharArrr += "</td>";
                        CharArrr += "<td class='product-quantity' data-title='Quantity'>";
                        CharArrr += "<div class='quantity buttons_added'>";
                        CharArrr += "<input type = 'button' value='-' class='minus'>";
                        CharArrr += "<label class='screen-reader-text' for='quantity_5e82063d1e673'>Digital Clock - White quantity</label>";
                        CharArrr += "<input disabled type='number'onchange='UpdateProductTubTotal(" + element.ID + ", " + element.Costo + ")' id='ProductoListID_" + element.ID + "' class='input-text qty text' step='1' min='0' max='' name='cart[975bddd97c5167b253173d5ef97b7ec9][qty]' value='1' title='Qty' size='4' inputmode='numeric'>";
                        CharArrr += "<input type='button' value='+' class='plus'>";
                        CharArrr += "</div>";
                        CharArrr += "</td>";

                        CharArrr += "  <td class='product-subtotal' data-title='Subtotal'>";
                        CharArrr += "     <span class='woocommerce-Price-amount amount' id='ProductSubtotalID_" + element.ID + "'><span class='woocommerce-Price-currencySymbol'>$</span>" + element.Costo + "</span>";
                        CharArrr += "<h6 style='display:none' name='InvisiblePriceAdjustemt' id='InvisiblePriceAdjustemtID_" + element.ID + "'>" + element.Costo + "</h6>";
                        CharArrr += " </td>";
                        CharArrr += "</tr>";
                    }
                }
                return CharArrr;
            }
            catch (Exception)
            {
                return "";

            }/*
              < tr class="woocommerce-cart-form__cart-item cart_item">

                                    <td class="product-remove">
                                        <a href = "#" class="remove" aria-label="Remove this item" data-product_id="31" data-product_sku=""><i class="flaticon-cross37"></i></a>
                                    </td>

                                    <td class="product-thumbnail">
                                        <a href = "https://demo.kaliumtheme.com/shop/product/ninja-silhouette/?attribute_pa_color=white" class=""><img width = "550" height="700" src="https://demokaliumsites-laborator.netdna-ssl.com/shop/wp-content/uploads/2015/05/yes_030_braun_travel_alarm_clock_3-1340x7851-550x700.jpg" class="attachment-woocommerce_thumbnail size-woocommerce_thumbnail" alt=""></a>
                                    </td>

                                    <td class="product-name" data-title="Product">
                                        <a href = "https://demo.kaliumtheme.com/shop/product/ninja-silhouette/?attribute_pa_color=white" class="">Digital Clock - White</a>
                                    </td>

                                    <td class="product-price" data-title="Price">
                                        <span class="woocommerce-Price-amount amount"><span class="woocommerce-Price-currencySymbol">£</span>28.00</span>
                                    </td>

                                    <td class="product-quantity" data-title="Quantity">
                                        <div class="quantity buttons_added">
                                            <input type = "button" value="-" class="minus">
                                            <label class="screen-reader-text" for="quantity_5e82063d1e673">Digital Clock - White quantity</label>
                                            <input type = "number" id= "quantity_5e82063d1e673" class="input-text qty text" step="1" min="0" max="" name="cart[975bddd97c5167b253173d5ef97b7ec9][qty]" value="2" title="Qty" size="4" inputmode="numeric">
                                            <input type = "button" value="+" class="plus">
                                        </div>
                                    </td>

                                    <td class="product-subtotal" data-title="Subtotal">
                                        <span class="woocommerce-Price-amount amount"><span class="woocommerce-Price-currencySymbol">£</span>56.00</span>
                                    </td>
                                </tr>
                                */
        }




    }



    public class UtilityJavascriptInterfaceGrocery : Java.Lang.Object
    {
        Context mContext;
        WebView webi;
        public UtilityJavascriptInterfaceGrocery(Context Act, WebView web)
        {
            mContext = Act;
            webi = web;
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

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/PostMessageInRideShare?" +
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
        [Export("UpdateProductList")]
        public static void UpdateProductList(int ProductHash, int quantity)
        {

            var micopia = ListaDeProductos;
            // var casde = Base64Decode(ProductHash);
            //var Cadena = JsonConvert.DeserializeObject<CarppiGroceryProductos>((ProductHash));
            var sss = ListaDeProductos.Where(X => X.ID == ProductHash).FirstOrDefault();
            if (sss == null)
            {
                var producto = new CarppiGroceryProductos();
                producto.ID = ProductHash;
                producto.Cantidad = 1;
                ///ListaDeProductos.Add(producto);
            }
            else
            {
                sss.Cantidad = quantity;
                if (quantity == 0)
                {
                    ListaDeProductos.Remove(sss);
                }

            }
        }





        [JavascriptInterface]
        [Export("GetAllMessagesFromConversation")]
        public async void GetAllMessagesFromConversation(Int64 TripID)
        {
            try
            {

                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetAllMessagesFromTheConversation?" +
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
            catch(Exception)
            { }
        }

        public enum TipoDePago { Efectivo, Tarjeta};

        [JavascriptInterface]
        [Export("GeneretaGroceryOrder")]
        public async void GeneretaGroceryOrder()
        {
            //GeneratePurchaseOrder(string FaceIDOfBuyer, string BuyList, double Lat, double Log)
            if (IsLocationAvailable())
            {
                try
                {
                    var MyLatLong = await Clases.Location.GetCurrentPosition();
                    if (MyLatLong == null)
                    {

                        Action action = () =>
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                            alert.SetTitle("Error");
                            alert.SetMessage("No Puedes pedir mandado si No Activas la ubicacion en tu telefono");

                           

                            alert.SetNegativeButton("Aceptar", (senderAlert, args) =>
                            {
                                //  count--;
                                //  button.Text = string.Format("{0} clicks!", count);
                            });

                            Dialog dialog = alert.Create();
                            dialog.Show();

                            //CurrentDialogReference = dialog;
                        };
                        ((Activity)mContext).RunOnUiThread(action);

                        //Sin Log En la base de datos
                    }
                    else { 
                    //ShopItem
                    List<ShopItem> ListaDEItems = new List<ShopItem>();
                    var cost = 0.0;
                    foreach (var element in ListaDeProductos)
                    {
                        var nuevoElemento = new ShopItem();
                        nuevoElemento.ItemID = element.ID;
                        nuevoElemento.Quantity = element.Cantidad;
                        ListaDEItems.Add(nuevoElemento);
                        cost += element.Costo * element.Cantidad;

                    }
                    cost += 25;
                    var ProductoHAsh = Base64Encode(JsonConvert.SerializeObject(ListaDEItems));
                        var query = new Log_info();
                        var Region = "";
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        try
                        {
                            var db5 = new SQLiteConnection(databasePath5);
                             query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();
                             Region = (query.Region_Delivery == null ? 1.ToString() : (query.Region_Delivery).ToString());
                        }
                        catch(Exception)
                        {

                        }
                    if (query.ProfileId != null)
                    {
                            Action action = async () =>
                            {
                                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                alert.SetTitle("seleccion");
                                alert.SetMessage("Selecciona un metodo de pago");

                                alert.SetPositiveButton("Pago en efectivo", async (senderAlert, args) =>
                                {

                                    HttpClient client = new HttpClient();

                                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GeneratePurchaseOrder?" +
                                        "FaceIDOfBuyer=" + query.ProfileId
                                        + "&BuyList=" + ProductoHAsh
                                        + "&Lat=" + MyLatLong.Latitude.ToString().Replace(",", ".")
                                        + "&Log=" + MyLatLong.Longitude.ToString().Replace(",", ".")
                                        + "&Region=" + Region
                                        + "&tipoDePago=" + ((int)(TipoDePago.Efectivo))
                                        ));
                                    HttpResponseMessage response;

                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                    response = await client.GetAsync(uri);

                                    switch (response.StatusCode)
                                    {
                                        case System.Net.HttpStatusCode.Gone:
                                            {
                                                Action action = () =>
                                                {
                                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                                    alert.SetTitle("Error");
                                                    alert.SetMessage("No Hay repartidores en tu area");

                                                    alert.SetPositiveButton("Aceptar", (senderAlert, args) =>
                                                    {

                                                    });



                                                    Dialog dialog = alert.Create();
                                                    dialog.Show();

                                                    //CurrentDialogReference = dialog;
                                                };
                                                ((Activity)mContext).RunOnUiThread(action);

                                                //Sin Log En la base de datos
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.Moved:
                                            {
                                                Action action = () =>
                                                {
                                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                                    alert.SetTitle("Error");
                                                    alert.SetMessage("El repartidor tiene muchas ordenes pendientes, intenta en unos minutos");

                                                    alert.SetPositiveButton("Aceptar", (senderAlert, args) =>
                                                    {

                                                    });



                                                    Dialog dialog = alert.Create();
                                                    dialog.Show();

                                                    //CurrentDialogReference = dialog;
                                                };
                                                ((Activity)mContext).RunOnUiThread(action);

                                                //Sin Log En la base de datos
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.Unauthorized:
                                            {
                                                Action action = () =>
                                                {
                                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                                    alert.SetTitle("Error");
                                                    alert.SetMessage("No Puedes pedir mandado sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

                                                    alert.SetPositiveButton("Loguear", (senderAlert, args) =>
                                                    {
                                                        MainActivity.LoadFragment_Static(Resource.Id.nav_LoginButton);
                                                    });

                                                    alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                                    {
                                                        //  count--;
                                                        //  button.Text = string.Format("{0} clicks!", count);
                                                    });

                                                    Dialog dialog = alert.Create();
                                                    dialog.Show();

                                                    //CurrentDialogReference = dialog;
                                                };
                                                ((Activity)mContext).RunOnUiThread(action);

                                                //Sin Log En la base de datos
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.Forbidden:
                                            {
                                                //Sin Metodo de pago
                                                Action action = () =>
                                                {
                                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                                    alert.SetTitle("Error");
                                                    alert.SetMessage("No Puedes pedir mandado sin un metodo de pago, deseas añadirlo ahora?");

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
                                                        sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiAddCard/Index?Amount=" + 1000 + "&User=" + query.ProfileId + "&ServiceArea=" + 1 + "&LatitudObjectivo=" + MyLatLong.Latitude.ToString().Replace(",", ".") + "&LongitudObjetivo=" + MyLatLong.Longitude.ToString().Replace(",", ".") + "&NombreDestino=" + "Noname" + "&Gender=" + (int)Gender.Male + "&LatitudOrigen=" + MyLatLong.Latitude.ToString().Replace(",", ".") + "&LongitudOrigen=" + MyLatLong.Longitude.ToString().Replace(",", "."));

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

                                                    //CurrentDialogReference = dialog;
                                                };
                                                ((Activity)mContext).RunOnUiThread(action);
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.OK:
                                            {
                                                MainActivity.LoadFragment_Static(Resource.Id.menu_video);
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
                                            break;
                                    }
                                });

                                alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
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
                    else
                    {
                            Action action = () =>
                            {
                                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                alert.SetTitle("Error");
                                alert.SetMessage("No Puedes pedir mandado sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

                                alert.SetPositiveButton("Loguear", (senderAlert, args) =>
                                {
                                    MainActivity.LoadFragment_Static(Resource.Id.nav_LoginButton);
                                });

                                alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
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
                }

                }
                catch (SQLiteException ex)
                {
                    Action action = () =>
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                        alert.SetTitle("Error");
                        alert.SetMessage("No Puedes pedir mandado sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

                        alert.SetPositiveButton("Loguear", (senderAlert, args) =>
                        {
                            MainActivity.LoadFragment_Static(Resource.Id.nav_LoginButton);
                        });

                        alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                        {
                            //  count--;
                            //  button.Text = string.Format("{0} clicks!", count);
                        });

                        Dialog dialog = alert.Create();
                        dialog.Show();

                        //CurrentDialogReference = dialog;
                    };
                    ((Activity)mContext).RunOnUiThread(action);

                    //logForFuckSake

                }
            }

            else
            {

                //Allow location
            }            


        }
        public class ShopItem
        {
            public int ItemID;
            public int Quantity;
        }

        public bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }






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

            MainActivity.mbottomSheetBehavior.PeekHeight = 0;

            MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

        }


        [JavascriptInterface]
        [Export("UpdateConectedIDStripe")]
        public async void UpdateConectedIDStripe(string ConnectedAccoundID)
        {
            Action action = async () =>
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                HttpClient client = new HttpClient();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/UpdateStripeTutorID?" +
                    "Tutori_participantToUpdate=" + query.ProfileId
                    + "&StripeComercianteID=" + ConnectedAccoundID
                    ));
                HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(uri);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    MainActivity.LoadFragmentStatic(Resource.Id.nav_messages);


                    //MainActivity.Activity_1.LoadFragment(Resource.Id.menu_audio);
                }
            };
            ((Activity)mContext).RunOnUiThread(action);


        }


    }

}
