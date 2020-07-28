using System;
using Foundation;
using WebKit;

namespace Carppi_Cliente.MainViewJavasciptInterface
{
    public class DetailedViewJavascriptHandler : NSObject, IWKScriptMessageHandler
    {
        public WKWebView webi;

        // public IntPtr Handle => throw new NotImplementedException();

        public DetailedViewJavascriptHandler(WKWebView arg1)
        {
            webi = arg1;
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
           // throw new NotImplementedException();
        }
    }
}
