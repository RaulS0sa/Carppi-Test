using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Carppi_Cliente.AidViews;
using Carppi_Cliente.DatabaseTypes;
using Firebase.CloudMessaging;
using Foundation;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SQLite;
using UIKit;
using WebKit;
using Xamarin.Auth;

namespace Carppi_Cliente.MainViewJavasciptInterface
{
    public class MAinViewJavascriptHandler
    {
        protected UIView Views;
        protected WKWebView webi;
        protected static UIView Views_statis;
        protected static WKWebView webi_Static;
        protected long RestID;
        protected static long static_RestID;
        CancellationTokenSource taskController = new CancellationTokenSource();
        public DetailedProductViewFromRestauran Details;
        public static long TimeToWait = 35000;
        public static bool KeepQuering = true;
        public static List<CarppiGroceryProductos> ListaDeProductos = new List<CarppiGroceryProductos>();
        static NSObject Invoker = new NSObject();
        public MAinViewJavascriptHandler(UIView view,WKWebView web )
        {
            webi = web;
            Views = view;
            Views_statis = Views;
            webi_Static = web;
            //Publiccontrol  UIView
            //webi  WKWebView
        }
        public void RestaurantDetailedView(long ContextualID, string RestaurantHash, string Imagen, string NombreDelRestaurante, bool? EstaAbierto)
        {

            ShoppingKartViewController.CarppiHash = RestaurantHash;
            RestID = ContextualID;
            static_RestID = ContextualID;
          //  WKWebViewConfiguration config = new WKWebViewConfiguration();
          //  WKWebView webView = new WKWebView(Views.Frame, config);
          //  Views.AddSubview(webView);
          //var messageHandler = new DetailedViewJavascriptHandler(webView);
          //  var messageHandler = new MainView_JavascriptInterface(webView);
          //  config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


            string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/CarppiDeliveryRestaurantDetailedView.html");
            var text = File.ReadAllText(index);
            text = text.Replace("---REstaurantName-----", NombreDelRestaurante);//Restaurante.RestaurantData.Nombre);

            text = text.Replace("mall-dedicated-banner-replace----", Imagen);// "data:image/png;base64," + Restaurante.RestaurantData.Foto);

            text = text.Replace("---OpenTag---", (EstaAbierto == null || EstaAbierto == false) ? "Cerrado" : "Abierto");
            text = text.Replace("---OpenTagColor---", (EstaAbierto == null || EstaAbierto == false) ? "danger" : "success");




            webi.LoadHtmlString(text, null);
           // var DetailedInterface = new DetailedViewavascriptInterface(Views, webView, RestID);
           // DetailedInterface.StartUP();
            CancellationToken token = taskController.Token;
            // var script = "UpdateProductSelector(" + 1 + ")";
            //webi.EvaluateJavaScript(script, null);
            Invoker.BeginInvokeOnMainThread(() =>
            {
                var GlobalQueryTask = Task.Run(() =>
                BootUP(), token);

                /*
                FirstViewRequestHandler.webi.EvaluateJavaScript(script, (result, error) =>
                {
                    if (error != null) Console.WriteLine(error);
                });*/
            });
          

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
                    Invoker.BeginInvokeOnMainThread(() =>
                    {
                        var script = "HideLoadingBar(" + ")";
                        // var script = "PrintTagResponse(" + S_Response.Response + ")";

                        webi.EvaluateJavaScript(script, null);
                        /*
                        FirstViewRequestHandler.webi.EvaluateJavaScript(script, (result, error) =>
                        {
                            if (error != null) Console.WriteLine(error);
                        });*/
                    });

                
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
                //webi.EvaluateJavaScript(script, null);
                Invoker.BeginInvokeOnMainThread(() =>
                {
                    var script = "UpdateProductSelector(" + Details.FoodCategories + ")";
                    webi.EvaluateJavaScript(script, null);

                    /*
                    FirstViewRequestHandler.webi.EvaluateJavaScript(script, (result, error) =>
                    {
                        if (error != null) Console.WriteLine(error);
                    });*/
                });
              
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

                    Invoker.BeginInvokeOnMainThread(() =>
                    {
                        var S_Response = t.Result;
                        var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);
                        Respuesta.Foto = Decompress(Respuesta.Foto);
                        var script = "UpdateProductGrid(" + JsonConvert.SerializeObject(Respuesta) + ")";

                        webi.EvaluateJavaScript(script, null);
                        /*
                        FirstViewRequestHandler.webi.EvaluateJavaScript(script, (result, error) =>
                        {
                            if (error != null) Console.WriteLine(error);
                        });*/
                    });
                  
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
        public static UIViewController ShoppingKartModal;
        private async void Auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                var databasePath5 = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                //  GlobalFragmentManager = FragmentManager;
                var db5 = new SQLiteConnection(databasePath5);
                db5.CreateTable<DatabaseTypes.Log_info>();

                var request = new OAuth2Request("GET",
                                                new Uri("https://graph.facebook.com/me?fields=id,name,picture,cover,birthday,first_name,last_name")
                                                , null, e.Account);
                var response = await request.GetResponseAsync();
                var user = JsonValue.Parse(response.GetResponseText());
                var ID = "144315493921";//user["id"];
                var fbName = user["name"];
                var LastName = user["last_name"];

                var FirstName = user["first_name"];
                var fbProfile = user["picture"]["data"]["url"];
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                if (query == null)
                {


                    var s = db5.Insert(new DatabaseTypes.Log_info()
                    {
                        Name = fbName,
                        LastName = LastName,
                        Firstname = FirstName,
                        // Photo = cadena,
                        Viajesrelizados = "0",
                        Calificacion = "0",
                        ProfileId = ID

                    });
                }
                else
                {
                    //db5.RunInTransaction(() =>
                    //{
                    //    db5.Update(query);
                    //});
                }

                //El nombre
                // lblName.Text = fbName.ToString();
                //La foto
                // imgProfile.Image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(fbProfile)));

