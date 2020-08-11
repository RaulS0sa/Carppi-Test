using CarppiWebService.Models;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CarppiWebService.Controllers
{
    public class CarppiDeliveryAddCardController : Controller
    {
        // GET: CarppiDeliveryAddCard
        PidgeonEntities db = new PidgeonEntities();
        public ActionResult Index(string User)
        {

            StripeConfiguration.ApiKey = "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";
            var Us = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User).FirstOrDefault();

            // Set your secret key. Remember to switch to your live secret key in production!
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            // StripeConfiguration.ApiKey = "sk_test_51H5LXPKb0TehYbrqUWBlmlV1geXY11IQMFpRiqQz6jxItHfET5PLGjQrid5iq469fBSeO0VhXu8YSs4n0cQgiUFF00J27QK5vu";

            var options1 = new CustomerCreateOptions
            {
                //PaymentMethod = paymentIntent.PaymentMethodId,
                Email = Us.Correo,
                Name = Us.FirstName + " " + Us.LastName
            };
            var service1 = new CustomerService();

            Customer customer = service1.Create(options1);
            Us.StripeClientID = customer.Id;
            Session["UserDbID"] = User;

            db.SaveChanges();

            //https://localhost:44323/CarppiDeliveryAddCard/Index?user=10217260473614661
            db.SaveChanges();

            /*

            ViewBag.ClientName = Us.FirstName + " " + Us.LastName;
           
            var service = new PaymentIntentService();
           
           
            var options = new PaymentIntentCreateOptions
            {
                Amount = (int)(1000),
                Currency = "mxn",
                CaptureMethod = "manual",
                PaymentMethodTypes = new List<string> { "card" },
                SetupFutureUsage = "off_session",
                // ApplicationFeeAmount = TutoriFee,

            };
            PaymentIntent paymentIntent = service.Create(options);
            Us.StripeLastPaymentIntent_ID = paymentIntent.Id;
            // Session["StripeChargeID"] = paymentIntent.Charges.FirstOrDefault().Id;
            Session["paymentIntentID"] = paymentIntent.Id;

            ViewBag.ClientSecret = paymentIntent.ClientSecret;
            var rnd = new Random();
         
          
            if (Us != null)
            {
                if (Us.StripeClientID == null)
                {
                    var options1 = new CustomerCreateOptions
                    {
                        PaymentMethod = paymentIntent.PaymentMethodId,
                        Email = Us.Correo,
                        Name = Us.FirstName + " " + Us.LastName
                    };
                    var service1 = new CustomerService();
                 
                    Customer customer = service1.Create(options1);
                    Us.StripeClientID = customer.Id;
                   
                    db.SaveChanges();
                  
                }
                else
                {

                 
                }
                db.SaveChanges();
            }
            db.SaveChanges();
         
            */


            return View();
        }

        public JsonResult UpdatePaymentMethod(string number, string Expiry, string cvc)
        {
          
            /*var LatitudObjectivo = Convert.ToDouble(Session["Latitud"]);
            var LongitudObjetivo = Convert.ToDouble(Session["Longitud"]);
            var LatitudOrigen = Convert.ToDouble(Session["LatitudOrigen"]);
            var LongitudOrigen = Convert.ToDouble(Session["LongitudOrigen"]);
            var Destino = Session["NombreDestino"].ToString();
            */
            // enumGender Gender = (enumGender)Convert.ToInt32(Session["Gender"]);

            //var TripID = Convert.ToInt32(Session["TripID"].ToString());
            /*
            string User = Session["UserName"].ToString();
            var Us = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User).FirstOrDefault();
            Us.PaymentMethod = SuperID;
            db.SaveChanges();
            //   string FaceID = Session["FaceID"].ToString();
            //   var driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID).FirstOrDefault();
           // Int32 ServiceArea = Convert.ToInt32(Session["ServiceArea"]);
           // var Debt = Convert.ToDouble(Session["Debt"].ToString());
            var PaymentIntntId = Session["paymentIntentID"].ToString();
            */
            //            CreateTripRequest(PaymentIntntId, Us, ServiceArea, (int)Debt, LatitudObjectivo, LongitudObjetivo, Destino, Gender, LatitudOrigen, LongitudOrigen);
            try
            {
                StripeConfiguration.ApiKey = "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";
                // StripeConfiguration.ApiKey = "sk_test_51H5LXPKb0TehYbrqUWBlmlV1geXY11IQMFpRiqQz6jxItHfET5PLGjQrid5iq469fBSeO0VhXu8YSs4n0cQgiUFF00J27QK5vu";
                var strarr = Expiry.Split('/');
                var expmonth = Convert.ToInt64(strarr[0]);
                var expyear = Convert.ToInt64(strarr[1]);
                var options = new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardCreateOptions
                    {
                        Number = number,
                        ExpMonth = expmonth,
                        ExpYear = expyear,
                        Cvc = cvc,
                    },
                };
                var service = new PaymentMethodService();
                var paymentmethosresult = service.Create(options);
                var Destino = Session["UserDbID"].ToString();
                var Us = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Destino).FirstOrDefault();
                Us.PaymentMethod = paymentmethosresult.Id;

                var options_ = new PaymentMethodAttachOptions
                {
                    Customer = Us.StripeClientID,
                };
                var service_ = new PaymentMethodService();


                var paymentMethod = service_.Attach(paymentmethosresult.Id, options_);

                db.SaveChanges();

            }
            catch (Exception ep)
            {
                var ME = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();
                Push(ep.Message, "failed to send email with the following error:", ME.FirebaseID, "");
                return Json("Failure");
            }
            //RefundOnCancelSolicitud(PaymentIntntId);
            return Json("Success");

        }
        public bool RefundOnCancelSolicitud(string Solicitud)
        {
            bool r_turn = false;
            try
            {
                StripeConfiguration.ApiKey = "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";

                var service = new PaymentIntentService();
                var options = new PaymentIntentCancelOptions { };
                var intent = service.Cancel(Solicitud, options);

                //var refunds = new RefundService();
                //var refundOptions = new RefundCreateOptions
                //{
                //    PaymentIntent = c
                //};
                // var refund = refunds.Create(refundOptions);
                r_turn = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return r_turn;


        }

        void Push(string CuerpoMensaje, string Titulo, string token, string ExtraData)
        {
            string json = "";
            //jsonObject.materias = informacion.Materias;
            var data_ = new
            {
                to = token,

                notification = new
                {
                    body = CuerpoMensaje,
                    title = Titulo,
                    sound = ExtraData,
                    message = " "
                }
            };
            json = JsonConvert.SerializeObject(data_);


            //// topicos



            //   string serverKey = "AAAAxf154jQ:APA91bFhBQCh-kWqddgCVPeB_KVJBaQf_03vcP0dx4gYzJimNMX3i2ErzBOghPfaGmP8H--HHmFk3r3_vZxcf4qPAuarv6fh-Uaq3fA3ibUvg8ox6jsKfMeOj2ipYKqUaKZkrOC21BOr";
            //     string serverKey = "AAAAo6FSX9k:APA91bGYVwMJ58zr2lGlKThP46WvAxHKQdWpgq3vRhWn_iESF__N58gYnzGJcorgh6iAjwS-9gnL9_zcF-ID6SlWx2LLEIyfpUjttXn2LDZ0MkKaKl0eFz_eoYN9AgYl0Obl2EbiLv-x";
            string serverKey = "AAAAytq6L3w:APA91bFZ_9edh77Kz3Au2xBLL2shnCwbPayGPUlPKvM6-HCFi7bpqqul9CmFLeX09a8jaJRUT-uvnhGNDyEo2-X0TrBmhawzUh-6xeVbnfrcaWdetH-T8SauwcTTwIg5aShBXmdn9Vlk";
            var result_ = "-1";

            try
            {

                var webAddr = "https://fcm.googleapis.com/fcm/send";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    // string json = "{\"to\": \"/topics/news\",\"data\": {\"message\": \"This is a Firebase Cloud Messaging Topic Message!\",}}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result_ = streamReader.ReadToEnd();
                }

                // return result;
            }
            catch (Exception ex)
            {
                result_ = ex.ToString();
                //  Response.Write(ex.Message);
            }


        }
    }
}