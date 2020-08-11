using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarppiRestaurant.Models;
using Newtonsoft.Json;
using Stripe;

namespace CarppiRestaurant.Controllers
{
    public class CarppiRestaurantAddCardController : Controller
    {
        // GET: CarppiRestautantAddAcount
        PidgeonEntities db = new PidgeonEntities();

        public ActionResult Index(string code)
        {
            
            try
            {
                ViewBag.Code = code;

                StripeConfiguration.ApiKey = "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";
                //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

                var options = new OAuthTokenCreateOptions
                {
                    GrantType = "authorization_code",
                    Code = code,
                };

                var service = new OAuthTokenService();
                var response = service.Create(options);


                // Push(JsonConvert.SerializeObject(response), "ke", Raul.FirebaseID, "");

                // Access the connected account id in the response
                var FaceID_speaker = Session["RestaurantID"].ToString();
                var Rest = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == FaceID_speaker).FirstOrDefault();
                Rest.StripeAccount = response.StripeUserId;
                Rest.StripeHash = response.StripeUserId;
                db.SaveChanges();
                //ViewBag.connected_account_id = response.StripeUserId;
                return View();
            }
            catch (Exception ex)
            {
                var Raul = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();//10217260473614661
                Push(ex.ToString(), "Error" , Raul.FirebaseID, "");
            }
            return View();
        }
        public void Push(string CuerpoMensaje, string Titulo, string token, string ExtraData)
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
