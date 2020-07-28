
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
using SQLite;
using Fragment = Android.Support.V4.App.Fragment;

namespace Carppi.Fragments
{
    public class FragmentAddCardConfirmMail : Fragment
    {
        private object view1;

        public enum IndexOfConnectedAccount { Restaurant, Deliverman};//serviceProvider

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);
            MainActivity.StripeScreenNotSet = false;
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            string content;
            AssetManager assets = this.Activity.Assets;
            // act = this.Activity;
            //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))
            var Template = "6DigitTemplate.html";
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
                //webi.LoadUrl("https://dashboard.stripe.com/express/oauth/authorize?response_type=code&client_id=ca_HefGcaqsN1stYmIBiakDDfvPcpEIfEk6&scope=read_write#/");
                webi.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                // webi.LoadData(content, "text/html", null);
                webi.SetWebViewClient(new CodeValidatemWebClient());
                // WebInterfaceProfile.RetriveProfile();

                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView

            }

            return view1;

            // return base.OnCreateView(inflater, container, savedInstanceState);
        }
        public class CodeValidatemWebClient : WebViewClient
        {/*
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return false;
                //return base.ShouldOverrideUrlLoading(view, url);
            }
            */
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                try
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                    var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                    HttpClient client = new HttpClient();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantRequestApi/GenerateValidationCodeForStripeValidation?" +
                        "ServiceProviderHash=" + HAsh
                        + "&serviceProvider=" + ((int)IndexOfConnectedAccount.Restaurant).ToString()
                        ));


                    var t = Task.Run(() => GetResponseFromURI(uri));
                    // t.Wait();
                    var S_Ressult = t.Result;
                    if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        /*
                        var Response = JsonConvert.DeserializeObject<bool?>(S_Ressult.Response);
                        Action action = () =>
                        {
                            //var jsr = new JavascriptResult();
                            var script = "UpdateOprionsButton(" + S_Ressult.Response + ")";
                            s_Webview.EvaluateJavascript(script, null);


                        };


                        s_Webview.Post(action);


                        RestaurantCatalogSequentialLoad();
                        */
                    }



                }
                catch (Exception)
                { }
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

        public static FragmentAddCardConfirmMail NewInstance()
        {
            var frag1 = new FragmentAddCardConfirmMail { Arguments = new Bundle() };
            return frag1;
        }

    }
}
