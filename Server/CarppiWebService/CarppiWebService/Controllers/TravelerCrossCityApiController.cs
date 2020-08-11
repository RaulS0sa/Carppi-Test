//#define Production 
using CarppiWebService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using EASendMail;
using Stripe;

namespace CarppiWebService.Controllers
{
    public class TravelerCrossCityApiController : ApiController
    {
        PidgeonEntities db = new PidgeonEntities();
        OpenpayAPI openpayAPI = new OpenpayAPI("sk_377b0485df5040938338dae0be9ac38e", "mzc1tq4umka1na6yut6f", false);
        //openpayAPI.pro

        public enum enumRoles { Pasajero, Conductor, Observador, RideDriver };
        public enum enumEstado_del_usuario { Sin_actividad, PidiendoViajePasajero, ViajeEncontradoPasajero, ViajandoPasajero, EntregadoPasajero, PidiendoViajeConductor, ViajandoConductor, ViajeAceptadoNoRecogido_Conductor, PasajeroSinArribar, PasajeroDejoPlantado, ViajandoPasajeroRideShare, ConductorEsperando, EsperandoCalificar, BusquedaEnProceso, DisponibleParaRideshare, EsperandoCalificarRideshare , Sin_actividadRideShare , ViajeNoEncontradoUsuario, PasajeroEsperandoconductorRideshare, BusquedaEnProcesoRideShare };
        public enum enumEstado_de_los_viajes { EnEspera, Aceptado, EnCurso, EsperaDeAbordaje, aceptado_y_llendo_a_ubicacion, Finalizado, Conduciendo, FinalizoLejosDelObjetivo };
        public enum enumEstado_de_Solicitud { EnEspera, Aceptado, Rechazado, Conduciendo, NoLlego };
        public enum enumTipoDePAgoPreferido { Tarjeta, EnEfectivo, Ambos };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ActualizaLocalizacion(string user5, string Latitud, string Longitud)
        {
            
            
            var obj1 = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user5).FirstOrDefault();
            obj1.Latitud = Latitud;
            obj1.Longitud = Longitud;
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }



        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GEetOwnProfile(string user10_Hijo)
        {

            var obj1 = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user10_Hijo).FirstOrDefault();




            return Request.CreateResponse(HttpStatusCode.Accepted, obj1);


        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ChangeRoleOfUser(string user11_Hijo, int NewRole)
        {

            var obj1 = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user11_Hijo).FirstOrDefault();
            obj1.Rol = NewRole;
            db.SaveChanges();




