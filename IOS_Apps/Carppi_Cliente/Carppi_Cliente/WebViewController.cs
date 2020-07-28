using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using Foundation;
using Newtonsoft.Json;
using SQLite;
using UIKit;
using WebKit;
using static Carppi_Cliente.MainViewJavasciptInterface.MAinViewJavascriptHandler;

namespace Carppi_Cliente
{
    public partial class WebViewController : UIViewController
    {
        public static UIView Publiccontrol;
        public static UIViewController GlobalViewController;

        public static System.Timers.Timer aTimer = null;
        public static WKWebView GlobalWebView;
        public static NSObject Invoker = new NSObject();
        public MainView_JavascriptInterface GlobalMessageHandler;

        public static MainView_JavascriptInterface StaticMessageHandler;
        public static MainViewJavasciptInterface.MAinViewJavascriptHandler GlobalInterfaceReference;
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }
        public WebViewController()
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected WebViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        public override void LoadView()
        {
            base.LoadView();
            //NSCoder nc = new NSCoder();
            //WebView = new WKWebView(null);
           // View.Add(WebView);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Clases.Location.StartListening();
            //  FragmentRestaurantDetailedView.ListaDeProductos = new List<CarppiGroceryProductos>();
            Publiccontrol = View;
            GlobalViewController = this;
           // PresentViewController(null, false, null);

            string content;
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
            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/UserUIStateDetermination?" +
              "UserChain=" + FaceID


              ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
            t.Wait();

            var S_Response = t.Result;
            WKWebViewConfiguration config = new WKWebViewConfiguration();
            WKWebView webView = new WKWebView(View.Frame, config);
            View.AddSubview(webView);
            GlobalWebView = webView;
            var messageHandler = new MainView_JavascriptInterface(webView, View);

            config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
            {

              
               
                string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                var text = File.ReadAllText(index);


                webView.LoadHtmlString(text, null);
                GlobalMessageHandler = messageHandler;
                StaticMessageHandler = messageHandler;
                //messageHandler.LoadAvailableProductsStartUp();


            }
            else if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
            {


                var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Response.Response);
                OrderIDIfActive = Order.ID;
                if (Order.Stat == GroceryOrderState.RequestEnded)
                {

                    string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/RateFoodDeliverMan.html");
                    var text = File.ReadAllText(index);


                    webView.LoadHtmlString(text, null);
                    //messageHandler.LoadAvailableProductsStartUp();

                    stateOfRequest = stateOfRequestEnum.ShowwingMap;
                    messageHandler.GetMapState();

                    
                }
                else
                {
                  
                    //var messageHandler = new MainView_JavascriptInterface(webView);
                   // config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


                    string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/FragmentGrocery_Map.html");
                    var text = File.ReadAllText(index);


                    webView.LoadHtmlString(text, null);
                    //messageHandler.LoadAvailableProductsStartUp();

                    stateOfRequest = stateOfRequestEnum.ShowwingMap;
                    messageHandler.GetMapState();

                    aTimer = new System.Timers.Timer(2000);
                    // Hook up the Elapsed event for the timer. 
                    aTimer.Elapsed += messageHandler.OnTimedEvent;
                    aTimer.AutoReset = true;
                    aTimer.Enabled = true;
                }
                /*
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
                
                */
            }


            //____________________________________
            // WebView.LoadHtmlString("<h4>Hol</h4>", null);
            //WebView.url
            //WebView = new WebKit.WKWebView();
            //var asd = new NSUrl("https://geolocale.azurewebsites.net");
            // WebView.LoadRequest(new NSUrlRequest(asd));
            // string url = "https://www.apple.com";
            // NSUrl nSUrl = new NSUrl(asd);
           
            
            //  webView.LoadData(text,"text/html", null,null);
            //  var asdddd =  WebView.LoadHtmlString(text,null);

