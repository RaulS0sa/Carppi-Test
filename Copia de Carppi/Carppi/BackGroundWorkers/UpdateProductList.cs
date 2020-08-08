using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Carppi.DatabaseTypes;
using Carppi.Fragments;
using Newtonsoft.Json;
using SQLite;

namespace Carppi.BackGroundWorkers
{
    public class UpdateProductList
    {
        public long P_Index;
        public UpdateProductList( long RestaurantIndex)
        {
            P_Index = RestaurantIndex;

            BeginingTask();


        }

        public async Task BeginingTask()
        {
            /*
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                
                //  Console.WriteLine("Hello, world");
                await DownloadREstaurantIndexes();
            }).Start();
            */
            await Task.Run(() => DownloadREstaurantIndexes());
        }

        public async Task DownloadREstaurantIndexes()
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

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantDetailedViewWithCategories?" +
                "RestaurantDetailID_ForCategories=" + P_Index


                ));


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var t = Task.Run(() => GetResponseFromURI(uri));
            t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
            {

                var Restaurante = JsonConvert.DeserializeObject<DetailedProductViewFromRestauran>(S_Response.Response);
                var Details = Restaurante;

               // SetTagsInsideRestaurant();
                if (Details.Products.Count() > 0)
                {
                    DateTime dt1970 = new DateTime(1970, 1, 1);
                    DateTime current = DateTime.Now;//DateTime.UtcNow for unix timestamp
                    TimeSpan span = current - dt1970;
                    //Console.WriteLine(span.TotalMilliseconds.ToString());
                    var ahorainicio = span.TotalMilliseconds;
                    foreach (var index in Details.Products)
                    {
                        await Task.Run(() => QueryAllProducts(index, ahorainicio));

                    }

                }
               
               
            }

        }
        public async Task QueryAllProducts(long Index, double inicio)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);
            var ahora = (DateTime.Now - dt1970).TotalMilliseconds;
            var tiepo = ahora - inicio;
            if (true)
            {


                var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Productos.db");
                var db10 = new SQLiteConnection(databasePath10);
                var Productquery = new Carppi_ProductosPorRestaurantes();
                try
                {
                    Productquery = db10.Table<DatabaseTypes.Carppi_ProductosPorRestaurantes>().Where(v => v.ID == Index).FirstOrDefault();
                }
                catch (Exception) { }
                if (Productquery == null)
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
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    //t.Wait();

                    var S_Response = t.Result;
                    var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);
                    try
                    {
                        var AddingProduct = db10.Insert(new DatabaseTypes.Carppi_ProductosPorRestaurantes()
                        {
                            ID = Respuesta.ID,
                            IDdRestaurante = Respuesta.IDdRestaurante,
                            Nombre = Respuesta.Nombre,
                            Descripcion = Respuesta.Descripcion,
                            Foto = Respuesta.Foto,
                            Costo = Respuesta.Costo,
                            Categoria = Respuesta.Categoria,
                            Disponibilidad = Respuesta.Disponibilidad,
                            ComprasDelProducto = Respuesta.ComprasDelProducto,
                            Delta = ""

                        });
                    }
                    catch (Exception) { }

                }
                else
                {
                    
                }

            }
          
        }
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream input = new MemoryStream(data);
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    dstream.CopyTo(output);
                }
                return output.ToArray();
            }

            catch (Exception)
            {
                return new byte[] { };

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
}
