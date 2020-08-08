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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace CarppiWebService.Controllers
{
    public class CarppiAddCardController : Controller
    {
        // GET: CarppiAddCard
        PidgeonEntities db = new PidgeonEntities();
        public enum typeOfFirstPayment { HomeworkPayment, TutorPayment };
        public enum enumEstado_del_usuario { Sin_actividad, PidiendoViajePasajero, ViajeEncontradoPasajero, ViajandoPasajero, EntregadoPasajero, PidiendoViajeConductor, ViajandoConductor, ViajeAceptadoNoRecogido_Conductor, PasajeroSinArribar, PasajeroDejoPlantado, ViajandoPasajeroRideShare, ConductorEsperando, EsperandoCalificar, BusquedaEnProceso, DisponibleParaRideshare, EsperandoCalificarRideshare, Sin_actividadRideShare, ViajeNoEncontradoUsuario, PasajeroEsperandoconductorRideshare, BusquedaEnProcesoRideShare };
        public enum StateOfRdeshare { RequestCreated, RequestAccepted, RequestCanceled };
        public enum enumRoles { Pasajero, Conductor, Observador, RideDriver };

        public enum enumGender { Male, Female, Genderless };


        public ActionResult Index(double Amount, string User, Int32 ServiceArea, double LatitudObjectivo, double LongitudObjetivo, string NombreDestino, enumGender Gender, double LatitudOrigen, double LongitudOrigen)
        {
            //var trip = db.Traveler_Viajes.Where(x => x.ID == (TripID)).FirstOrDefault();
            Session["UserName"] = User;
            Session["Gender"] = (int)Gender;
            Session["ServiceArea"] = ServiceArea.ToString();
            Session["Latitud"] = LatitudObjectivo.ToString();
            Session["Longitud"] = LongitudObjetivo.ToString();
            Session["LatitudOrigen"] = LatitudOrigen.ToString();
            Session["LongitudOrigen"] = LongitudOrigen.ToString();
            Session["NombreDestino"] = NombreDestino;
            //  Session["FaceID"] = trip.FaceIdDelConductor;
            //  Session["TripID"] = TripID;
            //  Session["InitialPayment"] = (int)InitialPayment;
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
           // StripeConfiguration.ApiKey = "sk_test_6P04GsrUgxVEdGIkt43NXflH00awsFl7rR";
            var Us = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User).FirstOrDefault();


            ViewBag.ClientName = Us.FirstName + " " + Us.LastName;
            //   var TutorToPay = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == trip.FaceIdDelConductor).FirstOrDefault();
            var service = new PaymentIntentService();
            var TutoriFee = (long)(((Amount * 100) * 0.1) + 450);
            var servicecost = Amount * 100;
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
            ViewBag.Cost = ((int)(100 * Amount))/ 100.0;//(int)(rnd.NextDouble() * 1000);
            Session["Debt"] = ((Amount *100) / 1).ToString();//(int)(rnd.NextDouble() * 1000);


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
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
            var LatitudObjectivo =Convert.ToDouble( Session["Latitud"]);
            var LongitudObjetivo = Convert.ToDouble(Session["Longitud"]);
            var LatitudOrigen = Convert.ToDouble(Session["LatitudOrigen"]);
            var LongitudOrigen = Convert.ToDouble(Session["LongitudOrigen"]);
            var Destino = Session["NombreDestino"].ToString();
            enumGender Gender = (enumGender)Convert.ToInt32(Session["Gender"]);

            //var TripID = Convert.ToInt32(Session["TripID"].ToString());

            string User = Session["UserName"].ToString();
            var Us = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User).FirstOrDefault();
            Us.PaymentMethod = SuperID;
            db.SaveChanges();
            //   string FaceID = Session["FaceID"].ToString();
            //   var driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID).FirstOrDefault();
            Int32 ServiceArea = Convert.ToInt32(Session["ServiceArea"]);
            var Debt = Convert.ToDouble(Session["Debt"].ToString());
            var PaymentIntntId = Session["paymentIntentID"].ToString();

//            CreateTripRequest(PaymentIntntId, Us, ServiceArea, (int)Debt, LatitudObjectivo, LongitudObjetivo, Destino, Gender, LatitudOrigen, LongitudOrigen);
            try
            {
                var options_ = new PaymentMethodAttachOptions
                {
                    Customer = Us.StripeClientID,
                };
                var service_ = new PaymentMethodService();


                var paymentMethod = service_.Attach(SuperID, options_);
            
              }
            catch (Exception ep)
            {
                var ME = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();
        Push(ep.Message, "failed to send email with the following error:", ME.FirebaseID, "");
    }
    RefundOnCancelSolicitud(PaymentIntntId);
            return Json("Success");

        }

        public bool RefundOnCancelSolicitud(string Solicitud)
        {
            bool r_turn = false;
            try
            {
                StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";

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

        public void CreateTripRequest(string PaymentIntntId, Traveler_Perfil Traveler, Int32 ServiceArea, int Debt, double LatitudObjectivo, double LongitudObjetivo, string NombreDestino, enumGender Gender, double LatitudOrigen, double LongitudOOrigen)
        {
            var DriversInServiceArea = db.Traveler_Perfil.Where(x => x.Region == ServiceArea && x.pidiendo_viaje == (int)enumEstado_del_usuario.DisponibleParaRideshare && x.Rol == (int)enumRoles.RideDriver);
            if(Gender == enumGender.Female)
            {
                DriversInServiceArea = DriversInServiceArea.Where(x => x.Gender == (int)enumGender.Female);
            }
            if (DriversInServiceArea.Count() > 0)
            {
                if (Traveler != null)
                {
                    Traveler.pidiendo_viaje = (int)enumEstado_del_usuario.BusquedaEnProcesoRideShare;
                    db.SaveChanges();
                }
                var Driver = requestNearesDriver(Traveler.Facebook_profile_id, Convert.ToDouble(Traveler.Latitud), Convert.ToDouble(Traveler.Longitud), ServiceArea, Gender);
                if (CreateListOfrequest(Traveler.Facebook_profile_id, Driver.Facebook_profile_id, Debt, LatitudObjectivo, LongitudObjetivo, NombreDestino, LatitudOrigen, LongitudOOrigen, Gender))
                {


                    var Request_ = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id && x.FaceIDPassenger == Traveler.Facebook_profile_id).FirstOrDefault();
                    Push("Nueva Solicitud de Viaje", "Solicitud", Driver.FirebaseID, JsonConvert.SerializeObject(Request_));

                    // Task.Delay(60000).ContinueWith(t => bar(Request_.ID));
                }
                //var Traveler = Client;// db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User1).FirstOrDefault();
                //CreateListOfrequest(PaymentIntntId, Client.Facebook_profile_id, DriversInServiceArea, Debt / 100.0, LatitudObjectivo, LongitudObjetivo, NombreDestino);
                //var Driver = requestNearesDriver(Client.Facebook_profile_id, Convert.ToDouble(Traveler.Latitud), Convert.ToDouble(Traveler.Longitud));

                //var Request_ = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id && x.FaceIDPassenger == Client.Facebook_profile_id).FirstOrDefault();
                //Push("Nueva Solicitud de Viaje", "Solicitud Rideshare", Driver.FirebaseID, Base64Encode(JsonConvert.SerializeObject(Request_)));
                //Task.Delay(60000).ContinueWith(t => bar(Client,Request_, Traveler));
            }
        }
     
        public void AreTherePendingRequest(string UserRequesting)
        {
            var pendingRequestToAccept = db.CarppiRequestForDrive.Where(x => x.IsRequestNotTimedOut == false && x.FaceIDPassenger == UserRequesting && x.Accepted == false);
            var allTheRequest = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == UserRequesting);
            if (pendingRequestToAccept.LongCount() >= allTheRequest.LongCount())
            {
                var Traveler = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == UserRequesting).FirstOrDefault();
                Traveler.pidiendo_viaje = (int)enumEstado_del_usuario.ViajeNoEncontradoUsuario;
                foreach (var rr in allTheRequest)
                {
                    db.CarppiRequestForDrive.Remove(rr);
                }
            }

            db.SaveChanges();

        }


        string Base64Encode(string plainText)
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }


        public bool CreateListOfrequest(string User1, string DriverID, double Cost, double Lat, double lon, string name, double Latitud_Origen_Arg, double Longitud_Origen_arg, enumGender GenderRequest)
        {
            // StripeConfiguration.ApiKey = "sk_test_6P04GsrUgxVEdGIkt43NXflH00awsFl7rR";
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";

            var r_turn = false;
            try
            {
                //---------------------------------------
                var ConformityRequest = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == User1 && x.FaceIDDriver == DriverID).FirstOrDefault();
                if (ConformityRequest == null)
                {
                    var Usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User1).FirstOrDefault();


                    //Us.StripeLastPaymentIntent_ID = paymentIntent.Id;
                    //-------------------------------------------------

                    var sada = new CarppiRequestForDrive();
                    sada.Cost = Cost;
                    sada.FaceIDDriver = DriverID;
                    sada.FaceIDPassenger = User1;
                    sada.Stat = (int)StateOfRdeshare.RequestCreated;
                    sada.LatitudViajePendiente = Lat;
                    sada.LongitudViajePendiente = lon;
                    sada.NameOfPlace = name;
                    sada.IsRequestNotTimedOut = true;
                    sada.OriginOfRequest_Longitude = Longitud_Origen_arg;
                    sada.OriginOfRequest_Latitude = Latitud_Origen_Arg;
                    sada.CreationTime = (long)(((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
                    sada.NameOfRequester = Usuario.FirstName;
                    sada.GenderSearch = (int)GenderRequest;
                    // sada.PaymentIntentID = ScopedPaymentIntent;
                    db.CarppiRequestForDrive.Add(sada);


                    db.SaveChanges();
                    r_turn = true;
                }
            }
            catch (Exception)
            { }
            return r_turn;
        }


        public Traveler_Perfil requestNearesDriver(string FaceIDOfRequester, double LatPass, double LongPass, long AreaOfService, enumGender GenderDriver)
        {
            double Distance = 100000;
            //  var Requests = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == FaceIDOfRequester);

            //List<Traveler_Perfil> ListOfDrivers = new List<Traveler_Perfil>();
            //foreach (var r in Requests)
            //{
            //    ListOfDrivers.Add(db.Traveler_Perfil.Where(x => x.Facebook_profile_id == r.FaceIDDriver).FirstOrDefault());
            //}
            var IsItDriver = (int)enumEstado_del_usuario.DisponibleParaRideshare;
            var ListOfDrivers = db.Traveler_Perfil.Where(x => x.Region == AreaOfService && x.pidiendo_viaje == IsItDriver);
            if (GenderDriver == enumGender.Female)
            {
                ListOfDrivers = ListOfDrivers.Where(x => x.Gender == (int)enumGender.Female);
            }
            Traveler_Perfil Conductor = ListOfDrivers.FirstOrDefault();
            foreach (var Driver in ListOfDrivers)
            {
                var Temp_TOpassenger = ManhattanDisane(LatPass, LongPass, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));
                var Temp_TODestiny = 0.0;
                var Petition = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == FaceIDOfRequester && x.FaceIDDriver == Driver.Facebook_profile_id && x.IsRequestNotTimedOut == false).FirstOrDefault();

                Temp_TODestiny += Petition == null ? 0.0 : 20;
                if (Driver.LongitudObjetivoRideSharer != null && Driver.LatitudObjetivoRideSharer != null)
                {
                    Temp_TODestiny += ManhattanDisane(Convert.ToDouble(Driver.Longitud), Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.LongitudObjetivoRideSharer), Convert.ToDouble(Driver.LatitudObjetivoRideSharer));
                }
                if ((Temp_TODestiny + Temp_TOpassenger) < Distance)
                {
                    Distance = (Temp_TODestiny + Temp_TOpassenger);
                    Conductor = Driver;
                }
            }
            return Conductor;
        }

        string ComputeSha256Hash(string rawData)
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


            Double ManhattanDisane(double LatOrige, double longOriges, double LatDestino, double LongDestiono)
            {
                return Math.Abs(LatOrige - LatDestino) + Math.Abs(longOriges - LongDestiono);
            }




        }
    }