            return Request.CreateResponse(HttpStatusCode.Accepted, obj1);


        }



        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GEtLastSearchOfUser(string user7_Hijo)
        {

            var obj1 = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user7_Hijo).FirstOrDefault();


            var search = new SearchLocationObject();
            search.LatitudeOrigen = Convert.ToDouble(obj1.BusquedaGuardada_Latitud_salida);
            search.LongitudOrigen = Convert.ToDouble(obj1.BusquedaGuardada_Longitud_salida);
            search.LatitudDestino = Convert.ToDouble(obj1.BusquedaGuardada_Latitud_Llegada);
            search.LongitudDestino = Convert.ToDouble(obj1.BusquedaGuardada_Longitud_Llegada);


            return Request.CreateResponse(HttpStatusCode.Accepted, search);


        }
        //RefundOnCancelSolicitud


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CancellProposals(string user9_Hijo)
        {

            var obj1 = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user9_Hijo).FirstOrDefault();
            obj1.pidiendo_viaje = (int)enumEstado_del_usuario.Sin_actividad;

            var Solicitudes = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == user9_Hijo);

            foreach (var aca in Solicitudes)
            {
                RefundOnCancelSolicitud(aca);
                db.Traveler_SolicitudDeViajeTemporal.Remove(aca);
            }
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted, "Borrado");


        }
        public class SearchLocationObject
        {
            public double LatitudeOrigen;
            public double LongitudOrigen;


            public double LatitudDestino;
            public double LongitudDestino;
            public int? Day;
            public int? Month;
            public int? Year;
            public bool AtentionToOrigin;
            public bool AtentionToDate;
            public string Departure;
            public string Arrival;



        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetAllTheSuccesfullReservation(int ReservationTripId, string DriverID)
        {
            int Aceptado = (int)enumEstado_de_Solicitud.Aceptado;

            int Conduciendo = (int)enumEstado_de_Solicitud.Conduciendo;
            int NuevaSolicitud = (int)enumEstado_de_Solicitud.EnEspera;
            var obj1 = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == ReservationTripId
            && (x.Estado_De_solicitud == Aceptado || x.Estado_De_solicitud == Conduciendo || x.Estado_De_solicitud == NuevaSolicitud)
            );
            List<SuccesFullReservationResponseList> lista = new List<SuccesFullReservationResponseList>();
            foreach (var Request in obj1)
            {
                var Objet0 = new SuccesFullReservationResponseList();
                Objet0._Solicitud = Request;
                var myPerfil = db.Traveler_Perfil.Where(v => v.Facebook_profile_id == Request.Face_id_solicitante).FirstOrDefault();
                Objet0._Perfil = myPerfil;
                lista.Add(Objet0);
            }
            var Respuesta = new SuccesFullReservationResponse();
            Respuesta.lista = lista;
            Respuesta.PerfilConductor = db.Traveler_Perfil.Where(v => v.Facebook_profile_id == DriverID).FirstOrDefault();
            ////////

            string yourJson = JsonConvert.SerializeObject(Respuesta);// jsonObject;
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            return response;
        }
        public class SuccesFullReservationResponse
        {
            public Traveler_Perfil PerfilConductor;
            public List<SuccesFullReservationResponseList> lista;
        }
        public class SuccesFullReservationResponseList
        {
            public Traveler_SolicitudDeViajeTemporal _Solicitud;
            public Traveler_Perfil _Perfil;

        }

        [HttpGet]
        [ActionName("ApibyAction")]
        public HttpResponseMessage GetPassengerResetvations(string FID_PassengerReservation)
        {
            var Reservation = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == FID_PassengerReservation);
            List<PassengerResponseReservation> ResponseList = new List<PassengerResponseReservation>();
            foreach (var reservacion in Reservation)
            {
                var MyTrip = db.Traveler_Viajes.Where(x => x.ID == reservacion.Id_del_viaje).FirstOrDefault();
                PassengerResponseReservation Response_object = new PassengerResponseReservation();
                Response_object.Reservation = reservacion;
                Response_object.Trip = MyTrip;

                ResponseList.Add(Response_object);
            }


            string yourJson = JsonConvert.SerializeObject(ResponseList);// jsonObject;
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            return response;
        }
        public class PassengerResponseReservation
        {
            public Traveler_SolicitudDeViajeTemporal Reservation;
            public Traveler_Viajes Trip;

        }

        public class ReservationObject
        {
            public string FacebookIdentifier;
            public int TripID;
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage RateTrip(string IDOfRater, Int32 Rate)
        {
            // var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == DriverToRate).FirstOrDefault();
            var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == IDOfRater).FirstOrDefault();

            var ViajeAsiciado = Convert.ToInt32(User.Viaje_asociado);
            if (User.pidiendo_viaje == (int)enumEstado_del_usuario.EsperandoCalificar)
            {

                var Trip = db.Traveler_Viajes.Where(x => x.ID == ViajeAsiciado).FirstOrDefault();
                User.pidiendo_viaje = (int)enumEstado_del_usuario.Sin_actividad;
                var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Trip.FaceIdDelConductor).FirstOrDefault();
                Driver.Rating_double = ((0.75) * Driver.Rating_double) + (0.25 * Rate);

                db.SaveChanges();
            }
            else if (User.pidiendo_viaje == (int)enumEstado_del_usuario.EsperandoCalificarRideshare)
            {
                User.pidiendo_viaje = (int)enumEstado_del_usuario.Sin_actividad;
                var Trip = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == IDOfRater).FirstOrDefault();
                var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Trip.FaceIDDriver).FirstOrDefault();
                Driver.Rating_double = ((0.75) * Driver.Rating_double) + (0.25 * Rate);
                db.CarppiRequestForDrive.Remove(Trip);
                db.SaveChanges();
            }

                return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");

        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage UpdatePassengerStateInDriveMode(string Face_identifier_2, Int32 RequestID, enumEstado_de_Solicitud SetState)
        {
            //   var study = Base64Decode(SuccesFullReservationResponse);
            //   var onjec = JsonConvert.DeserializeObject<ReservationObject>(study);


            var Reservacion = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.ID == RequestID).FirstOrDefault();
            if (Reservacion != null)
            {
                switch (SetState)
                {//Todo:uncoment Lines After Test
                    case enumEstado_de_Solicitud.Rechazado:
                        {
                            Reservacion.Estado_De_solicitud = (int)enumEstado_de_Solicitud.Rechazado;
                            var Usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Reservacion.Face_id_solicitante).FirstOrDefault();
                            Push("Sera mejor que busques de nuevo", "Tu solicitud a Sido Rechazada", Usuario.FirebaseID);
                            // Usuario.pidiendo_viaje = (int)(enumEstado_del_usuario.PasajeroDejoPlantado);
                        }
                        break;
                    case enumEstado_de_Solicitud.Aceptado:
                        {
                            Reservacion.Estado_De_solicitud = (int)enumEstado_de_Solicitud.Aceptado;
                            var Usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Reservacion.Face_id_solicitante).FirstOrDefault();
                            Usuario.pidiendo_viaje = (int)enumEstado_del_usuario.ViajandoPasajero;
                            Push("Recuerda estar listo a tiempo para tu viaje", "Tu solicitud a Sido Aceptada", Usuario.FirebaseID);
                            // Usuario.pidiendo_viaje = (int)(enumEstado_del_usuario.PasajeroEnViaje);
                        }
                        break;
                    default:
                        break;
                }

            }
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");

            //var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/UpdatePassengerStateInDriveMode?" +
            //   "Face_identifier_2=" + query.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
            //   "&SuccesFullReservationResponse=" + Context_de_respuesta +
            //   "&SetState=" + respuesta

            //   ));
            //int Aceptado = (int)enumEstado_de_Solicitud.Aceptado;

            //int Conduciendo = (int)enumEstado_de_Solicitud.Conduciendo;
            //int NuevaSolicitud = (int)enumEstado_de_Solicitud.EnEspera;
            //var obj1 = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == ReservationTripId
            //&& x.Estado_De_solicitud == Aceptado || x.Estado_De_solicitud == Conduciendo || x.Estado_De_solicitud == NuevaSolicitud
            //);
            //List<SuccesFullReservationResponse> lista = new List<SuccesFullReservationResponse>();
            //foreach (var Request in obj1)
            //{
            //    var Objet0 = new SuccesFullReservationResponse();
            //    Objet0._Solicitud = Request;
            //    var myPerfil = db.Traveler_Perfil.Where(v => v.Facebook_profile_id == Request.Face_id_solicitante).FirstOrDefault();
            //    Objet0._Perfil = myPerfil;
            //    lista.Add(Objet0);
            //}

            //////////

            //string yourJson = JsonConvert.SerializeObject(lista);// jsonObject;
            //var response = Request.CreateResponse(HttpStatusCode.Accepted);
            //response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            //return response;
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage Actualiza_estado_del_usuario(string user1_Facebook_profile_id, int Estado, string TripId)
        {
            var obj1 = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user1_Facebook_profile_id).FirstOrDefault();
            obj1.pidiendo_viaje = Estado;
            if (TripId != "")
            {
                obj1.Viaje_asociado = TripId;
            }
            db.SaveChanges();
            ////////
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }

        //[HttpGet]
        //[ActionName("ApiByAction")]
        //public HttpResponseMessage AppendCardTokenToUser(string token, string user2_Facebook_profile_id, string Clabe)
        //{//Todo: Add update each trip
        //    var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user2_Facebook_profile_id).FirstOrDefault();
        //    User.TarjetaValidada = true;

        //    string decoded = Base64Decode(token);
        //    var Tarjeta = JsonConvert.DeserializeObject<CardObject>(decoded);
        //    //User.Stripe_id = token;
        //    Card request = new Card();
        //    request.HolderName = Tarjeta.holder_name;
        //    request.CardNumber = Tarjeta.card_number;
        //    request.Cvv2 = Tarjeta.cvv2;
        //    request.ExpirationMonth = Tarjeta.expiration_month;
        //    request.ExpirationYear = Tarjeta.expiration_year;
        //    request.DeviceSessionId = User.deviceSessionId;
        //    request.AllowsPayouts = true;
        //    request.AllowsCharges = true;
        //    Address address = new Address();
        //    address.City = Tarjeta.address.city;
        //    address.CountryCode = Tarjeta.address.country_code;
        //    address.State = Tarjeta.address.state;
        //    address.PostalCode = Tarjeta.address.postal_code;
        //    address.Line1 = Tarjeta.address.line1;
        //    address.Line2 = Tarjeta.address.line2;
        //    address.Line3 = Tarjeta.address.line3;
        //    request.Address = address;


        //    Customer customer = new Customer();

        //    customer.Name = User.FirstName;
        //    customer.LastName = User.LastName;
        //    customer.Email = "net@c.com";
        //    customer.Address = address;


        //    Customer customerCreated = openpayAPI.CustomerService.Create(customer);
        //    var CustomerID = customerCreated.Id;
        //    User.CustomerID = CustomerID;

        //    request = openpayAPI.CardService.Create(User.CustomerID, request);

        //    User.Stripe_id = request.Id;

        //    try
        //    {
        //        BankAccount Bankrequest = new BankAccount();
        //        Bankrequest.HolderName = User.Nombre_usuario;
        //        Bankrequest.Alias = "Cuenta principal";
        //        Bankrequest.CLABE = Clabe;


        //        Bankrequest = openpayAPI.BankAccountService.Create(User.CustomerID, Bankrequest);
        //        User.CuentaBancariaID = Bankrequest.Id;
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    try
        //    {
        //        var obj1 = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == user2_Facebook_profile_id);
        //        if (obj1 != null)
        //        {
        //            foreach (var trip in obj1)
        //            {
        //                trip.Conductor_a_asociado_su_tarjeta = true;
        //                //trip.
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }


        //    db.SaveChanges();
        //    return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        //}
        class Address_Object
        {
            public string city;
            public string line3;
            public string postal_code;
            public string line1;
            public string line2;
            public string state;
            public string country_code;
        }

        class CardObject
        {
            public string card_number;
            public string holder_name;
            public string expiration_year;
            public string expiration_month;
            public string cvv2;
            public Address_Object address;
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CreateChat(string FaceIdConductor, string FaceIdPasajero)
        {
            var conversation = FaceIdConductor + "&" + FaceIdPasajero;
            var obj1 = db.Travelers_Grupos_de_Conversacion.Where(x => x.AliasConversacion == conversation).FirstOrDefault();
            var my_id = 0;
            if (obj1 == null)
            {
                var Travel = new Travelers_Grupos_de_Conversacion();
                Travel.AliasConversacion = conversation;
                db.Travelers_Grupos_de_Conversacion.Add(Travel);
                my_id = Travel.ID;
                db.SaveChanges();
            }
            //obj1.pidiendo_viaje = Estado;

            ////////
            return Request.CreateResponse(HttpStatusCode.Accepted, my_id.ToString());
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage PostMessage(string message, string IDPasajero, string IDconductor, int IDConversation)
        {
            var conversation_obj = db.Travelers_Grupos_de_Conversacion.Where(x => x.ID == IDConversation).FirstOrDefault();
            //            ,[Id_de_conversacion]
            //,[FaceIdDeConductor]
            //,[FaceIdDePasajero]
            //,[Mensaje]
            //,[MensajeGrupal]
            //,[Entregado]
            //,[Leido]

            var Conversacion = new Travelers_Mensajes_Grupos_de_Conversacion();
            Conversacion.Id_de_conversacion = conversation_obj.ID.ToString();
            Conversacion.FaceIdDeConductor = IDconductor;
            Conversacion.FaceIdDePasajero = IDPasajero;
            Conversacion.Mensaje = message;
            Conversacion.MensajeGrupal = false;
            Conversacion.Entregado = false;
            Conversacion.Leido = false;
            db.Travelers_Mensajes_Grupos_de_Conversacion.Add(Conversacion);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, Conversacion.ID.ToString());


        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetConversationContextMessages(string GroupConversationIdentifier)
        {
            var message_list = db.Travelers_Mensajes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == GroupConversationIdentifier);
            var Utility = new ConverssationMessagesContextType();
            Utility.Mensajes = message_list.ToList();
            int New_ID = Convert.ToInt32(GroupConversationIdentifier);
            Utility.Grupo = db.Travelers_Grupos_de_Conversacion.Where(x => x.ID == New_ID).FirstOrDefault();
            string yourJson = JsonConvert.SerializeObject(Utility);// jsonObject;
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            return response;

        }
        class ConverssationMessagesContextType
        {
            public List<Travelers_Mensajes_Grupos_de_Conversacion> Mensajes;
            public Travelers_Grupos_de_Conversacion Grupo;
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetOutOfTheGroup(string faceIdOfReluctantPartiipant, int Id_ofGroupToAbandon)
        {
            try
            {
                var UserInGroup = db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == Id_ofGroupToAbandon.ToString() && x.FaceIdDeIntegrante == faceIdOfReluctantPartiipant).FirstOrDefault();
                db.Travelers_Integrantes_Grupos_de_Conversacion.Remove(UserInGroup);

                var allTheMembersInconversation = db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == Id_ofGroupToAbandon.ToString());
                allTheMembersInconversation.FirstOrDefault().IsGroupAdministrator = true;
                db.SaveChanges();


                return Request.CreateResponse(HttpStatusCode.Accepted, "Success");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Success");
            }

        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage MarkAsSeen(string faceIdOfReluctantParticipant, int Id_ofGroupConversayionSeen)
        {
            // var newId = Convert.ToInt3(Id_ofGroupConversayionSeen;)
            var queryOfbeingSeenGroup = db.Travelers_Grupos_de_Conversacion.Where(x => x.ID == Id_ofGroupConversayionSeen).FirstOrDefault();
            queryOfbeingSeenGroup.HasDriverSeenIt = true;
            queryOfbeingSeenGroup.HaspassengerSeenIt = true;

            try
            {
                if (queryOfbeingSeenGroup.IsGroupConversation == true)
                {
                    var member = db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == Id_ofGroupConversayionSeen.ToString() && x.FaceIdDeIntegrante == faceIdOfReluctantParticipant).FirstOrDefault();
                    member.HasSeenMesageInGroup = true;
                }
            }
            catch (Exception ex)
            {

            }
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, Id_ofGroupConversayionSeen);


        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage MessageSentFromUser(string MessageData, string SenderUser, string ConversationID, string FaceIDInterlocutor)
        {
            //    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/MessageSentFromUser?MessageData=" +
            //Mensaje +
            //"&SenderUser=" + queryLog.ProfileId +
            //"&ConversationID=" + Fragment_Conversation.IDdeConversacion.ToString()

            //));
            //ToDo: Create a new conversation if participants of this do not have any conversation
            int ConvId = Convert.ToInt32(ConversationID);
            var query1 = db.Travelers_Grupos_de_Conversacion.Where(x => x.FaceIdDeConductor == SenderUser && x.FaceIdDePasajero == FaceIDInterlocutor).FirstOrDefault();
            var query2 = db.Travelers_Grupos_de_Conversacion.Where(x => x.FaceIdDeConductor == FaceIDInterlocutor && x.FaceIdDePasajero == SenderUser).FirstOrDefault();
            var query3 = db.Travelers_Grupos_de_Conversacion.Where(x => x.ID == ConvId).FirstOrDefault();


            if (query1 != null)
            {
                query1.HasDriverSeenIt = query2 == null ? true : false;//Driver Is the sender
                query1.HaspassengerSeenIt = query2 == null ? false : true;//Driver Is the sender
            }
            if (query2 != null)
            {
                query2.HaspassengerSeenIt = query1 == null ? true : false;//Passenger Is the sender
                query2.HasDriverSeenIt = query1 == null ? false : true;//Driver Is the sender
            }
            if (query3 != null)
            {
                try
                {
                    if (query3.IsGroupConversation == true)
                    {
                        var ListOfAllMembersInGroup = db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == ConversationID);
                        foreach (var member in ListOfAllMembersInGroup)
                        {
                            if (member.FaceIdDeIntegrante != SenderUser)
                            {
                                member.HasSeenMesageInGroup = false;
                            }
                            else
                            {
                                member.HasSeenMesageInGroup = true;
                            }
                            // member.
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }


            if (query1 == null && query2 == null && query3 == null)
            {
                var newConversation = new Travelers_Grupos_de_Conversacion();
                newConversation.AliasConversacion = "";
                newConversation.FaceIdDeConductor = FaceIDInterlocutor;
                newConversation.FaceIdDePasajero = SenderUser;
                newConversation.IsGroupConversation = false;
                db.Travelers_Grupos_de_Conversacion.Add(newConversation);
                db.SaveChanges();
                //newConversation.
                ConversationID = newConversation.ID.ToString();

            }

            var Sender = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == SenderUser).FirstOrDefault();

            var mensaje = new Travelers_Mensajes_Grupos_de_Conversacion();
            mensaje.FaceIdDeConductor = SenderUser;
            mensaje.Id_de_conversacion = ConversationID;
            mensaje.Mensaje = MessageData;
            mensaje.NameOfSender = Sender.Nombre_usuario;
            db.Travelers_Mensajes_Grupos_de_Conversacion.Add(mensaje);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, ConversationID);

        }

        public class GroupMemberType
        {
            public List<string> FaceId;
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CreateGroupConversation(string FaceIDOfAplicationUser_GroupCreation, string GroupName, string ListOfMembers, int IDOfGrouptConversation)
        {
            //Add existance of group
            try
            {
                if (IDOfGrouptConversation == 0)
                {
                    var newGroup = new Travelers_Grupos_de_Conversacion();
                    newGroup.AliasConversacion = GroupName;
                    newGroup.FaceIdDeConductor = "";
                    newGroup.FaceIdDePasajero = "";
                    newGroup.IsGroupConversation = true;
                    db.Travelers_Grupos_de_Conversacion.Add(newGroup);
                    db.SaveChanges();

                    var newmemeber = new Travelers_Integrantes_Grupos_de_Conversacion();
                    newmemeber.FaceIdDeIntegrante = FaceIDOfAplicationUser_GroupCreation;
                    newmemeber.Id_de_conversacion = newGroup.ID.ToString();
                    newmemeber.IsGroupAdministrator = true;
                    db.Travelers_Integrantes_Grupos_de_Conversacion.Add(newmemeber);

                    var NewChain = Base64Decode(ListOfMembers);
                    var Array = JsonConvert.DeserializeObject<GroupMemberType>(NewChain);
                    foreach (var Member in Array.FaceId)
                    {
                        var newmemeber_L = new Travelers_Integrantes_Grupos_de_Conversacion();
                        newmemeber_L.FaceIdDeIntegrante = Member;
                        newmemeber_L.Id_de_conversacion = newGroup.ID.ToString();
                        newmemeber_L.IsGroupAdministrator = false;
                        db.Travelers_Integrantes_Grupos_de_Conversacion.Add(newmemeber_L);
                    }



                    db.SaveChanges();
                }
                else
                {
                    var OldGroup = db.Travelers_Grupos_de_Conversacion.Where(x => x.ID == IDOfGrouptConversation).FirstOrDefault();
                    var PersonRequestinToAddPersonToTheGroup = db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.FaceIdDeIntegrante == FaceIDOfAplicationUser_GroupCreation).FirstOrDefault();
                    if (OldGroup != null && PersonRequestinToAddPersonToTheGroup.IsGroupAdministrator == true)
                    {
                        var NewChain = Base64Decode(ListOfMembers);
                        var Array = JsonConvert.DeserializeObject<GroupMemberType>(NewChain);
                        foreach (var Member in Array.FaceId)
                        {
                            if (db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == IDOfGrouptConversation.ToString() && x.FaceIdDeIntegrante == Member).FirstOrDefault() == null)
                            {
                                var newmemeber_L = new Travelers_Integrantes_Grupos_de_Conversacion();
                                newmemeber_L.FaceIdDeIntegrante = Member;
                                newmemeber_L.Id_de_conversacion = OldGroup.ID.ToString();
                                newmemeber_L.IsGroupAdministrator = false;
                                db.Travelers_Integrantes_Grupos_de_Conversacion.Add(newmemeber_L);
                            }
                        }
                        db.SaveChanges();
                    }



                }
                return Request.CreateResponse(HttpStatusCode.Accepted, "");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "");
            }
        }

        public class UserInGroup
        {
            public string Name;
            public string faceId;
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage showMembersOfThegroup(int NumberOfBesearched_Groupidex, string UserIDOfWhomWantToSeeMembers)
        {
            if (db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == NumberOfBesearched_Groupidex.ToString() && x.FaceIdDeIntegrante == UserIDOfWhomWantToSeeMembers).FirstOrDefault() != null)
            {
                var all_members = db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.Id_de_conversacion == NumberOfBesearched_Groupidex.ToString());
                List<UserInGroup> MMM = new List<UserInGroup>();
                foreach (var member in all_members)
                {
                    var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == member.FaceIdDeIntegrante).FirstOrDefault();
                    var User_Obj = new UserInGroup();
                    User_Obj.faceId = User.Facebook_profile_id;
                    User_Obj.Name = User.Nombre_usuario;
                    MMM.Add(User_Obj);

                }
                //return Request.CreateResponse(HttpStatusCode.Accepted, all_members);
                // var Utility = new MessageResponseUtility();
                // Utility.Lista = Lista;
                // Utility.RolInConvo = RolInConvo;
                // Utility.IsThisRemarqued = Remarcs;
                //return Request.CreateResponse(HttpStatusCode.Accepted, list.ToArray());
                string yourJson = JsonConvert.SerializeObject(MMM);// jsonObject;
                var response = Request.CreateResponse(HttpStatusCode.Accepted);
                response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
                return response;

            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "");

            }
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchForUserToAddToGroup(string NumberOfBesearched)
        {
            //Travelers_Grupos_de_Conversacion


            try
            {

                var people = db.Traveler_Perfil.Where(x => x.Numero_de_telefono.Contains(NumberOfBesearched));
                return Request.CreateResponse(HttpStatusCode.Accepted, people);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "");
            }
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetConversationID(string FaceIDOfAplicationUser, string FaceIDOFPeopleTobeReached)
        {
            var ID1 = db.Travelers_Grupos_de_Conversacion.Where(x => x.FaceIdDeConductor == FaceIDOfAplicationUser && x.FaceIdDePasajero == FaceIDOFPeopleTobeReached).FirstOrDefault();
            var ID2 = db.Travelers_Grupos_de_Conversacion.Where(x => x.FaceIdDeConductor == FaceIDOFPeopleTobeReached && x.FaceIdDePasajero == FaceIDOfAplicationUser).FirstOrDefault();
            string ID_tosend = "";
            ID_tosend = ID1 != null && ID2 == null ? ID1.ID.ToString() : "";
            ID_tosend = ID2 != null && ID1 == null ? ID2.ID.ToString() : "";
            ID_tosend = ID2 == null && ID1 == null ? "NoConv" : ID_tosend;
            ID_tosend = ID2 != null && ID1 != null ? "Error" : ID_tosend;

            return Request.CreateResponse(HttpStatusCode.Accepted, ID_tosend);
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetAllMessagesFromConvo(string LogID)
        {
            //var list = db.Travelers_Mensajes_Grupos_de_Conversacion.Where(x => x.FaceIdDeConductor == LogID || x.FaceIdDePasajero == LogID);
            var list2 = db.Travelers_Grupos_de_Conversacion.Where(x => x.FaceIdDeConductor == LogID);
            var list3 = db.Travelers_Grupos_de_Conversacion.Where(x => x.FaceIdDePasajero == LogID);
            var list4 = db.Travelers_Integrantes_Grupos_de_Conversacion.Where(x => x.FaceIdDeIntegrante == LogID);
            List<Travelers_Grupos_de_Conversacion> Lista = new List<Travelers_Grupos_de_Conversacion>();
            List<String> RolInConvo = new List<string>();
            List<bool> Remarcs = new List<bool>();
            //query1.HasDriverSeenIt = query2 == null ? true : false;//Driver Is the sender
            //query2.HasDriverSeenIt = query1 == null ? true : false;//Passenger Is the sender
            foreach (var elem1 in list2)
            {
                if (!Lista.Contains(elem1))
                {
                    Lista.Add(elem1);
                    RolInConvo.Add("Conductor");
                    Remarcs.Add(elem1.HasDriverSeenIt == null ? false : Convert.ToBoolean(elem1.HasDriverSeenIt));
                }
            }
            foreach (var elem1 in list3)
            {
                if (!Lista.Contains(elem1))
                {
                    Lista.Add(elem1);
                    RolInConvo.Add("Pasajero");
                    Remarcs.Add(elem1.HaspassengerSeenIt == null ? false : Convert.ToBoolean(elem1.HaspassengerSeenIt));
                }
            }
            foreach (var elem1 in list4)
            {
                var NumericalConvId = Convert.ToInt32(elem1.Id_de_conversacion);
                var newElement = db.Travelers_Grupos_de_Conversacion.Where(x => x.ID == NumericalConvId).FirstOrDefault();
                if (!Lista.Contains(newElement))
                {
                    Lista.Add(newElement);
                    var ProvisionalTExt = !Convert.ToBoolean(elem1.IsGroupAdministrator) ? "Integrante" : "Administrador";
                    RolInConvo.Add(ProvisionalTExt);
                    Remarcs.Add(elem1.HasSeenMesageInGroup);
                }
            }
            var Utility = new MessageResponseUtility();
            Utility.Lista = Lista;
            Utility.RolInConvo = RolInConvo;
            Utility.IsThisRemarqued = Remarcs;
            //return Request.CreateResponse(HttpStatusCode.Accepted, list.ToArray());
            string yourJson = JsonConvert.SerializeObject(Utility);// jsonObject;
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            return response;

        }
        class MessageResponseUtility
        {
            public List<Travelers_Grupos_de_Conversacion> Lista;
            public List<String> RolInConvo;
            public List<bool> IsThisRemarqued;
        }
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetUserNameWithFaceID(string LogID_1)
        {
            var lisst = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == LogID_1).FirstOrDefault();
            if (lisst != null)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, lisst.Nombre_usuario);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "");
            }
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage Acciones_usuarioPasajero(string user1_Facebook_profile_id, int Estado, string latitud_un_decimal, string longitud_un_decimal)
        {
            var obj_conductores = db.Traveler_Perfil.Where(x => x.Rol == (int)(enumRoles.Conductor) && x.Latitud_un_decimal == latitud_un_decimal && x.Longitud_un_decimal == longitud_un_decimal && x.pidiendo_viaje == (int)enumEstado_del_usuario.PidiendoViajeConductor);
            var objeto_usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user1_Facebook_profile_id).FirstOrDefault();
            var viaje_temporal = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == user1_Facebook_profile_id).FirstOrDefault();
            estadoRespuetaPasajero respuesta = new estadoRespuetaPasajero();
            switch (Estado)
            {
                case (int)enumEstado_del_usuario.PidiendoViajePasajero:
                    switch (viaje_temporal.Estado_De_solicitud)
                    {
                        case (int)enumEstado_de_los_viajes.EnEspera:
                            respuesta.ViajesDisponibles = obj_conductores.Count();
                            respuesta.Status_para_pasajero = (int)enumEstado_de_los_viajes.EnEspera;
                            break;
                        case (int)enumEstado_de_los_viajes.Aceptado:
                            //var viaje_aceptado = db.Traveler_Viajes.Where(x => x.ID == viaje_temporal.Id_del_viaje).FirstOrDefault();
                            //var nuevo_conductor = obj_conductores.Where(x => x.Facebook_profile_id == viaje_aceptado.FaceIdDelConductor).FirstOrDefault();
                            //respuesta.Status_para_pasajero = (int)enumEstado_de_los_viajes.Aceptado;
                            //respuesta.Color_auto = "";
                            //respuesta.Destino = "";
                            //respuesta.distancia = "";
                            //respuesta.Latitud = "";
                            //respuesta.Longitud = "";
                            //respuesta.Modelo_de_auto = "";
                            //respuesta.nombreconductor = "";
                            //respuesta.ViajesDisponibles = "";


                            break;

                    }


                    break;
                case (int)enumEstado_del_usuario.ViajeEncontradoPasajero:
                    var viaje_aceptado = db.Traveler_Viajes.Where(x => x.ID == viaje_temporal.Id_del_viaje).FirstOrDefault();
                    var nuevo_conductor = obj_conductores.Where(x => x.Facebook_profile_id == viaje_aceptado.FaceIdDelConductor).FirstOrDefault();
                    respuesta.Status_para_pasajero = (int)enumEstado_de_los_viajes.aceptado_y_llendo_a_ubicacion;
                    //respuesta.Color_auto = "";
                    //respuesta.Destino = "";
                    //respuesta.distancia = "";
                    //respuesta.Latitud = "";
                    //respuesta.Longitud = "";
                    //respuesta.Modelo_de_auto = "";
                    //respuesta.nombreconductor = "";
                    //respuesta.ViajesDisponibles = "";
                    break;
                case (int)enumEstado_del_usuario.ViajandoPasajero:
                    break;
                case (int)enumEstado_del_usuario.EntregadoPasajero:
                    break;


            }
            // obj1.pidiendo_viaje = Estado;
            // db.SaveChanges();
            ////////
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo, string inicio, string destino)
        {


            var conductor = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceId).FirstOrDefault();
            var Disponible_wallet = false;

            try
            {
                if (conductor != null)
                {
                    Disponible_wallet = conductor.Stripe_id == "" ? false : true;
                }
            }
            catch (Exception) { }



            var cadena = Base64Decode(Argument);
            var aca = JsonConvert.DeserializeObject<Obj_publicacion>(cadena);

            var Traveler_Viajes_ = new Traveler_Viajes();
            Traveler_Viajes_.Asientos_Totales = aca.plazas;
            Traveler_Viajes_.Conductor_a_asociado_su_tarjeta = Disponible_wallet;
            Traveler_Viajes_.asientos_disponibles = aca.plazas;
            Traveler_Viajes_.Destino = destino;
            Traveler_Viajes_.Disponible = true;
            Traveler_Viajes_.En_curso = false;
            Traveler_Viajes_.Estado_del_viaje = (int)enumEstado_de_los_viajes.EnEspera;
            Traveler_Viajes_.FaceIdDelConductor = FaceId;
            Traveler_Viajes_.Fecha_y_hora = DateTime.UtcNow;
            Traveler_Viajes_.Inicio = inicio;
            Traveler_Viajes_.Kilometros = aca.Kilometros;
            Traveler_Viajes_.Latitud = aca.Lugar_Salida_Lat;
            Traveler_Viajes_.Latitud_destino = aca.Lugar_llegada_Lat;
            Traveler_Viajes_.Longitud = aca.Lugar_Salida_Long;
            Traveler_Viajes_.Longitud_destino = aca.Lugar_llegada_Long;
            Traveler_Viajes_.Tiempo_estimado = aca.Duracion;
            Traveler_Viajes_.TimeData = aca.fecha + "^" + aca.time;
            Traveler_Viajes_.Vehiculo = Vehiculo;
            Traveler_Viajes_.Costo_po_Usuario = Costo;
            Traveler_Viajes_.Viaje_Periodico = aca.Periodico;
            try
            {
                var date1 = new DateTime(Convert.ToInt32(aca.Ano), Convert.ToInt32(aca.Mes), Convert.ToInt32(aca.Dia), 8, 30, 52);
                date1.AddDays(1);
                Traveler_Viajes_.DiaDeViaje = date1.Day;
                Traveler_Viajes_.MesDeViaje = date1.Month;
                Traveler_Viajes_.AnoDeViaje = date1.Year;
                Traveler_Viajes_.Viaje_Periodico = aca.ViajePeriodico ? "1" : "0";
                Traveler_Viajes_.TipoDePago = aca.TipoDePago;
                //ToDo:  Set this accordingly to user preference
                Traveler_Viajes_.Visible = true & conductor.IdentidadComprobada;

            }
            catch (Exception)
            {

            }

            db.Traveler_Viajes.Add(Traveler_Viajes_);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage Get_Posted_travels(string Face_identifier)
        {

            //var obj1 = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == user6_Hijo).FirstOrDefault();
            var obj1 = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == Face_identifier);
            var Conductor = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_identifier).FirstOrDefault();
            List<PublicationDriverObj> myList = new List<PublicationDriverObj>();
            bool IdentityFlag = false;
            try
            {
                IdentityFlag = Convert.ToBoolean(Conductor.IdentidadComprobada);
            }
            catch (Exception) { }
            ////////
            if (obj1 != null)
            {
                foreach (var Trip in obj1)
                {
                    var TripRequest = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == Trip.ID);
                    List<RequestType> RequestList = new List<RequestType>();

                    Trip.Conductor_a_asociado_su_tarjeta &= Conductor.TarjetaValidada;
                    //Trip.Visible &= Conductor.IdentidadComprobada;


                    foreach (var requ in TripRequest)
                    {
                        var Solicitante = db.Traveler_Perfil.Where(c => c.Facebook_profile_id == requ.Face_id_solicitante).FirstOrDefault();
                        if (Solicitante != null)
                        {
                            RequestType request = new RequestType(requ.Face_id_solicitante, (int)(requ.Estado_De_solicitud), (int)(requ.Id_del_viaje), Solicitante.Foto, Solicitante.Nombre_usuario, requ.ID, IdentityFlag);
                            RequestList.Add(request);
                        }
                        else
                        {
                            RequestType request = new RequestType(requ.Face_id_solicitante, (int)(requ.Estado_De_solicitud), (int)(requ.Id_del_viaje), "", "Null Name", requ.ID, IdentityFlag);
                            RequestList.Add(request);
                        }

                    }
                    myList.Add(new PublicationDriverObj(Trip, RequestList, IdentityFlag));
                }
            }

            //Estado de la Solicitud !Important
            // public enum enumEstado_de_Solicitud {EnEspera, Aceptado, Rechazado };

            string yourJson = JsonConvert.SerializeObject(myList);// jsonObject;
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SendListOfAcceptedHitchHikers(string Face_Identifier_4, string EncodedStringHitchHikers)
        {
            var cadena = Base64Decode(EncodedStringHitchHikers);
            var aca = JsonConvert.DeserializeObject<List<SolicitudType>>(cadena);
            var counter = 0;
            foreach (var solicitante in aca)
            {
                if (solicitante.Aceptada == true)
                {
                    counter++;
                }
                //var myTrip2 = db.Traveler_Viajes.Where(x => x.ID == solicitante.TripID).FirstOrDefault();
                //var HitchHikerRequest = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == solicitante.ID_Solicitud).FirstOrDefault();
                //var myTrip1 = db.Traveler_Viajes.Where(x => x.ID == solicitante.TripID).FirstOrDefault();
                //if()

            }

            var Myval = aca.FirstOrDefault().TripID;
            var myTrip = db.Traveler_Viajes.Where(x => x.ID == Myval).FirstOrDefault();
            if (counter <= Convert.ToInt32(myTrip.asientos_disponibles))
            {
                foreach (var solicitante in aca)
                {
                    var HitchHikerRequest = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.ID == solicitante.ID_Solicitud).FirstOrDefault();
                    //public enum enumEstado_de_Solicitud {EnEspera, Aceptado, Rechazado };
                    HitchHikerRequest.Estado_De_solicitud = solicitante.Aceptada ? (int)(enumEstado_de_Solicitud.Aceptado) : (int)(enumEstado_de_Solicitud.Rechazado);
                    if (HitchHikerRequest.Estado_De_solicitud == (int)(enumEstado_de_Solicitud.Aceptado))
                    {
                        var FaceIDTemp = HitchHikerRequest.Face_id_solicitante;
                        var Usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDTemp).FirstOrDefault();
                        Push("Recuerda estar listo a tiempo para tu viaje", "Tu solicitud a Sido Aceptada", Usuario.FirebaseID);
                    }
                    else
                    {
                        var FaceIDTemp = HitchHikerRequest.Face_id_solicitante;
                        var Usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDTemp).FirstOrDefault();
                        Push("Sera mejor que busques de nuevo", "Tu solicitud a Sido Rechazada", Usuario.FirebaseID);

                    }
                }
                myTrip.asientos_disponibles = (Convert.ToInt32(myTrip.Asientos_Totales) - counter).ToString();
                db.SaveChanges();
            }
            //Asientos Totales son todos los asientos
            //Asientos disponibles son los que quedan


            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetStateOfRequestUnitarian(string Face_Id_Hitchhicker, Int32 TripID)
        {
            var request = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == Face_Id_Hitchhicker && x.Id_del_viaje == TripID).FirstOrDefault();

            var Empty = new Traveler_SolicitudDeViajeTemporal();
            Empty.Id_del_viaje = -1;

            return Request.CreateResponse(HttpStatusCode.Accepted, request == null ? Empty : request);

        }



        //Solicitud.Solicitante = Arg_Lis[i].Face_id_solicitante;
        //        Solicitud.Aceptada = false;
        //        Solicitud.ID_Solicitud = Arg_Lis[i].ID_de_request;
        //        Solicitud.TripID = Arg_Lis[i].Id_del_viaje;
        class SolicitudType
        {
            public string Solicitante;
            public bool Aceptada;
            public int ID_Solicitud;
            public int TripID;
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage UpdateFirebaseToken(string FaceID, string FirebaseID)
        {
            var usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID).FirstOrDefault();
            usuario.FirebaseID = FirebaseID;
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");

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

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage Change_travel_state(string Face_identifier_1, int Trip_Id, enumEstado_de_los_viajes Estado_de_viaje)
        {
            switch (Estado_de_viaje)
            {
                case enumEstado_de_los_viajes.Aceptado:
                    break;
                case enumEstado_de_los_viajes.aceptado_y_llendo_a_ubicacion:
                    break;
                case enumEstado_de_los_viajes.Conduciendo:
                    {
                        var obj = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == Face_identifier_1 && x.ID == Trip_Id).FirstOrDefault();
                        obj.En_curso = true;
                        obj.Estado_del_viaje = (int)(Estado_de_viaje);

                        var usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_identifier_1).FirstOrDefault();
                        usuario.pidiendo_viaje = (int)(enumEstado_del_usuario.ViajandoConductor);
                        usuario.Viaje_asociado = Trip_Id.ToString();
                        //usuario.pidiendo_viaje = (int)(enumEstado_del_usuario.PasajeroSinArribar);
                        int EstadoSolicitudAceptada = (int)(enumEstado_de_Solicitud.Aceptado);
                        var solicitudes = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == Trip_Id && x.Estado_De_solicitud == EstadoSolicitudAceptada);
                        foreach (var solicitud in solicitudes)
                        {
                            var Cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == solicitud.Face_id_solicitante).FirstOrDefault();
                            Cliente.pidiendo_viaje = (int)(enumEstado_del_usuario.ViajandoPasajero);
                            Cliente.Viaje_asociado = Trip_Id.ToString();
                        }
                        /////////////
                        db.SaveChanges();
                    }
                    break;
                case enumEstado_de_los_viajes.EnCurso:
                    {
                        //Todo: add validacion conducinendo
                        int EstadoTemp = (int)(enumEstado_de_los_viajes.EnCurso);
                        int EstadoTemp_Conduciendo = (int)(enumEstado_de_los_viajes.Conduciendo);
                        var trips = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == Face_identifier_1 && (x.Estado_del_viaje == EstadoTemp || x.Estado_del_viaje == EstadoTemp_Conduciendo) && x.ID == Trip_Id).FirstOrDefault();

                        if (trips != null)
                        {
                            //Todo; add response in app to this
                            return Request.CreateResponse(HttpStatusCode.Accepted, "AlreadyInTrip");
                        }
                        var obj = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == Face_identifier_1 && x.ID == Trip_Id).FirstOrDefault();

                        obj.Estado_del_viaje = (int)(Estado_de_viaje);

                        ///////////
                        int EstadoSolicitudAceptada = (int)(enumEstado_de_Solicitud.Aceptado);
                        var solicitudes = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == Trip_Id && x.Estado_De_solicitud == EstadoSolicitudAceptada);
                        foreach (var solicitud in solicitudes)
                        {
                            var usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == solicitud.Face_id_solicitante).FirstOrDefault();
                            usuario.pidiendo_viaje = (int)(enumEstado_del_usuario.PasajeroSinArribar);
                            usuario.Viaje_asociado = Trip_Id.ToString();
                        }
                        var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_identifier_1).FirstOrDefault();
                        //Driver.pidiendo_viaje = (int)(enumEstado_del_usuario.ViajandoConductor);
                        Driver.Viaje_asociado = Trip_Id.ToString();
                        Driver.pidiendo_viaje = (int)(enumEstado_del_usuario.ConductorEsperando);
                        db.SaveChanges();
                    }
                    break;
                case enumEstado_de_los_viajes.EnEspera:
                    break;
                //case enumEstado_de_los_viajes.EsperaDeAbordaje:
                //    break;
                case enumEstado_de_los_viajes.EsperaDeAbordaje:
                    {

                        var usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_identifier_1).FirstOrDefault();
                        usuario.pidiendo_viaje = (int)(enumEstado_del_usuario.Sin_actividad);
                        usuario.Viaje_asociado = 0.ToString();
                        var Solicitudes = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == Trip_Id);
                        foreach(var viajero in Solicitudes)
                        {
                            var Viajero_Perfil = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == viajero.Face_id_solicitante && x.Viaje_asociado ==Trip_Id.ToString() ).FirstOrDefault();
                            Viajero_Perfil.pidiendo_viaje = (int)(enumEstado_del_usuario.Sin_actividad);
                            Viajero_Perfil.Viaje_asociado = 0.ToString();
                        }


                        var obj = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == Face_identifier_1 && x.ID == Trip_Id).FirstOrDefault();

                        List<string> listaDePasajeros = new List<string>();
                        List<string> listaDePasajerosQuePagaronConTarjeta = new List<string>();
                        List<string> listaDePasajerosQueFaltaronConTarjeta = new List<string>();
                        var EstadoConduccion = (int)(enumEstado_de_Solicitud.Conduciendo);
                        var QueryPasajerosQueViajaron = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == Trip_Id && x.Estado_De_solicitud == EstadoConduccion);
                        var EstadoNollego = (int)(enumEstado_de_Solicitud.NoLlego);
                        var QueryPasajerosQueNollegaron = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == Trip_Id && x.Estado_De_solicitud == EstadoNollego);

                        foreach (var Solicitud in QueryPasajerosQueViajaron)
                        {
                            listaDePasajeros.Add(Solicitud.Face_id_solicitante);
                            if (Solicitud.TipoDePago != null)
                            {
                                if (Solicitud.TipoDePago == (int)enumTipoDePAgoPreferido.Tarjeta)
                                {
                                    listaDePasajerosQuePagaronConTarjeta.Add(Solicitud.Face_id_solicitante);
                                }
                            }
                            var personitav = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Solicitud.Face_id_solicitante).FirstOrDefault();
                            //SendReceiptEmail(string email, string TotalAmount, string TotalCard, string TotalCash, string TotalFare)
                            var CostoUsuario1 = Convert.ToDouble(obj.Costo_po_Usuario);
                            //CostoUsuario * 0.9255
                            var Cobro_por_tarjeta = (Solicitud.TipoDePago == (int)enumTipoDePAgoPreferido.Tarjeta) ? (CostoUsuario1 * 0.9255).ToString() : 0.ToString();
                            var Cobro_por_Efectivo = (Solicitud.TipoDePago == (int)enumTipoDePAgoPreferido.EnEfectivo) ? (CostoUsuario1).ToString() : 0.ToString();
                            var Cuota_de_solicitud = (CostoUsuario1 * 0.745).ToString();
                            SendReceiptEmail(personitav.Correo, "Costo Total $" + obj.Costo_po_Usuario, "Cobro Por Tarjeta $" + Cobro_por_tarjeta,
                                 "Cobro en efetivo" + Cobro_por_Efectivo,"Cuota de Solicitud $" + Cuota_de_solicitud);
                        }
                        foreach (var Solicitud in QueryPasajerosQueNollegaron)
                        {
                            listaDePasajeros.Add(Solicitud.Face_id_solicitante);
                            if (Solicitud.TipoDePago != null)
                            {
                                if (Solicitud.TipoDePago == (int)enumTipoDePAgoPreferido.Tarjeta)
                                {
                                    listaDePasajerosQueFaltaronConTarjeta.Add(Solicitud.Face_id_solicitante);
                                }
                            }
                        }

                        var ViajeAlHistorial = new Traveler_Viajes_historial();
                        ViajeAlHistorial.Inicio = obj.Inicio;
                        ViajeAlHistorial.Destino = obj.Destino;
                        ViajeAlHistorial.Kilometros = obj.Kilometros;
                        ViajeAlHistorial.Tiempo_estimado = obj.Tiempo_estimado;
                        ViajeAlHistorial.Fecha_y_hora = obj.Fecha_y_hora;
                        ViajeAlHistorial.Pasajeros_numero = (Convert.ToInt32(obj.Asientos_Totales) - Convert.ToInt32(obj.asientos_disponibles)).ToString();
                        ViajeAlHistorial.Pasajeros_IDS = String.Join("^", listaDePasajeros);

                        ViajeAlHistorial.FaceIdDelConductor = Face_identifier_1;

                        ViajeAlHistorial.TimeData = obj.TimeData;
                        var CostoUsuario = Convert.ToDouble(obj.Costo_po_Usuario);

                        ViajeAlHistorial.Costo_por_Usuario = obj.Costo_po_Usuario;
                        //((costoTemp + 1.086) + 3.0)
                        var cobro = (Convert.ToDouble(obj.Costo_po_Usuario));// * 1.0735) + 3);
                        ViajeAlHistorial.CobroPorPasajero = (cobro).ToString();
                        ViajeAlHistorial.GananciaConductor = CostoUsuario.ToString();
                        // ViajeAlHistorial.PasajerosQuePagaronConTarjeta = listaDePasajerosQuePagaronConTarjeta.Count() + listaDePasajerosQueFaltaronConTarjeta.Count();
                        ViajeAlHistorial.GananciaCarppi = (listaDePasajerosQuePagaronConTarjeta.Count() * cobro * 0.0435).ToString();

                        ViajeAlHistorial.ComicionMotorPago = ((listaDePasajerosQueFaltaronConTarjeta.Count() * cobro * 0.029) + 3).ToString();
                        db.Traveler_Viajes_historial.Add(ViajeAlHistorial);


                        //Customer customer = new Customer();
                        //customer.Name = usuario.Nombre_usuario;
                        //customer.LastName = lastName;
                        //customer.PhoneNumber = usuario.Numero_de_telefono;
                        //customer.Email = usuario.Correo;

                        var inicio_de_viaje = obj.Inicio.Split(','); ;
                        var Fin_de_viaje = obj.Destino.Split(','); ;
                        var Concepto = inicio_de_viaje[0] + " " + inicio_de_viaje[1] + " " + " -> " + Fin_de_viaje[0] + " " + Fin_de_viaje[1];

                        var Contador_de_pago = 0;
                        foreach (var Solicitud in QueryPasajerosQueViajaron)
                        {
                            //Todo: agregar Id del Cliente
                            //string customer_id = "adyytoegxm6boiusecxm";

                            if (Solicitud.TipoDePago == (int)enumTipoDePAgoPreferido.Tarjeta)
                            {
                                //var Pasajero = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Solicitud.Face_id_solicitante).FirstOrDefault();

                                ////Pasajero.Stripe_id = "kwkoqpg6fcvfse8k8mg2";
                                ////Pasajero.CustomerID= "adyytoegxm6boiusecxm";
                                //ChargeRequest request = new ChargeRequest();
                                //request.Method = "card";
                                //request.DeviceSessionId = Pasajero.deviceSessionId;
                                //request.SourceId = Pasajero.Stripe_id;
                                //request.Description = "Viaje " + Concepto;
                                //request.Amount = new Decimal(Math.Round(cobro, 2,
                                //                 MidpointRounding.ToEven));

                                //Charge charge = openpayAPI.ChargeService.Create(Pasajero.CustomerID, request);
                                //Contador_de_pago += 1;
                            }

                        }
                        var Contador_NoLlegaron = 0;
                        foreach (var Solicitud in QueryPasajerosQueNollegaron)
                        {
                            if (Solicitud.TipoDePago == (int)enumTipoDePAgoPreferido.Tarjeta)
                            {
                                var Pasajero = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Solicitud.Face_id_solicitante).FirstOrDefault();

                                //Pasajero.Stripe_id = "kwkoqpg6fcvfse8k8mg2";
                                //Pasajero.CustomerID= "adyytoegxm6boiusecxm";
                                ChargeRequest request = new ChargeRequest();
                                request.Method = "card";
                               // request.DeviceSessionId = Pasajero.deviceSessionId;
                                request.SourceId = Pasajero.Stripe_id;
                                request.Description = "Ausencia en el Viaje " + Concepto;
                                request.Amount = new Decimal(Math.Round(26.0, 2,
                                                 MidpointRounding.ToEven));

                                //Charge charge = openpayAPI.ChargeService.Create(Pasajero.CustomerID, request);
                                Contador_NoLlegaron += 1;
                            }
                            //listaDePasajeros.Add(Solicitud.Face_id_solicitante);
                        }

                       // string customer_id = usuario.CustomerID;
                        //BankAccount bankAccount = new BankAccount();
                        //bankAccount.CLABE = "012298026516924616";// usuario.ClabeInterbancaria;
                        //bankAccount.HolderName = usuario.Nombre_usuario;
                        Decimal TotalAmount = new Decimal(0.0);
                        try
                        {
                            switch (obj.TipoDePago)
                            {
                                case (int)enumTipoDePAgoPreferido.Tarjeta:
                                    {
                                        //BankAccount bankAccount = openpayAPI.BankAccountService.Get(usuario.CustomerID, usuario.CuentaBancariaID);

                                        //PayoutRequest reques_pt = new PayoutRequest();
                                        //reques_pt.Method = "bank_account";
                                        //reques_pt.DestinationId = bankAccount.Id;
                                        ////reques_pt.BankAccount = bankAccount;
                                        ////reques_pt.Card = card;
                                        //reques_pt.Amount = new Decimal(Math.Round(((CostoUsuario * 0.9255) * Contador_de_pago) + (Contador_NoLlegaron * 20.0) + 3.0, 2,
                                        //                     MidpointRounding.ToEven));
                                        //reques_pt.Description = "Viaje " + Concepto;
                                        //TotalAmount = reques_pt.Amount;

                                        //Payout payout = openpayAPI.PayoutService.Create(usuario.CustomerID, reques_pt);
                                    }
                                    break;
                                case (int)enumTipoDePAgoPreferido.Ambos:
                                    {
                                        //BankAccount bankAccount = openpayAPI.BankAccountService.Get(usuario.CustomerID, usuario.CuentaBancariaID);

                                        //PayoutRequest reques_pt = new PayoutRequest();
                                        //reques_pt.Method = "bank_account";
                                        //reques_pt.DestinationId = bankAccount.Id;
                                        ////reques_pt.BankAccount = bankAccount;
                                        ////reques_pt.Card = card;
                                        //reques_pt.Amount = new Decimal(Math.Round(((CostoUsuario * 0.9255) * listaDePasajerosQuePagaronConTarjeta.Count()) + (listaDePasajerosQueFaltaronConTarjeta.Count() * 20.0) + 3.0, 2,
                                        //                     MidpointRounding.ToEven));
                                        //reques_pt.Description = "Viaje " + Concepto;
                                        //TotalAmount = reques_pt.Amount;

                                        //Payout payout = openpayAPI.PayoutService.Create(usuario.CustomerID, reques_pt);
                                    }
                                    //listaDePasajerosQuePagaronConTarjeta.Count() + listaDePasajerosQueFaltaronConTarjeta.Count();
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {

                        }




                        db.Traveler_Viajes.Remove(obj);
                        foreach (var Solicitud in QueryPasajerosQueViajaron)
                        {
                            db.Traveler_SolicitudDeViajeTemporal.Remove(Solicitud);
                            //listaDePasajeros.Add(Solicitud.Face_id_solicitante);
                        }



                        //obj.Estado_del_viaje = (int)(enumEstado_de_los_viajes.Finalizado);

                        //usuario.pidiendo_viaje = (int)(enumEstado_del_usuario.PasajeroSinArribar);
                        try
                        {
                            var GenteSinTarjeta = (Convert.ToInt32(obj.Asientos_Totales) - Convert.ToInt32(obj.asientos_disponibles)) - listaDePasajerosQuePagaronConTarjeta.Count;
                            var GanancialTotales = cobro * (Convert.ToInt32(obj.Asientos_Totales) - Convert.ToInt32(obj.asientos_disponibles));
                            //SendReceiptEmail(string email, string TotalAmount, string TotalCard, string TotalCash, string TotalFare)
                            var Gaancias_Tarjeta =( listaDePasajerosQuePagaronConTarjeta.Count * (cobro * 0.9255)) +(cobro * GenteSinTarjeta);
                            SendReceiptEmail(usuario.Correo,
                                "Cobro en Bruto $" + (GanancialTotales).ToString(),
                                "Ganancias Netas $" + (Gaancias_Tarjeta).ToString(), 
                               "Ganancias Por Tarjeta $"+ ((listaDePasajerosQuePagaronConTarjeta.Count * (cobro * 0.9255))).ToString(),
                               "-Cuota de solicitud $" + (TotalAmount * (Decimal)(1 - 0.9255)).ToString());
                        }
                        catch (Exception) { }
                        db.SaveChanges();
                    }
                    break;
                case enumEstado_de_los_viajes.Finalizado:
                    {
                        var wins = FinishRequest(Trip_Id, Face_identifier_1);

                        try
                        {
                            var usuario = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_identifier_1).FirstOrDefault();
                            SendReceiptEmail(usuario.Correo,
                                "Ganancias $" + (wins).ToString(),
                                "-",
                               "-",
                               "-");
                        }
                        catch (Exception) { }
                    }
                    break;
            }
            if (Estado_de_viaje == enumEstado_de_los_viajes.EnCurso)
            {

            }
            else
            {
                //var obj = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == Face_identifier_1 && x.ID == Trip_Id).FirstOrDefault();

                //obj.Estado_del_viaje = (int)(Estado_de_viaje);
                //db.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }
        [HttpGet]
        public HttpResponseMessage UpdateStripeTutorID(string Tutori_participantToUpdate, string StripeComercianteID)
        {
            var User = db.Traveler_Perfil.Where(x=> x.Facebook_profile_id == Tutori_participantToUpdate).FirstOrDefault();
           // var User = db.TutoriUsuarios.Where(x => x.FaceID == Tutori_participantToUpdate).FirstOrDefault();
            User.StripeDriverID = StripeComercianteID;
            db.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.Accepted);

        }
        public double FinishRequest(Int32 Trip_Id, string Face_identifier_1) 
        {
            var Solicitudes = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == Trip_Id);
            var driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_identifier_1).FirstOrDefault();
            driver.pidiendo_viaje = (int)(enumEstado_del_usuario.Sin_actividad);
           // driver.Viaje_asociado = 0.ToString();
            var r_return = 0.0;
            foreach (var viajero in Solicitudes)
            {
                if (viajero.Estado_De_solicitud == (int)enumEstado_de_Solicitud.Aceptado)
                {
                    var Viajero_Perfil = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == viajero.Face_id_solicitante && x.Viaje_asociado == Trip_Id.ToString()).FirstOrDefault();
                    var rreturn = CaptureFunds(viajero.PaymentIntentID);
                    if (rreturn == true)
                    {
                        Viajero_Perfil.pidiendo_viaje = (int)(enumEstado_del_usuario.EsperandoCalificar);
                        Viajero_Perfil.Viaje_asociado = 0.ToString();
                        r_return += (Convert.ToDouble(viajero.MoneyOwnedToDriver));

                        SendReceiptEmail(Viajero_Perfil.Correo, "Costo Total $" + ((Convert.ToDouble(viajero.MoneyOwnedToDriver)) + (((Convert.ToDouble(viajero.MoneyOwnedToDriver) * 0.09) + 4.50))).ToString(), "Cobro Por Tarjeta $" + viajero.MoneyOwnedToDriver,
                            "-", "Cuota de Solicitud $" + (((Convert.ToDouble(viajero.MoneyOwnedToDriver) * 0.09) + 4.50)).ToString());
                    }
                }
                else
                {
                    db.Traveler_SolicitudDeViajeTemporal.Remove(viajero);
                }
              
            }

            var viaje = db.Traveler_Viajes.Where(x => x.ID == Convert.ToInt32(driver.Viaje_asociado)).FirstOrDefault();
            db.Traveler_Viajes.Remove(viaje);



                 driver.Viaje_asociado = 0.ToString();
            db.SaveChanges();

            return r_return;

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

                var service = new PaymentIntentService();

                var intent = service.Capture(PaymentIntent);
                r_turn = true;
            }
            catch (Exception)
            {

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
                Push(ep.Message, "failed to send email with the following error:", ME.FirebaseID);
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage UpdateEmail(string FaceIdentifier, string Email)
        {
            //Todo: aDD "En Curso" when the driver switches
            var USER = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIdentifier).FirstOrDefault();
            // USER.deviceSessionId = deviceSessionId;
          
            try
            {
                USER.Correo = Email;
                //Driver.
            }
            catch (Exception) { }
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");

        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage UpdateIdentityVerification(string FaceIdentifier2, bool IdentityValidation)
        {
            //Todo: aDD "En Curso" when the driver switches
            var USER = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIdentifier2).FirstOrDefault();

            var trips = db.Traveler_Viajes.Where(x => x.FaceIdDelConductor == FaceIdentifier2);
            foreach(var trip in trips)
            {
                trip.Visible = IdentityValidation;
            }
            USER.IdentidadComprobada = IdentityValidation;
            db.SaveChanges();
           
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");

        }
        public enum StatesOfDriver {isInDeliverRegion,TripRequestAvailable, IsGoingForThePasenger, HasNoRequest, IsNotAvailable, isInPickUpPassengerRegion, IsGoingToTheGoal};

        public class RideShareDriverStateResponse 
        {
            public StatesOfDriver statesOfDriver;
            public CarppiRequestForDrive carppiRequest;
            public double LatitudPasajero;
            public double LongitudPasajero;
        }
        public enum StateOfRdeshare { RequestCreated, RequestAccepted, RequestCanceled, TripFinished, TripStarted, HasDriverArrived_pushNotif };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage DriverStateDetermination(string FaceBookIdentifierDriverState)
        {
            //Todo: aDD "En Curso" when the driver switches
            var USER = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceBookIdentifierDriverState).FirstOrDefault();
            // USER.deviceSessionId = deviceSessionId;
            if (USER.pidiendo_viaje == (int)enumEstado_del_usuario.DisponibleParaRideshare || USER.pidiendo_viaje == (int)enumEstado_del_usuario.Sin_actividadRideShare)
            {
                var response_driver = new RideShareDriverStateResponse();
                DriverStateDeterminationResponse variable = new DriverStateDeterminationResponse();
                var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceBookIdentifierDriverState && x.pidiendo_viaje == (int)enumEstado_del_usuario.DisponibleParaRideshare && x.IsUserADriver == true).FirstOrDefault();
                if (Driver != null)
                {
                    var canceled = (int)StateOfRdeshare.RequestCanceled;
                    //si hay una request con el usuario como conductor, y el estado es diferente de terminado, te da el viaje
                    var Request_Trip = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == Driver.Facebook_profile_id && x.Stat != (int)StateOfRdeshare.TripFinished).FirstOrDefault();
                    if (Request_Trip != null)
                    {
                        var CircleSoround = GenerateCircle(0.0003, Convert.ToDouble(Request_Trip.LatitudViajePendiente), Convert.ToDouble(Request_Trip.LongitudViajePendiente));
                        var IsInDeliverRegion = PointInPolygon(CircleSoround.X_Array, CircleSoround.Y_Array, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));


                        var passenger = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Request_Trip.FaceIDPassenger).FirstOrDefault();
                        var CircleSoroundingPasenger = GenerateCircle(0.0003, Convert.ToDouble(Request_Trip.OriginOfRequest_Latitude), Convert.ToDouble(Request_Trip.OriginOfRequest_Longitude));
                        var IsInPickUpRegion = PointInPolygon(CircleSoroundingPasenger.X_Array, CircleSoroundingPasenger.Y_Array, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));
                        if(Request_Trip.Stat == (int)StateOfRdeshare.RequestCreated)
                        {
                            response_driver.LatitudPasajero = Convert.ToDouble(passenger.Latitud);
                            response_driver.LongitudPasajero = Convert.ToDouble(passenger.Longitud);
                            response_driver.carppiRequest = Request_Trip;
                            response_driver.statesOfDriver = StatesOfDriver.TripRequestAvailable;
                        }
                        else if (Request_Trip.Stat == (int)StateOfRdeshare.RequestCanceled)
                        {
                            response_driver.carppiRequest = new CarppiRequestForDrive();
                            response_driver.statesOfDriver = StatesOfDriver.HasNoRequest;
                        }
                        else if (Request_Trip.Stat == (int)StateOfRdeshare.RequestAccepted && !IsInPickUpRegion)
                        {
                            response_driver.carppiRequest = Request_Trip;
                            response_driver.statesOfDriver = StatesOfDriver.IsGoingForThePasenger;
                            response_driver.LatitudPasajero =Convert.ToDouble(Request_Trip.OriginOfRequest_Latitude);
                            response_driver.LongitudPasajero = Convert.ToDouble(Request_Trip.OriginOfRequest_Longitude);


                        }
                        else if (Request_Trip.Stat == (int)StateOfRdeshare.RequestAccepted && IsInPickUpRegion)
                        {
                            var Passenger = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Request_Trip.FaceIDPassenger).FirstOrDefault();
                            response_driver.carppiRequest = Request_Trip;
                            Request_Trip.Stat = (int)StateOfRdeshare.HasDriverArrived_pushNotif;

                            Push("Tu Conductor llego!", "Llego!", Passenger.FirebaseID);
                            db.SaveChanges();
                            //response_driver.statesOfDriver = StatesOfDriver.;
                            //response_driver.statesOfDriver = StatesOfDriver.isInPickUpPassengerRegion;
                        }
                        else if (Request_Trip.Stat == (int)StateOfRdeshare.HasDriverArrived_pushNotif && IsInPickUpRegion)
                        {
                            response_driver.carppiRequest = Request_Trip;
                            //Request_Trip.Stat = (int)StateOfRdeshare.HasDriverArrived_pushNotif;
                            //db.SaveChanges();
                            //response_driver.statesOfDriver = StatesOfDriver.;
                            response_driver.statesOfDriver = StatesOfDriver.isInPickUpPassengerRegion;
                        }
                        else if (Request_Trip.Stat == (int)StateOfRdeshare.TripStarted && !IsInDeliverRegion)
                        {
                            response_driver.carppiRequest = Request_Trip;
                            response_driver.statesOfDriver = StatesOfDriver.IsGoingToTheGoal;
                        }
                        else if (Request_Trip.Stat == (int)StateOfRdeshare.TripStarted && IsInDeliverRegion)
                        {
                            response_driver.carppiRequest = Request_Trip;
                            response_driver.statesOfDriver = StatesOfDriver.isInDeliverRegion;
                        }


                        //if (IsInDeliverRegion && Request_Trip.Stat == (int) StateOfRdeshare.TripStarted)
                        //{
                        //    response_driver.carppiRequest = Request_Trip;
                        //    response_driver.statesOfDriver = StatesOfDriver.isInDeliverRegion;
                        //    //return Request.CreateResponse(HttpStatusCode.OK, Request_Trip);

                        //}
                        //else if (!IsInDeliverRegion && Request_Trip.Stat == (int)StateOfRdeshare.TripStarted)
                        //{
                        //    response_driver.carppiRequest = Request_Trip;
                        //    response_driver.statesOfDriver = StatesOfDriver.IsGoingForThePasenger;
                        //    //return Request.CreateResponse(HttpStatusCode.OK, Request_Trip);

                        //}
                        ///////
                        //else if (Request_Trip.Stat == (int)StateOfRdeshare.RequestCreated)
                        //{
                        //    response_driver.carppiRequest = Request_Trip;
                        //    response_driver.statesOfDriver = StatesOfDriver.isInDeliverRegion;
                        //    //return Request.CreateResponse(HttpStatusCode.OK, Request_Trip);

                        //}
                        //else if (Request_Trip.Stat == (int)StateOfRdeshare.RequestCreated)
                        //{
                        //    response_driver.carppiRequest = Request_Trip;
                        //    response_driver.statesOfDriver = StatesOfDriver.TripRequestAvailable;
                        //    // return Request.CreateResponse(HttpStatusCode.Found, Request_Trip);
                        //}
                        //else if (Request_Trip.Stat == (int)StateOfRdeshare.RequestAccepted)
                        //{
                        //    response_driver.carppiRequest = Request_Trip;
                        //    response_driver.statesOfDriver = StatesOfDriver.IsAttendingRequest;
                        //}
                        //else if (Request_Trip.Stat == (int)StateOfRdeshare.RequestCanceled)
                        //{
                        //    response_driver.carppiRequest = new CarppiRequestForDrive();
                        //    response_driver.statesOfDriver = StatesOfDriver.HasNoRequest;
                        //}
                        //return Request.CreateResponse(HttpStatusCode.Accepted, Request_Trip);
                    }
                    else
                    {
                        response_driver.carppiRequest = new CarppiRequestForDrive();
                        response_driver.statesOfDriver = StatesOfDriver.HasNoRequest;
                        //return Request.CreateResponse(HttpStatusCode.Continue, "None");
                    }
                }
                else {
                    response_driver.carppiRequest = new CarppiRequestForDrive();
                    response_driver.statesOfDriver = StatesOfDriver.IsNotAvailable;
                }
                variable.rideShareDriverStateResponse = response_driver;
                variable.pidiendoViaje = Convert.ToInt32(USER.pidiendo_viaje);
                // return Request.CreateResponse(HttpStatusCode.BadRequest, "None");
                return Request.CreateResponse(HttpStatusCode.Accepted, variable);
            }
            else
            {
                DriverStateDeterminationResponse variable = new DriverStateDeterminationResponse();
                variable.pidiendoViaje = Convert.ToInt32(USER.pidiendo_viaje);
                try
                {
                    variable.TripID = Convert.ToInt32(USER.Viaje_asociado);
                    var ResponseDeterminationVariable = (USER.pidiendo_viaje == (int)enumEstado_del_usuario.ViajandoPasajeroRideShare) ||
                        (USER.pidiendo_viaje == (int)enumEstado_del_usuario.PasajeroEsperandoconductorRideshare)
                        || (USER.pidiendo_viaje == (int)enumEstado_del_usuario.BusquedaEnProcesoRideShare)
                        || (USER.pidiendo_viaje == (int)enumEstado_del_usuario.EsperandoCalificarRideshare);

                    if (!ResponseDeterminationVariable)
                    {
                        var MyTrip = db.Traveler_Viajes.Where(x => x.ID == variable.TripID).FirstOrDefault();
                        Traveler_SolicitudDeViajeTemporal ReservationState;
                        if (variable.TripID != 0)
                        {
                            ReservationState = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Id_del_viaje == variable.TripID && x.Face_id_solicitante == FaceBookIdentifierDriverState).FirstOrDefault();
                        }
                        else
                        {
                            ReservationState = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == FaceBookIdentifierDriverState).FirstOrDefault();
                        }
                        var FaceIDDriver = MyTrip.FaceIdDelConductor;
                        var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDDriver).FirstOrDefault();

                        variable.LatitudConductor = Driver.Latitud;
                        variable.LongitudConductor = Driver.Longitud;
                        variable.LatitudObjetivo = MyTrip.Latitud_destino;
                        variable.LongitudObjetivo = MyTrip.Longitud_destino;
                        variable.EstadodelUsuario = Convert.ToInt32(USER.pidiendo_viaje);
                        variable.distane_Driver_Objective = HaversineDistance(new LatLng(Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud)), new LatLng(Convert.ToDouble(MyTrip.Latitud_destino), Convert.ToDouble(MyTrip.Longitud_destino)), DistanceUnit.Kilometers);
                    }
                    else if (USER.pidiendo_viaje == (int)enumEstado_del_usuario.PasajeroEsperandoconductorRideshare)
                    {
                        var MyTrip = db.CarppiRequestForDrive.Where(x => x.ID == variable.TripID).FirstOrDefault();
                        var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == MyTrip.FaceIDDriver).FirstOrDefault();

                        variable.LatitudConductor = Driver.Latitud;
                        variable.LongitudConductor = Driver.Longitud;
                        variable.LatitudObjetivo = (MyTrip.OriginOfRequest_Latitude).ToString();
                        variable.LongitudObjetivo = MyTrip.OriginOfRequest_Longitude.ToString();
                        variable.EstadodelUsuario = Convert.ToInt32(USER.pidiendo_viaje);
                        variable.Marca_Vehiculo = Driver.Marca_Vehiculo;
                        variable.Color_Vehiculo = Driver.Color_Vehiculo;
                        variable.Modelo_Vehiculo = Driver.Modelo_Vehiculo;
                        variable.Placa_Vehiculo = Driver.Placa;

                        variable.distane_Driver_Objective = HaversineDistance(new LatLng(Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud)), new LatLng(Convert.ToDouble(MyTrip.LatitudViajePendiente), Convert.ToDouble(MyTrip.LongitudViajePendiente)), DistanceUnit.Kilometers);

                    }
                    else if (USER.pidiendo_viaje == (int)enumEstado_del_usuario.ViajandoPasajeroRideShare)
                    {
                        var MyTrip = db.CarppiRequestForDrive.Where(x => x.ID == variable.TripID).FirstOrDefault();
                        var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == MyTrip.FaceIDDriver).FirstOrDefault();

                        variable.LatitudConductor = Driver.Latitud;
                        variable.LongitudConductor = Driver.Longitud;
                        variable.LatitudObjetivo = (MyTrip.LatitudViajePendiente).ToString();
                        variable.LongitudObjetivo = MyTrip.LongitudViajePendiente.ToString();
                        variable.EstadodelUsuario = Convert.ToInt32(USER.pidiendo_viaje);
                        variable.distane_Driver_Objective = HaversineDistance(new LatLng(Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud)), new LatLng(Convert.ToDouble(MyTrip.LatitudViajePendiente), Convert.ToDouble(MyTrip.LongitudViajePendiente)), DistanceUnit.Kilometers);

                    }



                    //Driver.
                }
                catch (Exception) { }
                db.SaveChanges();
                string yourJson = JsonConvert.SerializeObject(variable);// jsonObject;
                var response = Request.CreateResponse(HttpStatusCode.Accepted);
                response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
                return response;
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
        public class LatLng
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public LatLng()
            {
            }

            public LatLng(double lat, double lng)
            {
                this.Latitude = lat;
                this.Longitude = lng;
            }
        }

        //public double ConvertToRadians(double angle)
        //{
        //    return (Math.PI / 180) * angle;
        //}
        public double HaversineDistance(LatLng pos1, LatLng pos2, DistanceUnit unit)
        {
            double R = (unit == DistanceUnit.Miles) ? 3960 : 6371;
            var lat = ConvertToRadians(pos2.Latitude - pos1.Latitude);
            var lng = ConvertToRadians(pos2.Longitude - pos1.Longitude);
            var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                          Math.Cos(ConvertToRadians(pos1.Latitude)) * Math.Cos(ConvertToRadians(pos2.Latitude)) *
                          Math.Sin(lng / 2) * Math.Sin(lng / 2);
            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));
            return R * h2;
        }

        public enum DistanceUnit { Miles, Kilometers };
        public class DriverStateDeterminationResponse
        {
            public int pidiendoViaje;
            public int TripID;
            public string LatitudConductor;
            public string LongitudConductor;
            public string LatitudObjetivo;
            public string LongitudObjetivo;
            public double MyLatitud;
            public double MyLongitud;
            public double distane_Me_Driver;
            public double distane_Driver_Objective;
            public int EstadodelUsuario;
            public string Marca_Vehiculo;
            public string Modelo_Vehiculo;
            public string Placa_Vehiculo;
            public string Color_Vehiculo;
            public RideShareDriverStateResponse rideShareDriverStateResponse;
        }



        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ClientCalculateProximityLeaves(double Latitud_salida, double Longitud_salida, double Latitud_Llegada, double Longitud_Llegada, string FID_Solicitante, string day, string month, string year)
        {
            var Dist = Dist_LatLng(Latitud_salida, Longitud_salida, Latitud_Llegada, Longitud_Llegada);


            var Client = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FID_Solicitante).FirstOrDefault();
            if(Client != null)
            {
                Client.pidiendo_viaje = (int)enumEstado_del_usuario.BusquedaEnProceso;
                Client.BusquedaGuardada_Latitud_salida = Latitud_salida.ToString();
                Client.BusquedaGuardada_Longitud_salida = Longitud_salida.ToString();
                Client.BusquedaGuardada_Latitud_Llegada = Latitud_Llegada.ToString();
                Client.BusquedaGuardada_Longitud_Llegada = Longitud_Llegada.ToString();
                db.SaveChanges();

            }
            //Client.TarjetaValidada &= !String.IsNullOrEmpty(Client.Stripe_id);
            var DAteToBesearched = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), 8, 30, 52);
           // DAteToBesearched.AddDays(1);
            //Traveler_Viajes_.DiaDeViaje = date1.Day;
            //Traveler_Viajes_.MesDeViaje = date1.Month;
            //Traveler_Viajes_.AnoDeViaje = date1.Year;
            //Longitud es Y, latitud es X
            try
            {
                var complete_ = db.Traveler_Viajes.Where(x => x.ID > 0);
                List<Viaje_Propiedades> List_ofCompatible_trips = new List<Viaje_Propiedades>();


                //List<Traveler_Viajes> Shordistance_to_goal = new List<Traveler_Viajes>();
                //List<Traveler_Viajes> Mediumdistance_to_goal = new List<Traveler_Viajes>();
                //List<Traveler_Viajes> Longdistance_to_goal = new List<Traveler_Viajes>();



                //List<Traveler_Viajes> Shordistance_From_start = new List<Traveler_Viajes>();
                //List<Traveler_Viajes> Mediumdistance_From_start = new List<Traveler_Viajes>();
                //List<Traveler_Viajes> Longdistance_From_start = new List<Traveler_Viajes>();
                //0.009009009 is the relation between geodesic coordinates and kilometers
                var List1 = ListOfCirclePoints(Latitud_salida, Longitud_salida, ((Dist / 1000) * 0.07) * 0.009009009);
                var List2 = ListOfCirclePoints(Latitud_salida, Longitud_salida, ((Dist / 1000) * 0.15) * 0.009009009);
                var List3 = ListOfCirclePoints(Latitud_salida, Longitud_salida, ((Dist / 1000) * 0.30) * 0.009009009);
                var List4 = ListOfCirclePoints(Latitud_Llegada, Longitud_Llegada, ((Dist / 1000) * 0.07) * 0.009009009);
                var List5 = ListOfCirclePoints(Latitud_Llegada, Longitud_Llegada, ((Dist / 1000) * 0.15) * 0.009009009);
                var List6 = ListOfCirclePoints(Latitud_Llegada, Longitud_Llegada, ((Dist / 1000) * 0.30) * 0.009009009);
                foreach (var cord in complete_)
                {
                    var Flag1 = false;
                    var Flag2 = false;
                    var Flag3 = false;
                    var Flag4 = false;
                    var Flag5 = false;
                    var Flag6 = false;




                    //Todo: Mejor Poner una propiedad de que el viaje es periodico y una de que esta en la fecha, tambien hace que cuando se termine un viaje periodico,
                    // El viaje que ya existia actualice su dfecha
                    //aprte de la bandera de fecha, enviar la bandera del tipo de pago
                    try
                    {
                        var DAteToLookFor = new DateTime(Convert.ToInt32(cord.AnoDeViaje), Convert.ToInt32(cord.MesDeViaje), Convert.ToInt32(cord.DiaDeViaje), 8, 30, 52);
                        int DAteComparissionResult = DateTime.Compare(DAteToBesearched, DAteToLookFor);
                        if (DAteComparissionResult <= 0)
                        {
                            //// checking 
                            //if (value > 0)
                            //    Console.Write("date1 is later than date2. ");
                            //else if (value < 0)
                            //    Console.Write("date1 is earlier than date2. ");
                            //else
                            //    Console.Write("date1 is the same as date2. ");

                            //var temp = PointInPolygon(List1.X_Points.ToArray(), List1.Y_Points.ToArray(), Convert.ToDouble(cord.Latitud), Convert.ToDouble(cord.Longitud));
                            Flag1 = PointInPolygon(List1.X_Points.ToArray(), List1.Y_Points.ToArray(), Convert.ToDouble(cord.Latitud), Convert.ToDouble(cord.Longitud));
                            //if (true == Flag1)
                            //{
                            //    Shordistance_to_goal.Add(cord);
                            //}

                            //Longitud es Y, latitud es X
                            Flag2 = PointInPolygon(List2.X_Points.ToArray(), List2.Y_Points.ToArray(), Convert.ToDouble(cord.Latitud), Convert.ToDouble(cord.Longitud));
                            //if (true ==Flag2)
                            //{
                            //    Mediumdistance_to_goal.Add(cord);
                            //}

                            //Longitud es Y, latitud es X
                            Flag3 = PointInPolygon(List3.X_Points.ToArray(), List3.Y_Points.ToArray(), Convert.ToDouble(cord.Latitud), Convert.ToDouble(cord.Longitud));
                            //if (true== Flag3)
                            //{
                            //    Longdistance_to_goal.Add(cord);
                            //}
                            /////////////////////////////////


                            //Longitud es Y, latitud es X
                            Flag4 = PointInPolygon(List4.X_Points.ToArray(), List4.Y_Points.ToArray(), Convert.ToDouble(cord.Latitud_destino), Convert.ToDouble(cord.Longitud_destino));
                            //if (true==Flag4)
                            //{
                            //    Shordistance_From_start.Add(cord);
                            //}

                            //Longitud es Y, latitud es X
                            Flag5 = PointInPolygon(List5.X_Points.ToArray(), List5.Y_Points.ToArray(), Convert.ToDouble(cord.Latitud_destino), Convert.ToDouble(cord.Longitud_destino));
                            //if (true== Flag5)
                            //{
                            //    Mediumdistance_From_start.Add(cord);
                            //}

                            Flag6 = PointInPolygon(List6.X_Points.ToArray(), List6.Y_Points.ToArray(), Convert.ToDouble(cord.Latitud_destino), Convert.ToDouble(cord.Longitud_destino));
                            //if (true == Flag6)
                            //{
                            //    Longdistance_From_start.Add(cord);
                            //}

                            if ((Flag1 == true || Flag2 == true || Flag3 == true) && (Flag4 == true || Flag5 == true || Flag6 == true))
                            {
                                Flag2 = Flag1 == true ? false : Flag2;
                                Flag3 = Flag1 == true ? false : Flag3;
                                Flag3 = Flag2 == true ? false : Flag3;
                                ///
                                Flag5 = Flag4 == true ? false : Flag5;
                                Flag6 = Flag4 == true ? false : Flag6;
                                Flag6 = Flag2 == true ? false : Flag6;
                                var Dist_to_Goal = Dist_LatLng(Latitud_Llegada, Longitud_Llegada, Convert.ToDouble(cord.Latitud_destino), Convert.ToDouble(cord.Longitud_destino));
                                var Dist_to_Start = Dist_LatLng(Latitud_salida, Longitud_salida, Convert.ToDouble(cord.Latitud), Convert.ToDouble(cord.Longitud));



                                Traveler_Viajes PublicTrip = cord;
                                double costoTemp = Convert.ToDouble(PublicTrip.Costo_po_Usuario);
                                // costoTemp = costoTemp;//((costoTemp * 1.0735) + 3.0);
                                PublicTrip.Costo_po_Usuario = costoTemp.ToString();
                                //List_ofCompatible_trips.Add(new Viaje_Propiedades(cord, Flag1, Flag2, Flag3, Flag4, Flag5, Flag6, Dist_to_Goal, Dist_to_Start));
                                var ReservationObject = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == FID_Solicitante && x.Id_del_viaje == PublicTrip.ID).FirstOrDefault();

                                List_ofCompatible_trips.Add(new Viaje_Propiedades(PublicTrip, Flag1, Flag2, Flag3, Flag4, Flag5, Flag6, Dist_to_Goal, Dist_to_Start, null, ReservationObject));
                            }

                        }
                    }
                    catch (Exception)
                    {

                    }
                    //var Longdistance_From_start = db.Traveler_Viajes.Where(x => PointInPolygon(List6.X_Points.ToArray(), List6.Y_Points.ToArray(), Convert.ToDouble(x.Latitud), Convert.ToDouble(x.Longitud)));
                }
                //complete_.ForEach(p => p.CanSendMeetingRequest = CheckMeetingSettings(6327, p.Id));
                //var Shordist
                //people.ForEach(p => p.CanSendMeetingRequest = CheckMeetingSettings(6327, p.Id));
                // var Shordistance_to_goal = db.Traveler_Viajes.Where(x => PointInPolygon(List1.X_Points.ToArray(), List1.X_Points.ToArray(), Convert.ToDouble(x.Latitud_destino), Convert.ToDouble(x.Longitud_destino)));


                //Longitud es Y, latitud es X

                //var medium_donut_goal = Mediumdistance_to_goal.Except(Shordistance_to_goal);
                //var MEdium_donut_start = Mediumdistance_From_start.Except(Shordistance_From_start);


                //var Lista_de_viajes = new Listas_de_viajes(List_ofCompatible_trips);
                var orderedlid = List_ofCompatible_trips.OrderBy(pet => (pet.Bandera_Cerca_Final ? 1 : 0) * 1 +
                                                                        (pet.Bandera_Cerca_inicio ? 1 : 0) * 1 +
                                                                        (pet.Bandera_Lejos_Final ? 1 : 0) * 4 +
                                                                        (pet.Bandera_Lejos_inicio ? 1 : 0) * 4 +
                                                                        (pet.Bandera_Medio_Final ? 1 : 0) * 2 +
                                                                        (pet.Bandera_Medio_inicio ? 1 : 0) * 2);
                //string yourJson = JsonConvert.SerializeObject(List_ofCompatible_trips);// jsonObject;
                List<RewtunrDataOfSearch> n_return = new List<RewtunrDataOfSearch>();
                foreach(var elem in orderedlid)
                {
                    var Phothoofdriver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == elem.Viaje_Data.FaceIdDelConductor).FirstOrDefault();
                    var asd = new RewtunrDataOfSearch();
                    asd.Photo = Phothoofdriver.Foto;
                    asd.DriverLat =Convert.ToDouble( Phothoofdriver.Latitud);
                    asd.driverLong = Convert.ToDouble(Phothoofdriver.Longitud);
                    asd.Rate = 5;
                    asd.TripId = elem.Viaje_Data.ID;
                    n_return.Add(asd);
                }


                string yourJson = JsonConvert.SerializeObject(n_return);// jsonObject;
                var response = Request.CreateResponse(HttpStatusCode.Accepted);
                response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
                return response;

            }
            catch (Exception ex)
            {

            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }



        public class RewtunrDataOfSearch
        {
            public Int32 TripId;
            public double DriverLat;
            public double driverLong;
            public double Rate;
            public int Cost;
            public string Photo;

        }

        public enum enumGender { Male, Female, Genderless };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage RegisterUserProfile(string Face_identifier_2, enumRoles Role, string Nombre_usuario, string FirstName, string LastName, string FirebaseToken)
        {
            //ToDo: aqui faltan atributos al tipo del usuario, por favor, actualiza la db

            var uri = "https://graph.facebook.com/" + Face_identifier_2 + "/picture?type=large";

            //System.Net.WebRequest request = System.Net.WebRequest.Create(uri);  //  System.Net.WebRequest.CreateDefault();
            //System.Net.WebResponse response = request.GetResponse();
            //System.IO.Stream responseStream = response.GetResponseStream();
            //var arra = ReadFully(responseStream);

            //WebResponse myResp = response.GetResponse();
            var Validation = db.Traveler_Perfil.Where(x => x.Facebook_profile_id.Contains( Face_identifier_2)).FirstOrDefault();
            if (Validation == null)
            {
                var UserType = new Traveler_Perfil();
                UserType.Correo = "";
                UserType.Edad = "";
                UserType.Facebook_profile_id = Face_identifier_2;
                UserType.Fecha = DateTime.UtcNow;
                UserType.Latitud = "";
                UserType.Latitud_un_decimal = "";
                UserType.Longitud = "";
                UserType.Longitud_un_decimal = "";
                UserType.Nombre_usuario = Nombre_usuario;
                UserType.Numero_de_telefono = "";
                UserType.pidiendo_viaje = (int)(enumEstado_del_usuario.Sin_actividad);
                UserType.Resenas = "";
                UserType.Rol = (int)(Role);
                UserType.Stripe_id = "";
                UserType.Telefono_verificado = false;
                UserType.Viaje_asociado = "";
                UserType.TarjetaValidada = false;
                UserType.IdentidadComprobada = false;
                UserType.Telefono = "";
                UserType.Rating_double = 4.0;
                UserType.Gender = (int)enumGender.Male;
                UserType.hasConfirmedGender = false;
                UserType.FirstName = FirstName;
                UserType.LastName = LastName;
                UserType.FirebaseID = FirebaseToken;

                db.Traveler_Perfil.Add(UserType);
                db.SaveChanges();
            }
            else
            {
                Validation.Facebook_profile_id = Face_identifier_2;
                Validation.Fecha = DateTime.UtcNow;

                Validation.Nombre_usuario = Nombre_usuario;
                Validation.Rol = (int)(Role);
                Validation.FirstName = FirstName;
                Validation.LastName = LastName;
                Validation.FirebaseID = FirebaseToken;


                //var UserType = new Traveler_Perfil();
                //Validation.Correo = "";
                //Validation.Edad = "";
                //Validation.Latitud = "";
                //Validation.Latitud_un_decimal = "";
                //Validation.Longitud = "";
                //Validation.Longitud_un_decimal = "";
                //Validation.Numero_de_telefono = "";
                //Validation.pidiendo_viaje = (int)(enumEstado_del_usuario.Sin_actividad);
                //Validation.Resenas = "";
                //Validation.Rol = (int)(Role);
                //Validation.Stripe_id = "";
                //Validation.Telefono_verificado = false;
                //Validation.Viaje_asociado = "";
                Validation.TarjetaValidada = false;
                Validation.IdentidadComprobada = false;
                //Validation.Telefono = "";

                // db.Traveler_Perfil.Add(UserType);
                db.SaveChanges();
            }
            //StreamReader reader = new StreamReader(response.GetResponseStream());
            //var aa = reader.ReadToEnd();

            //var cadena =Convert.ToBase64String(GetStreamWithAuthAsync(Face_identifier_2));

            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");


        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage RequestSpaceInTrip(string Face_Identifier_3, int TripID, enumTipoDePAgoPreferido TipoDePAgo, int Amount)
        {

            var TripRequest = db.Traveler_SolicitudDeViajeTemporal.Where(x => x.Face_id_solicitante == Face_Identifier_3 && x.Id_del_viaje == TripID).FirstOrDefault();
            var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_Identifier_3).FirstOrDefault();
            if (User.StripeClientID == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Aceptado");
            }
            if (TripRequest == null)
            {
                var Trip = db.Traveler_Viajes.Where(x => x.ID == TripID).FirstOrDefault();

                string Charge = "";
                string PaymentIntentPointer = "";
                bool charged = ChargeUserForService((int)(Amount * 100), Face_Identifier_3, Trip.FaceIdDelConductor, ref Charge, ref PaymentIntentPointer);

                var New_trip_request = new Traveler_SolicitudDeViajeTemporal();
                New_trip_request.Face_id_solicitante = Face_Identifier_3;
                New_trip_request.Id_del_viaje = TripID;
                New_trip_request.Estado_De_solicitud = (int)(enumEstado_de_Solicitud.EnEspera);
                New_trip_request.TipoDePago = (int)TipoDePAgo;
                New_trip_request.MoneyOwnedToDriver = Amount.ToString();

                // Rrequest.MoneyOwnedTotutor = Debt;
                New_trip_request.PaymentIntentID = PaymentIntentPointer;
                New_trip_request.StripeCharge = Charge;


                // CaptureFunds(PaymentIntentPointer);

                if (charged)
                {
                    db.Traveler_SolicitudDeViajeTemporal.Add(New_trip_request);
                    db.SaveChanges();


                    //Envia Notificacion
                    var Viaje = db.Traveler_Viajes.Where(x => x.ID == TripID).FirstOrDefault();
                    var IdConductor = Viaje.FaceIdDelConductor;
                    var Conductor = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == IdConductor).FirstOrDefault();
                    Push("Abre la app para ver quien quiere reservar un viaje", "Tienes una solicitud!", Conductor.FirebaseID);

                }
            }
            else
            {

            }
           
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }




        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[input.Length + 1];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        class Viaje_Propiedades
        {
            public bool Bandera_Cerca_inicio;
            public bool Bandera_Medio_inicio;
            public bool Bandera_Lejos_inicio;
            public bool Bandera_Cerca_Final;
            public bool Bandera_Medio_Final;
            public bool Bandera_Lejos_Final;
            public double Distance_To_Goal;
            public double Distance_From_start;
            public Traveler_Viajes Viaje_Data;
            public Traveler_Perfil Client;
            public Traveler_SolicitudDeViajeTemporal request;

            /// <summary>
            /// //////////////////
            /// </summary>
            /// <param name="Viaje"></param>
            /// <param name="arg1"></param>
            /// <param name="arg2"></param>
            /// <param name="arg3"></param>
            /// <param name="arg4"></param>
            /// <param name="arg5"></param>
            /// <param name="arg6"></param>
            /// 
            //public string Inicio;
            //public string Destino;
            //public string Kilometros;
            //public string Tiempo_estimado;
            //public string Vehiculo;
            //public DateTime Fecha_y_hora;
            //public string asientos_disponibles;
            //public bool Disponible;
            //public bool En_curso;
            //public string Latitud;
            //public string Longitud;
            //public int? Estado_del_viaje;
            //public string FaceIdDelConductor;
            //public string Latitud_destino;
            //public string Longitud_destino;
            //public string TimeData;
            //public string Costo_po_Usuario;
            //public string Viaje_Periodico;
            //public string Asientos_Totales;
            //public bool Conductor_a_asociado_su_tarjeta;
            public Viaje_Propiedades(Traveler_Viajes Viaje, bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, double DistGoal, double distStart, Traveler_Perfil Cliente, Traveler_SolicitudDeViajeTemporal Solicitud)
            {
                Bandera_Cerca_inicio = arg1;
                Bandera_Medio_inicio = arg2;
                Bandera_Lejos_inicio = arg3;
                Bandera_Cerca_Final = arg4;
                Bandera_Medio_Final = arg5;
                Bandera_Lejos_Final = arg6;
                Viaje_Data = Viaje;
                Distance_From_start = distStart;
                Distance_To_Goal = DistGoal;
                Client = Cliente;
                request = Solicitud;
                //Inicio = Viaje.Inicio;
                //Destino = Viaje.Destino;
                //Kilometros = Viaje.Kilometros;
                //Tiempo_estimado = Viaje.Tiempo_estimado;
                //Vehiculo = Viaje.Vehiculo;
                //Fecha_y_hora = Viaje.Fecha_y_hora;
                //asientos_disponibles = Viaje.asientos_disponibles;
                //Disponible = Viaje.Disponible;
                //En_curso = Viaje.En_curso;
                //Latitud = Viaje.Latitud;
                //Longitud = Viaje.Longitud;
                //Estado_del_viaje = Viaje.Estado_del_viaje;
                //FaceIdDelConductor = Viaje.FaceIdDelConductor;
                //Latitud_destino = Viaje.Latitud_destino;
                //Longitud_destino = Viaje.Longitud_destino;
                //TimeData = Viaje.TimeData;
                //Costo_po_Usuario = Viaje.Costo_po_Usuario;
                //Viaje_Periodico = Viaje.Viaje_Periodico;
                //Asientos_Totales = Viaje.Asientos_Totales;
                //Conductor_a_asociado_su_tarjeta = Viaje.Conductor_a_asociado_su_tarjeta;
            }
        }
        class Listas_de_viajes
        {

            public List<Viaje_Propiedades> TripList;

            public Listas_de_viajes(List<Viaje_Propiedades> arg1)
            {
                TripList = arg1;

            }
        }
        public static Double Dist_LatLng(double Latitud_salida, double Longitud_salida, double Latitud_Llegada, double Longitud_Llegada)
        {
            var R = 6371e3; // metres
            var Omega_0 = ConvertToRadians(Latitud_salida);
            var Omega_1 = ConvertToRadians(Latitud_Llegada);
            var Delta_Omega = ConvertToRadians(Latitud_Llegada - Latitud_salida);
            var Delta_Alpha = ConvertToRadians(Longitud_Llegada - Longitud_salida);

            var a = Math.Sin(Delta_Omega / 2) * Math.Sin(Delta_Omega / 2) +
                    Math.Cos(Omega_0) * Math.Cos(Omega_1) *
                    Math.Sin(Delta_Alpha / 2) * Math.Sin(Delta_Alpha / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var d = R * c;
            return d;
        }
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        public static double PI = 3.1416;
        static TwoListObject ListOfCirclePoints(double center_Lat, double cemterLong, double radio_)
        {
            var Nueva_Lista = new TwoListObject();
            var NexY = new List<double>();
            var NexX = new List<double>();

            foreach (var Pi_div in new double[] { 0.0, PI * 1 / 3, PI * 2 / 3, PI * 3 / 3, PI * 4 / 3, PI * 5 / 3, PI * 2 })
            {
                NexY.Add((radio_ * Math.Sin(Pi_div)) + cemterLong);
                NexX.Add((radio_ * Math.Cos(Pi_div)) + center_Lat);
            }

            Nueva_Lista.X_Points = NexX;
            Nueva_Lista.Y_Points = NexY;

            return Nueva_Lista;
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

        public static string Base64Decode(string base64EncodedData)
        {
            int mod4 = base64EncodedData.Length % 4;
            //if (mod4 > 0)
            //{
            //    base64EncodedData += new string('=', 4 - mod4);
            //}
            //base64EncodedData += "==";
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            var output = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return output;
        }


    }

    public class TwoListObject
    {
        public List<double> X_Points;
        public List<double> Y_Points;
    }
    class Obj_publicacion
    {

        public string Lugar_Salida_texto;
        public string Lugar_Salida_Lat;
        public string Lugar_Salida_Long;

        public string Lugar_llegada_texto;
        public string Lugar_llegada_Lat;
        public string Lugar_llegada_Long;

        public string fecha;
        public string Dia;
        public string Mes;
        public string Ano;
        public string Periodico;
        public string plazas;
        public string time;
        public string Kilometros;

        public string Duracion;
        public bool ViajePeriodico;
        public int TipoDePago;

    }
    class RequestType
    {
        public int ID_de_request;
        public string Face_id_solicitante;
        public int Estado_De_solicitud;
        public int Id_del_viaje;
        public string Foto;
        public string Nombre;
        public bool IdentidadValidada;

        public RequestType(string FaceRef, int SolicitudStateref, int TripIdRef, string Photo, string Name, int Id_ref, bool ValIdentity)
        {

            Face_id_solicitante = FaceRef;
            Estado_De_solicitud = SolicitudStateref;
            Id_del_viaje = TripIdRef;
            Foto = Photo;
            Nombre = Name;
            ID_de_request = Id_ref;
            IdentidadValidada = ValIdentity;
        }
    }
    class PublicationDriverObj
    {
        public Traveler_Viajes Trip;
        public List<RequestType> Request;
        public bool IdentidadValidada;//IdentityFlag
        public PublicationDriverObj(Traveler_Viajes TripRef, List<RequestType> Req, bool Identity)
        {
            Trip = TripRef;
            Request = Req;
            IdentidadValidada = Identity;
        }
    }

    class estadoRespuetaPasajero
    {
        public int ViajesDisponibles;
        public string nombreconductor;
        public string Destino;
        public double distancia;
        public string Latitud;
        public string Longitud;
        public string Modelo_de_auto;
        public string Color_auto;
        public int Status_para_pasajero;

    }
}
