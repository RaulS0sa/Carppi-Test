using CarppiWebService.Models;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CarppiWebService.Controllers
{
    public class CarppiPaymentController : Controller
    {
        // GET: CarppiPayment

        PidgeonEntities db = new PidgeonEntities();
        public enum typeOfFirstPayment { HomeworkPayment, TutorPayment };
        public ActionResult Index(double Amount, string User, int TripID )
        {
            var trip = db.Traveler_Viajes.Where(x => x.ID == (TripID)).FirstOrDefault();
            Session["UserName"] = User;
            Session["FaceID"] = trip.FaceIdDelConductor;
            Session["TripID"] = TripID;
            //  Session["InitialPayment"] = (int)InitialPayment;
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
            var Us = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User).FirstOrDefault();


            ViewBag.ClientName = Us.FirstName + " " + Us.LastName;
            var TutorToPay = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == trip.FaceIdDelConductor).FirstOrDefault();
            var service = new PaymentIntentService();
            var TutoriFee = (long)(((Amount*100) * 0.09) + 450);
            var servicecost = Amount * 100;
            var options = new PaymentIntentCreateOptions
            {
                Amount = (int)(servicecost),
                Currency = "mxn",
                CaptureMethod = "manual",
                PaymentMethodTypes = new List<string> { "card" },
                SetupFutureUsage = "off_session",
                ApplicationFeeAmount = TutoriFee,
                TransferData = new PaymentIntentTransferDataOptions
                {
                    Destination = TutorToPay.StripeDriverID
                }
            };
            PaymentIntent paymentIntent = service.Create(options);
           // Session["StripeChargeID"] = paymentIntent.Charges.FirstOrDefault().Id;
            Session["paymentIntentID"] = paymentIntent.Id;

            ViewBag.ClientSecret = paymentIntent.ClientSecret;
            var rnd = new Random();
            if (Us != null)
            {
              //  Us.PaymentIntentSha256 = ComputeSha256Hash(paymentIntent.ClientSecret);
            }
            // var User = db.TutoriUsuarios.Where(x => x.PaymentIntentSha256 == hashToSearch).FirstOrDefault();
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
                    //var service = new PaymentIntentService();
                    //var service = new PaymentIntent();
                    //var customer = new CustomerService();
                    Customer customer = service1.Create(options1);
                    Us.StripeClientID = customer.Id;
                    //-------------------------------------------------------
                    //var options_PaymentMethod = new PaymentMethodCreateOptions
                    //{
                    //    Type = "card",
                    //    Customer = Us.StripeClientID
                    //    //Card = new PaymentMethodCardOptions
                    //    //{
                    //    //    Number = "4242424242424242",
                    //    //    ExpMonth = 2,
                    //    //    ExpYear = 2021,
                    //    //    Cvc = "314",
                    //    //},
                    //};
                    //var service_createPayment = new PaymentMethodService();
                    //var Rrr = service_createPayment.Create(options_PaymentMethod);
                    ////var paymentMethod = service2.Attach(options_PaymentMethod);
                    //Us.StripePaymentMethodID = Rrr.Id; //paymentIntent.PaymentMethodId;
                    db.SaveChanges();
                    //var options2 = new PaymentMethodAttachOptions
                    //{
                    //    Customer = Us.StripeClientID,
                    //};

                    //var paymentMethod = service2.Attach(paymentIntent.PaymentMethodId, options2);
                    //Us.StripePaymentMethodID = paymentMethod.Id; ;
                    //db.SaveChanges();
                }
                else
                {

                    //var options_ = new PaymentMethodAttachOptions
                    //{
                    //    Customer = Us.StripeClientID,
                    //};
                    //var service_ = new PaymentMethodService();


                    //var paymentMethod = service_.Attach(paymentIntent.PaymentMethodId, options_);

                    //var paymentMethod = service2.Attach(options_PaymentMethod);

                    //db.SaveChanges();
                }
                db.SaveChanges();
            }
            db.SaveChanges();
            ViewBag.Cost =(100* (Amount + ((Amount * 0.09) + 4.50))) / 100;//(int)(rnd.NextDouble() * 1000);
            Session["Debt"] = ((Amount*100) / 1).ToString();//(int)(rnd.NextDouble() * 1000);


            //var options3 = new WebhookEndpointCreateOptions
            //{
            //    Url = "http://localhost:51895/StripeWebHook",
            //    EnabledEvents = new List<String> { "charge.failed", "charge.succeeded" },
            //};
            //var service3 = new WebhookEndpointService();
            //var webhookEndpoint = service3.Create(options3);


            return View();
        }

        public enum statesOfRequest { RequestCreated, RequestAccepted, RequestCanceled, };
        public enum enumTipoDePAgoPreferido { Tarjeta, EnEfectivo, Ambos };
        public enum enumEstado_de_Solicitud { EnEspera, Aceptado, Rechazado, Conduciendo, NoLlego };
        [HttpPost]
        public JsonResult UpdatePaymentMethod(string SuperID)
        {

            var TripID = Convert.ToInt32(Session["TripID"].ToString());

            string User = Session["UserName"].ToString();
            var Us = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User).FirstOrDefault();
            string FaceID = Session["FaceID"].ToString();
            var driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID).FirstOrDefault();
           // typeOfFirstPayment InitialPayment = (typeOfFirstPayment)Convert.ToInt32(Session["InitialPayment"].ToString());
            string paymentIntentID = Session["paymentIntentID"].ToString();

            int Debt = Convert.ToInt32(Session["Debt"].ToString());

            var TripRequest = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == User && x.Id_del_viaje == TripID).FirstOrDefault();

            if (TripRequest == null)
            {
                var New_trip_request = new Traveler_SolicitudDeViajeTemporal();
                New_trip_request.Face_id_solicitante = User;
                New_trip_request.Id_del_viaje = TripID;
                New_trip_request.Estado_De_solicitud = (int)(enumEstado_de_Solicitud.EnEspera);
                New_trip_request.TipoDePago = (int)enumTipoDePAgoPreferido.Tarjeta;
                New_trip_request.MoneyOwnedToDriver = Debt.ToString();
                New_trip_request.PaymentIntentID = paymentIntentID;
                //New_trip_request.StripeCharge = Session["StripeChargeID"].ToString();
                db.Traveler_SolicitudDeViajeTemporal.Add(New_trip_request);
                db.SaveChanges();


                //Envia Notificacion
                var Viaje = db.Traveler_Viajes.Where(x => x.ID == TripID).FirstOrDefault();
                var IdConductor = Viaje.FaceIdDelConductor;
                var Conductor = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == IdConductor).FirstOrDefault();
                Push("Abre la app para ver quien quiere reservar un viaje", "Tienes una solicitud!", Conductor.FirebaseID);
            }
            else
            {

            }

           // return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");



            //if (InitialPayment == typeOfFirstPayment.TutorPayment)
            //{
            //    var Request_ = db.TutoriRequestForTutorings.Where(x => x.FaceID_OfSolicitant == Us.FaceID && x.FaceID_OfTutor == Tutor.FaceID).FirstOrDefault();
            //    if (Request_ == null)
            //    {
            //        var Rrequest = new TutoriRequestForTutoring();
            //        Rrequest.FaceID_OfSolicitant = Us.FaceID;
            //        Rrequest.FaceID_OfTutor = Tutor.FaceID;
            //        Rrequest.State_OfRequest = (int)statesOfRequest.RequestCreated;
            //        Rrequest.MoneyOwnedTotutor = Debt;
            //        Rrequest.PaymentIntentID = paymentIntentID;
            //        Rrequest.StripeCharge = Session["StripeChargeID"].ToString();


            //        db.TutoriRequestForTutorings.Add(Rrequest);
            //        db.SaveChanges();


            //    }

            //}


            var options_ = new PaymentMethodAttachOptions
            {
                Customer = Us.StripeClientID,
            };
            var service_ = new PaymentMethodService();


            var paymentMethod = service_.Attach(SuperID, options_);
            return Json("Success");

        }
        public static string ComputeSha256Hash(string rawData)
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
        public void Push(string CuerpoMensaje, string Titulo, string token)
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
                    sound = "Enabled",
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