
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SQLite;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using static Carppi.Fragments.FragmentMain;
using static Carppi.Fragments.WebInterfaceFragmentGrocery;
using Fragment = Android.Support.V4.App.Fragment;
namespace Carppi.Fragments
{
    public class FragmentAddElement : Fragment
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
            var Template = "AddItemToMenuView.html";
            //var Template = "ShoppingOptions.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            //Template = "ShoppingCatalog.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            using (StreamReader sr = new StreamReader(assets.Open(Template)))
            {
                content = sr.ReadToEnd();
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new WebInterfaceAddProduct(this.Activity, webi);
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
              //  webi.SetWebViewClient(new GroceryWebClient(this.Activity, Resources));
               // WebInterfaceProfile.RetriveProfile();

                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView

            }

            return view1;
            // return base.OnCreateView(inflater, container, savedInstanceState);
        }
        private static System.Timers.Timer aTimer;
     
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //WebInterfaceFragmentGrocery.UpdateGroceryMapState();
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

       

        public static FragmentAddElement NewInstance()
        {
            var frag1 = new FragmentAddElement { Arguments = new Bundle() };
            return frag1;
        }
        public bool isLoggedIn()
        {
           
              AccessToken accessToken = AccessToken.CurrentAccessToken;//AccessToken.getCurrentAccessToken();
              return accessToken != null;
            //return false;
        }
    }
    public class WebInterfaceAddProduct: Java.Lang.Object
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
        public enum ActivityIntentREturns { UpdateProfilePicture = 1000, SelecctionOfHomeworkPicture = 1100, SelectEcomercePicture = 1200 };
        public static byte[] Photo { get; set; }

        public WebInterfaceAddProduct(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            Static_Webi = web;
            Static_mContext = Act;
            OrderID = 0;


        }
        [JavascriptInterface]
        [Export("FetchAllTehData")]
        public async void FetchAllTehData(string Titulo, string Descripcion, string Costo, string tags, int Categoria)
        {
            Console.WriteLine(Titulo);
            Console.WriteLine(Descripcion);
            Console.WriteLine(Costo);
            Console.WriteLine(tags);
       if(Titulo != "" && Descripcion != "" && Costo != "" && Photo != null)
            {
                var Str_arr = string.Join(",", tags.Split(',').Where(val => val != "undefined").ToArray());

                await AddProducto(Titulo, Descripcion, Costo, "", Categoria);

            }
            else
            {
                Action action = () =>
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                    alert.SetTitle("Error");
                    alert.SetMessage("Te falta descripcion, El nombre de tu producto o el costo");



                    alert.SetNegativeButton("Aceptar", (senderAlert, args) =>
                    {
                        StopFetcherButton();
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
        [JavascriptInterface]
        [Export("SelecImage_HomeWork")]
        public async void SelecImage_HomeWork()
        {
            var Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            //Fragment_profile.OnActivityResult(1, 1, Intent.CreateChooser(Intent, "Select Picture"));
            ((Activity)mContext).StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)WebInterfaceAddProduct.ActivityIntentREturns.SelecctionOfHomeworkPicture);
            //Fragment_profile.act.StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), 1000);

        }
        async Task AddProducto(string Titulo, string Descripcion, string Costo, string tags, int Categoria)
        {
            /*
             *

                    <option value="none">Selecciona tu Categoria</option>
            <option value="Hamburguesa">Hamburguesa</option>
            <option value="Torta">Torta</option>
            <option value="Taco">Taco</option>
            <option value="Burrito">Burrito</option>
             */

            try
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));
                HttpClient Client = new HttpClient();
                Uri webService = new Uri("https://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/AddRestaurantItem?"
                    + "ProductCategory=" + Categoria
                    + "&CostTotal=" + Costo
                    + "&nombre=" + Titulo
                    + "&descriptcion=" + Descripcion
                    + "&restaurante=" + HAsh);
                using (var content = new MultipartFormDataContent())
                {

                    //HttpContent metaDataContent = new ByteArrayContent(File);
                    content.Add(CreateFileContent(new MemoryStream(Photo), "image.jpg", "image/jpeg"));
                     using (var message = await Client.PostAsync(webService, content))
                    {
                        Console.WriteLine(message.ReasonPhrase.ToLower());
                        
                        if (message.ReasonPhrase.ToLower() == "Created".ToLower())
                        {
                            //await AddFilesToHomeWork(Convert.ToInt32(message.Content.ReadAsStringAsync().Result));
                            StopFetcherButtonAndReload();
                            content.Dispose();
                        }
                        else
                        {
                            StopFetcherButton();
                        }
                    }

                  
                }
            }
            catch (System.Exception e)
            {
                StopFetcherButton();
                Console.WriteLine(e.ToString());
            }
            
        }
        async Task AddFilesToHomeWork(int IDOFHomework)
        {
            /*
            try
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "LogTutori.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_Info>().Where(v => v.ID > 0).FirstOrDefault();
                HttpClient Client = new HttpClient();
                Uri webService = new Uri("http://cataes.azurewebsites.net/api/TutoriApi/UploadFile?FileToBeUploaded_IDofFatherHomework=" + IDOFHomework + "&FileName=" + FileName);
                using (var content = new MultipartFormDataContent())
                {

                    HttpContent metaDataContent = new ByteArrayContent(File);

                    
                    content.Add(CreateFileContent(new MemoryStream(File), "file", "application/octet-stream"), "file", FileName);

                    using (var message = await Client.PostAsync(webService, content))
                    {
                        Console.WriteLine(message.ReasonPhrase.ToLower());
                        StopFetcherButton();
                        if (message.ReasonPhrase.ToLower() == "Created".ToLower())
                        {

                           
                            content.Dispose();
                        }
                    }

                }
            }
            catch (System.Exception e)
            {
                StopFetcherButton();
                Console.WriteLine(e.ToString());
            }
            */
            
        }

        private StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"files\"",
                FileName = "\"" + fileName + "\""
            }; // the extra quotes are key here
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }
        [JavascriptInterface]
        [Export("UpdateBottonOfImageSelect")]
        public static async void UpdateBottonOfImageSelect(string ImageName)
        {
            var script = "UpdateTextInPhotoSelectionOfButton('" + ImageName + "','" + Convert.ToBase64String(Photo) + "');";
            Action action = () =>
            {

                Static_Webi.EvaluateJavascript(script, null);

            };



            Static_Webi.Post(action);
        }
        //ReloadUpdateProduct
        [JavascriptInterface]
        [Export("ReloadUpdateProduct")]
        public static async void ReloadUpdateProduct()
        {
            MainActivity.LoadFragment_Static(Resource.Id.menu_video);
        }
        //StopFetcherButtonAndReload
        [JavascriptInterface]
        [Export("StopFetcherButtonAndReload")]
        public static async void StopFetcherButtonAndReload()
        {
            var script = "StopFetcherButtonAndReload();";
            Action action = () =>
            {

                Static_Webi.EvaluateJavascript(script, null);

            };



            Static_Webi.Post(action);
        }

        [JavascriptInterface]
        [Export("StopFetcherButton")]
        public static async void StopFetcherButton()
        {
            var script = "StopFetcherButton();";
            Action action = () =>
            {

                Static_Webi.EvaluateJavascript(script, null);

            };



            Static_Webi.Post(action);
        }




    }









}
