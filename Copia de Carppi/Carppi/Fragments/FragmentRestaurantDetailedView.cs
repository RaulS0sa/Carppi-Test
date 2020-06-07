
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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
using Newtonsoft.Json;
using Plugin.Geolocator;
using SQLite;
using static Carppi.Fragments.LocalWebViewClient_RestaurantDetailedView;
using Fragment = Android.Support.V4.App.Fragment;


namespace Carppi.Fragments
{
    public class FragmentRestaurantDetailedView : Fragment
    {

        public static long RestID { get; set; } = 0;
        public static String CarppiHash { get; set; } = "";
        public static String ImagenStatica { get; set; } = "";
        public static String NombreStatico { get; set; } = "";
        public static bool? AbiertoEstatico { get; set; } = false;
        public static long TimeToWait = 7500;
        //NombreStatico = NombreDelRestaurante;
        //AbiertoEstatico = EstaAbierto;

        public static List<CarppiGroceryProductos> ListaDeProductos = new List<CarppiGroceryProductos>();
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);

            string content;
            AssetManager assets = this.Activity.Assets;

            var Template = "CarppiDeliveryRestaurantDetailedView.html";
            using (StreamReader sr = new StreamReader(assets.Open(Template)))
            {
                content = sr.ReadToEnd();
                content = content.Replace("---REstaurantName-----", NombreStatico);//Restaurante.RestaurantData.Nombre);

                content = content.Replace("mall-dedicated-banner-replace----", ImagenStatica);// "data:image/png;base64," + Restaurante.RestaurantData.Foto);

                content = content.Replace("---OpenTag---", (AbiertoEstatico == null || AbiertoEstatico == false) ? "Cerrado" : "Abierto");
                content = content.Replace("---OpenTagColor---", (AbiertoEstatico == null || AbiertoEstatico == false) ? "danger" : "success");


                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new UtilityJavascriptInterface_RestaurantDetailedView(this.Activity, webi);

                webi.AddJavascriptInterface(wew, "Android");

                webi.Settings.JavaScriptEnabled = true;

                webi.Settings.DomStorageEnabled = true;
                webi.Settings.LoadWithOverviewMode = true;
                webi.Settings.UseWideViewPort = true;
                webi.Settings.BuiltInZoomControls = true;
                webi.Settings.DisplayZoomControls = false;
                webi.Settings.SetSupportZoom(true);

                webi.Settings.JavaScriptEnabled = true;

                webi.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);


