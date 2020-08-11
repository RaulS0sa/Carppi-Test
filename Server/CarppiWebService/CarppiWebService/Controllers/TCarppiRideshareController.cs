
using CarppiWebService.Models;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using CarppiWebService.Clases_Costo;
using EASendMail;

namespace CarppiWebService.Controllers
{
    public class TCarppiRideshareController : ApiController
    {
        PidgeonEntities db = new PidgeonEntities();
        public enum enumEstado_del_usuario { Sin_actividad, PidiendoViajePasajero, ViajeEncontradoPasajero, ViajandoPasajero, EntregadoPasajero, PidiendoViajeConductor, ViajandoConductor, ViajeAceptadoNoRecogido_Conductor, PasajeroSinArribar, PasajeroDejoPlantado, ViajandoPasajeroRideShare, ConductorEsperando, EsperandoCalificar, BusquedaEnProceso, DisponibleParaRideshare, EsperandoCalificarRideshare, Sin_actividadRideShare, ViajeNoEncontradoUsuario, PasajeroEsperandoconductorRideshare, BusquedaEnProcesoRideShare };
        public enum StateOfRdeshare { RequestCreated, RequestAccepted, RequestCanceled, TripFinished, TripStarted };
        public enum enumRoles { Pasajero, Conductor, Observador, RideDriver };

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage DriverISCancelingTrip(string FaceID_CancelerDriver, Int32 TripRequest_ToCancel)
        {
            var TripRequest = db.CarppiRequestForDrive.Where(x => x.ID == TripRequest_ToCancel).FirstOrDefault();
            var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID_CancelerDriver).FirstOrDefault();
            var Discount = Driver.Rating_double <= 3 ? 0 : Driver.Rating_double - 3.0;
            Driver.Rating_double = ((0.75) * Driver.Rating_double) + ((0.25) * Discount);
            if (RefundOnCancelSolicitud(TripRequest.PaymentIntentID))
            {
                var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == TripRequest.FaceIDPassenger).FirstOrDefault();
                User.pidiendo_viaje = (int)enumEstado_del_usuario.ViajeNoEncontradoUsuario;
                db.CarppiRequestForDrive.Remove(TripRequest);
                db.SaveChanges();
                Task.Delay(5000).ContinueWith(t => ChangeTravelerSateToNormal(User));
                return Request.CreateResponse(HttpStatusCode.Accepted, "");

            }




