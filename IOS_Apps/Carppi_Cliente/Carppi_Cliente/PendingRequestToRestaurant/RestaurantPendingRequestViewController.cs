using System;
using System.IO;
using Foundation;
using UIKit;
using WebKit;

namespace Carppi_Cliente.PendingRequestToRestaurant
{
    public partial class RestaurantPendingRequestViewController : UIViewController
    {
        public RestaurantPendingRequestViewController() : base("RestaurantPendingRequestViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var fileName1 = "Views/PassengerExtraData.html";
            string localHtmlUrl1 = Path.Combine(NSBundle.MainBundle.BundlePath, fileName1);
            using (StreamReader lectura = new StreamReader(localHtmlUrl1))
            {
                Title = "Dashboard";
                View.BackgroundColor = UIColor.White;
                string filecontent = lectura.ReadToEnd();
                var config = new WKWebViewConfiguration();
                WKWebView webView = new WKWebView(View.Frame, config);
                
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


                webView.LoadHtmlString(filecontent, NSBundle.MainBundle.BundleUrl);




            }

            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