                webi.SetWebViewClient(new LocalWebViewClient_RestaurantDetailedView(webi, new CarppiGrocery_BuyOrders(), LocalWebViewClient_RestaurantDetailedView.stateOfRequest.ShowingRestaurants));

            }

                /*
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

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantDetailedViewWithCategories?" +
                    "RestaurantDetailID_ForCategories=" + RestID


                    ));


                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var t = Task.Run(() => GetResponseFromURI(uri));


                var S_Response = t.Result;
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var Restaurante = JsonConvert.DeserializeObject<DetailedProductViewFromRestauran>(S_Response.Response);

                    string content;
                    AssetManager assets = this.Activity.Assets;

                    var Template = "CarppiDeliveryRestaurantDetailedView.html";
                    using (StreamReader sr = new StreamReader(assets.Open(Template)))
                    {
                        content = sr.ReadToEnd();
                        content = content.Replace("---REstaurantName-----", NombreStatico);//Restaurante.RestaurantData.Nombre);

                        content = content.Replace("mall-dedicated-banner-replace----", ImagenStatica);// "data:image/png;base64," + Restaurante.RestaurantData.Foto);

                         content = content.Replace("---OpenTag---", (AbiertoEstatico == null || AbiertoEstatico == false) ? "Cerrado" : "Abierto");
                        content = content.Replace("---OpenTagColor---", (AbiertoEstatico == null || AbiertoEstatico == false) ? "danger" : "success");


                        var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                        var wew = new UtilityJavascriptInterface_RestaurantDetailedView(this.Activity, webi);

                        webi.AddJavascriptInterface(wew, "Android");

                        webi.Settings.JavaScriptEnabled = true;

                        webi.Settings.DomStorageEnabled = true;
                        webi.Settings.LoadWithOverviewMode = true;
                        webi.Settings.UseWideViewPort = true;
                        webi.Settings.BuiltInZoomControls = true;
                        webi.Settings.DisplayZoomControls = false;
                        webi.Settings.SetSupportZoom(true);

                        webi.Settings.JavaScriptEnabled = true;

                        webi.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);


                        webi.SetWebViewClient(new LocalWebViewClient_RestaurantDetailedView( webi, Restaurante, new CarppiGrocery_BuyOrders(), LocalWebViewClient_RestaurantDetailedView.stateOfRequest.ShowingRestaurants));




                    }



                }
                */


                return view1;
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

            public Resources Resources;

            public WebView WebView_;

            public FragmentRestaurantView_WebClient(Context contexto, Resources res, WebView webi)
            {
                mContext = contexto;
                Resources = res;
                WebView_ = webi;
            }
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                LoadAvailableProductsStartUp();



            }
            public void LoadAvailableProductsStartUp()
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
                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                    "RegionParaDeterminacionDeProductos=" +2


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();

                var S_Response = t.Result;
                if(S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
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



        public static FragmentRestaurantDetailedView NewInstance(long RestaurantID, string RestaurantHash, string ImagenBase64,string  NombreDelRestaurante,bool EstaAbierto)
        {
            RestID = RestaurantID;
            CarppiHash = RestaurantHash;
            ImagenStatica = ImagenBase64;
            NombreStatico = NombreDelRestaurante;
            AbiertoEstatico = EstaAbierto;
            var frag1 = new FragmentRestaurantDetailedView { Arguments = new Bundle() };
            return frag1;
        }
    }
    

    public class LocalWebViewClient_RestaurantDetailedView : WebViewClient
    {
        public WebView web_ViewLocal;
        public DetailedProductViewFromRestauran Details;
        public CarppiGrocery_BuyOrders Order;
        public enum stateOfRequest { ShowingRestaurants, ShowwingMap };
        public stateOfRequest OptionToShow;

        public static Task GlobalQueryTask;
        public LocalWebViewClient_RestaurantDetailedView(WebView wv,
             CarppiGrocery_BuyOrders carppiGrocery_BuyOrders, stateOfRequest req)
        {

            web_ViewLocal = wv;
            //Details = detailedProductViewFromRestauran;
            Order = carppiGrocery_BuyOrders;
            OptionToShow = req;
        }
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return false; // then it is not handled by default action

            // return base.ShouldOverrideUrlLoading(view, url);
        }
        private static System.Timers.Timer aTimer;
        public static bool KeepQuering = true;
        void StopTheQuery()
        {
            KeepQuering = false;
        }
       
        public async Task BootUP()
        {
            KeepQuering = true;
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

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantDetailedViewWithCategories?" +
                "RestaurantDetailID_ForCategories=" + FragmentRestaurantDetailedView.RestID


                ));


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            

            var t = Task.Run(() => GetResponseFromURI(uri));
            t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
            {
                
                var Restaurante = JsonConvert.DeserializeObject<DetailedProductViewFromRestauran>(S_Response.Response);
                Details = Restaurante;

                SetTagsInsideRestaurant();
                if (Details.Products.Count() > 0)
                {
                    DateTime dt1970 = new DateTime(1970, 1, 1);
                    DateTime current = DateTime.Now;//DateTime.UtcNow for unix timestamp
                    TimeSpan span = current - dt1970;
                    //Console.WriteLine(span.TotalMilliseconds.ToString());
                    var ahorainicio = span.TotalMilliseconds;
                    foreach (var index in Details.Products)
                    {
                        var ahora = (DateTime.Now - dt1970).TotalMilliseconds;
                        if ((ahora - ahorainicio) < FragmentRestaurantDetailedView.TimeToWait)
                        {
                            if (KeepQuering)
                            {
                                var t_k = Task.Run(() => QueryAllProducts(index, ahorainicio));
                                t_k.Wait();

                            }
                            else
                            {
                                taskController.Cancel();
                                break;
                            }
                        }
                        else
                        {
                            KeepQuering = false;
                            taskController.Cancel();
                            break;

                        }
                        // t.Wait();

                    }
                    
                }
                else
                {

                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "HideLoadingBar(" + ")";
                        web_ViewLocal.EvaluateJavascript(script, null);


                    };


                    web_ViewLocal.Post(action);

                }
                aTimer = new System.Timers.Timer(1500);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;

            }


        }
        CancellationTokenSource taskController = new CancellationTokenSource();
        public override async void OnPageFinished(WebView view, string url)
        {
            
            CancellationToken token = taskController.Token;
            base.OnPageFinished(view, url);
            //GlobalQueryTask = BootUP;
            GlobalQueryTask = Task.Run(() => BootUP(), token);
            // WebInterfaceMenuCarppi.CenterThemap();

        }
        public void SetTagsInsideRestaurant()
        {
            Action action = () =>
            {
                //var jsr = new JavascriptResult();
                var script = "UpdateProductSelector(" + Details.FoodCategories + ")";
                web_ViewLocal.EvaluateJavascript(script, null);


            };


            web_ViewLocal.Post(action);
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //FragmentRestaurantDetailedView.ListaDeProductos
            //ChangeButtonTag
            //Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
            //                  e.SignalTime);
            List<int> vs = new List<int>();
            foreach(var product in FragmentRestaurantDetailedView.ListaDeProductos)
            {
                vs.Add(product.ID);
            }

           
                Action action = () =>
                {
                    //var jsr = new JavascriptResult();
                    var script = "ChangeButtonTag(" + JsonConvert.SerializeObject(vs) + ")";
                    web_ViewLocal.EvaluateJavascript(script, null);


                };


                web_ViewLocal.Post(action);



          
        }

        public void QueryAllProducts(long Index, double inicio)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);
            var ahora = (DateTime.Now - dt1970).TotalMilliseconds;
            var tiepo = ahora - inicio;
            if (KeepQuering && ((ahora - inicio  )< FragmentRestaurantDetailedView.TimeToWait))
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
                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiProductDetailedView_Compresed?" +
                    "ProductDetailID_CompressedData=" + Index


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                ahora = (DateTime.Now - dt1970).TotalMilliseconds;
                if (KeepQuering && ((ahora - inicio) < FragmentRestaurantDetailedView.TimeToWait))
                {
                    var S_Response = t.Result;

                    Action action = () =>
                    {
                    //var jsr = new JavascriptResult();
                    var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);
                    // var asas = new SevenZip.Compression.LZMA.Decoder();

                    Respuesta.Foto = Decompress(Respuesta.Foto);
                        var script = "UpdateProductGrid(" + JsonConvert.SerializeObject(Respuesta) + ")";
                        web_ViewLocal.EvaluateJavascript(script, null);


                    };


                    web_ViewLocal.Post(action);
                }
            }
            else
                {
                taskController.Cancel();
                KeepQuering = false;

            }
        }
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
        public partial class Carppi_ProductosPorRestaurantes
        {
            public long ID { get; set; }
            public string IDdRestaurante { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public byte[] Foto { get; set; }
            public double? Costo { get; set; }
            public long? Categoria { get; set; }
            public bool Disponibilidad { get; set; }
            public long ComprasDelProducto { get; set; }
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
    public class UtilityJavascriptInterface_RestaurantDetailedView : Java.Lang.Object
    {
        Context mContext;
        WebView webi;
        public static WebView webi_static;
        public static Context StaticContext;
        //public static enumEstado_del_usuario EstadoPrevioDelUsuario = enumEstado_del_usuario.Sin_actividad;
        public static double Costo_Global;
        
        //public static SearchLocationObject Static_WhereToGo;
        public UtilityJavascriptInterface_RestaurantDetailedView(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            webi_static = web;
            StaticContext = Act;
        }
        //window.Android.SearchByItsTagInDetailedView(IntergerToquery);
        [JavascriptInterface]
        [Export("SearchByItsTagInDetailedView")]
        public async void SearchByItsTagInDetailedView(int IntergerToquery)
        {
            try
            {
                LocalWebViewClient_RestaurantDetailedView.KeepQuering = false;
               // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask.Dispose();
               // LocalWebViewClient_RestaurantDetailedView.taskController.Cancel();
               // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask = null;
               //  GlobalQueryTask
            }
            catch(Exception)
            { }
            HttpClient client = new HttpClient();

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchInsideRestaurantProducts?" +
                "RestaurantDetailID_Regularseller=" + FragmentRestaurantDetailedView.RestID + 
                "&Bit=" + IntergerToquery


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
               
                CancellationTokenSource taskController = new CancellationTokenSource();
                CancellationToken token = taskController.Token;

               // LocalWebViewClient_RestaurantDetailedView.KeepQuering = true;

                  var Lista = JsonConvert.DeserializeObject<List<long>>(errorMessage1);
                foreach (var index in Lista)
                {
                    var t = Task.Run(() => QueryProductByProduct(index), token);
                 
                   // var t = Task.Run(() => QueryProductByProduct(index), token);
                   // t.Wait();

                }
            }
        }


        //SearchMostWanted
        [JavascriptInterface]
        [Export("SearchMostWanted")]
        public async void SearchMostWanted()
        {
            try
            {
               LocalWebViewClient_RestaurantDetailedView.KeepQuering = false;
                // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask.Dispose();
                //  LocalWebViewClient_RestaurantDetailedView.taskController.Cancel();
                // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask = null;
                //  GlobalQueryTask
            }
            catch (Exception)
            { }

            HttpClient client = new HttpClient();
           
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/BestSellersRestaurant?" +
                "RestaurantDetailID_Bestseller=" + FragmentRestaurantDetailedView.RestID 
              

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
               
               // LocalWebViewClient_RestaurantDetailedView.KeepQuering = true;
                CancellationTokenSource taskController = new CancellationTokenSource();
                CancellationToken token = taskController.Token;
                var Lista = JsonConvert.DeserializeObject<List<long>>(errorMessage1);
                foreach (var index in Lista)
                {
                    var t = Task.Run(() => QueryProductByProduct(index), token);
                  
                   
                  //  t.Wait();

                }
            }
        }



        public void QueryProductByProduct(long Index)
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
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiProductDetailedView_Compresed?" +
                "ProductDetailID_CompressedData=" + Index


                ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
            //t.Wait();

            var S_Response = t.Result;
            //EraseContentGrid
            Action action = () =>
            {
                //var jsr = new JavascriptResult();
                var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);
                // var asas = new SevenZip.Compression.LZMA.Decoder();

                Respuesta.Foto = Decompress(Respuesta.Foto);
                var script = "UpdateProductGrid(" + JsonConvert.SerializeObject(Respuesta) + ")";
                webi.EvaluateJavascript(script, null);


            };


            webi.Post(action);
        }


        //RateFoodDeliverMan

        [JavascriptInterface]
        [Export("RateFoodDeliverMan")]
        public async void RateFoodDeliverMan(int Rating, string Comentario)
        {
            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/RateDeliiverMan?" +
                "Order=" + FragmentSelectTypeOfPurchase.OrderIDIfActive +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&Rating=" + Rating +
                "&Coment=" + Comentario

                ));
            HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            response = await client.GetAsync(uri);


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Android.Support.V4.App.Fragment fragment = null;
                fragment = FragmentSelectTypeOfPurchase.NewInstance();
                MainActivity.static_FragmentMAnager.BeginTransaction()
                   .Replace(Resource.Id.content_frame, fragment)
                   .Commit();
               // MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

                /*
                var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
          {
                '"'
          });
                */
            }

        }
        [JavascriptInterface]
        [Export("SendMessageDeliverMan_Client")]
        public async void SendMessageDeliverMan_Client(string message)
        {

            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/PostMessageInRideShare?" +
                "FaceID_Interlocutor=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&TripRequest=" + FragmentSelectTypeOfPurchase.OrderIDIfActive +
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
        [Export("GetAllMessagesFromConversationDeliverManToClient")]
        public async void GetAllMessagesFromConversationDeliverManToClient()
        {

            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetAllMessagesFromTheConversation?" +
                "FaceID_Interest=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&TripRequest=" + FragmentSelectTypeOfPurchase.OrderIDIfActive

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



        public enum TipoDePago { Efectivo, Tarjeta };
        [JavascriptInterface]
        [Export("SendMessageRestaurantToclient")]
        public async void SendMessageRestaurantToclient(string message)
        {
            /* public class FragmentSelectTypeOfPurchase : Fragment
    {
        public static long OrderIDIfActive = 0;*/
            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var query = new Log_info();
            var Region = "";
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            try
            {
                var db5 = new SQLiteConnection(databasePath5);
                query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();
                Region = (query.Region_Delivery == null ? 2.ToString() : (query.Region_Delivery).ToString());
            }
            catch (Exception)
            {

            }
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/PostMessageInRestauranClient?" +
                "FaceID_speaker=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&shopRequest=" + FragmentSelectTypeOfPurchase.OrderIDIfActive +
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
        [Export("GetAllMessagesRestaurantClient")]
        public async void GetAllMessagesRestaurantClient()
        {

            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            //var ProductoHAsh = Base64Encode(JsonConvert.SerializeObject(ListaDEItems));
            var query = new Log_info();
            var Region = "";
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            try
            {
                var db5 = new SQLiteConnection(databasePath5);
                query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();
                Region = (query.Region_Delivery == null ? 2.ToString() : (query.Region_Delivery).ToString());
            }
            catch (Exception)
            {

            }
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/GetAlMessagesFromConversationRestauranClient?" +
                "FaceID_Interest=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&shopRequest=" + FragmentSelectTypeOfPurchase.OrderIDIfActive

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
        [JavascriptInterface]
        [Export("GeneretaGroceryOrder_WithComments")]
        public async void GeneretaGroceryOrder_WithComments(string Comentario)
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
                            alert.SetMessage("No Puedes pedir si No Activas la ubicacion en tu telefono");



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
                    else
                    {
                        //ShopItem
                        List<ShopItem> ListaDEItems = new List<ShopItem>();
                        var cost = 0.0;
                        foreach (var element in FragmentRestaurantDetailedView.ListaDeProductos)
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
                            Region = (query.Region_Delivery == null ? 2.ToString() : (query.Region_Delivery).ToString());
                        }
                        catch (Exception)
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

                                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/GeneratePurchaseOrder_Comentarios?" +
                                        "FaceIDOfBuyer=" + query.ProfileId
                                        + "&BuyList=" + ProductoHAsh
                                        + "&Lat=" + MyLatLong.Latitude.ToString().Replace(",", ".")
                                        + "&Log=" + MyLatLong.Longitude.ToString().Replace(",", ".")
                                        + "&Region=" + Region
                                        + "&tipoDePago=" + ((int)(TipoDePago.Efectivo))
                                        + "&Comentario=" + Comentario
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
                                                    alert.SetMessage("No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

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
                                                /*
                                                Action action = () =>
                                                {
                                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                                    alert.SetTitle("Error");
                                                    alert.SetMessage("No Puedes pedir mandado sin un metodo de pago, deseas añadirlo ahora?");

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
                                                        sss.AddJavascriptInterface(wew, "Android_BottomModal");
                                                        sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiAddCard/Index?Amount=" + 1000 + "&User=" + query.ProfileId + "&ServiceArea=" + 1 + "&LatitudObjectivo=" + MyLatLong.Latitude.ToString().Replace(",", ".") + "&LongitudObjetivo=" + MyLatLong.Longitude.ToString().Replace(",", ".") + "&NombreDestino=" + "Noname" + "&Gender=" + (int)Gender.Male + "&LatitudOrigen=" + MyLatLong.Latitude.ToString().Replace(",", ".") + "&LongitudOrigen=" + MyLatLong.Longitude.ToString().Replace(",", "."));

                                                        alert.SetView(sss);


                                                        alert.SetPositiveButton("Cerrar", (senderAlert, args) =>
                                                        {
                                                        });
                                                        Dialog dialog = alert.Create();
                                                        dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                                                        dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                                               
                                                    });

                                                    alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                                    {
                                                       
                                                    });

                                                    Dialog dialog = alert.Create();
                                                    dialog.Show();

                                                   
                                                };
                                                ((Activity)mContext).RunOnUiThread(action);
                                            }
                                            */
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.OK:
                                            {
                                                MainActivity.LoadFragment_Static(Resource.Id.menu_video);
                                                //fragment = FragmentSelectTypeOfPurchase.NewInstance();

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
                                                    //var Viewww = new UtilityJavascriptInterface(mContext, sss);
                                                    //sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                                                    using (StreamReader sr = new StreamReader(assets.Open("EmptyPage.html")))
                                                    {
                                                        content = sr.ReadToEnd();
                                                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                                                    }
                                                    //sss.SetWebViewClient(new LocalWebViewClient());


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
                                alert.SetMessage("No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

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
                        alert.SetMessage("No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

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
                            alert.SetMessage("No Puedes pedir si No Activas la ubicacion en tu telefono");



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
                    else
                    {
                        //ShopItem
                        List<ShopItem> ListaDEItems = new List<ShopItem>();
                        var cost = 0.0;
                        foreach (var element in FragmentRestaurantDetailedView.ListaDeProductos)
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
                            Region = (query.Region_Delivery == null ? 2.ToString() : (query.Region_Delivery).ToString());
                        }
                        catch (Exception)
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

                                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/GeneratePurchaseOrder?" +
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
                                                    alert.SetMessage("No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

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
                                                /*
                                                Action action = () =>
                                                {
                                                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                                                    alert.SetTitle("Error");
                                                    alert.SetMessage("No Puedes pedir mandado sin un metodo de pago, deseas añadirlo ahora?");

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
                                                        sss.AddJavascriptInterface(wew, "Android_BottomModal");
                                                        sss.LoadUrl("https://geolocale.azurewebsites.net/CarppiAddCard/Index?Amount=" + 1000 + "&User=" + query.ProfileId + "&ServiceArea=" + 1 + "&LatitudObjectivo=" + MyLatLong.Latitude.ToString().Replace(",", ".") + "&LongitudObjetivo=" + MyLatLong.Longitude.ToString().Replace(",", ".") + "&NombreDestino=" + "Noname" + "&Gender=" + (int)Gender.Male + "&LatitudOrigen=" + MyLatLong.Latitude.ToString().Replace(",", ".") + "&LongitudOrigen=" + MyLatLong.Longitude.ToString().Replace(",", "."));

                                                        alert.SetView(sss);


                                                        alert.SetPositiveButton("Cerrar", (senderAlert, args) =>
                                                        {
                                                        });
                                                        Dialog dialog = alert.Create();
                                                        dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                                                        dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                                               
                                                    });

                                                    alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                                    {
                                                       
                                                    });

                                                    Dialog dialog = alert.Create();
                                                    dialog.Show();

                                                   
                                                };
                                                ((Activity)mContext).RunOnUiThread(action);
                                            }
                                            */
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.OK:
                                            {
                                                MainActivity.LoadFragment_Static(Resource.Id.menu_video);
                                                //fragment = FragmentSelectTypeOfPurchase.NewInstance();
                                               
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
                                                    //var Viewww = new UtilityJavascriptInterface(mContext, sss);
                                                    //sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                                                    using (StreamReader sr = new StreamReader(assets.Open("EmptyPage.html")))
                                                    {
                                                        content = sr.ReadToEnd();
                                                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                                                    }
                                                    //sss.SetWebViewClient(new LocalWebViewClient());


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
                                alert.SetMessage("No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

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
                        alert.SetMessage("No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar");

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


        //GetProductSelection
        //CenterThemap(lat, lng)
        //ReturnToRestaurantView

        [JavascriptInterface]
        [Export("GetProductSelection")]
        public async void GetProductSelection(long ID)
        {
            var sss = FragmentRestaurantDetailedView.ListaDeProductos.Where(X => X.ID == ID).FirstOrDefault();

            Action action = () =>
            {
                //var jsr = new JavascriptResult();
                var script = "ChangeButtonTag(" + ID + "," + (sss == null? 0 : 1) + ")";
                webi.EvaluateJavascript(script, null);


            };


            webi.Post(action);


        }

        [JavascriptInterface]
        [Export("ReturnToRestaurantView")]
        public async void ReturnToRestaurantView()
        {
            Android.Support.V4.App.Fragment fragment = null;
            FragmentRestaurantDetailedView.ListaDeProductos = new List<CarppiGroceryProductos>();
            fragment = FragmentSelectTypeOfPurchase.NewInstance();
            MainActivity.static_FragmentMAnager.BeginTransaction()
               .Replace(Resource.Id.content_frame, fragment)
               .Commit();

         
        }
        /*
        "{\"ID\":4,\"IDdRestaurante\":\"fda804663ad75d74d4745a57254e5b6f39146041662ab2d111226b151b69a2ee\",\"Nombre\":\"Portobello burger\",\"Descripcion\":\"Una hamburguesa con portobello \",\"Foto\"
             */

        class restaurantProducto
        {
            public int ID;
            public string IDdRestaurante;
            public string Nombre;
            public string Descripcion;
            public byte[] Foto;
            public double Costo;
            public int Cantidad;
            public int RegionID;

        }
        
        [JavascriptInterface]
        [Export("UpdateProductList")]
        public static void UpdateProductList(string ProductHash, int direction)
        {
            // var casde = Base64Decode(ProductHash);
            var Producto = JsonConvert.DeserializeObject<restaurantProducto>((ProductHash));
            var Cadena = new CarppiGroceryProductos();
            Cadena.Cantidad = Producto.Cantidad;
            Cadena.Foto = Producto.Foto;
            Cadena.Costo = Producto.Costo;
            Cadena.Producto = Producto.Nombre;
            Cadena.ID = Producto.ID;
            Cadena.RegionID = Producto.RegionID;
            Cadena.Descripcion = Producto.Descripcion;

            //var Cadena = JsonConvert.DeserializeObject<CarppiGroceryProductos>((ProductHash));

            var sss = FragmentRestaurantDetailedView.ListaDeProductos.Where(X => X.ID == Cadena.ID).FirstOrDefault();
            if (sss == null)
            {

                Cadena.Cantidad = 1;
                FragmentRestaurantDetailedView.ListaDeProductos.Add(Cadena);
            }
            else
            {
                if (sss.Cantidad == 1 && direction == -1)
                {
                    FragmentRestaurantDetailedView.ListaDeProductos.Remove(sss);
                }
                else
                {
                    sss.Cantidad = sss.Cantidad + direction;
                }
            }
        }


        [JavascriptInterface]
        [Export("UpdateProductQuantity")]
        public static void UpdateProductQuantity(int ProductHash, int quantity)
        {

            var micopia =new  List<CarppiGroceryProductos>(FragmentRestaurantDetailedView.ListaDeProductos);
            // var casde = Base64Decode(ProductHash);
            //var Cadena = JsonConvert.DeserializeObject<CarppiGroceryProductos>((ProductHash));
            var sss = micopia.Where(X => X.ID == ProductHash).FirstOrDefault();
            if (sss == null)
            {
                var producto = new CarppiGroceryProductos();
                producto.ID = ProductHash;
                producto.Cantidad = 1;
                micopia.Add(producto);
            }
            else
            {
                sss.Cantidad = quantity;
                if (quantity == 0)
                {
                    micopia.Remove(sss);
                }

            }
            FragmentRestaurantDetailedView.ListaDeProductos = micopia;
        }


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
            //CarppiHash
            /*
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/CalculateCostOfTrip?" +
                "Region_costo=" + Region +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&LatitudPedido=" + MyLatLong.Latitude +
                "&LongitudPedido=" + MyLatLong.Longitude

                ));
            */
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/CalculateCostOfTripWithRestaurant?" +
               "RestaurantHash=" + FragmentRestaurantDetailedView.CarppiHash +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
               "&LatitudPedido=" + MyLatLong.Latitude.ToString().Replace(",", ".") +
               "&LongitudPedido=" + MyLatLong.Longitude.ToString().Replace(",", ".")

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
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");

                var TransportCost = (int)Convert.ToDouble(errorMessage1, culture);
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
                    using (StreamReader sr = new StreamReader(assets.Open("ShoppingKart2.html")))
                    {
                        content = sr.ReadToEnd();
                        // var cadea = GenerateRowsForCheckOutModal(FragmentRestaurantDetailedView.ListaDeProductos);
                        var ccc = Shoppingkart2List(FragmentRestaurantDetailedView.ListaDeProductos);
                        //content = content.Replace("<!--ReplaceProductFromTheList-->", cadea);
                        content = content.Replace("<div id=\"OrderItems\"></div>", ccc);
                        content = content.Replace("0000000000", TransportCost.ToString());
                        content = content.Replace("CompleCostOFGroceryy", CompleteCostOFGrocery(FragmentRestaurantDetailedView.ListaDeProductos));
                        //GroceryCostPlusFee
                        content = content.Replace("1329", CompleteCostOFGroceryPlusFee(FragmentRestaurantDetailedView.ListaDeProductos, (int)TransportCost));
                        sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                    }
                    sss.SetWebViewClient(new FragmentMain.LocalWebViewClient());

                    //<span class="woocommerce-Price-currencySymbol">£</span>90.00</span>
                    //CompleCostOFGroceryy


                };

                ((Activity)StaticContext).RunOnUiThread(action_WhowAlert);

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;



            }


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
                var Viewww = new UtilityJavascriptInterface_RestaurantDetailedView((Activity)mContext, sss);
                sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
                using (StreamReader sr = new StreamReader(assets.Open("EmptyPage.html")))
                {
                    content = sr.ReadToEnd();
                    sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                }
                //sss.SetWebViewClient(new LocalWebViewClient());


            };

            ((Activity)mContext).RunOnUiThread(action_WhowAlert);

            MainActivity.mbottomSheetBehavior.PeekHeight = 0;

            MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

        }


        public static string Shoppingkart2List(List<CarppiGroceryProductos> carppiGroceryProductos)
        {
            try
            {
                var CharArrr = "";
                foreach (var element in carppiGroceryProductos)
                {
                    if (element.Foto != null)
                    {
                        CharArrr += "<div id='TagForElement_" + element.ID + "' class='row'>";
                        CharArrr += "<div class='col-md-6'>";
                        CharArrr += "<div class='bg-white card addresses-item mb-4 border border-success'>";
                        CharArrr += "<div class='gold-members p-4'>";
                        CharArrr += "<div class='media'>";
                        CharArrr += "<div class='mr-3'><i class='icofont-ui-home icofont-3x'></i></div>";
                        CharArrr += "<div class='media-body'>";
                        CharArrr += "<h6 class='mb-1 text-black'>"+ element.Producto + "</h6>";
                        CharArrr += "<p class='text-black'>" + element.Descripcion  + "</p>";
                        CharArrr += "<p class='text-black'>$" + element.Costo + "</p>";
                        CharArrr += "<p class='mb-0 text-black font-weight-bold'>";
                        CharArrr += "<div class='btn btn-sm btn-success mr-2' style='width:100%'>Cantidad: </div>";
                        CharArrr += "<div class='btn btn-sm btn-success mr-2' style='width:100%;display:inline-block'><div><button onclick='IncrementByID(" + element.ID + ",-1, " + element.Costo+ " );' class='btn btn-sm btn-success' style='width:29%'>-</button><button id='Counter_" + element.ID + "' class='btn btn-sm btn-success' style='width:29%' disabled />"+ element.Cantidad + "</button> <button onclick='IncrementByID(" + element.ID + ",1, " + element.Costo + " );' class='btn btn-sm btn-success' style='width:29%'>+</button></div></div>";
                        CharArrr += "<h6 style='width:100%;display:none' id='InvisiblePrice_" + element.ID + "' name='InvisiblePriceAdjustemt'>" + element.Costo + "</h6>";
                        CharArrr += "<hr />";
                        CharArrr += "<div class='btn btn-sm btn-danger mr-2' style='width:100%' onclick='RemoveFromList(" + element.ID + ")'> Retirar</div>";
                        //CharArrr += "<span>30MIN</span>";
                        CharArrr += "</p>";
                        CharArrr += "</div>";
                        CharArrr += "</div>";
                        CharArrr += "</div>";
                        CharArrr += "</div>";
                        CharArrr += "</div>";
                        CharArrr += "</div>";
                    }
                }
                return CharArrr;
            }
            catch (Exception)
            {
                return "";

            }
            /*
            <div class="col-md-6">
        <div class="bg-white card addresses-item mb-4 border border-success">
            <div class="gold-members p-4">
                <div class="media">
                    <div class="mr-3"><i class="icofont-ui-home icofont-3x"></i></div>
                    <div class="media-body">
                        <h6 class="mb-1 text-black">Home</h6>
                        <p class="text-black">
                            291/d/1, 291, Jawaddi Kalan, Ludhiana, Punjab 141002, India
                        </p>
                        <p class="mb-0 text-black font-weight-bold">
                            <a class="btn btn-sm btn-success mr-2" href="#"> DELIVER HERE</a>
                            <span>30MIN</span>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
             */
        }
        public static string CompleteCostOFGrocery(List<CarppiGroceryProductos> carppiGroceryProductos)
        {
            var Cost = 0.0;
            foreach (var element in carppiGroceryProductos)
            {
                Cost += element.Costo;

            }
            return "<span class='woocommerce-Price-currencySymbol'>$</span>" + Cost.ToString() + "</span>";
        }
        public static string CompleteCostOFGroceryPlusFee(List<CarppiGroceryProductos> carppiGroceryProductos, int CostOfTravel)
        {
            var Cost = 0.0;
            foreach (var element in carppiGroceryProductos)
            {
                Cost += (element.Costo * element.Cantidad);

            }
            return (Cost + CostOfTravel).ToString();
            //return "<span class='woocommerce-Price-currencySymbol'>$</span>" + (Cost + CostOfTravel).ToString() + "</span>";
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
                        CharArrr += "<input disabled type='number' onchange='UpdateProductTubTotal(" + element.ID + ", " + element.Costo + ")' id='ProductoListID_" + element.ID + "' class='input-text qty text' step='1' min='0' max='' name='cart[975bddd97c5167b253173d5ef97b7ec9][qty]' value='1' title='Qty' size='4' inputmode='numeric'>";
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

            }

            /*
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

    public class IndiceDeRestaurante
    {
        public int ID;
        public string IDRestaurante;
        public string Nombre;
        public string Descripcion;
        public string Foto;
        public bool? EstaAbierto;

    }
    public class DetailedProductViewFromRestauran
    {
        public IndiceDeRestaurante RestaurantData;
        public List<long> Products;
        public long FoodCategories;
    }

    public class CarppiGroceryProductos
    {
        public int ID;
        public int RegionID;
        public int Cantidad;
        public string Producto;
        public double Costo;
        public byte[] Foto;
        public string Descripcion;




    }
    public class CarppiGrocery_BuyOrders
    {
        public long ID { get; set; }
        public long? RegionID { get; set; }
        public string UserID { get; set; }
        public string paymentIntent { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public FragmentSelectTypeOfPurchase.GroceryOrderState Stat { get; set; }
        public string ListaDeProductos { get; set; }
        public double? Latitud_Repartidor { get; set; }
        public double? Longitud_Repartidor { get; set; }
        public double? FaceIDRepartidor_Repartidor { get; set; }
        public string FaceIDRepartidor_RepartidorCadena { get; set; }
    }


}