            //var Calculator = new RegionalCostCalculator(LatOrigen_RequestCost, LongOrigen_RequestCost, LatDestino_RequestCost, LongDestino_RequestCost);
            //var Costs = Calculator.CalculateRegularCost();
            //var RValue = User.Gender == null ? (int)enumGender.Male : User.Gender;
            return Request.CreateResponse(HttpStatusCode.Conflict, "");

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage PassengerISCancelingTrip(string FaceID_CancelerPassenger, Int32 TripRequest_ToCancel)
        {
            var TripRequest = db.CarppiRequestForDrive.Where(x => x.ID == TripRequest_ToCancel).FirstOrDefault();
            var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID_CancelerPassenger).FirstOrDefault();
            //var Discount = Driver.Rating_double <= 3 ? 0 : Driver.Rating_double - 3.0;
            //Driver.Rating_double = ((0.75) * Driver.Rating_double) + ((0.25) * Discount);
            if (RefundOnCancelSolicitud(TripRequest.PaymentIntentID))
            {
                var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == TripRequest.FaceIDPassenger).FirstOrDefault();
                User.pidiendo_viaje = (int)enumEstado_del_usuario.ViajeNoEncontradoUsuario;
                db.CarppiRequestForDrive.Remove(TripRequest);
                db.SaveChanges();
                Task.Delay(5000).ContinueWith(t => ChangeTravelerSateToNormal(User));
                return Request.CreateResponse(HttpStatusCode.Accepted, "");

            }




            //var Calculator = new RegionalCostCalculator(LatOrigen_RequestCost, LongOrigen_RequestCost, LatDestino_RequestCost, LongDestino_RequestCost);
            //var Costs = Calculator.CalculateRegularCost();
            //var RValue = User.Gender == null ? (int)enumGender.Male : User.Gender;
            return Request.CreateResponse(HttpStatusCode.Conflict, "");

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

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage RequestCostOfTraveling(double LatOrigen_RequestCost, double LongOrigen_RequestCost, double LatDestino_RequestCost, double LongDestino_RequestCost, Int32 Region_RequestCost)
        {
            var Calculator = new RegionalCostCalculator(LatOrigen_RequestCost, LongOrigen_RequestCost, LatDestino_RequestCost, LongDestino_RequestCost);
            var Costs = Calculator.CalculateRegularCost();
            //var RValue = User.Gender == null ? (int)enumGender.Male : User.Gender;
            return Request.CreateResponse(HttpStatusCode.Accepted, Costs);

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage UpdateGenderOfTraveler(string FaceIDOfUserToUpdateGenderOfTraveler, enumGender GenderOfTraveler)
        {
            var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfUserToUpdateGenderOfTraveler).FirstOrDefault();
            User.Gender = (int)GenderOfTraveler;
            db.SaveChanges();
            //var RValue = User.Gender == null ? (int)enumGender.Male : User.Gender;
            return Request.CreateResponse(HttpStatusCode.Accepted, User);

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage IsUserAGirl(string FaceIDOfUserMeantTOBeAgirl)
        {
            var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfUserMeantTOBeAgirl).FirstOrDefault();
            var RValue = User.Gender == null ? (int)enumGender.Male : User.Gender;
            return Request.CreateResponse(HttpStatusCode.Accepted, RValue);

        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetExtraDataFromTheTrip(Int32 PendingTrip_ToExtractData)
        {
            var Trip = db.CarppiRequestForDrive.Where(x => x.ID == PendingTrip_ToExtractData).FirstOrDefault();
            var Driver = db.Traveler_Perfil.Where(x=> x.Facebook_profile_id == Trip.FaceIDDriver).FirstOrDefault();
            var Comments = db.Carppi_ComentariosHaciaElPerfil.Where(x => x.FaceID_OfComentedFolk == Trip.FaceIDDriver);
            var Extra = new TripExtraDatacaCommentUtility();
            Extra.DriverName = Driver.FirstName;
            Extra.Rate = Driver.Rating_double;
            Extra.Marca_Vehiculo = Driver.Marca_Vehiculo;
            Extra.Marca_Vehiculo = Driver.Marca_Vehiculo;
            Extra.Color_Vehiculo = Driver.Color_Vehiculo;
            Extra.Modelo_Vehiculo = Driver.Modelo_Vehiculo;
            Extra.Placa_Vehiculo = Driver.Placa;
            try
            {
                Extra.Driverphoto = Driver.Foto_ByteArray != null? Convert.ToBase64String(Driver.Foto_ByteArray) : null;
                Extra.Vehiclephoto = Driver.FotoDelVehiculo_ByteArray != null ? Convert.ToBase64String(Driver.FotoDelVehiculo_ByteArray) : null;
            }
            catch (Exception) { }
            List<CarppiComentUtility> ListOfComents = new List<CarppiComentUtility>();
            foreach (var comment in Comments)
            {
                try
                {
                    var util = new CarppiComentUtility();
                    util.ComentData = comment;
                    util.PhotoOfComenter = Convert.ToBase64String(db.Traveler_Perfil.Where(x => x.Facebook_profile_id == comment.FaceID_OfRater).FirstOrDefault().Foto_ByteArray);
                    ListOfComents.Add(util);
                }
                catch(Exception)
                { }

            }
            Extra.ListOfComents = ListOfComents;

            return Request.CreateResponse(HttpStatusCode.Accepted, Extra);


        }
        public class TripExtraDatacaCommentUtility
        {
            public string DriverName;
            public double Rate;
            public string Marca_Vehiculo;
            public string Modelo_Vehiculo;
            public string Placa_Vehiculo;
            public string Color_Vehiculo;
            public string Vehiclephoto;
            public string Driverphoto;
            public List<CarppiComentUtility> ListOfComents;

        }
        public class CarppiComentUtility
        {
            public Carppi_ComentariosHaciaElPerfil ComentData;
            public string PhotoOfComenter;

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage FinishRide(String FaceIDOfDriver_2, Int32 TripRequestToFinish)
        {
            var TripRequest = db.CarppiRequestForDrive.Where(x => x.ID == TripRequestToFinish).FirstOrDefault();
            var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == TripRequest.FaceIDDriver).FirstOrDefault();
            var Pasajero = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == TripRequest.FaceIDPassenger).FirstOrDefault();
            Pasajero.pidiendo_viaje = (int)enumEstado_del_usuario.EsperandoCalificarRideshare;
            //if (Driver.LongitudObjetivoRideSharer != null && Driver.LatitudObjetivoRideSharer != null)
            Driver.LongitudObjetivoRideSharer = null;
            Driver.LatitudObjetivoRideSharer = null;

            var Captured = CaptureFunds(TripRequest.PaymentIntentID);
            if (Captured)
            {
                var Calculator = new RegionalCostCalculator(Convert.ToDouble(TripRequest.OriginOfRequest_Latitude),
                    Convert.ToDouble(TripRequest.OriginOfRequest_Longitude),
                    Convert.ToDouble(TripRequest.LatitudViajePendiente),
                    Convert.ToDouble(TripRequest.LongitudViajePendiente));
                //var Costo = (TripRequest.Cost) * (TripRequest.Cost * 0.1) + 4.5;
                TripRequest.Stat = (int)StateOfRdeshare.TripFinished;
                SendReceiptEmail(Pasajero.Correo,
                            "costo $" + (Calculator.CostoRideShare + Calculator.ComisionRideShare).ToString(),
                            "-",
                           "-",
                           "-");
                try
                {
                    var Messages = db.Carppi_MensajesRideshare.Where(x => x.RequestID == TripRequestToFinish);
                    foreach (var Message in Messages)
                    {
                        db.Carppi_MensajesRideshare.Remove(Message);
                    }
                }
                catch(Exception)
                {

                }
            }
            TripRequest.Stat = (int)StateOfRdeshare.TripFinished;
            //TripRequest.PaymentIntentID = "";
           // TripRequest.FaceIDDriver = "";
            //db.CarppiRequestForDrive.Remove(TripRequest);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "");



        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage StartPendingTrip(String FaceIDOfDriver_StartTrip, Int32 TripRequestToStart)
        {
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
            var TripRequest = db.CarppiRequestForDrive.Where(x => x.ID == TripRequestToStart).FirstOrDefault();
            var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfDriver_StartTrip).FirstOrDefault();
            var Pasajero = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == TripRequest.FaceIDPassenger).FirstOrDefault();
            Pasajero.pidiendo_viaje = (int)enumEstado_del_usuario.ViajandoPasajeroRideShare;
            TripRequest.Stat = (int)StateOfRdeshare.TripStarted;
            try 
            {
                

                var service = new PaymentIntentService();

                var options = new PaymentIntentConfirmOptions
                {

                   PaymentMethod = Pasajero.PaymentMethod

                };
                PaymentIntent paymentIntent = service.Confirm(TripRequest.PaymentIntentID);

            }
            catch (Exception ep)
            {
                var ME = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();
                Push(ep.Message, "failed to send email with the following error:", ME.FirebaseID, "");
            }

            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "");



        }



        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage FinishSession(String FaceIDOfDriver_FinishSession)
        {
            
            var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfDriver_FinishSession).FirstOrDefault();
            if (Driver.pidiendo_viaje == (int)enumEstado_del_usuario.DisponibleParaRideshare)
            {
                Driver.pidiendo_viaje = (int)enumEstado_del_usuario.Sin_actividadRideShare;
            }

            //db.CarppiRequestForDrive.Remove(TripRequest);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "");



        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage StartSession(String FaceIDOfDriver_StartSession)
        {

            var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfDriver_StartSession).FirstOrDefault();
            if (Driver.pidiendo_viaje == (int)enumEstado_del_usuario.Sin_actividadRideShare)
            {
                Driver.pidiendo_viaje = (int)enumEstado_del_usuario.DisponibleParaRideshare;
            }

            //db.CarppiRequestForDrive.Remove(TripRequest);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "");



        }

        //[HttpGet]
        //[ActionName("ApiByAction")]
        //public HttpResponseMessage GetTripStatForDriver(String FaceIDOfDriver_1)
        //{
        //    var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfDriver_1 && x.pidiendo_viaje == (int)enumEstado_del_usuario.DisponibleParaRideshare && x.Rol == (int)enumRoles.RideDriver).FirstOrDefault();
        //    if (Driver != null)
        //    {
        //        var Request_Trip = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id).FirstOrDefault();
        //        if (Request_Trip != null)
        //        {
        //            var CircleSoround = GenerateCircle(0.001, Convert.ToDouble(Request_Trip.LatitudViajePendiente), Convert.ToDouble(Request_Trip.LongitudViajePendiente));
        //            var IsInRegion = PointInPolygon(CircleSoround.X_Array, CircleSoround.Y_Array, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));
        //            if (IsInRegion)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, Request_Trip);
        //            }
        //            if (Request_Trip.IsRequestNotTimedOut == true)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.Found, Request_Trip);
        //            }
        //            return Request.CreateResponse(HttpStatusCode.Accepted, Request_Trip);
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.Continue, "None");
        //        }
        //    }
        //    return Request.CreateResponse(HttpStatusCode.BadRequest, "None");

        //}


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage AcceptTripRequest(Int32 RequestID, StateOfRdeshare Desicion)
        {

            var Request_ = db.CarppiRequestForDrive.Where(x => x.ID == RequestID).FirstOrDefault();
            if (Desicion == StateOfRdeshare.RequestAccepted)
            {
                var user = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Request_.FaceIDPassenger).FirstOrDefault();
                user.pidiendo_viaje = (int)enumEstado_del_usuario.PasajeroEsperandoconductorRideshare;
                user.Viaje_asociado = RequestID.ToString();
                var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Request_.FaceIDDriver).FirstOrDefault();
                var AcceptedRequest = db.CarppiRequestForDrive.Where(x => x.ID == RequestID && x.FaceIDPassenger == Request_.FaceIDPassenger).FirstOrDefault();

                //SecondRequest.Accepted = false;
                //SecondRequest.Stat = (int)StateOfRdeshare.RequestCanceled;
                var Request_new = db.CarppiRequestForDrive.Where(x =>x.Stat == (int)StateOfRdeshare.RequestCanceled && x.Accepted == false && x.FaceIDPassenger == Request_.FaceIDPassenger);
                AcceptedRequest.IsRequestNotTimedOut = false;
                AcceptedRequest.Accepted = true;
                AcceptedRequest.Stat = (int)StateOfRdeshare.RequestAccepted;
                //AcceptedRequest.Cost = 10;
                //if (Driver.LongitudObjetivoRideSharer != null && Driver.LatitudObjetivoRideSharer != null)
                Driver.LatitudObjetivoRideSharer = AcceptedRequest.LatitudViajePendiente;//LatitudObjetivoRideSharer
                Driver.LongitudObjetivoRideSharer = AcceptedRequest.LongitudViajePendiente;
                db.SaveChanges();
                try
                {

                    foreach (var r in Request_new)
                    {
                        db.CarppiRequestForDrive.Remove(r);
                    }
                    StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
                    // StripeConfiguration.ApiKey = "sk_test_6P04GsrUgxVEdGIkt43NXflH00awsFl7rR";
                    var Calculator = new RegionalCostCalculator(Convert.ToDouble(AcceptedRequest.OriginOfRequest_Latitude),
                  Convert.ToDouble(AcceptedRequest.OriginOfRequest_Longitude),
                  Convert.ToDouble(AcceptedRequest.LatitudViajePendiente),
                  Convert.ToDouble(AcceptedRequest.LongitudViajePendiente));

                    var service = new PaymentIntentService();
                    var TutoriFee = (long)(Calculator.ComisionRideShare * 100);//(long)(((AcceptedRequest.Cost * 100) * 0.1) + 450);
                    var servicecost = AcceptedRequest.Cost * 100;
                    var options = new PaymentIntentCreateOptions
                    {

                        Customer = user.StripeClientID,
                        PaymentMethod = user.PaymentMethod,
                        Amount = (int)(servicecost),
                        Currency = "mxn",
                        CaptureMethod = "manual",
                        PaymentMethodTypes = new List<string> { "card" },
                        SetupFutureUsage = "off_session",
                        ApplicationFeeAmount = TutoriFee,
                        TransferData = new PaymentIntentTransferDataOptions
                        {
                            Destination = Driver.StripeDriverID
                        }

                    };
                    PaymentIntent paymentIntent = service.Create(options);

                    AcceptedRequest.PaymentIntentID = paymentIntent.Id;

                    db.SaveChanges();
                }
                catch (Exception ep)
                {
                    var ME = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();
                    Push(ep.Message, "failed to send email with the following error:", ME.FirebaseID, "");
                }


            }
            else
            {
                var Request_new = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Request_.FaceIDDriver && x.FaceIDPassenger == Request_.FaceIDPassenger).FirstOrDefault();
                Request_new.IsRequestNotTimedOut = false;
                Request_new.Accepted = false;
                Request_new.Stat = (int)StateOfRdeshare.RequestCanceled;
                var user = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Request_.FaceIDPassenger).FirstOrDefault();
                //  db.CarppiRequestForDrive.Remove(Request_new);
                try
                {
                    var Driver = requestNearesDriver(Request_.FaceIDPassenger, Convert.ToDouble(Request_.LatitudViajePendiente), Convert.ToDouble(Request_.LongitudViajePendiente), Convert.ToInt64(user.Region), (enumGender)Request_.GenderSearch);
                    if (Driver.ID > 0)
                    {
                        if (CreateListOfrequest(user.Facebook_profile_id, Driver.Facebook_profile_id, Convert.ToDouble(Request_.Cost), Convert.ToDouble(Request_.LatitudViajePendiente), Convert.ToDouble(Request_.LongitudViajePendiente), Request_.NameOfPlace, Convert.ToDouble(Request_.OriginOfRequest_Latitude), Convert.ToDouble(Request_.OriginOfRequest_Longitude), (enumGender)Request_new.GenderSearch))
                        {
                            var Request_kwaii = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id && x.FaceIDPassenger == Request_.FaceIDPassenger).FirstOrDefault();
                            Push("Nueva Solicitud de Viaje", "Solicitud", Driver.FirebaseID, JsonConvert.SerializeObject(Request_kwaii));
                        }
                    }
                }
                catch(Exception)
                {

                }
                AreTherePendingRequest(Request_.FaceIDPassenger);

            }

            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }
        public enum enumGender {Male, Female, Genderless};

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchForRide(string User1, Int32 AreaOfService, double Cost, double Latitud_arg, double LongitudARG, string n_destino, double Latitud_Origen, double Longitud_Origen, enumGender Gender)
        {

            var Client = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User1).FirstOrDefault();
            if (Client.StripeClientID != null && Client.PaymentMethod != null) 
            //if (false)
            {

                var DriversInServiceArea = db.Traveler_Perfil.Where(x => x.Region == AreaOfService && x.pidiendo_viaje == (int)enumEstado_del_usuario.DisponibleParaRideshare);
                if(Gender == enumGender.Female) 
                {
                    DriversInServiceArea = DriversInServiceArea.Where(x => x.Gender == (int)enumGender.Female);
                }
                if (DriversInServiceArea.Count() > 0)
                {
                    if (Client != null)
                    {
                        Client.pidiendo_viaje = (int)enumEstado_del_usuario.BusquedaEnProcesoRideShare;
                        db.SaveChanges();
                    }
                    var Traveler = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User1).FirstOrDefault();

                    //---------------------------------------
                    //var service = new PaymentIntentService();
                    //var TutoriFee = (long)(((Cost * 100) * 0.1) + 450);
                    //var servicecost = Cost * 100;
                    //var options = new PaymentIntentCreateOptions
                    //{
                    //    Amount = (int)(servicecost),
                    //    Currency = "mxn",
                    //    CaptureMethod = "manual",
                    //    PaymentMethodTypes = new List<string> { "card" },
                    //    SetupFutureUsage = "off_session",
                    //    // ApplicationFeeAmount = TutoriFee,

                    //};
                    //PaymentIntent paymentIntent = service.Create(options);
                    //Us.StripeLastPaymentIntent_ID = paymentIntent.Id;
                    //-------------------------------------------------
                    var Calculator = new RegionalCostCalculator(Latitud_Origen, Longitud_Origen, Latitud_arg, LongitudARG);
                    //var ServerCost = Calculator.CalculateRegularCost();


                    var Driver = requestNearesDriver(User1, Convert.ToDouble(Traveler.Latitud), Convert.ToDouble(Traveler.Longitud), AreaOfService, Gender);
                    if (Driver.ID > 0)
                    {
                        if (CreateListOfrequest(User1, Driver.Facebook_profile_id, Calculator.CostoRideShare, Latitud_arg, LongitudARG, n_destino, Latitud_Origen, Longitud_Origen, Gender))
                        {


                            var Request_ = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id && x.FaceIDPassenger == User1).FirstOrDefault();
                            Push("Nueva Solicitud de Viaje", "Solicitud", Driver.FirebaseID, JsonConvert.SerializeObject(Request_));


                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
                }
                else
                {

                    return Request.CreateResponse(HttpStatusCode.Conflict, "Aceptado");
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Aceptado");


        }
       

        string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
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

        public void AreTherePendingRequest(string UserRequesting) 
        {
            //  var pendingRequestToAccept = db.CarppiRequestForDrive.Where(x => x.IsRequestNotTimedOut == false && x.FaceIDPassenger == UserRequesting & x.Accepted == false);

            var allTheRequest = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == UserRequesting);
            var Traveler = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == UserRequesting).FirstOrDefault();
            var DriversInServiceArea = db.Traveler_Perfil.Where(x => x.Region == Traveler.Region && x.pidiendo_viaje == (int)enumEstado_del_usuario.DisponibleParaRideshare && x.Rol == (int)enumRoles.RideDriver);
            var AcceptedRequest = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == UserRequesting && x.Stat == (int)StateOfRdeshare.RequestAccepted).FirstOrDefault();
            var UnAcceptedRequest = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == UserRequesting && x.Stat != (int)StateOfRdeshare.RequestAccepted);

            if (DriversInServiceArea.Count() == allTheRequest.Count() )
            {
                
                Traveler.pidiendo_viaje = (int)enumEstado_del_usuario.ViajeNoEncontradoUsuario;
                foreach (var rr in UnAcceptedRequest)
                {

                   
                    db.CarppiRequestForDrive.Remove(rr);
                 
                }

                Task.Delay(5000).ContinueWith(t => ChangeTravelerSateToNormal(Traveler));
            }
          
            db.SaveChanges();

        }




        public void ChangeTravelerSateToNormal(Traveler_Perfil Viajero )
        {
            var Traveler = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Viajero.Facebook_profile_id).FirstOrDefault();
            if (Traveler.pidiendo_viaje == (int)enumEstado_del_usuario.ViajeNoEncontradoUsuario)
            {
                Traveler.pidiendo_viaje = (int)enumEstado_del_usuario.Sin_actividad;
                db.SaveChanges();
            }

        }

        public bool CreateListOfrequest(string User1, string DriverID, double Cost, double Lat, double lon, string name, double Latitud_Origen_Arg, double Longitud_Origen_arg, enumGender GenderRequest)
        {
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
            //StripeConfiguration.ApiKey = "sk_test_6P04GsrUgxVEdGIkt43NXflH00awsFl7rR";

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
                    sada.CreationTime = (long)(((DateTime.UtcNow - new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc)).TotalMilliseconds) / 1000);
                    sada.NameOfRequester = Usuario.FirstName;
                    sada.GenderSearch = (int)GenderRequest;
                    // sada.PaymentIntentID = ScopedPaymentIntent;
                    db.CarppiRequestForDrive.Add(sada);


                    db.SaveChanges();
                    r_turn = true;
                }
            }
            catch(Exception)
            { }
            return r_turn ;
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
            var ListOfDrivers = db.Traveler_Perfil.Where(x => x.Region == AreaOfService && x.pidiendo_viaje == IsItDriver && x.IsUserADriver == true);
            if(GenderDriver == enumGender.Female)
            {
                ListOfDrivers = ListOfDrivers.Where(x => x.Gender == (int)enumGender.Female);
            }
            Traveler_Perfil Conductor = ListOfDrivers.FirstOrDefault();
            foreach (var Driver in ListOfDrivers)
            {
                var Temp_TOpassenger = ManhattanDisane(LatPass, LongPass, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));
                var Temp_TODestiny = 0.0;
                var Petition = db.CarppiRequestForDrive.Where(x =>x.FaceIDPassenger== FaceIDOfRequester &&  x.FaceIDDriver == Driver.Facebook_profile_id && x.IsRequestNotTimedOut == false).FirstOrDefault();
                var All_Petitions = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id && x.Accepted == true);
                Temp_TODestiny += Petition == null ? 0.0 : 20;
                if (All_Petitions.Count() < 2 )
                {
                    //Temp_TODestiny += 20;
                    if (Driver.Rating_double < 3.0)
                    {
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
                }
            }
            return Conductor;
        }

        public Double ManhattanDisane(double LatOrige, double longOriges, double LatDestino, double LongDestiono)
        {
            return Math.Abs(LatOrige - LatDestino) + Math.Abs(longOriges - LongDestiono);
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchForPassengerArea(double LatitudUser, double LongitudUser, string FacebookID_UpdateArea)
        {

            var Areas = db.CarppiRegiones.Where(x => x.ID > 0);
            var IsInRegion = false;
            var Region = 0;
            var ReginObject = new CarppiRegiones();
            foreach (var area in Areas)
            {
                var CircleSoround = GenerateCircle(Convert.ToDouble(area.Radio), Convert.ToDouble(area.LatitudCentroide), Convert.ToDouble(area.LongitudCentroide));
                IsInRegion = PointInPolygon(CircleSoround.X_Array, CircleSoround.Y_Array, LatitudUser, LongitudUser);
                if (IsInRegion)
                {
                    if (FacebookID_UpdateArea != "")
                    {
                        var USer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FacebookID_UpdateArea).FirstOrDefault();
                        if (USer != null)
                        {
                            USer.Region = area.ID;
                            db.SaveChanges();
                        }
                    }
                    //USer.se
                    Region = area.ID;
                    ReginObject = area;
                    
                    break;
                }
            }
            if (IsInRegion)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, ReginObject);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ReginObject);
            }

        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchForPassengerAreaByStateAndCountry(string Country, string State, string FacebookID_UpdateArea)
        {
            var New_Area = db.CarppiRegiones.Where(x => x.Pais.Contains(Country) && x.Estado.Contains(State)).FirstOrDefault();
           
            var IsInRegion = false;
            var Region = 0;
            var ReginObject = new CarppiRegiones();
      
            if (New_Area != null)
            {
                if (FacebookID_UpdateArea != "")
                {
                    var USer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FacebookID_UpdateArea).FirstOrDefault();
                    if (USer != null)
                    {
                        USer.Region = New_Area.ID;
                        db.SaveChanges();
                    }
                }
                //USer.se
                Region = New_Area.ID;
                ReginObject = New_Area;

                return Request.CreateResponse(HttpStatusCode.Accepted, ReginObject);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ReginObject);
            }

        }

        public bool ChargeUserForService(int Amount, string ClientId, string TutorID, ref string ChargeRef_Pointer, ref string PaymentIntentRef_Pointer)
        {
            bool r_val = false;
            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
            var USer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == ClientId).FirstOrDefault();
            var TutorToPay = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == TutorID).FirstOrDefault();
            var options = new PaymentMethodListOptions
            {
                Customer = USer.StripeClientID,
                Type = "card",
            };
            var service = new PaymentMethodService();
            var lis = service.List(options);
            var TutoriFee = (long)((Amount * 0.09) + 450);
            try
            {
                var service_paymentIntent = new PaymentIntentService();
                var options_Payment_intent = new PaymentIntentCreateOptions
                {
                    Amount = Amount,
                    Currency = "mxn",
                    Customer = USer.StripeClientID,
                    PaymentMethod = lis.FirstOrDefault().Id,
                    Confirm = true,
                    OffSession = true,
                    CaptureMethod = "manual",
                    ApplicationFeeAmount = TutoriFee,
                    TransferData = new PaymentIntentTransferDataOptions
                    {
                        Destination = TutorToPay.StripeDriverID
                    }
                };
                PaymentIntent P_I = service_paymentIntent.Create(options_Payment_intent);
                ChargeRef_Pointer = P_I.Charges.FirstOrDefault().Id;
                PaymentIntentRef_Pointer = P_I.Id;
                r_val = true;
            }
            catch (StripeException e)
            {
                switch (e.StripeError.Code)
                {
                    case "card_error":
                        // Error code will be authentication_required if authentication is needed
                        Console.WriteLine("Error code: " + e.StripeError.Code);
                        var paymentIntentId = e.StripeError.PaymentIntent.Id;
                        var service_SecondPaymentIntent = new PaymentIntentService();
                        var paymentIntent = service.Get(paymentIntentId);

                        Console.WriteLine(paymentIntent.Id);
                        break;
                    default:
                        break;
                }
            }
            return r_val;
        }
        public bool CaptureFunds(string PaymentIntent)
        {
            bool r_turn = false;
            try
            {
                 StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
                //StripeConfiguration.ApiKey = "sk_test_6P04GsrUgxVEdGIkt43NXflH00awsFl7rR";

                var service = new PaymentIntentService();

                var intent = service.Capture(PaymentIntent);
                r_turn = true;
            }
            catch (Exception ex)
            {
                var ME = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();
                Push(ex.Message, "failed to send email with the following error:", ME.FirebaseID, "");
                Console.WriteLine(ex.ToString());
            }
            return r_turn;
        }
        public bool RefundOnCancelSolicitud(Traveler_SolicitudDeViajeTemporal Solicitud)
        {
            bool r_turn = false;
            try
            {
                StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";

                var service = new PaymentIntentService();
                var options = new PaymentIntentCancelOptions { };
                var intent = service.Cancel(Solicitud.PaymentIntentID, options);

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
        public void SendReceiptEmail(string email, string TotalAmount, string TotalCard, string TotalCash, string TotalFare)
        {
            System.Net.ServicePointManager.SecurityProtocol =
       System.Net.SecurityProtocolType.Ssl3
       | System.Net.SecurityProtocolType.Tls12
       | SecurityProtocolType.Tls11
       | SecurityProtocolType.Tls;

            //    var TotalAmount = 15.ToString();
            //    var TotalCard = 15.ToString();
            //    var TotalCash = 15.ToString();
            //    var TotalFare = 15.ToString();

            //< h2 >%%% TagForMoney %%%</ h2 >

            //                                < !--Total: $200.18 mxn-- >

            //                                 < p >%%% TagForCard %%%</ p >

            //                                 < !--Ganancias en efectivo: $400 mxn-- >

            //                                  < p >%%% TagForCash %%%</ p >

            //                                  < !--< p > Ganancias en tarjeta: $400 mxn </ p > -->


            //                                      < p >%%% TagForFare %%%</ p >
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();

                // Your gmail email address
                oMail.From = "carppi_mexico@carppi.com.mx";

                // Set recipient email address
                // oMail.To = "raul.sosa.cortes@gmail.com";
                oMail.To = email;// "raul.sosa.cortes@gmail.com";
                                 // Set email subject
                oMail.Subject = "Tu Recibo de viaje";

                // Set email body
                //oMail.TextBody = "this is a test email sent from c# project with gmail.";
                //            string path = System.Web.HttpContext.Current.Request.MapPath("~\\dataset.csv");
                string path = System.Web.HttpContext.Current.Request.MapPath("~/App_Data/Receipt.html");

                //string pathReceipt = Server.MapPath("~/App_Data/Receipt.html");
                var fileContents = System.IO.File.ReadAllText(path);
                //   var fileContents = System.IO.File.ReadAllText(path2);
                var r1 = fileContents.Replace("%%%TagForMoney%%%", TotalAmount);
                r1 = r1.Replace("%%%TagForCard%%%", TotalCard);
                r1 = r1.Replace("%%%TagForCash%%%", TotalCash);
                r1 = r1.Replace("%%%TagForFare%%%", TotalFare);
                oMail.HtmlBody = r1;//"<h1>Hello!</h1>";
                                    // Gmail SMTP server address
                SmtpServer oServer = new SmtpServer("smtp.gmail.com");

                // Set 465 port
                oServer.Port = 465;

                // detect SSL/TLS automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                // Gmail user authentication
                // For example: your email is "gmailid@gmail.com", then the user should be the same
                oServer.User = "carppi_mexico@carppi.com.mx";
                oServer.Password = "THELASTTIMEaround";


                Console.WriteLine("start to send email over SSL ...");
                oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                var ME = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();
                Push(ep.Message, "failed to send email with the following error:", ME.FirebaseID, "");
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }

        public CircleArray GenerateCircle(double Radio, double CentroX, double CentroY)
        {
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for (var i = 0.0; i < Math.PI * 2; i = i + 0.1)
            {
                x.Add(CentroX + (Radio * Math.Sin(i)));
                y.Add(CentroY + (Radio * Math.Cos(i)));
            }
            return new CircleArray(x, y);

        }
        public class CircleArray
        {
            public CircleArray(List<double> X, List<double> y)
            {
                X_Array = X.ToArray();
                Y_Array = y.ToArray();

            }
            public double[] X_Array;
            public double[] Y_Array;
        }


        static bool PointInPolygon(double[] polyX, double[] polyY, double x, double y)
        {
            int polyCorners = polyX.Length;

            int i, j = polyCorners - 1;
            bool oddNodes = false;
            if (x < polyX.Min() || x > polyX.Max() || y < polyY.Min() || y > (polyY.Max()))
            {

                // We're outside the polygon!
            }
            else
            {
                // oddNodes = true;


                for (i = 0; i < polyCorners; i++)
                {
                    if ((polyY[i] < y && polyY[j] >= y || polyY[j] < y && polyY[i] >= y) && (polyX[i] <= x || polyX[j] <= x))
                    {
                        if (polyX[i] + (y - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]) < x)
                        {
                            oddNodes = !oddNodes;
                        }
                    }
                    j = i;
                }

            }
            return oddNodes;
        }




    }
}