                //var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
                //var mainStoryboard = appDelegate.MainStoryboard;
                //var tabBarController = appDelegate.GetViewController(mainStoryboard, "MainTabBarController");
                //appDelegate.SetRootViewController(tabBarController, true);

                try
                {
                   // await LoggInWebTask();
                }
                catch (System.Exception) { }
               // DismissViewController(true, null);
             
            }
            else
            {
             //   DismissViewController(true, null);
            }

        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }
        public class ShopItem
        {
            public int ItemID;
            public int Quantity;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public enum TipoDePago { Efectivo, Tarjeta };

        public async void GenerateFoodOrder(string Comment)
        {
            if (true)
            {
                //var token = Messaging.SharedInstance.FcmToken ?? "";

                if (IsLocationAvailable())
                {
                    try
                    {
                        var MyLatLong = await Clases.Location.GetCurrentPosition();
                     //   createprofile(Mail);
                        if (MyLatLong == null)
                        {


                            //Create Alert
                            var okCancelAlertController = UIAlertController.Create("Error", "No Puedes pedir si No Activas la ubicacion en tu telefono", UIAlertControllerStyle.Alert);

                            //Add Actions
                            okCancelAlertController.AddAction(UIAlertAction.Create("Aceptar", UIAlertActionStyle.Default, alert => Console.WriteLine("Okay was clicked")));
                            //okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                            //Present Alert
                            ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okCancelAlertController, true, null);



                            //Sin Log En la base de datos
                        }
                        else
                        {
                            //ShopItem
                            List<ShopItem> ListaDEItems = new List<ShopItem>();
                            var cost = 0.0;
                            foreach (var element in MAinViewJavascriptHandler.ListaDeProductos)
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
                                //Create Alert
                                var okCancelAlertController = UIAlertController.Create("Opciones", "Selecciona tu metodo de pago", UIAlertControllerStyle.Alert);

                                //Add Actions
                                okCancelAlertController.AddAction(UIAlertAction.Create("Efectivo", UIAlertActionStyle.Default, async alert =>
                                {
                                    var token = Messaging.SharedInstance.FcmToken ?? "";
                                    var tk = query.FirebaseID == null ? token : query.FirebaseID;



                                    HttpClient client = new HttpClient();

                                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/GeneratePurchaseOrder_Comentarios?" +
                                       "FaceIDOfBuyer=" + query.ProfileId
                                       + "&BuyList=" + ProductoHAsh
                                       + "&Lat=" + MyLatLong.Latitude.ToString().Replace(",", ".")
                                       + "&Log=" + MyLatLong.Longitude.ToString().Replace(",", ".")
                                       + "&Region=" + Region
                                       + "&tipoDePago=" + ((int)(TipoDePago.Efectivo))
                                       + "&Comentario=" + Comment
                                       ));
                                    HttpResponseMessage response;

                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                    response = await client.GetAsync(uri);

                                    switch (response.StatusCode)
                                    {
                                        case System.Net.HttpStatusCode.Gone:
                                            {

                                                //Create Alert
                                                var okAlertController = UIAlertController.Create("Error", "No hay repartidores en el area, intenta mas tarde", UIAlertControllerStyle.Alert);

                                                //Add Action
                                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                                                // Present Alert
                                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);


                                                //Sin Log En la base de datos
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.Moved:
                                            {

                                                //Create Alert
                                                var okAlertController = UIAlertController.Create("Error", "El repartidor tiene muchas ordenes pendientes, intenta en unos minutos", UIAlertControllerStyle.Alert);

                                                //Add Action
                                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                                                // Present Alert
                                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);


                                            }
                                            break;
                                        case System.Net.HttpStatusCode.Unauthorized:
                                            {

                                                //Create Alert
                                                var okAlertController = UIAlertController.Create("Error", "No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar", UIAlertControllerStyle.Alert);

                                                //Add Action
                                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                                                // Present Alert
                                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);
                                                /*
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
                                                */
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
                                                aTimer = new System.Timers.Timer(2000);
                                                // Hook up the Elapsed event for the timer. 
                                                aTimer.Elapsed += RestaurantClock;//MAinViewJavascriptHandler.OnTimedEvent;
                                                aTimer.AutoReset = true;
                                                aTimer.Enabled = true;

                                                DissmissBottomModal();
                                                ReturnToRestaurantView();
                                                /*
                                                MainActivity.LoadFragment_Static(Resource.Id.menu_video);
                                                //fragment = FragmentSelectTypeOfPurchase.NewInstance();
                                                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
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

                                                */
                                            }
                                            break;
                                    }


                                }));
                                okCancelAlertController.AddAction(UIAlertAction.Create("Cancelar", UIAlertActionStyle.Cancel, null));

                                //Present Alert
                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okCancelAlertController, true, null);


                            }

                        }

                    }
                    catch (SQLiteException ex)
                    {
                       // createprofile(Mail);
                        var okAlertController = UIAlertController.Create("Error", "Hubo un error en la creacion de tu perfil, podrias intentarlo de nuevo?", UIAlertControllerStyle.Alert);

                        //Add Action
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        // Present Alert
                        ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);



                        //logForFuckSake

                    }
                }

            }
            else
            {
                var script = "ChangeButtonToInvalidEmail(" + ")";

                webi.EvaluateJavaScript(script, null);
            }
            //  var ttt = 2;
            /*  var auth = new OAuth2Authenticator(
                   clientId: "353621488737383",
                   scope: "",
                   authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                   redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"));

              auth.Completed += Auth_Completed;
              var ui = auth.GetUI();
              WebViewController.GlobalViewController.PresentViewController(ui, true, null);
              */
            /*
           
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

                var sssaaaeee = new Random();

                var TransportCost = (int)Convert.ToDouble(errorMessage1, culture) + ((int)(sssaaaeee.NextDouble() * 1000));
                var fileName1 = "Views/ShoppingKart2.html";
                string localHtmlUrl1 = Path.Combine(NSBundle.MainBundle.BundlePath, fileName1);
                using (StreamReader lectura = new StreamReader(localHtmlUrl1))
                {
                    // Title = "Dashboard";
                    Views.BackgroundColor = UIColor.White;
                    string filecontent = lectura.ReadToEnd();
                    var config = new WKWebViewConfiguration();
                  //  WKWebView webView = new WKWebView(Views.Frame, config);
                    var messageHandler = new MainView_JavascriptInterface(ShoppingKartViewController.VistaShoppingKart);
                    config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


                    ShoppingKartViewController.VistaShoppingKart.TranslatesAutoresizingMaskIntoConstraints = false;



                    Views.AddSubview(ShoppingKartViewController.VistaShoppingKart);
                    ShoppingKartViewController.VistaShoppingKart.MultipleTouchEnabled = false;

                    ShoppingKartViewController.VistaShoppingKart.ScrollView.ScrollEnabled = true;
                    ShoppingKartViewController.VistaShoppingKart.ScrollView.Bounces = false;
                    ShoppingKartViewController.VistaShoppingKart.UserInteractionEnabled = true;
                    ShoppingKartViewController.VistaShoppingKart.SizeToFit();
                    ShoppingKartViewController.VistaShoppingKart.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;



                    var ccc = Shoppingkart2List(MAinViewJavascriptHandler.ListaDeProductos);
                    //content = content.Replace("<!--ReplaceProductFromTheList-->", cadea);
                    filecontent = filecontent.Replace("<div id=\"OrderItems\"></div>", ccc);
                    filecontent = filecontent.Replace("0000000000", TransportCost.ToString());
                    filecontent = filecontent.Replace("CompleCostOFGroceryy", CompleteCostOFGrocery(MAinViewJavascriptHandler.ListaDeProductos));
                    //GroceryCostPlusFee
                    //No has Logueado
                    filecontent = filecontent.Replace("No has Logueado", faceID == null || String.IsNullOrEmpty(faceID) ? "No has logueado, no podemos darte promiciones si no te conocemos" : "Gracias por usar Carppi :D");
                    filecontent = filecontent.Replace("1329", CompleteCostOFGroceryPlusFee(MAinViewJavascriptHandler.ListaDeProductos, (int)TransportCost));




                    ShoppingKartViewController.VistaShoppingKart.LoadHtmlString(filecontent, NSBundle.MainBundle.BundleUrl);




                }







            }
            */


            //-------------------------

        }
        public void GetNotStaticMapState()
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
                            webi_Static.EvaluateJavaScript(script, (result, error) =>
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
                            //webi_Static
                            string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/RateFoodDeliverMan.html");
                            var text = File.ReadAllText(index);
                            webi.LoadHtmlString(text, null);
                            webi_Static.LoadHtmlString(text, null);
                            try
                            {
                                stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                                MAinViewJavascriptHandler.aTimer.Enabled = false;
                                MAinViewJavascriptHandler.aTimer.AutoReset = false;
                                MAinViewJavascriptHandler.aTimer.Stop();
                                MAinViewJavascriptHandler.aTimer = null;
                            }
                            catch (Exception)
                            {

                            }
                            /*
                            //__________________
                            WebViewController.aTimer.Enabled = false;
                            WebViewController.aTimer.AutoReset = false;
                            stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            WebViewController.aTimer.Stop();
                            WebViewController.aTimer = null;
                            // ShowRateDeliverManRestaurant((Activity)Static_mContext);



                            WKWebViewConfiguration config = new WKWebViewConfiguration();
                           // WKWebView webView = new WKWebView(Views_statis.Frame, config);
                          //  Views_statis.AddSubview(webView);

                            webi.LoadHtmlString(text, null);
                           // webView.LoadHtmlString(text, null);
                          //  WebViewController.GlobalViewController.ViewDidLoad();
                            //messageHandler.LoadAvailableProductsStartUp();

                            stateOfRequest = stateOfRequestEnum.ShowwingMap;
                            try
                            {
                                ShoppingKartModal.DismissViewController(true, null);
                            }
                            catch (Exception)
                            { }
                            */
                            // messageHandler.GetMapState();

                        }
                        else if (Order.Stat == GroceryOrderState.RequestRejected)
                        {
                            stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            //-----------------------------
                            System.Threading.Tasks.Task.Delay(5 * 1000).ContinueWith((_) => {

                            Invoker.BeginInvokeOnMainThread(() =>
                            {
                               // string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                               // var text = File.ReadAllText(index);
                               // webi.LoadHtmlString(text, null);
                               // webi_Static.LoadHtmlString(text, null);
                                ReturnToRestaurantView();
                                try
                                {
                                    MAinViewJavascriptHandler.aTimer.Enabled = false;
                                    MAinViewJavascriptHandler.aTimer.AutoReset = false;
                                    MAinViewJavascriptHandler.aTimer.Stop();
                                    MAinViewJavascriptHandler.aTimer = null;
                                }
                                catch(Exception)
                                {

                                }
                                //LoadAvailableProductsStartUp(webi);
                                });
                            });

                            //-----------------
                            /*
                            WebViewController.aTimer.Enabled = false;
                            WebViewController.aTimer.AutoReset = false;
                            //stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            WebViewController.aTimer.Stop();
                            WebViewController.aTimer = null;
                            //-----------------------------
                            string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                            var text = File.ReadAllText(index);
                            webi_Static.LoadHtmlString(text, null);

                            //-----------------
                            Invoker.BeginInvokeOnMainThread(() =>
                            {
                                System.Threading.Tasks.Task.Delay(5 * 1000).ContinueWith((_) => {
                                    webi.LoadHtmlString(text, null);
                                    

                                    WebViewController.aTimer.Enabled = false;
                                    WebViewController.aTimer.AutoReset = false;
                                    stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                                    WebViewController.aTimer.Stop();
                                    WebViewController.aTimer = null;
                                    // ShowRateDeliverManRestaurant((Activity)Static_mContext);


                                    //StaticReturnToRestaurantView();
                                });
                            });

                            */


                            // ReturnToRestaurantView();
                        }


                    }
                    else if(S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {

                        {
                            stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            //-----------------------------
                            System.Threading.Tasks.Task.Delay(2 * 1000).ContinueWith((_) => {

                                Invoker.BeginInvokeOnMainThread(() =>
                                {
                                 
                                    ReturnToRestaurantView();
                                    try
                                    {
                                        MAinViewJavascriptHandler.aTimer.Enabled = false;
                                        MAinViewJavascriptHandler.aTimer.AutoReset = false;
                                        MAinViewJavascriptHandler.aTimer.Stop();
                                        MAinViewJavascriptHandler.aTimer = null;
                                    }
                                    catch (Exception)
                                    {

                                    }
                                   
                                });
                            });

                            
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }

        public async void LoadAvailableProductsStartUp(WKWebView ReferendceView)
        {
            Invoker.BeginInvokeOnMainThread(async () =>
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
                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                    "CadenadelUsuarioRestaurant=" + FaceID


                    ));

                 var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDeterminationTest?" +
                    "CadenadelUsuarioRestaurant_TOTEst=" + FaceID


                    ));

                 var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                    "CadenadelUsuarioRestaurant=" + FaceID


                    ));

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiRestaurantExistanceDetermination?" +
                  "CadenadelUsuarioRestaurant=" + FaceID


                  ));
                */
                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiIOSRestaurantApi/CarppiRestaurantExistanceDeterminationTest?" +
                    "CadenadelUsuarioRestaurant_TOTEst=" + FaceID


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

                            WebViewController.GlobalWebView.EvaluateJavaScript(script, (s, e) => {

                            Console.WriteLine(e.ToString()) ;
                        });
                      //  webi_Static.EvaluateJavaScript(script, null);
                        
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


        public static void GetMapState()
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
                            webi_Static.EvaluateJavaScript(script, (result, error) =>
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
                            //webi_Static
                            string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/RateFoodDeliverMan.html");
                            var text = File.ReadAllText(index);
                            webi_Static.LoadHtmlString(text, null);
                            //__________________
                            WebViewController.aTimer.Enabled = false;
                            WebViewController.aTimer.AutoReset = false;
                            stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            WebViewController.aTimer.Stop();
                            WebViewController.aTimer = null;
                            // ShowRateDeliverManRestaurant((Activity)Static_mContext);


                           
                            WKWebViewConfiguration config = new WKWebViewConfiguration();
                            WKWebView webView = new WKWebView(Views_statis.Frame, config);
                            Views_statis.AddSubview(webView);


                            webView.LoadHtmlString(text, null);
                            WebViewController.GlobalViewController.ViewDidLoad();
                            //messageHandler.LoadAvailableProductsStartUp();

                            stateOfRequest = stateOfRequestEnum.ShowwingMap;
                            try
                            {
                                ShoppingKartModal.DismissViewController(true, null);
                            }
                            catch (Exception)
                            { }
                            // messageHandler.GetMapState();

                        }
                        else if(Order.Stat == GroceryOrderState.RequestRejected)
                        {
                            WebViewController.aTimer.Enabled = false;
                            WebViewController.aTimer.AutoReset = false;
                            //stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                            WebViewController.aTimer.Stop();
                            WebViewController.aTimer = null;
                            //-----------------------------
                            string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                            var text = File.ReadAllText(index);
                            webi_Static.LoadHtmlString(text, null);

                            //-----------------
                            Invoker.BeginInvokeOnMainThread(() =>
                            {
                                System.Threading.Tasks.Task.Delay(5 * 1000).ContinueWith((_) => {

                                    WebViewController.aTimer.Enabled = false;
                                    WebViewController.aTimer.AutoReset = false;
                                    stateOfRequest = stateOfRequestEnum.ShowingRestaurants;
                                    WebViewController.aTimer.Stop();
                                    WebViewController.aTimer = null;
                                    // ShowRateDeliverManRestaurant((Activity)Static_mContext);


                                   // string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                                   // var text = File.ReadAllText(index);
                                    WKWebViewConfiguration config = new WKWebViewConfiguration();
                                    WKWebView webView = new WKWebView(Views_statis.Frame, config);
                                    Views_statis.AddSubview(webView);


                                    webView.LoadHtmlString(text, null);
                                    //messageHandler.LoadAvailableProductsStartUp();

                                    stateOfRequest = stateOfRequestEnum.ShowwingMap;

                                    WebViewController.GlobalViewController.ViewDidLoad();
                                    //StaticReturnToRestaurantView();
                                });
                            });
                          
                            
                           // ReturnToRestaurantView();
                        }


                    }
                }
            }
            catch (Exception)
            { }
        }


        public async void RestaurantClock(Object source, ElapsedEventArgs e)
        {
            //  MainViewJavasciptInterface_Reference.UpdateMap();


            WebViewController.Invoker.BeginInvokeOnMainThread(() =>
            {
                GetNotStaticMapState();
                //  var GlobalQueryTask = Task.Run(() =>
                //  BootUP(), token);

                /*
                FirstViewRequestHandler.webi.EvaluateJavaScript(script, (result, error) =>
                {
                    if (error != null) Console.WriteLine(error);
                });*/
            });




        }

        public static async void OnTimedEvent(Object source, ElapsedEventArgs e)
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

        public async void UpdateShoppingKartView(string Mail, string Comment)
        {
            if(IsValidEmail(Mail))
            {
                //var token = Messaging.SharedInstance.FcmToken ?? "";

                if (IsLocationAvailable())
                {
                    try
                    {
                        var MyLatLong = await Clases.Location.GetCurrentPosition();
                        createprofile(Mail);
                        if (MyLatLong == null)
                        {

                          
                                //Create Alert
                                var okCancelAlertController = UIAlertController.Create("Error", "No Puedes pedir si No Activas la ubicacion en tu telefono", UIAlertControllerStyle.Alert);

                                //Add Actions
                                okCancelAlertController.AddAction(UIAlertAction.Create("Aceptar", UIAlertActionStyle.Default, alert => Console.WriteLine("Okay was clicked")));
                            //okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                            //Present Alert
                            ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okCancelAlertController, true, null);
                            


                            //Sin Log En la base de datos
                        }
                        else
                        {
                            //ShopItem
                            List<ShopItem> ListaDEItems = new List<ShopItem>();
                            var cost = 0.0;
                            foreach (var element in MAinViewJavascriptHandler.ListaDeProductos)
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
                                //Create Alert
                                var okCancelAlertController = UIAlertController.Create("Opciones", "Selecciona tu metodo de pago", UIAlertControllerStyle.Alert);

                                //Add Actions
                                okCancelAlertController.AddAction(UIAlertAction.Create("Efectivo", UIAlertActionStyle.Default, async alert =>
                                {
                                    var token = Messaging.SharedInstance.FcmToken ?? "";
                                    var tk = query.FirebaseID == null ? token : query.FirebaseID;



                                    HttpClient client = new HttpClient();

                                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiIOSRestaurantApi/GeneratePurchaseOrder_Comentarios?" +
                                        "FaceIDOfBuyer=" + query.ProfileId
                                        + "&BuyList=" + ProductoHAsh
                                        + "&Lat=" + MyLatLong.Latitude.ToString().Replace(",", ".")
                                        + "&Log=" + MyLatLong.Longitude.ToString().Replace(",", ".")
                                        + "&Region=" + Region
                                        + "&tipoDePago=" + ((int)(TipoDePago.Efectivo))
                                        + "&Comentario=" + Comment
                                        + "&Correo=" + Mail
                                        + "&FirebaseID=" + tk
                                        ));
                                    HttpResponseMessage response;

                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                    response = await client.GetAsync(uri);

                                    switch (response.StatusCode)
                                    {
                                        case System.Net.HttpStatusCode.Gone:
                                            {

                                                //Create Alert
                                                var okAlertController = UIAlertController.Create("Error", "No hay repartidores en el area, intenta mas tarde", UIAlertControllerStyle.Alert);

                                                //Add Action
                                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                                                // Present Alert
                                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);
                                               

                                                //Sin Log En la base de datos
                                            }
                                            break;
                                        case System.Net.HttpStatusCode.Moved:
                                            {

                                                //Create Alert
                                                var okAlertController = UIAlertController.Create("Error", "El repartidor tiene muchas ordenes pendientes, intenta en unos minutos", UIAlertControllerStyle.Alert);

                                                //Add Action
                                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                                                // Present Alert
                                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);


                                            }
                                            break;
                                        case System.Net.HttpStatusCode.Unauthorized:
                                            {

                                                //Create Alert
                                                var okAlertController = UIAlertController.Create("Error", "No Puedes pedir sin Logear, deseas hacerlo ahora?, presiona cerrar al terminar", UIAlertControllerStyle.Alert);

                                                //Add Action
                                                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                                                // Present Alert
                                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);
                                                /*
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
                                                */
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
                                                aTimer = new System.Timers.Timer(2000);
                                                // Hook up the Elapsed event for the timer. 
                                                aTimer.Elapsed += RestaurantClock;
                                                aTimer.AutoReset = true;
                                                aTimer.Enabled = true;
                                                DissmissBottomModal();
                                                ReturnToRestaurantView();
                                                /*
                                                MainActivity.LoadFragment_Static(Resource.Id.menu_video);
                                                //fragment = FragmentSelectTypeOfPurchase.NewInstance();
                                                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
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

                                                */
                                            }
                                            break;
                                    }


                                }));
                                okCancelAlertController.AddAction(UIAlertAction.Create("Cancelar", UIAlertActionStyle.Cancel,null));

                                //Present Alert
                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okCancelAlertController, true, null);


                            }
                     
                        }

                    }
                    catch (SQLiteException ex)
                    {
                        createprofile(Mail);
                        var okAlertController = UIAlertController.Create("Error", "Hubo un error en la creacion de tu perfil, podrias intentarlo de nuevo?", UIAlertControllerStyle.Alert);

                        //Add Action
                        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        // Present Alert
                        ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);

                       

                        //logForFuckSake

                    }
                }

            }
            else
            {
                var script = "ChangeButtonToInvalidEmail(" + ")";

                webi.EvaluateJavaScript(script, null);
            }
          //  var ttt = 2;
          /*  var auth = new OAuth2Authenticator(
                 clientId: "353621488737383",
                 scope: "",
                 authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                 redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"));

            auth.Completed += Auth_Completed;
            var ui = auth.GetUI();
            WebViewController.GlobalViewController.PresentViewController(ui, true, null);
            */
            /*
           
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

                var sssaaaeee = new Random();

                var TransportCost = (int)Convert.ToDouble(errorMessage1, culture) + ((int)(sssaaaeee.NextDouble() * 1000));
                var fileName1 = "Views/ShoppingKart2.html";
                string localHtmlUrl1 = Path.Combine(NSBundle.MainBundle.BundlePath, fileName1);
                using (StreamReader lectura = new StreamReader(localHtmlUrl1))
                {
                    // Title = "Dashboard";
                    Views.BackgroundColor = UIColor.White;
                    string filecontent = lectura.ReadToEnd();
                    var config = new WKWebViewConfiguration();
                  //  WKWebView webView = new WKWebView(Views.Frame, config);
                    var messageHandler = new MainView_JavascriptInterface(ShoppingKartViewController.VistaShoppingKart);
                    config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


                    ShoppingKartViewController.VistaShoppingKart.TranslatesAutoresizingMaskIntoConstraints = false;



                    Views.AddSubview(ShoppingKartViewController.VistaShoppingKart);
                    ShoppingKartViewController.VistaShoppingKart.MultipleTouchEnabled = false;

                    ShoppingKartViewController.VistaShoppingKart.ScrollView.ScrollEnabled = true;
                    ShoppingKartViewController.VistaShoppingKart.ScrollView.Bounces = false;
                    ShoppingKartViewController.VistaShoppingKart.UserInteractionEnabled = true;
                    ShoppingKartViewController.VistaShoppingKart.SizeToFit();
                    ShoppingKartViewController.VistaShoppingKart.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;



                    var ccc = Shoppingkart2List(MAinViewJavascriptHandler.ListaDeProductos);
                    //content = content.Replace("<!--ReplaceProductFromTheList-->", cadea);
                    filecontent = filecontent.Replace("<div id=\"OrderItems\"></div>", ccc);
                    filecontent = filecontent.Replace("0000000000", TransportCost.ToString());
                    filecontent = filecontent.Replace("CompleCostOFGroceryy", CompleteCostOFGrocery(MAinViewJavascriptHandler.ListaDeProductos));
                    //GroceryCostPlusFee
                    //No has Logueado
                    filecontent = filecontent.Replace("No has Logueado", faceID == null || String.IsNullOrEmpty(faceID) ? "No has logueado, no podemos darte promiciones si no te conocemos" : "Gracias por usar Carppi :D");
                    filecontent = filecontent.Replace("1329", CompleteCostOFGroceryPlusFee(MAinViewJavascriptHandler.ListaDeProductos, (int)TransportCost));




                    ShoppingKartViewController.VistaShoppingKart.LoadHtmlString(filecontent, NSBundle.MainBundle.BundleUrl);




                }







            }
            */


            //-------------------------

        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void createprofile(string email)
        {
            try
            {

                var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db10 = new SQLiteConnection(databasePath10);
                db10.CreateTable<DatabaseTypes.Log_info>();

                var cripticmail = ComputeSha256Hash(email.ToLower());

                var query = db10.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                if (query == null)
                {


                    var s = db10.Insert(new DatabaseTypes.Log_info()
                    {
                        Name = "---",
                        LastName = "---",
                        Firstname = "---",
                        // Photo = cadena,
                        Viajesrelizados = "0",
                        Calificacion = "0",
                        ProfileId = cripticmail

                    });
                }
                else
                {
                    query.Name = "---";
                    query.LastName = "---";
                    query.Firstname = "---";
                    // Photo = cadena,
                    query.Viajesrelizados = "0";
                    query.Calificacion = "0";
                    query.ProfileId = cripticmail;
                    db10.RunInTransaction(() =>
                    {
                        db10.Update(query);
                    });
                }
            }



            catch (System.Exception ex)
            {

            }
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


        public void UpdateProductQuantity(int ProductHash, int quantity)
        {

            var micopia = new List<CarppiGroceryProductos>(ListaDeProductos);
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
            ListaDeProductos = micopia;
        }

        public void DissmissBottomModal()
        {
            ShoppingKartModal.DismissViewController(true, null);

        }
        public void ShowExtraOptionsOnDeliverManAwait()
        {

            var controllerTwo = new PendingRequestToDeliverMan.DeliverManExtraOptionsViewController();
            ShoppingKartModal = controllerTwo;
            WebViewController.GlobalViewController.PresentViewController(controllerTwo, true, null);


        }
        public void ShowExtraOptionsOnGroceryAwait()
        {

            var controllerTwo = new PendingRequestToRestaurant.RestaurantPendingRequestViewController();
            ShoppingKartModal = controllerTwo;
            WebViewController.GlobalViewController.PresentViewController(controllerTwo, true, null);


        }


        public void ShowOptionInOrderCreated()
        {
            var okCancelAlertController = UIAlertController.Create("Opciones", "Que deseas hacer?", UIAlertControllerStyle.Alert);

            //Add Actions
            okCancelAlertController.AddAction(UIAlertAction.Create("Cancelar orden", UIAlertActionStyle.Default, alert => {
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


                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CancelOrder?" +
                        "FaceIDOfBuyer=" + FaceID
                        + "&IdfBuy=" + OrderIDIfActive.ToString()


                        ));
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    //t.Wait();

                    var S_Response = t.Result;
                    if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //MainActivity.LoadFragment_Static(Resource.Id.menu_video);
                    }

                }
                catch (Exception)
                { }

            }));
            okCancelAlertController.AddAction(UIAlertAction.Create("Cerrar dialogo", UIAlertActionStyle.Default, alert => Console.WriteLine("Okay was clicked")));
            //okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

            //Present Alert
            WebViewController.GlobalViewController.PresentViewController(okCancelAlertController, true, null);
           // ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okCancelAlertController, true, null);

