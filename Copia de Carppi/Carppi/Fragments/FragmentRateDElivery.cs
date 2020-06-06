
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
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Newtonsoft.Json;
using SQLite;
using static Carppi.Fragments.LocalWebViewClient_RestaurantDetailedView;
using Fragment = Android.Support.V4.App.Fragment;

namespace Carppi.Fragments
{
    public class FragmentRateDElivery : Fragment
    {
        public static long OrderIDIfActive = 0;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);

            string content;
            AssetManager assets = this.Activity.Assets;
            var sss = view1.FindViewById<WebView>(Resource.Id.webView_);

            sss.Settings.JavaScriptEnabled = true;

            sss.Settings.DomStorageEnabled = true;
            sss.Settings.LoadWithOverviewMode = true;
            sss.Settings.UseWideViewPort = true;
            sss.Settings.BuiltInZoomControls = true;
            sss.Settings.DisplayZoomControls = false;
            sss.Settings.SetSupportZoom(true);
            sss.Settings.JavaScriptEnabled = true;

           // AssetManager assets = ((Activity)Esta).Assets;
            //string content;
            var Viewww = new UtilityJavascriptInterface_RestaurantDetailedView(this.Activity, sss);
            sss.AddJavascriptInterface(Viewww, "Android_BottomModal");
            //using (StreamReader sr = new StreamReader(assets.Open("ShoppingKart.html")))
            using (StreamReader sr = new StreamReader(assets.Open("RateFoodDeliverMan.html")))
            {
                content = sr.ReadToEnd();
                // var cadea = GenerateRowsForCheckOutModal(FragmentRestaurantDetailedView.ListaDeProductos);
                sss.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

            }
            sss.SetWebViewClient(new FragmentMain.LocalWebViewClient());

            //<span class="woocommerce-Price-currencySymbol">£</span>90.00</span>

           

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
       


        public static FragmentRateDElivery NewInstance()
        {
            var frag1 = new FragmentRateDElivery { Arguments = new Bundle() };
            return frag1;
        }
    }
   
      


    }

   


