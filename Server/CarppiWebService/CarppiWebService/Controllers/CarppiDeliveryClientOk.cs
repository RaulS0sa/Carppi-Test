using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoLoc.Controllers
{
    public class CarppiDeliveryClientOkController : Controller
    {
        // GET: CarppiOK
        public ActionResult Index(string sessionId)
        {
            /*
            ViewBag.Code = code;
            ViewBag.State = state;
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";

            var options = new OAuthTokenCreateOptions
            {
                GrantType = "authorization_code",
                Code = code,
            };

            var service = new OAuthTokenService();
            var response = service.Create(options);

            // Access the connected account id in the response
            ViewBag.connected_account_id = response.StripeUserId;

            */
            return View();
        }
    }
}