/*
            var controllerTwo = new PendingRequestToRestaurant.RestaurantPendingRequestViewController();
                ShoppingKartModal = controllerTwo;
                WebVi ewController.GlobalViewController.PresentViewController(controllerTwo, true, null);
            */

        }

      
        public async void SendMessageDeliverMan_Client(string message)
        {

            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();

            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiGroceryApi/PostMessageInRideShare?" +
                "FaceID_Interlocutor=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&TripRequest=" + OrderIDIfActive +
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



      
        public async void GetAllMessagesFromConversationDeliverManToClient()
        {

            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiGroceryApi/GetAllMessagesFromTheConversation?" +
                "FaceID_Interest=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&TripRequest=" + OrderIDIfActive

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
                var script = "UpdateConversationLayout(" + response.Content.ReadAsStringAsync().Result + ")";
                webi.EvaluateJavaScript(script, null);

               
            }
            // Action Toatsaction = () =>
            // {
            //     Toast.MakeText(mContext, ss, ToastLength.Short).Show();
            // };
            //  ((Activity)mContext).RunOnUiThread(Toatsaction);
        }


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
            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/PostMessageInRestauranClient?" +
                "FaceID_speaker=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&shopRequest=" + OrderIDIfActive +
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
            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/GetAlMessagesFromConversationRestauranClient?" +
                "FaceID_Interest=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&shopRequest=" + OrderIDIfActive

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

                var script = "UpdateConversationLayout(" + response.Content.ReadAsStringAsync().Result + ")";
                webi.EvaluateJavaScript(script, null);

              
            }
            // Action Toatsaction = () =>
            // {
            //     Toast.MakeText(mContext, ss, ToastLength.Short).Show();
            // };
            //  ((Activity)mContext).RunOnUiThread(Toatsaction);
        }



        public async void DisplayShopingKart()
        {
            try
            {
                if (ListaDeProductos.Count() > 0)
                {
                    if (IsLocationAvailable())
                    {
                        try
                        {
                            var MyLatLong = await Clases.Location.GetCurrentPosition();
                           
                            if (MyLatLong == null)
                            {


                                //Create Alert
                                var okCancelAlertController = UIAlertController.Create("Error", "No Puedes pedir si No Activas la ubicacion en tu telefono", UIAlertControllerStyle.Alert);

                                //Add Actions
                                okCancelAlertController.AddAction(UIAlertAction.Create("Aceptar", UIAlertActionStyle.Default, alert => Console.WriteLine("Okay was clicked")));
                                //okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                                //Present Alert
                                ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okCancelAlertController, true, null);



                                //Sin Log En la base de datos
                            }
                            else
                            {
                                var controllerTwo = new AidViews.ShoppingKartViewController();
                                ShoppingKartModal = controllerTwo;
                                WebViewController.GlobalViewController.PresentViewController(controllerTwo, true, null);
                            }

                        }
                        catch (SQLiteException ex)
                        {
                           
                            var okAlertController = UIAlertController.Create("Error", "Hubo un error en la creacion de tu perfil, podrias intentarlo de nuevo?", UIAlertControllerStyle.Alert);

                            //Add Action
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                            // Present Alert
                            ShoppingKartViewController.ShoppingKartViewControllerReference.PresentViewController(okAlertController, true, null);



                            //logForFuckSake

                        }
                    }
                    /*
                    var auth = new OAuth2Authenticator(
                clientId: "353621488737383",
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html"));

                    auth.Completed += Auth_Completed;
                    var ui = auth.GetUI();
                    WebViewController.GlobalViewController.PresentViewController(ui, true, null);
                    */
                 
                }
                else
                {
                    var okCancelAlertController = UIAlertController.Create("Error", "No puedes abrir tu carrito de compra si no hay productos", UIAlertControllerStyle.Alert);

                    //Add Actions
                    okCancelAlertController.AddAction(UIAlertAction.Create("Aceptar", UIAlertActionStyle.Default, null));
                    //okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                    //Present Alert
                    WebViewController.GlobalViewController.PresentViewController(okCancelAlertController, true, null);

                }
            }
            catch(Exception)
            { }

        }
        public async void SearchMostWanted()
        {
            try
            {
                MAinViewJavascriptHandler.KeepQuering = false;
                // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask.Dispose();
                //  LocalWebViewClient_RestaurantDetailedView.taskController.Cancel();
                // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask = null;
                //  GlobalQueryTask
            }
            catch (Exception)
            { }

            HttpClient client = new HttpClient();

            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/BestSellersRestaurant?" +
                "RestaurantDetailID_Bestseller=" + MAinViewJavascriptHandler.static_RestID 


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

        public async void SearchByItsTagInDetailedView(int IntergerToquery)
        {
            try
            {
                MAinViewJavascriptHandler.KeepQuering = false;
                // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask.Dispose();
                // LocalWebViewClient_RestaurantDetailedView.taskController.Cancel();
                // LocalWebViewClient_RestaurantDetailedView.GlobalQueryTask = null;
                //  GlobalQueryTask
            }
            catch (Exception)
            { }
            HttpClient client = new HttpClient();

            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchInsideRestaurantProducts?" +
                "RestaurantDetailID_Regularseller=" + MAinViewJavascriptHandler.static_RestID +
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
            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/CarppiProductDetailedView_Compresed?" +
                "ProductDetailID_CompressedData=" + Index


                ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
            //t.Wait();

            var S_Response = t.Result;
            //EraseContentGrid

            Invoker.BeginInvokeOnMainThread(() =>
            {
                //var S_Response = t.Result;
                var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);
                // var asas = new SevenZip.Compression.LZMA.Decoder();

                Respuesta.Foto = Decompress(Respuesta.Foto);
                var script = "UpdateProductGrid(" + JsonConvert.SerializeObject(Respuesta) + ")";
                webi.EvaluateJavaScript(script, null);

                /*
                var S_Response = t.Result;
                var Respuesta = JsonConvert.DeserializeObject<Carppi_ProductosPorRestaurantes>(S_Response.Response);
                Respuesta.Foto = Decompress(Respuesta.Foto);
                var script = "UpdateProductGrid(" + JsonConvert.SerializeObject(Respuesta) + ")";

                webi.EvaluateJavaScript(script, null);
                */
                /*
                FirstViewRequestHandler.webi.EvaluateJavaScript(script, (result, error) =>
                {
                    if (error != null) Console.WriteLine(error);
                });*/
            });
            /*
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
            */
        }

        public void UpdateProductList(string ProductHash, int direction)
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

            var sss = MAinViewJavascriptHandler.ListaDeProductos.Where(X => X.ID == Cadena.ID).FirstOrDefault();
            if (sss == null)
            {

                Cadena.Cantidad = 1;
                MAinViewJavascriptHandler.ListaDeProductos.Add(Cadena);
            }
            else
            {
                if (sss.Cantidad == 1 && direction == -1)
                {
                    MAinViewJavascriptHandler.ListaDeProductos.Remove(sss);
                }
                else
                {
                    sss.Cantidad = sss.Cantidad + direction;
                }
            }
        }
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
        public void UpdateMap()
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
                        webi.EvaluateJavaScript(script, null);
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
                        }


                    }
                }
            }
            catch (Exception)
            { }
        }
        public async void SearchBackAllRestaurants()
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
                    */
                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiIOSRestaurantApi/CarppiRestaurantExistanceDetermination?" +
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

                                webi.EvaluateJavaScript(script, (s, e) => {
                                if (e != null)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            });
                        }
                        else if (S_Response.Response == null)
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
            
          
        }

        public void SearchByItsBox(long SearchType)
        {
            try
            {
                //PrintTagResponse
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
               

                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByTag?" +
                    "ServiceArea=" + 2
                    + "&availableFoodListing=" + SearchType


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                //t.Wait();

                var S_Response = t.Result;
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    var script = "PrintTagResponse(" + S_Response.Response + ")";

                    webi.EvaluateJavaScript(script, null);
                  
                }
            }
            catch (Exception)
            { }
        }
        public void SearchByText(string texttosearch)
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
             

                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/SearchByText?" +
                    "ServiceAreaText=" + 2
                    + "&TextMeantToesearched=" + texttosearch.ToLower()


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                //t.Wait();

                var S_Response = t.Result;
                if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
                {

                    var script = "PrintTagResponse(" + S_Response.Response + ")";

                    webi.EvaluateJavaScript(script, null);


                }
            }
            catch (Exception)
            { }
        }

        public async void RateFoodDeliverMan(int Rating, string Comentario)
        {
            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();

            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/RateDeliiverMan?" +
                "Order=" + OrderIDIfActive +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&Rating=" + Rating +
                "&Coment=" + Comentario

                ));
            HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            response = await client.GetAsync(uri);


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ReturnToRestaurantView();

                /*
                Android.Support.V4.App.Fragment fragment = null;
                fragment = FragmentSelectTypeOfPurchase.NewInstance();
                MainActivity.static_FragmentMAnager.BeginTransaction()
                   .Replace(Resource.Id.content_frame, fragment)
                   .Commit();

                */
                // MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

                /*
                var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
          {
                '"'
          });
                */
            }

        }
        public static void StaticReturnToRestaurantView()
        {
            ListaDeProductos = new List<CarppiGroceryProductos>();
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

            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/UserUIStateDetermination?" +
              "UserChain=" + FaceID


              ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
           // t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
            {


                WebViewController.Invoker.BeginInvokeOnMainThread(() =>
                {
                    string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                    var text = File.ReadAllText(index);


                    webi_Static.LoadHtmlString(text, null);


                    var c = new WebViewController();
                    c.ViewDidLoad();
                    // LoadAvailableProductsStartUp();
                });


             





            }
            else if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
            {

                var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Response.Response);
                OrderIDIfActive = Order.ID;
                if (Order.Stat == GroceryOrderState.RequestEnded)
                {
                    //ShowRateDeliverManRestaurant(this.Activity);
                }
                else
                {
                    WKWebViewConfiguration config = new WKWebViewConfiguration();
                    WKWebView webView = new WKWebView(Views_statis.Frame, config);
                    Views_statis.AddSubview(webView);
                    var messageHandler = new MainView_JavascriptInterface(webView, Views_statis);
                    config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


                    string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/FragmentGrocery_Map.html");
                    var text = File.ReadAllText(index);


                    webView.LoadHtmlString(text, null);
                    // messageHandler.LoadAvailableProductsStartUp();


                    stateOfRequest = stateOfRequestEnum.ShowwingMap;

                    aTimer = new System.Timers.Timer(2000);
                    // Hook up the Elapsed event for the timer. 
                    aTimer.Elapsed += OnTimedEvent;
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


        }

        public void ReturnToRestaurantView()
        {
            ListaDeProductos = new List<CarppiGroceryProductos>();
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
          
            var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/UserUIStateDetermination?" +
              "UserChain=" + FaceID


              ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
           // t.Wait();

            var S_Response = t.Result;
            if (S_Response.httpStatusCode == System.Net.HttpStatusCode.OK)
            {

                WKWebViewConfiguration config = new WKWebViewConfiguration();
                WKWebView webView = new WKWebView(Views.Frame, config);
                Views.AddSubview(webView);
                var messageHandler = new MainView_JavascriptInterface(webView,Views);
                config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


                string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/MainView.html");
                var text = File.ReadAllText(index);


                webView.LoadHtmlString(text, null);
                messageHandler.LoadAvailableProductsStartUp();
                
                



            }
            else if (S_Response.httpStatusCode == System.Net.HttpStatusCode.Accepted)
            {

                var Order = JsonConvert.DeserializeObject<CarppiGrocery_BuyOrders>(S_Response.Response);
                OrderIDIfActive = Order.ID;
                if (Order.Stat == GroceryOrderState.RequestEnded)
                {
                    //ShowRateDeliverManRestaurant(this.Activity);
                }
                else
                {
                    WKWebViewConfiguration config = new WKWebViewConfiguration();
                    WKWebView webView = new WKWebView(Views.Frame, config);
                    Views.AddSubview(webView);
                    var messageHandler = new MainView_JavascriptInterface(webView,Views);
                    config.UserContentController.AddScriptMessageHandler(messageHandler, name: "IOSInterface");


                    string index = Path.Combine(NSBundle.MainBundle.ResourcePath, "Views/FragmentGrocery_Map.html");
                    var text = File.ReadAllText(index);


                    webView.LoadHtmlString(text, null);
                   // messageHandler.LoadAvailableProductsStartUp();


                    stateOfRequest = stateOfRequestEnum.ShowwingMap;

                    aTimer = new System.Timers.Timer(2000);
                    // Hook up the Elapsed event for the timer. 
                    aTimer.Elapsed += OnTimedEvent;
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
     

        }
    

        private static System.Timers.Timer aTimer = null;
        public enum stateOfRequestEnum { ShowingRestaurants, ShowwingMap };
        public static stateOfRequestEnum stateOfRequest;

        public static long OrderIDIfActive = 0;
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
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
    public class DetailedProductViewFromRestauran
    {
        public IndiceDeRestaurante RestaurantData;
        public List<long> Products;
        public long FoodCategories;
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
}
