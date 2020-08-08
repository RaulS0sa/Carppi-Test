using CarppiWebService.Models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace CarppiWebService.Clases_Temporizador
{
    public class PeriodicTask_Carppi : IJob
    {
        PidgeonEntities db = new PidgeonEntities();
        public enum StateOfRdeshare { RequestCreated, RequestAccepted, RequestCanceled, TripFinished, TripStarted };
        public enum enumEstado_del_usuario { Sin_actividad, PidiendoViajePasajero, ViajeEncontradoPasajero, ViajandoPasajero, EntregadoPasajero, PidiendoViajeConductor, ViajandoConductor, ViajeAceptadoNoRecogido_Conductor, PasajeroSinArribar, PasajeroDejoPlantado, ViajandoPasajeroRideShare, ConductorEsperando, EsperandoCalificar, BusquedaEnProceso, DisponibleParaRideshare, EsperandoCalificarRideShare, Sin_actividadRideShare, ViajeNoEncontradoUsuario };
        public enum enumRoles { Pasajero, Conductor, Observador, RideDriver };
        public async Task Execute(IJobExecutionContext context)
        {
            ThrowOldRequest();
            //throw new NotImplementedException();
        }
        public enum enumGender { Male, Female, Genderless };
        public async void ThrowOldRequest()
        {
            var AllTheReqs = AllExpiredRequest();
            //var SecondRequest = db.CarppiRequestForDrive.Where(x => x.ID == Id_reference).FirstOrDefault();
            foreach (var SecondRequest in AllTheReqs)
            {
                if (SecondRequest.Stat == (int)StateOfRdeshare.TripFinished) {
                    db.CarppiRequestForDrive.Remove(SecondRequest);
                }
                else { 

                if (SecondRequest.Accepted == false && SecondRequest.Stat == (int)StateOfRdeshare.RequestCreated)
                {
                    SecondRequest.IsRequestNotTimedOut = false;
                    SecondRequest.Accepted = false;
                    SecondRequest.Stat = (int)StateOfRdeshare.RequestCanceled;

                    var client = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == SecondRequest.FaceIDPassenger).FirstOrDefault();
                    db.SaveChanges();
                    try
                    {
                        var Driver = requestNearesDriver(client.Facebook_profile_id, Convert.ToDouble(client.Latitud), Convert.ToDouble(client.Longitud), Convert.ToInt64(client.Region));
                        if (CreateListOfrequest(client.Facebook_profile_id, Driver.Facebook_profile_id, Convert.ToDouble(SecondRequest.Cost), Convert.ToDouble(SecondRequest.LatitudViajePendiente), Convert.ToDouble(SecondRequest.LongitudViajePendiente), SecondRequest.NameOfPlace, SecondRequest.OriginOfRequest_Latitude, SecondRequest.OriginOfRequest_Longitude, (enumGender)SecondRequest.GenderSearch))
                        {
                            var Request_2 = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id && x.FaceIDPassenger == client.Facebook_profile_id).FirstOrDefault();
                            if (Request_2.IsRequestNotTimedOut != false)
                            {

                                Push("Nueva Solicitud de Viaje", "Solicitud Rideshare", Driver.FirebaseID, Base64Encode(JsonConvert.SerializeObject(Request_2)));
                            }
                        }
                    }
                    catch (Exception) { }

                }
            }
            AreTherePendingRequest(SecondRequest.FaceIDPassenger);
            db.SaveChanges();
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

            if (DriversInServiceArea.Count() == UnAcceptedRequest.Count())
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

        public void ChangeTravelerSateToNormal(Traveler_Perfil Viajero)
        {
            var Traveler = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Viajero.Facebook_profile_id).FirstOrDefault();
            if (Traveler.pidiendo_viaje == (int)enumEstado_del_usuario.ViajeNoEncontradoUsuario)
            {
                Traveler.pidiendo_viaje = (int)enumEstado_del_usuario.Sin_actividad;
                db.SaveChanges();
            }

        }



        public bool CreateListOfrequest(string User1, string DriverID, double Cost, double Lat, double lon, string name, double? Latitud_Origen_Arg, double? Longitud_Origen_arg, enumGender GenderRequest)
        {
            //StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";
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


        public List<CarppiRequestForDrive> AllExpiredRequest() 
        {
            var AllCurrentRequest = db.CarppiRequestForDrive.Where(x => x.ID > 0);
            var now = DateTime.UtcNow;
            List<CarppiRequestForDrive> Expired = new List<CarppiRequestForDrive>();
            try
            {
                foreach (var req in AllCurrentRequest)
                {
                    var DateRequest = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt64(req.CreationTime));
                    var TimeDiference = (now - DateRequest).TotalSeconds;
                    if (TimeDiference >= 45 && (req.Stat == (int)StateOfRdeshare.RequestCreated || req.Stat == (int)StateOfRdeshare.RequestCanceled))
                    {
                        Expired.Add(req);
                    }
                    if(TimeDiference >= 3600 && req.Stat == (int)StateOfRdeshare.TripFinished)
                    {
                        Expired.Add(req);
                    }

                }
            }
            catch(Exception)
            {

            }
            return Expired;

        }

        public Traveler_Perfil requestNearesDriver(string FaceIDOfRequester, double LatPass, double LongPass, long AreaOfService)
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

        public Double ManhattanDisane(double LatOrige, double longOriges, double LatDestino, double LongDestiono)
        {
            return Math.Abs(LatOrige - LatDestino) + Math.Abs(longOriges - LongDestiono);
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
    }
}