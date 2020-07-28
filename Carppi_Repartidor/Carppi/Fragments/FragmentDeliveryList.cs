
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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
//using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Firebase.Iid;
using Java.Interop;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Essentials;
using Fragment = Android.Support.V4.App.Fragment;

namespace Carppi.Fragments
{
    public class FragmentDeliveryList : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static FragmentDeliveryList NewInstance()
        {
            var frag1 = new FragmentDeliveryList { Arguments = new Bundle() };
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var StringComplete = "";
            var DebtAmount = "";
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

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetListOfDeliveryBoyOrderAndStatus?" +
                    "FaceIDHash_DeliveryBoyAndStatus=" + FaceID


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var Status = JsonConvert.DeserializeObject<DeliverManStatus>(S_Ressult.Response);
                    DebtAmount = ((long)Status.Debt).ToString();
                    var OrderList = Status.OrderList;//JsonConvert.DeserializeObject<List<DeliveryOrderQuery>>(S_Ressult.Response);
                    StringComplete=  GenerateListofOrders(OrderList);
                    // SetDeliverManStatus(Status.DelivermanStatus, OrderList.Count(), view);
                    //var ListaCoordenadas = new List<CoordenadasDeOrdenes>();





                }
                else
                {

                }


            }

            catch (Exception Ex)
            {

            }

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
            var Template = "OrderList.html";
            //var Template = "ShoppingOptions.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            //Template = "ShoppingCatalog.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            using (StreamReader sr = new StreamReader(assets.Open(Template)))
            {
                content = sr.ReadToEnd();
                //<h6>deudaReplace</h6>
                content = content.Replace("<h6>deudaReplace</h6>", "<h6>La app te debe: $" +  DebtAmount + "</h6>");
                content =  content.Replace("PlainTextReplace", StringComplete);
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                //var wew = new WebInterfaceGroceryRequest(this.Activity, webi);
                //MainWebView = wew;
                // ;
                //webi.AddJavascriptInterface(wew, "Android");

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

                
               //webi.SetWebViewClient(new GroceryRequestWebClient(this.Activity, Resources));
                // WebInterfaceProfile.RetriveProfile();

                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView

            }

            return view1;
            // return base.OnCreateView(inflater, container, savedInstanceState);
        }
        class DeliverManStatus
        {
            public List<DeliveryOrderQuery> OrderList;
            public bool DelivermanStatus;
            public long Debt;
        }
        public string GenerateListofOrders(List<DeliveryOrderQuery> ar1)
        {
            var RegularStr = "";
            foreach(var orden in ar1)
            {
               
                var TempProductArray = "";
                foreach(var producto in orden.Productos)
                {
                    TempProductArray += "<div class='widget-subheading'> Producto:" + producto.Nombre + "</div>" ;
                    TempProductArray += "<div class='widget-subheading'> Cantidad:" + producto.Cantidad + "</div>";
                }
                var t = Task.Run(() => getDirection(orden.Orden.LatitudPeticion, orden.Orden.LongitudPeticion));
                t.Wait();
                var Direccion = t.Result;
                var aca =  "<div class='mb-3 card'>" +
                     "<div class='card-header-tab card-header'>" +
                         "<div class='card-header-title font-size-lg text-capitalize font-weight-normal'>" +
                             "<i class='header-icon lnr-charts icon-gradient bg-happy-green'>" + "</i>" +
                            "Orden: " + orden.Orden.ID + "\n" + 
                            " Restaurante: " + orden.Orden.NombreDelRestaurante +
                              "</div>" +
                             // "<div class='btn-actions-pane-right text-capitalize'>" +
                             //     "<button class='btn-wide btn-outline-2x mr-md-2 btn btn-outline-focus btn-sm'>" + "View All"+"</button>" +
                             //   "</div>" +
                            "</div>" +
                            "<div class='no-gutters row'>" +
                                "<div class='col-sm-6 col-md-4 col-xl-4'>" +
                                    "<div class='card no-shadow rm-border bg-transparent widget-chart text-left'>" +
                                        "<div class='icon-wrapper rounded-circle'>" +
                                            "<div class='icon-wrapper-bg opacity-10 bg-warning'>" + "</div>" +
                                           // "<i class='lnr-laptop-phone text-dark opacity-8'>" + "</i>" +
                                        "</div>" +
                                        "<div class='widget-chart-content'>" +
                                            "<div class='widget-subheading'>" + "Costo Producto:" +"</div>" +
                                              "<div class='widget-subheading'> $ " + orden.Orden.Precio+ "</div>" +
                                              "<div class='widget-subheading'>" + "Tarifa Envio:" + "</div>" +
                                              "<div class='widget-subheading'> $ " + orden.Orden.TarifaDelServicio + "</div>" +
                                              "<div class='widget-subheading'>" + "Envio:" + "</div>" +
                                              "<div class='widget-subheading pl-1' style='color:green;font-size: 21px;'> $ " + (orden.Orden.Precio + orden.Orden.TarifaDelServicio) + "</div>" +
                                              "<div class='widget-subheading'>" + "------------" + "</div>" +
                                              TempProductArray+
                                              "<div class='widget-subheading'>" + "------------" + "</div>" +
                                               
                                               Direccion+
                                      /*     "<div class='widget-description opacity-8 text-focus'>" +
                                               "<div class='d-inline text-danger pr-1'>" +
                                                   "<i class='fa fa-angle-down'>" + "</i>" +
                                                   "<span class='class='pl-1''>" + " 54.1 %"+ "</span>" +
                                               "</div>" +
                                               "less earnings"+
                                     "</div>" +*/
                                      "</div>" +
                                  "</div>" +
                                  "<div class='divider m-0 d-md-none d-sm-block'>" + "</div>" +
                              "</div>" +

                          "</div>" +
                          /*
                          "<div class='text-center d-block p-3 card-footer'>" +
                              "<button class='btn-pill btn-shadow btn-wide fsize-1 btn btn-primary btn-lg'>" +
                                  "<span class='mr-2 opacity-7'>" +
                                      "<i class='icon icon-anim-pulse ion-ios-analytics-outline'>" + "</i>" +
                                  "</span>" +//----------------------
                                  "<span class='mr-1'>" + "View Complete Report " + "</span>" +
                                "</button>" +
                            "</div>" +
                            */
                        "</div>";
                RegularStr += aca;
            }
            return RegularStr;


            /*
                       <div class="mb-3 card">
                          <div class="card-header-tab card-header">
                              <div class="card-header-title font-size-lg text-capitalize font-weight-normal">
                                  <i class="header-icon lnr-charts icon-gradient bg-happy-green"> </i>
                                  Portfolio Performance
                              </div>
                              <div class="btn-actions-pane-right text-capitalize">
                                  <button class="btn-wide btn-outline-2x mr-md-2 btn btn-outline-focus btn-sm">View All</button>
                              </div>
                          </div>
                          <div class="no-gutters row">
                              <div class="col-sm-6 col-md-4 col-xl-4">
                                  <div class="card no-shadow rm-border bg-transparent widget-chart text-left">
                                      <div class="icon-wrapper rounded-circle">
                                          <div class="icon-wrapper-bg opacity-10 bg-warning"></div>
                                          <i class="lnr-laptop-phone text-dark opacity-8"></i>
                                      </div>
                                      <div class="widget-chart-content">
                                          <div class="widget-subheading">Cash Deposits</div>
                                          <div class="widget-numbers">1,7M</div>
                                          <div class="widget-description opacity-8 text-focus">
                                              <div class="d-inline text-danger pr-1">
                                                  <i class="fa fa-angle-down"></i>
                                                  <span class="pl-1">54.1%</span>
                                              </div>
                                              less earnings
                                          </div>
                                      </div>
                                  </div>
                                  <div class="divider m-0 d-md-none d-sm-block"></div>
                              </div>

                          </div>
                          <div class="text-center d-block p-3 card-footer">
                              <button class="btn-pill btn-shadow btn-wide fsize-1 btn btn-primary btn-lg">
                                  <span class="mr-2 opacity-7">
                                      <i class="icon icon-anim-pulse ion-ios-analytics-outline"></i>
                                  </span>
                                  <span class="mr-1">View Complete Report</span>
                              </button>
                          </div>
                      </div>
                   */
        }
        public async Task<string> getDirection(double? lat, double? lon)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(Convert.ToDouble(lat), Convert.ToDouble(lon));

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var geocodeAddress =
                        $"AdminArea:       {placemark.AdminArea}\n" +
                        $"CountryCode:     {placemark.CountryCode}\n" +
                        $"CountryName:     {placemark.CountryName}\n" +
                        $"FeatureName:     {placemark.FeatureName}\n" +
                        $"Locality:        {placemark.Locality}\n" +
                        $"PostalCode:      {placemark.PostalCode}\n" +
                        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                        $"SubLocality:     {placemark.SubLocality}\n" +
                        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                        $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    //   Console.WriteLine(geocodeAddress);
                    return "<div class='widget-subheading'>" + "Direccion" + "</div>" + "<div class='widget-subheading'>" + placemark.SubLocality + ", " + placemark.SubThoroughfare + ", " + placemark.Thoroughfare + "</div>";
                }
                return "";
            }
            catch(Exception)
            {
                return "";
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
