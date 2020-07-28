using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;
using UIKit;
using WebKit;

namespace Carppi_Cliente.MainViewJavasciptInterface
{
    public class DetailedViewavascriptInterface
    {
        protected WKWebView webi;
        protected UIView Views;
        CancellationTokenSource taskController = new CancellationTokenSource();
        public DetailedProductViewFromRestauran Details;
        public static long TimeToWait = 35000;
        public static bool KeepQuering = true;
        protected long RestID;
        public DetailedViewavascriptInterface(UIView view, WKWebView web, long ID)
        {
            webi = web;
            Views = view;
            RestID = ID;
            //Publiccontrol  UIView
            //webi  WKWebView
        }
        public void StartUP()
        {
            CancellationToken token = taskController.Token;
            var GlobalQueryTask = Task.Run(() => BootUP(), token);
        }
        protected async Task BootUP()
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

            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantDetailedViewWithCategories?" +
                "RestaurantDetailID_ForCategories=" + RestID


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
                        if ((ahora - ahorainicio) < MAinViewJavascriptHandler.TimeToWait)
                        {
                            if (MAinViewJavascriptHandler.KeepQuering)
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

                    var script = "HideLoadingBar(" + ")";
                    // var script = "PrintTagResponse(" + S_Response.Response + ")";

                    webi.EvaluateJavaScript(script, null);
                    /*
                                        Action action = () =>
                                        {
                                            //var jsr = new JavascriptResult();
                                            var script = "HideLoadingBar(" + ")";
                                            web_ViewLocal.EvaluateJavascript(script, null);


                                        };


                                        web_ViewLocal.Post(action);
                                        */

                }

                /*
                aTimer = new System.Timers.Timer(1500);
               
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
                */

            }


        }
        public void SetTagsInsideRestaurant()
        {
            try
            {
                var script = "UpdateProductSelector(" + Details.FoodCategories + ")";
                webi.EvaluateJavaScript(script, null);
                // webi.EvaluateJavaScript(script, null);

                /*Action action = () =>
                {
                    //var jsr = new JavascriptResult();
                    var script = "UpdateProductSelector(" + Details.FoodCategories + ")";
                    web_ViewLocal.EvaluateJavascript(script, null);


                };


                web_ViewLocal.Post(action);
                */
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
        public void QueryAllProducts(long Index, double inicio)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);
            var ahora = (DateTime.Now - dt1970).TotalMilliseconds;
            var tiepo = ahora - inicio;
            if (KeepQuering && ((ahora - inicio) < MAinViewJavascriptHandler.TimeToWait))
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
                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiProductDetailedView_Compresed?" +
                    "ProductDetailID_CompressedData=" + Index


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                ahora = (DateTime.Now - dt1970).TotalMilliseconds;
                if (KeepQuering && ((ahora - inicio) < MAinViewJavascriptHandler.TimeToWait))
                {
                    var S_Response = t.Result;
                    var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);
                    Respuesta.Foto = Decompress(Respuesta.Foto);
                    var script = "UpdateProductGrid(" + JsonConvert.SerializeObject(Respuesta) + ")";

                    MainView_JavascriptInterface.StaticWebi.EvaluateJavaScript(script, null);
                    //webi.EvaluateJavaScript(script, null);
                    /*
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
                    */
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
}
