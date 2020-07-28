
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Newtonsoft.Json;
using SQLite;
using Fragment = Android.Support.V4.App.Fragment;

namespace Carppi.Fragments
{
    public class FragmentAddCard : Fragment
    {
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
            // act = this.Activity;
            //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))
            var Template = "AddItemToMenuView.html";
            //var Template = "ShoppingOptions.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            //Template = "ShoppingCatalog.html";//!isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            using (StreamReader sr = new StreamReader(assets.Open(Template)))
            {
                content = sr.ReadToEnd();
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new WebInterfaceAddCardToRestaurant(this.Activity, webi);
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
                webi.LoadUrl("https://dashboard.stripe.com/express/oauth/authorize?response_type=code&client_id=ca_HefGcaqsN1stYmIBiakDDfvPcpEIfEk6&scope=read_write#/");
                //webi.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                // webi.LoadData(content, "text/html", null);
                  webi.SetWebViewClient(new AddCardCustomWebClient());
                // WebInterfaceProfile.RetriveProfile();

                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView

            }

            return view1;
            /*
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
            */

            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public static FragmentAddCard NewInstance()
        {
            var frag1 = new FragmentAddCard { Arguments = new Bundle() };
            return frag1;
        }
    }

    public class AddCardCustomWebClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return false;
            //return base.ShouldOverrideUrlLoading(view, url);
        }
    }
    public class WebInterfaceAddCardToRestaurant : Java.Lang.Object
    {
        Context mContext;
        public static Context Static_mContext;
        WebView webi;
        public static WebView Static_Webi;
        //private ICallbackManager mFBCallManager;
       
        private static MyProfileTracker mprofileTracker;
       
        public static facebookCallback facebookCallback_Static;
        public static Int64 OrderID;
        public enum ActivityIntentREturns { UpdateProfilePicture = 1000, SelecctionOfHomeworkPicture = 1100, SelectEcomercePicture = 1200 };
        public static byte[] Photo { get; set; }

        public WebInterfaceAddCardToRestaurant(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            Static_Webi = web;
            Static_mContext = Act;
            OrderID = 0;


        }
        public enum IndexOfConnectedAccount { Restaurant, Deliverman };//serviceProvider


        //TransferrMyMoney
        [JavascriptInterface]
        [Export("TransferrMyMoney")]
        public async void TransferrMyMoney()
        {//public HttpResponseMessage ValidateServiceVoce(string ServiceProviderCodeHash, string CodeToValidate, IndexOfConnectedAccount serviceProvider)
            Action action = async () =>
            {
                try
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/TransferToAcoount?" +
                        "ServiceProviderToTransferHash=" + HAsh +
                        "&TypeOfAcoount=" + ((int)IndexOfConnectedAccount.Restaurant).ToString()
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                    // t.Wait();
                    var S_Ressult = t.Result;
                    if(S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Action action = () =>
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                            alert.SetTitle("Respuesta");
                            alert.SetMessage("Tu Transferencia ha sido exitosa, puedes cerrar esta ventana");



                         
                          
                            alert.SetNeutralButton("Cerrar dialogo", (senderAlert, args) =>
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
                    else if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        Action action = () =>
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                            alert.SetTitle("Respuesta");
                            alert.SetMessage("Ha habido un error, intenta mas tarde o contacta a soporte");





                            alert.SetNeutralButton("Cerrar dialogo", (senderAlert, args) =>
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
                    else if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Action action = () =>
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                            alert.SetTitle("Error");
                            alert.SetMessage("No hay una cuenta bancaria asociada a tu restaurante");





                            alert.SetNeutralButton("Cerrar dialogo", (senderAlert, args) =>
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
                    else if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        Action action = () =>
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                            alert.SetTitle("Error");
                            alert.SetMessage("No puedes hacer transferencias, o sentimos");





                            alert.SetNeutralButton("Cerrar dialogo", (senderAlert, args) =>
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

                    //  var Response = JsonConvert.DeserializeObject<bool?>(S_Ressult.Response);
                    /*
                      Action action = () =>
                      {
                          //var jsr = new JavascriptResult();
                          var script = "IsCodeCorrect(" + (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK).ToString().ToLower() + ")";
                          webi.EvaluateJavascript(script, null);


                      };


                      webi.Post(action);
                    */



                }
                catch (Exception)
                { }
            };
            ((Activity)mContext).RunOnUiThread(action);


        }




        [JavascriptInterface]
        [Export("QueryChainedNumber")]
        public async void QueryChainedNumber(string ChainedNumber)
        {//public HttpResponseMessage ValidateServiceVoce(string ServiceProviderCodeHash, string CodeToValidate, IndexOfConnectedAccount serviceProvider)
            Action action = async () =>
            {
                try
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/ValidateServiceVoce?" +
                        "ServiceProviderCodeHash=" + HAsh+
                        "&CodeToValidate=" + ChainedNumber
                        + "&serviceProvider=" + ((int)IndexOfConnectedAccount.Restaurant).ToString()
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                    // t.Wait();
                    var S_Ressult = t.Result;
                  

                    var Response = JsonConvert.DeserializeObject<bool?>(S_Ressult.Response);
                    Action action = () =>
                    {
                        //var jsr = new JavascriptResult();
                        var script = "IsCodeCorrect(" + (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK).ToString().ToLower() + ")";
                        webi.EvaluateJavascript(script, null);


                    };


                    webi.Post(action);



                }
                catch (Exception)
                { }
            };
            ((Activity)mContext).RunOnUiThread(action);


        }

        //ChangeToStipeView
        [JavascriptInterface]
        [Export("ChangeToStipeView")]
        public async void ChangeToStipeView()
        {
            Android.Support.V4.App.Fragment fragment = null;
            fragment = FragmentAddCard.NewInstance();
            MainActivity.static_FragmentMAnager.BeginTransaction()
               .Replace(Resource.Id.content_frame, fragment)
               .Commit();

        }




        [JavascriptInterface]
        [Export("UpdateConectedIDStripe")]
        public async void UpdateConectedIDStripe(string ConnectedAccoundID)
        {
            MainActivity.StripeScreenNotSet = true;
            //public HttpResponseMessage UpdateStripeServiceProviderID(string ServiceProviderHash, string StripeComercianteID, IndexOfConnectedAccount serviceProvider)
            Action action = async () =>
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));

                HttpClient client = new HttpClient();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/UpdateStripeServiceProviderID?" +
                         "ServiceProviderHash=" + HAsh +
                         "&StripeComercianteID=" + ConnectedAccoundID
                         + "&serviceProvider=" + ((int)IndexOfConnectedAccount.Restaurant).ToString()
                         ));
                
                HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(uri);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                   // MainActivity.LoadFragmentStatic(Resource.Id.nav_messages);


                    //MainActivity.Activity_1.LoadFragment(Resource.Id.menu_audio);
                }
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
}
