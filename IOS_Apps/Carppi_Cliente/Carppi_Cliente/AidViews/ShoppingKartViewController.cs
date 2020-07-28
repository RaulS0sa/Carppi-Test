using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Carppi_Cliente.DatabaseTypes;
using Carppi_Cliente.MainViewJavasciptInterface;
using Foundation;
using SQLite;
using UIKit;
using WebKit;
using static Carppi_Cliente.MainViewJavasciptInterface.MAinViewJavascriptHandler;

namespace Carppi_Cliente.AidViews
{
    public partial class ShoppingKartViewController : UIViewController
    {
        public static string CarppiHash { get; set; } = "";
        public static WKWebView VistaShoppingKart { get; set; }
        public static UIViewController ShoppingKartViewControllerReference;
        public ShoppingKartViewController() : base("ShoppingKartViewController", null)
        {
        }

        public override async void ViewDidLoad()
        {
            //ShowIfAlreadyLogged
            //ShowIfNotLogged
            base.ViewDidLoad();
            try
            {

                ShoppingKartViewControllerReference = this;
                //0000000000
                //CalculateCostOfTrip(Int32 Region_costo, double LatitudPedido, double LongitudPedido)
                var MyLatLong = await Clases.Location.GetCurrentPosition();



                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)

                var query = new Log_info();
                var Region = "";
                var faceID = "";
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                try
                {
                    var db5 = new SQLiteConnection(databasePath5);
                    query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();
                    Region = (query.Region_Delivery == null ? 0.ToString() : (query.Region_Delivery).ToString());
                    faceID = (query.ProfileId == null ? null : (query.ProfileId).ToString());
                }
                catch (Exception)
                {

                }


                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiGroceryApi/CalculateCostOfTripWithRestaurant_AndLogDatat?" +
                 "RestaurantHash=" + ShoppingKartViewController.CarppiHash +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                 "&LatitudPedido=" + MyLatLong.Latitude.ToString().Replace(",", ".") +
                 "&LongitudPedido=" + MyLatLong.Longitude.ToString().Replace(",", ".") +
                 "&userTag_Log=" + faceID

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
                    var fileName1 = "Views/ShoppingKart2.html";
                    string localHtmlUrl1 = Path.Combine(NSBundle.MainBundle.BundlePath, fileName1);
                    using (StreamReader lectura = new StreamReader(localHtmlUrl1))
                    {
                        Title = "Dashboard";
                        View.BackgroundColor = UIColor.White;
                        string filecontent = lectura.ReadToEnd();
                        var config = new WKWebViewConfiguration();
                        WKWebView webView = new WKWebView(View.Frame, config);
                        VistaShoppingKart = webView;
                        var messageHandler = new MainView_JavascriptInterface(webView, View);
                        config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


                        webView.TranslatesAutoresizingMaskIntoConstraints = false;



                        View.AddSubview(webView);
                        webView.MultipleTouchEnabled = false;

                        webView.ScrollView.ScrollEnabled = true;
                        webView.ScrollView.Bounces = false;
                        webView.UserInteractionEnabled = true;
                        webView.SizeToFit();
                        webView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;



                        var ccc = Shoppingkart2List(MAinViewJavascriptHandler.ListaDeProductos);
                        //content = content.Replace("<!--ReplaceProductFromTheList-->", cadea);
                        filecontent = filecontent.Replace("<div id=\"OrderItems\"></div>", ccc);
                        filecontent = filecontent.Replace("0000000000", TransportCost.ToString());
                        filecontent = filecontent.Replace("CompleCostOFGroceryy", CompleteCostOFGrocery(MAinViewJavascriptHandler.ListaDeProductos));
                        //GroceryCostPlusFee
                        //No has Logueado
                        filecontent = filecontent.Replace("ShowIfAlreadyLogged", !(faceID == null || String.IsNullOrEmpty(faceID)) ? "block" : "none");
                        filecontent = filecontent.Replace("ShowIfNotLogged", !(faceID == null || String.IsNullOrEmpty(faceID)) ? "none" : "block");
                        filecontent = filecontent.Replace("No has Logueado", faceID == null || String.IsNullOrEmpty(faceID) ? "No has logueado, no podemos darte promiciones si no te conocemos" : "Gracias por usar Carppi :D");
                        filecontent = filecontent.Replace("1329", CompleteCostOFGroceryPlusFee(MAinViewJavascriptHandler.ListaDeProductos, (int)TransportCost));




                        webView.LoadHtmlString(filecontent, NSBundle.MainBundle.BundleUrl);




                    }
                }
            }
            catch (Exception)
            {

            }



              //  content = sr.ReadToEnd();
                // var cadea = GenerateRowsForCheckOutModal(FragmentRestaurantDetailedView.ListaDeProductos);
                
                /*  Action action_WhowAlert = () =>
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
                          //No has Logueado
                          content = content.Replace("No has Logueado", faceID == null ? "No has logueado, no podemos darte promiciones si no te conocemos" : "Gracias por usar Carppi :D");
                          content = content.Replace("1329", CompleteCostOFGroceryPlusFee(FragmentRestaurantDetailedView.ListaDeProductos, (int)TransportCost));
                          sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                      }
                      sss.SetWebViewClient(new FragmentMain.LocalWebViewClient());




                  };
                  */




            



            //-------------------------
           
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
                        CharArrr += "<h6 class='mb-1 text-black'>" + element.Producto + "</h6>";
                        CharArrr += "<p class='text-black'>" + element.Descripcion + "</p>";
                        CharArrr += "<p class='text-black'>$" + element.Costo + "</p>";
                        CharArrr += "<p class='mb-0 text-black font-weight-bold'>";
                        CharArrr += "<div class='btn btn-sm btn-success mr-2' style='width:100%'>Cantidad: </div>";
                        CharArrr += "<div class='btn btn-sm btn-success mr-2' style='width:100%;display:inline-block'><div><button onclick='IncrementByID(" + element.ID + ",-1, " + element.Costo + " );' class='btn btn-sm btn-success' style='width:29%'>-</button><button id='Counter_" + element.ID + "' class='btn btn-sm btn-success' style='width:29%' disabled />" + element.Cantidad + "</button> <button onclick='IncrementByID(" + element.ID + ",1, " + element.Costo + " );' class='btn btn-sm btn-success' style='width:29%'>+</button></div></div>";
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

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