            //WebView. += HandleShouldStartLoad;
            /*

            // Intercept URL loading to handle native calls from browser
            WebView.ShouldStartLoad += HandleShouldStartLoad;

            // Render the view from the type generated from RazorView.cshtml
            var model = new Model1 { Text = "Text goes here" };
            var template = new RazorView { Model = model };
            var page = template.GenerateString();

            // Load the rendered HTML into the view with a base URL 
            // that points to the root of the bundled Resources folder
            string index = Path.Combine(NSBundle.MainBundle.BundlePath, "Views/MainView.html");
            // string html = File.ReadAllText(index);
            var url = new NSUrl(index, false);
            var request = new NSUrlRequest(url);
            var text = File.ReadAllText("./Views/MainView.html");
            using (StreamReader sr = new StreamReader(index))
            {
               var  content = sr.ReadToEnd();
                var brea = 0;
            }
                //WebView.LoadRequest(html, NSBundle.MainBundle.BundleUrl);
                WebView.LoadRequest(request);



            // Perform any additional setup after loading the view, typically from a nib.
            */
        }
        public override void ViewDidAppear(bool animated)
        {
            //GlobalMessageHandler = messageHandler;
            // System.Threading.Tasks.Task.Delay(5 * 1000).ContinueWith((_) => {
            var t = Task.Run(() => GlobalMessageHandler.LoadAvailableProductsStartUp());
          ;
            base.ViewDidAppear(animated);
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        bool HandleShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            // If the URL is not our own custom scheme, just let the webView load the URL as usual
            const string scheme = "hybrid:";

            if (request.Url.Scheme != scheme.Replace(":", ""))
                return true;

            // This handler will treat everything between the protocol and "?"
            // as the method name.  The querystring has all of the parameters.
            var resources = request.Url.ResourceSpecifier.Split('?');
            var method = resources[0];
            var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);

            if (method == "UpdateLabel")
            {
                var textbox = parameters["textbox"];

                // Add some text to our string here so that we know something
                // happened on the native part of the round trip.
                var prepended = string.Format("C# says: {0}", textbox);

                // Build some javascript using the C#-modified result
                var js = string.Format("SetLabelText('{0}');", prepended);

                webView.EvaluateJavascript(js);
            }

            return false;
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

    public class MainView_JavascriptInterface : NSObject,IWKScriptMessageHandler
    {
        public WKWebView webi;
        public static WKWebView StaticWebi;
        public static MainViewJavasciptInterface.MAinViewJavascriptHandler MainViewJavasciptInterface_Reference;
        public MainViewJavasciptInterface.MAinViewJavascriptHandler Interface;
        // public IntPtr Handle => throw new NotImplementedException();
        protected UIView View2;
        public MainView_JavascriptInterface(WKWebView arg1, UIView arg2)
        {
            webi = arg1;
            StaticWebi = arg1;
            var Interface_var = new MainViewJavasciptInterface.MAinViewJavascriptHandler(WebViewController.Publiccontrol, webi);
            MainViewJavasciptInterface_Reference = Interface_var;
            Interface = Interface_var;
            WebViewController.GlobalInterfaceReference = Interface_var;
            View2 = arg2;
        }


        

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            var Methods = JsonConvert.DeserializeObject<JsonInterfaceDialog>(message.Body.ToString());
            //Publiccontrol  UIView
            //webi  WKWebView
            //RestaurantDetailedView
            //SearchFoodByBox
            //SearchByText
            //ShowExtraOptionsOnDeliverManAwait
            //ShowExtraOptionsOnGroceryAwait

            Console.WriteLine(Methods.Method);
            switch (Methods.Method)
            {
                case "RestaurantDetailedView":
                    Interface.RestaurantDetailedView(Convert.ToInt64(Methods.Argument1), Methods.Argument2, Methods.Argument3, Methods.Argument4, Convert.ToBoolean(Methods.Argument5));
                    break;
                case "SearchFoodByBox":
                    Interface.SearchByItsBox(Convert.ToInt64(Methods.Argument1));
                    break;
                case "SearchBackAllRestaurants":
                    Interface.SearchBackAllRestaurants();
                    break;
                case "SearchByText":
                    Interface.SearchByText(Methods.Argument1);
                    break;
                case "ReturnToRestaurantView":
                    Interface.ReturnToRestaurantView();
                    break;
                case "UpdateProductList":
                    Interface.UpdateProductList(Methods.Argument1, Convert.ToInt32(Methods.Argument2));
                    break;
                //DisplayShopingKart
                case "DisplayShopingKart":
                    Interface.DisplayShopingKart();
                    break;
                case "SearchByItsTagInDetailedView":
                    Interface.SearchByItsTagInDetailedView(Convert.ToInt32(Methods.Argument1));
                    break;//SearchMostWanted
                case "SearchMostWanted":
                    Interface.SearchMostWanted();
                    break;
                case "DissmissBottomModal":
                    Interface.DissmissBottomModal();
                    break;
                case "UpdateProductQuantity":
                    Interface.UpdateProductQuantity(Convert.ToInt32(Methods.Argument1), Convert.ToInt32(Methods.Argument2));
                    break;
                case "SignIN":
                    Interface.UpdateShoppingKartView(Methods.Argument1, Methods.Argument2);
                    //var init = 0;
                    break;
                case "ShowExtraOptionsOnGroceryAwait":
                    Interface.ShowExtraOptionsOnGroceryAwait();
                    break;
                case "ShowOptionInOrderCreated":
                    Interface.ShowOptionInOrderCreated();
                    break;
                case "SendMessageRestaurantToclient":
                    Interface.SendMessageRestaurantToclient(Methods.Argument1);
                    break;

                case "GetAllMessagesRestaurantClient":
                    Interface.GetAllMessagesRestaurantClient();
                    break;
                case "ShowExtraOptionsOnDeliverManAwait":
                    Interface.ShowExtraOptionsOnDeliverManAwait();
                    break;
                case "GetAllMessagesFromConversationDeliverManToClient":
                    Interface.GetAllMessagesFromConversationDeliverManToClient();
                    break;
                case "SendMessageDeliverMan_Client":
                    Interface.SendMessageDeliverMan_Client(Methods.Argument1);
                    break;
                case "RateFoodDeliverMan":
                    Interface.RateFoodDeliverMan(Convert.ToInt32(Methods.Argument1), Methods.Argument2);
                    break;
                case "RequestFood":
                    Interface.GenerateFoodOrder((Methods.Argument1));
                    break;



            }



            //  var controllerTwo = new AidViews.ShoppingKartViewController();

            //  WebViewController.GlobalViewController.PresentViewController(controllerTwo, true, null);

        }


        public static NSObject Invoker2 = new NSObject();
        public async void LoadAvailableProductsStartUp()
        {
            Invoker2.BeginInvokeOnMainThread(async () =>
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
                        FaceID = query.ProfileId == null ? "ppp" : query.ProfileId;
                    }
                    catch (Exception ex)
                    {
                        FaceID = "ppp";
                    }
                    // SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
                    /*
                   var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiIOSRestaurantApi/CarppiRestaurantExistanceDeterminationTest?" +
                        "CadenadelUsuarioRestaurant_TOTEst=" + FaceID


                        ));

                       var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                        "CadenadelUsuarioRestaurant=" + FaceID


                        ));
                    
                    */

                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                         "CadenadelUsuarioRestaurant=" + FaceID


                         ));

                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    //var t = Task.Run(() => GetResponseFromURI(uri));
                    //t.Wait();
                    try
                    {
                        var aca = await GetResponseFromURI(uri);

                        var S_Response = aca;//.Result;
                        if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var script = "SetStartupMenu(" + S_Response.Response + ")";

                            StaticWebi.EvaluateJavaScript(script, (s, e) => {
                                if (e != null)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            });
                        }
                        else if(S_Response.Response == null)
                        {
                            try
                            {
                                await System.Threading.Tasks.Task.Delay(5 * 1000).ContinueWith(async (_) =>
                                {
                                      WebViewController.StaticMessageHandler.LoadAvailableProductsStartUp();
                                });
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                catch (Exception ex)
                {

                }
            });
            
        }

        public void GetMapState()
        {
            try
            {
                if (stateOfRequest == stateOfRequestEnum.ShowwingMap)
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
                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
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
                        var script = "ShowStatusOfGroceryOrder(" + ((int)Order.Stat) + "," + S_Response.Response + ")";
                        try
                        {
                            webi.EvaluateJavaScript(script, (result, error) =>
                            {
                                if (error != null) Console.WriteLine(error);
                            });
                        }
                        catch (Exception ex)
                        {
                        }
                        /*
                        simpleTestCall
                        StaticWebi.EvaluateJavaScript(script, (result, error) =>
                        {
                            if (error != null) Console.WriteLine(error);
                        });
                        */
                        /*
                        Action action2 = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "ShowStatusOfGroceryOrder(" + ((int)Order.Stat) + "," + S_Response.Response + ")";
                            webi_Static.EvaluateJavaScript(script, null);

                           // StaticWebView_.EvaluateJavascript(script, null);


                        };

                      
                        StaticWebView_.Post(action2);
                        
                          */
                        if (Order.Stat == GroceryOrderState.RequestEnded) 
                        {
                            WebViewController.aTimer.Enabled = false;
                            WebViewController.aTimer.AutoReset = false;
                            stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            WebViewController.aTimer.Stop();
                            WebViewController.aTimer = null;
                            // ShowRateDeliverManRestaurant((Activity)Static_mContext);


                            string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/RateFoodDeliverMan.html");
                            var text = File.ReadAllText(index);
                            webi.LoadHtmlString(text, null);


                            WKWebViewConfiguration config = new WKWebViewConfiguration();
                            WKWebView webView = new WKWebView(View2.Frame, config);
                            View2.AddSubview(webView);


                            webView.LoadHtmlString(text, null);
                            //messageHandler.LoadAvailableProductsStartUp();

                            stateOfRequest = stateOfRequestEnum.ShowwingMap;
                            try
                            {
                                ShoppingKartModal.DismissViewController(true, null);
                            }
                            catch(Exception)
                            { }
                           // messageHandler.GetMapState();

                        }
                        else if(Order.Stat == GroceryOrderState.RequestRejected)
                        {
                          
                            WebViewController.aTimer.Enabled = false;
                            WebViewController.aTimer.AutoReset = false;
                            stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            WebViewController.aTimer.Stop();
                            WebViewController.aTimer = null;
                            // ShowRateDeliverManRestaurant((Activity)Static_mContext);

                            string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                            var text = File.ReadAllText(index);


                            WKWebViewConfiguration config = new WKWebViewConfiguration();
                            WKWebView webView = new WKWebView(View2.Frame, config);
                            View2.AddSubview(webView);


                            webView.LoadHtmlString(text, null);
                            //messageHandler.LoadAvailableProductsStartUp();

                            stateOfRequest = stateOfRequestEnum.ShowwingMap;
                            try
                            {

                                
                                webi.LoadHtmlString(text, null);
                                LoadAvailableProductsStartUp();

                                ShoppingKartModal.DismissViewController(true, null);
                            }
                            catch (Exception)
                            { }
                            
                        }


                    }
                    else if(S_Response.httpStatusCode != System.Net.HttpStatusCode.Accepted)
                    {
                        WebViewController.GlobalInterfaceReference.ReturnToRestaurantView();
                        stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                        /*
                        WebViewController.aTimer.Enabled = false;
                        WebViewController.aTimer.AutoReset = false;
                        stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                        WebViewController.aTimer.Stop();
                        WebViewController.aTimer = null;
                        // ShowRateDeliverManRestaurant((Activity)Static_mContext);

                        string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                        var text = File.ReadAllText(index);


                        WKWebViewConfiguration config = new WKWebViewConfiguration();
                        WKWebView webView = new WKWebView(View2.Frame, config);
                        View2.AddSubview(webView);


                        webView.LoadHtmlString(text, null);
                        //messageHandler.LoadAvailableProductsStartUp();

                        stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                        try
                        {


                            webi.LoadHtmlString(text, null);
                            LoadAvailableProductsStartUp();

                            ShoppingKartModal.DismissViewController(true, null);
                        }
                        catch (Exception)
                        { }

                        */
                    }
                }
            }
            catch (Exception)
            { }
        }

        public async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //  MainViewJavasciptInterface_Reference.UpdateMap();


            WebViewController.Invoker.BeginInvokeOnMainThread(() =>
                   {
                       GetMapState();
                       //  var GlobalQueryTask = Task.Run(() =>
                       //  BootUP(), token);

                       /*
                       FirstViewRequestHandler.webi.EvaluateJavaScript(script, (result, error) =>
                       {
                           if (error != null) Console.WriteLine(error);
                       });*/
                   });

         


        }
        public static async Task<UriResponse> GetResponseFromURI(Uri u)
        {

            try
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

            catch (Exception ex) {
               
                return new UriResponse();

            }
        }
        public class UriResponse
        {
            public String Response;
            public System.Net.HttpStatusCode httpStatusCode;
        }
    }
    class JsonInterfaceDialog
    {
        public string Method;
        public string Argument1;
        public string Argument2;
        public string Argument3;
        public string Argument4;
        public string Argument5;
    }


}

