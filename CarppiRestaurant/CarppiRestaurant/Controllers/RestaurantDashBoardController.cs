using CarppiRestaurant.Clases;
using CarppiRestaurant.Models;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CarppiRestaurant.Controllers
{
    public class RestaurantDashBoardController : Controller
    {
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected, RequestDeliverManInRestaurantSpot };

        public enum IndexOfConnectedAccount { Restaurant, DeliveryMan };

        public enum enumTipoDePago { Efectivo, Tarjeta };

        public enum RoleInCoversation { Sender, Receiver };
        public enum AvailableFoodListing
        {
            Hamburgesas = 1,
            Guajolotes = 2,
            Postres = 4,
            Pollo = 8,
            Indio = 16,
            Americano = 32,
            Pizza = 64,
            Saludable = 128,
            Vegetariano = 256,
            Chino = 512,
            Continental = 1024,
            Pastes = 2048,
            Tacos = 4096,
            Antojitos = 8192,
            ComidaCorrida = 16384,
            SushiYJapones = 32768,
            BebidasSinAlcohol = 65536,
            BebidasConAlcohol = 131072,
            Desayuno = 262144,
            Chilaquiles = 524288,
            Papas = 1048576
        };

        PidgeonEntities db = new PidgeonEntities();
        // GET: RestaurantDashBoard
        public ActionResult Index()
        {
            //https://dashboard.stripe.com/express/oauth/authorize?redirect_uri=https://carppirestaurant.azurewebsites.net&response_type=code&client_id=ca_HefGcaqsN1stYmIBiakDDfvPcpEIfEk6&scope=read_write#/
            //http://metropolitanhost.com/themes/themeforest/angular/costic/order
            Session["RestaurantID"] = "4501fa592738def70c450dcd5320e613bd6811bff9cef49eeb872f5da9c2d13c";
            return View();
        }
        [HttpPost]
        public JsonResult DownloadProductDetails(Int64 ProductID)
        {
            var P = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == ProductID).FirstOrDefault();
            return Json(new { StatusCode = "Accepted", Response = P });
        }

            [HttpPost]
        public JsonResult PostMessageInRestauranDeliver(string DeliverRequest, String Message)
        {
            var FaceID_speaker = Session["RestaurantID"].ToString();
            var REquest_OfTrip = db.CarppiRestaurant_BuyOrders.Where(x => x.RestaurantHash == FaceID_speaker && x.FaceIDRepartidor_RepartidorCadena == DeliverRequest).FirstOrDefault();
            if (REquest_OfTrip == null)
            {
                var NewesID = Convert.ToInt32(DeliverRequest);
                REquest_OfTrip = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceID_speaker && x.ID == NewesID).FirstOrDefault();
            }

            if (REquest_OfTrip != null)
            {
                //                --CREATE TABLE Carppi_MensajesRideshare(
                //--ID bigint primary key IDENTITY(1,1) NOT NULL,
                //--Mensaje varchar(max),
                //--FaceID_Sender varchar(450),
                //--RequestID bigint,
                //--DateInSeconds bigint,
                //--Entregado bit not null default 0,
                //--Leido bit not null default 0);
                var NewMessage = new CarppiRestaurant_MensajesRepartidorRestaurante();
                NewMessage.Mensaje = Message;
                NewMessage.FaceID_Sender = FaceID_speaker;
                NewMessage.BuyOrderID = REquest_OfTrip.ID;
                NewMessage.DateInSeconds = (long)(((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
                db.CarppiRestaurant_MensajesRepartidorRestaurante.Add(NewMessage);
                db.SaveChanges();

                //var Driver = db.CarppiRequestForDrives.Where(x => x.FaceIDDriver == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();
                //var Passenger = db.CarppiRequestForDrives.Where(x => x.FaceIDPassenger == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();

                //var Token = "";
                //Token = Driver == null ? "" : db.Traveler_Perfil.Where(x=> x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                //Token = Passenger == null ? Token : db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                if (FaceID_speaker == REquest_OfTrip.RestaurantHash)
                {
                    //var intermitentID= 

                    var repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor.Contains(REquest_OfTrip.FaceIDRepartidor_RepartidorCadena)).FirstOrDefault();
                    Push_Repartidor(Message, "Mensaje restaurante, orden " + REquest_OfTrip.ID.ToString(), repartidor.FirebaseID, "");

                }
                else
                {
                    // var Token = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == REquest_OfTrip.UserID).FirstOrDefault().FirebaseID;
                    var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash.Contains(REquest_OfTrip.RestaurantHash)).FirstOrDefault();
                    // Push_re(Message, "Mensaje del cliente" + REquest_OfTrip.ID.ToString(), repartidor.FirebaseID, "");
                    Push_Restaurante(Message, "Mensaje repartidor, orden" + REquest_OfTrip.ID.ToString(), Restaurante.FirebaseID, "");

                    //Push(Message, "Mensaje Del Repartidor", Token, "");
                    //Push(Message, "Mensaje Del cliente", Token, "");
                }


            }
            return Json(new { StatusCode = "Accepted", Response = "" });
            //return Request.CreateResponse(HttpStatusCode.Accepted, "");

        }

        [HttpPost]
        public JsonResult GetAlMessagesFromConversationRestauranDeliver(string DeliverRequest)
        {
            var FaceID_Interest = Session["RestaurantID"].ToString();
            var REquest_OfTrip = db.CarppiRestaurant_BuyOrders.Where(x => x.RestaurantHash == FaceID_Interest && x.FaceIDRepartidor_RepartidorCadena == DeliverRequest).FirstOrDefault();
            if (REquest_OfTrip == null)
            {
                var NewesID = Convert.ToInt32(DeliverRequest);
                REquest_OfTrip = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceID_Interest && x.ID == NewesID).FirstOrDefault();

            }
            var Messages = db.CarppiRestaurant_MensajesRepartidorRestaurante.Where(x => x.BuyOrderID == REquest_OfTrip.ID);
            List<MesssageValue> messsageDatas = new List<MesssageValue>();
            foreach (var aca in Messages)
            {
                var OldMessage = new MesssageValue();
                OldMessage.MessageContext = aca;
                OldMessage.RoleInConversation = aca.FaceID_Sender == FaceID_Interest ? RoleInCoversation.Sender : RoleInCoversation.Receiver;
                messsageDatas.Add(OldMessage);
            }
            return Json(new { StatusCode = "Accepted", Response = messsageDatas });
            //return Request.CreateResponse(HttpStatusCode.Accepted, messsageDatas);

        }


        [HttpPost]
        public JsonResult SetOrderStatus(GroceryOrderState Estado, Int32 OrderID)
        {
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.ID == OrderID).FirstOrDefault();
            var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Order.UserID).FirstOrDefault();

            if (Estado == GroceryOrderState.RequestAccepted)
            {
                if (Order.TipoDePago == (int)enumTipoDePago.Efectivo)
                {
                    Push("Tu orden fue aceptada, abre la app para conocer mas detalles", "Tu orden ha sido aceptada", cliente.FirebaseID, "");
                    Task.Delay(10000).ContinueWith(t => ChangeBuyOrderstateToAttended(Order));
                }
                else
                {
                    if (CaptureFunds(Order.paymentIntent, cliente.Facebook_profile_id))
                    {
                        Push("Tu orden fue aceptada, abre la app para conocer mas detalles", "Tu orden ha sido aceptada", cliente.FirebaseID, "");
                        try
                        {
                            var Restauranr = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == Order.RestaurantHash).FirstOrDefault();
                            Restauranr.DebtToRestaurant += (long)Convert.ToDouble(Order.Precio);
                        }
                        catch (Exception)
                        {

                        }
                        Task.Delay(10000).ContinueWith(t => ChangeBuyOrderstateToAttended(Order));
                    }
                    else
                    {
                        return Json(new { StatusCode = "InternalServerError", Response = "No se pudieron capturar los fondos del cliente" });
                        //   return Request.CreateResponse(HttpStatusCode.InternalServerError, "");
                    }
                }
            }
            else if (Estado == GroceryOrderState.RequestGoingToClient)
            {
                if (Order.FaceIDRepartidor_RepartidorCadena != null)
                {
                    var Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == Order.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                    var IsDriverInRegion = IsInThisRegion(Convert.ToDouble(Order.LatitudRestaurante), (Convert.ToDouble(Order.LongitudRestaurante)), (Convert.ToDouble(Repartidor.Latitud)), (Convert.ToDouble(Repartidor.Longitud)));
                    if (IsDriverInRegion)
                    {
                        Push("Tu repartidor esta llendo a tu ubicación actual, abre la app para conocer mas detalles", "El repartidor corre hacia ti", cliente.FirebaseID, "");
                    }
                    else
                    {
                        return Json(new { StatusCode = "Forbidden", Response = "El Repartidor no esta cerca" });
                       // return Request.CreateResponse(HttpStatusCode.Forbidden, "");
                    }
                }
                else
                {
                    return Json(new { StatusCode = "Conflict", Response = "No Puedes finalizar la orden si no has asignado un repartidor" });
                    // return Request.CreateResponse(HttpStatusCode.Conflict, "");
                }


                //Task.Delay(15000).ContinueWith(t => ChangeBuyOrderstateToAttended(Order));
            }

            else if (Estado == GroceryOrderState.RequestEnded)
            {

                var Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == Order.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                var IsDriverInRegion = IsInThisRegion(Convert.ToDouble(Order.LatitudPeticion), (Convert.ToDouble(Order.LongitudPeticion)), (Convert.ToDouble(Repartidor.Latitud)), (Convert.ToDouble(Repartidor.Longitud)));
                if (IsDriverInRegion)
                {

                    Push("Tu orden fue entregada, califica al repartidor", "Tu orden fue entregada", cliente.FirebaseID, "");
                    Order.Stat = (int)Estado;
                    db.SaveChanges();
                }

                /*
                var mensajesEnConvo = db.CarppiGrocery_Mensajes.Where(x => x.BuyOrderID == Order.ID);
                foreach (var mensaje in mensajesEnConvo)
                {
                    db.CarppiGrocery_Mensajes.Remove(mensaje);
                }
                */

                //Task.Delay(24000).ContinueWith(t => EraseBuyOrder(Order));

            }
            else if (Estado == GroceryOrderState.RequestRejected)
            {

                if (Order.TipoDePago == (int)enumTipoDePago.Tarjeta)
                {
                    RefundOnCancelSolicitud(Order);
                }

                Push("Prueba intentando mas tarde", "Tu orden fue rechazada", cliente.FirebaseID, "");
                Order.Stat = (int)Estado;
                db.SaveChanges();
                Task.Delay(4000).ContinueWith(t => EraseBuyOrder(Order));

            }
            Order.Stat = (int)Estado;
            db.SaveChanges();
            return Json(new { StatusCode = "Accepted", Response = "Cambio Realizado" });
            //return Request.CreateResponse(HttpStatusCode.Accepted, "");

        }

        [HttpPost]
        public JsonResult RequestDriver(long NewOrderID)
        {
            var restaurantHashRequestingDriver = Session["RestaurantID"].ToString();
            var restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == restaurantHashRequestingDriver).FirstOrDefault();
            var Busqueda = new BusquedaRepartidor(Convert.ToDouble(restaurante.Latitud), Convert.ToDouble(restaurante.Longitud), Convert.ToInt64(restaurante.Region));
            CarppiGrocery_Repartidores myrepartidor = Busqueda.SearchForNearestDeliveryBoy();
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.ID == NewOrderID).FirstOrDefault();
            if (Order.FaceIDRepartidor_RepartidorCadena != null)
            {
                return Json(new { StatusCode = "NotAcceptable", Response = "Repartidor ya asignado" });
                //return Request.CreateResponse(HttpStatusCode.NotAcceptable, "0repartidores");
            }

            //SearchForNearestDeliveryBoy
            if (myrepartidor == null)
            {
                return Json(new { StatusCode = "Gone", Response = "Sin repartidores disponibles, intente mas tarde" });
                //return Request.CreateResponse(HttpStatusCode.Gone, "0repartidores");
            }
            else if (Busqueda.DriverHasToomuchOrders == true)
            {
                return Json(new { StatusCode = "Moved", Response = "El repartidor mas cercano tiene muchas ordenes, intente en unos minutos" });
                //return Request.CreateResponse(HttpStatusCode.Moved, "0repartidores");
            }
            else
            {

                Order.FaceIDRepartidor_RepartidorCadena = myrepartidor.FaceID_Repartidor;
                Order.LatitudRepartidor = myrepartidor.Latitud;
                Order.LongitudRepartidor = myrepartidor.Longitud;
                Order.LatitudRestaurante = restaurante.Latitud;
                Order.LongitudRestaurante = restaurante.Longitud;



                try
                {
                    var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Order.UserID).FirstOrDefault();
                    var OnjetoRepartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == Order.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                    var fee = 0.0;
                    cliente.IsFirstOrder = false;
                    fee = Busqueda.OperationCost(Convert.ToDouble(Order.LatitudPeticion), Convert.ToDouble(Order.LongitudPeticion));
                    Order.TarifaDelServicio = (fee + 0.0);
                    if (Order.TipoDePago == (int)enumTipoDePago.Tarjeta)
                    {
                        OnjetoRepartidor.DebtToDeliverMan += fee > 5 ? (int)fee : 15;
                    }
                    db.SaveChanges();
                    /*
                    if (cliente.IsFirstOrder == true && cliente.ID <= 150)
                    {
                        cliente.IsFirstOrder = false;
                        Order.TarifaDelServicio = 0;
                         fee = Busqueda.OperationCost(Convert.ToDouble(Order.LatitudPeticion), Convert.ToDouble(Order.LongitudPeticion));
                       
                       
                        OnjetoRepartidor.DebtToDeliverMan += fee > 5 ?(int)fee : 15;
                        db.SaveChanges();
                    }
                    else
                    {
                         fee = Busqueda.OperationCost(Convert.ToDouble(Order.LatitudPeticion), Convert.ToDouble(Order.LongitudPeticion));
                        Order.TarifaDelServicio = (fee + 0.0);
                    }
                    */
                    OnjetoRepartidor.MonetMadeByDeliverMan += fee;

                }
                catch (Exception)
                { }
                db.SaveChanges();
                Push_Repartidor("Tienes una nueva orden", "Nueva Orden", myrepartidor.FirebaseID, "");
                return Json(new { StatusCode = "OK", Response = "Repartidor Asignado" });
                //return Request.CreateResponse(HttpStatusCode.OK, "0repartidores");

            }


        }

        [HttpPost]
        public JsonResult RestaurantDeliveryManAssigned()
        {
            //var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.RestaurantHash == RestaurantHashDeliveryMan);
            var RestaurantHashDeliveryMan = Session["RestaurantID"].ToString();
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.RestaurantHash == RestaurantHashDeliveryMan && x.Stat == ((int)GroceryOrderState.RequestCreated) || x.Stat == (int)GroceryOrderState.RequestBeingAttended || x.Stat == (int)GroceryOrderState.RequestAccepted || x.Stat == ((int)GroceryOrderState.RequestDeliverManInRestaurantSpot));
            List<CarppiGrocery_Repartidores> ListaRepartidores = new List<CarppiGrocery_Repartidores>();
            foreach (var ordenes in Order)
            {
                var Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == ordenes.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                ListaRepartidores.Add(Repartidor);
            }
            if (ListaRepartidores.Count() != 0)
            {
                var ListaUnnica = ListaRepartidores.Distinct();
                List<RepartidorYOrdenes> ListaDeOrdener = new List<RepartidorYOrdenes>();
                foreach (var rep in ListaUnnica)
                {
                    RepartidorYOrdenes repartidorYOrdenes = new RepartidorYOrdenes();
                    List<long> IDTemporales = new List<long>();
                    repartidorYOrdenes.Repartidor = rep;

                    var OrdenNueva = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == rep.FaceID_Repartidor);
                    foreach (var ord in OrdenNueva)
                    {
                        IDTemporales.Add(ord.ID);


                    }
                    repartidorYOrdenes.ListaDeOrdenes = IDTemporales;


                    ListaDeOrdener.Add(repartidorYOrdenes);
                }
                return Json(new { StatusCode = "OK", Response = ListaDeOrdener });
                //return Request.CreateResponse(HttpStatusCode.OK, ListaDeOrdener);
            }
            else
            {
                return Json(new { StatusCode = "Moved", Response = "" });
                //return Request.CreateResponse(HttpStatusCode.Moved, "sinrepartidores");
            }

        }
        /*
        public class ItemsPorRepartidor 
        { 
            public long IdDeOrden
        }
        */
        public class RepartidorYOrdenes
        {
            public CarppiGrocery_Repartidores Repartidor;
            public List<long> ListaDeOrdenes;
        }
        public enum ItemAvailability { SetUnavailable, SetAvailable };



        [HttpPost]
        public JsonResult AddProduct(int ProductCategory, double CostTotal, string nombre, string descriptcion)
        {

            //      var User = db.TutoriUsuarios.Where(x => x.FaceID == FaceBookIDTutoriToAddHomework).FirstOrDefault();
            try
            {
                var ServiceProviderToTransferHash = Session["RestaurantID"].ToString();
                var NewHome = new Carppi_ProductosPorRestaurantes();
/*
                var httpRequest = HttpContext.Current.Request;
                var fileCollection = httpRequest.Files;
                var keycollection = httpRequest.Files.AllKeys;
*/
                try
                {
                    var Photo = Request.Files[0];
                    NewHome.Foto = ConvertToByteArray_InputStream(Photo);//ToByteArray(Photo);
                    /*
                    var httpRequest = HttpContext.Current.Request;
                    var fileCollection = httpRequest.Files;
                    var keycollection = httpRequest.Files.AllKeys;


                    var MMfile = fileCollection[keycollection.FirstOrDefault()];

                    NewHome.Foto = ToByteArray(MMfile.InputStream);
                    */
                }
                catch (Exception)
                {

                }


                //var MMfile = fileCollection[keycollection.FirstOrDefault()];
               // var NewHome = new Carppi_ProductosPorRestaurantes();
              //  NewHome.Foto = ToByteArray(MMfile.InputStream);
                NewHome.Categoria = ReturnRealBit(ProductCategory);
                NewHome.Costo = CostTotal;
                NewHome.Descripcion = descriptcion;
                NewHome.Nombre = nombre;
                NewHome.Disponibilidad = true;
                NewHome.IDdRestaurante = ServiceProviderToTransferHash;
                db.Carppi_ProductosPorRestaurantes.Add(NewHome);




                db.SaveChanges();

                return Json(new { StatusCode = "Created", Response = "" });
                //  return Request.CreateResponse(HttpStatusCode.Created, NewHome.ID);
            }
            catch (Exception)
            {

            }


            return Json(new { StatusCode = "BadRequest", Response = "" });
            //  return Request.CreateResponse(HttpStatusCode.BadRequest, "");

        }


        public int ReturnRealBit(int Regularbit)
        {
            switch (Regularbit)
            {
                case 1:
                    return (Int32)AvailableFoodListing.Hamburgesas;
                case 2:
                    return (Int32)AvailableFoodListing.Guajolotes;
                case 3:
                    return (Int32)AvailableFoodListing.Postres;
                case 4:
                    return (Int32)AvailableFoodListing.Pollo;
                case 5:
                    return (Int32)AvailableFoodListing.Indio;
                case 6:
                    return (Int32)AvailableFoodListing.Americano;
                case 7:
                    return (Int32)AvailableFoodListing.Pizza;
                case 8:
                    return (Int32)AvailableFoodListing.Saludable;
                case 9:
                    return (Int32)AvailableFoodListing.Vegetariano;
                case 10:
                    return (Int32)AvailableFoodListing.Chino;
                case 11:
                    return (Int32)AvailableFoodListing.Continental;
                case 12:
                    return (Int32)AvailableFoodListing.Pastes;
                case 13:
                    return (Int32)AvailableFoodListing.Tacos;
                case 14:
                    return (Int32)AvailableFoodListing.Antojitos;
                case 15:
                    return (Int32)AvailableFoodListing.ComidaCorrida;
                case 16:
                    return (Int32)AvailableFoodListing.SushiYJapones;
                case 17:
                    return (Int32)AvailableFoodListing.BebidasSinAlcohol;
                case 18:
                    return (Int32)AvailableFoodListing.BebidasConAlcohol;

                case 19:
                    return (Int32)AvailableFoodListing.Desayuno;
                case 20:
                    return (Int32)AvailableFoodListing.Chilaquiles;
                case 21:
                    return (Int32)AvailableFoodListing.Papas;
                default:
                    return 0;


            }
        }
        public byte[] ToByteArray(Stream stream)
        {
            try
            {
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                    totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
                return buffer;
            }
            catch (Exception)
            {
                return new byte[] { };
            }
        }

        [HttpPost]
        public JsonResult TransferToAcoount()
        {
            try
            {
                var ServiceProviderToTransferHash = Session["RestaurantID"].ToString();
                var TypeOfAcoount = IndexOfConnectedAccount.Restaurant;
                if (TypeOfAcoount == IndexOfConnectedAccount.Restaurant)
                {
                    var Reataurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == ServiceProviderToTransferHash).FirstOrDefault();
                    if (Reataurante.StripeHash != null)
                    {
                        if (Reataurante.DebtToRestaurant > 15)
                        {
                            //CarppiDeliveryHash
                            try
                            {
                                StripeConfiguration.ApiKey = "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";
                                var Fecha = DateTime.UtcNow.ToString();
                                
                                var debt = new DebtReturnType();

                                debt.RawDebt = Reataurante.DebtToRestaurant;
                                debt.CarppiComision = Reataurante.DebtToRestaurant * 0.05;
                                debt.StripeComision = (Reataurante.DebtToRestaurant * 0.025) + 12;
                                debt.Total = Reataurante.DebtToRestaurant - (Reataurante.DebtToRestaurant * 0.05) - ((Reataurante.DebtToRestaurant * 0.025) + 12);

                                var amount = (long)(debt.Total * 100);//(long)(((Reataurante.DebtToRestaurant) - (Reataurante.DebtToRestaurant * 0.05)) * 100);
                                var options = new TransferCreateOptions
                                {
                                    Amount = amount,
                                    Currency = "mxn",
                                    Destination = Reataurante.StripeHash,

                                    Description = "Carppi restaurant Transfer Date " + Fecha,
                                };
                                var service = new TransferService();
                                var tresponse = service.Create(options);

                                SendReceipt(debt, Reataurante.Correo, "Tu recibo " + Reataurante.Nombre, tresponse.Id);
                                Reataurante.DebtToRestaurant = 0;
                                db.SaveChanges();
                                return Json(new { StatusCode = "OK", Response = "" });
                                // return new HttpResponseMessage(HttpStatusCode.OK);
                            }
                            catch (Exception ex)
                            {
                                return Json(new { StatusCode = "InternalServerError", Response = "" });
                                // return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            }

                        }
                    }
                    else
                    {
                        return Json(new { StatusCode = "NotFound", Response = "" });
                        // return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                }
                else if (TypeOfAcoount == IndexOfConnectedAccount.DeliveryMan)
                {

                    var Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == ServiceProviderToTransferHash).FirstOrDefault();
                    if (Repartidor.StripeHash != null)
                    {
                        if (Repartidor.DebtToDeliverMan > 15)
                        {
                            //CarppiDeliveryHash
                            try
                            {
                                StripeConfiguration.ApiKey = "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";
                                var Fecha = DateTime.UtcNow.ToString();
                                var amount = Repartidor.DebtToDeliverMan * 100;
                                var options = new TransferCreateOptions
                                {
                                    Amount = amount,
                                    Currency = "mxn",
                                    Destination = Repartidor.StripeHash,

                                    Description = "Carppi restaurant Transfer Date " + Fecha,
                                };
                                var service = new TransferService();
                                var tresponse = service.Create(options);

                                var debt = new DebtReturnType();

                                debt.RawDebt = Repartidor.DebtToDeliverMan;
                                debt.CarppiComision = Repartidor.DebtToDeliverMan * 0.00;
                                debt.StripeComision = ((Repartidor.DebtToDeliverMan * 0.025) + 12);
                                debt.Total = debt.RawDebt - (debt.RawDebt * 0.00) - ((debt.RawDebt * 0.025) + 12);
                                SendReceipt(debt, Repartidor.Email, "Tu recibo " + Repartidor.Nombre, tresponse.Id);
                                Repartidor.DebtToDeliverMan = 0;
                                db.SaveChanges();
                                return Json(new { StatusCode = "OK", Response = "" });
                                //return new HttpResponseMessage(HttpStatusCode.OK);
                            }
                            catch (Exception ex)
                            {
                                return Json(new { StatusCode = "InternalServerError", Response = "" });
                                // return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            }

                        }
                    }
                    else
                    {
                        return Json(new { StatusCode = "NotFound", Response = "" });
                        // return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                }



                db.SaveChanges();
                return Json(new { StatusCode = "Accepted", Response = "" });
                //return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = "InternalServerError", Response = "" });
                //return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            // var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == ServiceProviderHash).FirstOrDefault();
            // var User = db.TutoriUsuarios.Where(x => x.FaceID == Tutori_participantToUpdate).FirstOrDefault();
            //User.StripeDriverID = StripeComercianteID;


        }

        [HttpPost]
        public JsonResult GetDebtData()
        {
            try
            {
                var DebtorHash = Session["RestaurantID"].ToString();
                var DebtorType = IndexOfConnectedAccount.Restaurant;
                if (DebtorType == IndexOfConnectedAccount.Restaurant)
                {
                    var Reataurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == DebtorHash).FirstOrDefault();
                    var debt = new DebtReturnType();

                    debt.RawDebt = Reataurante.DebtToRestaurant;
                    debt.CarppiComision = Reataurante.DebtToRestaurant * 0.05;
                    debt.StripeComision = (Reataurante.DebtToRestaurant * 0.025) + 12;
                    debt.Total = Reataurante.DebtToRestaurant - (Reataurante.DebtToRestaurant * 0.05) - ((Reataurante.DebtToRestaurant * 0.025) + 12);

                    return Json(new { StatusCode = "OK", Response = debt });
                    // return Request.CreateResponse(HttpStatusCode.OK, debt);
                    // return new HttpResponseMessage(HttpStatusCode.OK, debt);
                }
                else if (DebtorType == IndexOfConnectedAccount.DeliveryMan)
                {
                    var Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == DebtorHash).FirstOrDefault();
                    var debt = new DebtReturnType();

                    debt.RawDebt = Repartidor.DebtToDeliverMan;
                    debt.CarppiComision = Repartidor.DebtToDeliverMan * 0;
                    debt.StripeComision = ((Repartidor.DebtToDeliverMan * 0.025) + 12);
                    debt.Total = debt.RawDebt - (debt.RawDebt * 0) - ((debt.RawDebt * 0.025) + 12);
                    //return Request.CreateResponse(HttpStatusCode.OK, debt);
                    return Json(new { StatusCode = "OK", Response = debt });
                }



                //                db.SaveChanges();
                return Json(new { StatusCode = "Accepted", Response = "" });
                //return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = "InternalServerError", Response = "" });
               // return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            // var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == ServiceProviderHash).FirstOrDefault();
            // var User = db.TutoriUsuarios.Where(x => x.FaceID == Tutori_participantToUpdate).FirstOrDefault();
            //User.StripeDriverID = StripeComercianteID;


        }
        public class DebtReturnType
        {
            public string RestaurantName;
            public string Mail;
            public long RawDebt;
            public double CarppiComision;
            public double StripeComision;
            public double Total;
        }
    


        [HttpPost]
        public JsonResult CarppiProductDetailedView_Compresed(string ProductDetailID_CompressedData)
        {
            var ID = Convert.ToInt32(ProductDetailID_CompressedData);
            var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == ID).FirstOrDefault();

            /*var nuevoProducto = new Carppi_ProductosPorRestaurantes();
            nuevoProducto = Producto;
            nuevoProducto.Foto = Compress(Producto.Foto);
            */
            return Json(new { StatusCode = "OK", Response = Producto });
            //return Request.CreateResponse(HttpStatusCode.Accepted, nuevoProducto);
        }

        [HttpPost]
        public JsonResult RestaurantToDownloadItsOwnMenuIDs()
        {
            var RestaurantHashDownloadMenu_IDList = Session["RestaurantID"].ToString();
            var Items = db.Carppi_ProductosPorRestaurantes.Where(x => x.IDdRestaurante == RestaurantHashDownloadMenu_IDList);
            var ListaDeIds = new List<long>();
            foreach (var aca in Items)
            {
                ListaDeIds.Add(aca.ID);
            }
            return Json(new { StatusCode = "OK", Response = ListaDeIds });
            // return Request.CreateResponse(HttpStatusCode.OK, ListaDeIds);
        }
        [HttpPost]
        public JsonResult GetAlMessagesFromConversationRestauranClient( Int64 shopRequest)
        {
            var RestaurantHash = Session["RestaurantID"].ToString();
            var Messages = db.CarppiREstaurant_MensajesClienteRestaurante.Where(x => x.BuyOrderID == shopRequest);
            List<MesssageData> messsageDatas = new List<MesssageData>();
            foreach (var aca in Messages)
            {
                var OldMessage = new MesssageData();
                OldMessage.MessageContext = aca;
                OldMessage.RoleInConversation = aca.FaceID_Sender == RestaurantHash ? RoleInCoversation.Sender : RoleInCoversation.Receiver;
                messsageDatas.Add(OldMessage);
            }
            return Json(new { StatusCode = "OK", Response = messsageDatas });
            //return Request.CreateResponse(HttpStatusCode.Accepted, messsageDatas);

        }
        [HttpPost]
        
        public JsonResult PostMessageInRestauranClient( Int64 shopRequest, String Message)
        {
            var FaceID_speaker = Session["RestaurantID"].ToString();
            var REquest_OfTrip = db.CarppiRestaurant_BuyOrders.Where(x => x.ID == shopRequest).FirstOrDefault();
            if (REquest_OfTrip != null)
            {
                //                --CREATE TABLE Carppi_MensajesRideshare(
                //--ID bigint primary key IDENTITY(1,1) NOT NULL,
                //--Mensaje varchar(max),
                //--FaceID_Sender varchar(450),
                //--RequestID bigint,
                //--DateInSeconds bigint,
                //--Entregado bit not null default 0,
                //--Leido bit not null default 0);
                var NewMessage = new CarppiREstaurant_MensajesClienteRestaurante();
                NewMessage.Mensaje = Message;
                NewMessage.FaceID_Sender = FaceID_speaker;
                NewMessage.BuyOrderID = shopRequest;
                NewMessage.DateInSeconds = (long)(((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
                db.CarppiREstaurant_MensajesClienteRestaurante.Add(NewMessage);
                db.SaveChanges();

                //var Driver = db.CarppiRequestForDrives.Where(x => x.FaceIDDriver == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();
                //var Passenger = db.CarppiRequestForDrives.Where(x => x.FaceIDPassenger == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();

                //var Token = "";
                //Token = Driver == null ? "" : db.Traveler_Perfil.Where(x=> x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                //Token = Passenger == null ? Token : db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                if (FaceID_speaker == REquest_OfTrip.UserID)
                {
                    //var intermitentID= 

                    var repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor.Contains(REquest_OfTrip.FaceIDRepartidor_RepartidorCadena)).FirstOrDefault();
                    var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash.Contains(REquest_OfTrip.RestaurantHash)).FirstOrDefault();
                    // Push_re(Message, "Mensaje del cliente" + REquest_OfTrip.ID.ToString(), repartidor.FirebaseID, "");
                    Push_Restaurante(Message, "Mensaje de la orden" + REquest_OfTrip.ID.ToString(), Restaurante.FirebaseID, "");

                }
                else
                {
                    var Token = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == REquest_OfTrip.UserID).FirstOrDefault().FirebaseID;
                    Push(Message, "Mensaje Del Restaurante", Token, "");
                    //Push(Message, "Mensaje Del cliente", Token, "");
                }


            }
            return Json(new { StatusCode = "OK", Response = "" });
            //  return Request.CreateResponse(HttpStatusCode.Accepted, "");

        }


        [HttpPost]
        public JsonResult PoolRestaurantOrders()
        {
            //https://localhost:44322/RestaurantDashBoard/Index
           

            try
            {
                var RestaurantHash = Session["RestaurantID"].ToString();
                var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.RestaurantHash == RestaurantHash && (x.Stat == ((int)GroceryOrderState.RequestCreated) || x.Stat == (int)GroceryOrderState.RequestBeingAttended || x.Stat == (int)GroceryOrderState.RequestAccepted || x.Stat == ((int)GroceryOrderState.RequestDeliverManInRestaurantSpot)));
                List<RestaurantReturn> Retorn = new List<RestaurantReturn>();
                foreach (var aca in Order)
                {
                    var miorden = new RestaurantReturn();
                    miorden.Orden = aca;
                    var cadenadeOrdenes = Base64Decode(aca.ListaDeProductos);
                    List<OrderData> datas = new List<OrderData>();
                    var TotalCost = 0.0;
                    if (cadenadeOrdenes.Contains("ItemID") && cadenadeOrdenes.Contains("Quantity"))
                    { 
                        var ListOfItems = JsonConvert.DeserializeObject<List<ShopItem>>(cadenadeOrdenes);
                    

                    foreach (var Item in ListOfItems)
                    {
                        var Produc = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == Item.ItemID).FirstOrDefault();
                        TotalCost += (Convert.ToDouble(Produc.Costo) * Item.Quantity);
                        var nuevaOrden = new OrderData();
                        nuevaOrden.Cantidad = Item.Quantity;
                        nuevaOrden.Producto = Produc.Nombre;
                        datas.Add(nuevaOrden);

                    }
                }
                    else
                    {
                        var ListOfItems = JsonConvert.DeserializeObject<List<ShopListItem>>(cadenadeOrdenes);
                        

                        foreach (var Item in ListOfItems)
                        {
                            var Produc = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == Item.ID).FirstOrDefault();
                            var tempproductCost = Convert.ToDouble(Produc.Costo);
                            var ExtraOptions = "";
                            if (Item.ObligatoryOptions != null || Item.PersonalOptions != null)
                            {
                                ExtraOptions = " Seleccion: ";
                            }
                            try
                            {
                               
                                foreach (var ExtraItem in Item.ObligatoryOptions)
                                {
                                    var Opcion = db.OptionalChoice.Where(x => x.ID == ExtraItem.ID).FirstOrDefault();
                                    // tempproductCost += Convert.ToDouble(Opcion.CostoExtra);
                                    ExtraOptions += " +" + Opcion.OptionHash;
                                }
                            }
                            catch (Exception) { }
                            try
                            {
                                foreach (var ExtraItem in Item.PersonalOptions)
                                {
                                    var Opcion = db.OptionalChoice.Where(x => x.ID == ExtraItem.ID).FirstOrDefault();
                                    ExtraOptions += " +" + Opcion.OptionHash;
                                    tempproductCost += Convert.ToDouble(Opcion.CostoExtra);
                                }
                            }
                            catch (Exception) { }
                            TotalCost += (tempproductCost * Item.Cantidad);
                            //TotalCost += (Convert.ToDouble(Produc.Costo) * Item.Cantidad);
                            var nuevaOrden = new OrderData();
                            nuevaOrden.Cantidad = Item.Cantidad;
                            nuevaOrden.Producto = Produc.Nombre + ExtraOptions;
                            datas.Add(nuevaOrden);

                        }
                    }
                    miorden.ListaDeProductos = datas;
                    miorden.Costo = TotalCost;
                    if(aca.FaceIDRepartidor_RepartidorCadena != null)
                    {
                        try
                        {
                            var Nombre = db.CarppiGrocery_Repartidores.Where(x=> x.FaceID_Repartidor == aca.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                            miorden.DeliveryManName = Nombre.Nombre + " " + Nombre.Apellido;
                        }
                        catch(Exception)
                        { }
                    }


                    
                    Retorn.Add(miorden);

                }
                return Json(new { StatusCode = "OK", Response = Retorn });
                //return Request.CreateResponse(HttpStatusCode.OK, Retorn);
            }
            catch (Exception)
            {
              
                return Json(new { StatusCode = "NotOK", Response = "" });
                //return Request.CreateResponse(HttpStatusCode.OK, Retorn);
            }


            return Json(new { result = "Redirect", url = Url.Action("Index", "Tutori") });

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
        public void Push_Restaurante(string CuerpoMensaje, string Titulo, string token, string ExtraData)
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
            string serverKey = "AAAAFXhHtWU:APA91bE0qkKYRH3UmNanpsnbegkSq8BLm062z9Ashi0StP8vhVaY8Jp-us8WTi-B80vQbFiejaOmKpTWNzZHq73uG4U0xP3RHHKYmqyz7KQtU5WkUn0W9qJvkpwcvYZPgze87ZLjjNx3";
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
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        void SendReceipt(DebtReturnType debtdata, string email, string subject, string transferID)
        {
            var Raul = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();//10217260473614661
            var Raul_Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == "10221568290107381").FirstOrDefault();//10217260473614661
            try
            {
                string path = Server.MapPath("~/App_Data/Sample.jpg");
                // string path = HttpContext.Current.Server.MapPath("~/App_Data/Sample.jpg");
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                //ViewBag.ImageData = imageDataURL;


                //  var TotalAmount = 15.ToString();
                //  var TotalCard = 15.ToString();
                //  var TotalCash = 15.ToString();
                //  var TotalFare = 15.ToString();


                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new System.Net.Mail.MailAddress("raul.sosa.cortes@gmail.com"));  // replace with valid value 
                message.To.Add(new System.Net.Mail.MailAddress(email));  // replace with valid value 
                message.From = new System.Net.Mail.MailAddress("carppi_mexico@carppi.com.mx");  // replace with valid value
                message.Subject = subject;
                string path2 = Server.MapPath("~/App_Data/Receipt.html");
                //string path2 = HttpContext.Current.Server.MapPath("~/App_Data/Receipt.html");
                var fileContents = System.IO.File.ReadAllText(path2);
                var r1 = fileContents.Replace("%%%TagForMoney%%%", "Total bruto: " + debtdata.RawDebt.ToString());
                r1 = r1.Replace("%%%TagForCard%%%", "Comision Carppi: " + debtdata.CarppiComision.ToString());
                r1 = r1.Replace("%%%TagForCash%%%", "Comision Bancaria: " + debtdata.StripeComision.ToString());
                r1 = r1.Replace("%%%TagForFare%%%", "Total: " + debtdata.Total.ToString());

                r1 = r1.Replace("%%%TagForID%%%", "Id de seguimiento: " + transferID);

                message.Body = r1;
                message.IsBodyHtml = true;

                using (var smtp = new System.Net.Mail.SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "carppi_mexico@carppi.com.mx",  // replace with valid value
                        Password = "THELASTTIMEaround"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp-relay.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    // smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    smtp.Send(message);
                    //await smtp.SendMailAsync(message);
                    //  return RedirectToAction("Sent");
                }


                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                //var Raul_Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == "10221568290107381").FirstOrDefault();//10217260473614661
                // Push_Repartidor(ep.ToString(), "holi", Raul_Repartidor.FirebaseID, "");
                Push(ep.ToString(), "Error", Raul.FirebaseID, "");
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }
        }
        public byte[] ConvertToByteArray_InputStream(HttpPostedFileBase file)
        {
            byte[] data;
            using (Stream inputStream = file.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
                return data;
            }
        }

        public void Push_Repartidor(string CuerpoMensaje, string Titulo, string token, string ExtraData)
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
            string serverKey = "AAAAMW1l0Ug:APA91bEyEEAAfz5JR8cyCgdD5CftyMqQ5i9UNuNhzzh_C3vh_GScpP52XQNfL9Tg-PkHLcsJWuv0-ySQbwbjlp5blbuVBppT2M6q4OLcbt_L4iLYBRYlkSZfNb_3Y2B5K4okEBPVGdbk";
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
        public bool IsInThisRegion(double RestaurantLat, double RestaurantLong, double DriverLat, double DriverLong)
        {
            // var CircleSoround = GenerateCircle(0.0003, Convert.ToDouble(Request_Trip.LatitudViajePendiente), Convert.ToDouble(Request_Trip.LongitudViajePendiente));
            //            var IsInDeliverRegion = PointInPolygon(CircleSoround.X_Array, CircleSoround.Y_Array, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));

            var CircleSoround = GenerateCircle(0.0006, Convert.ToDouble(RestaurantLat), Convert.ToDouble(RestaurantLong));
            var IsInDeliverRegion = PointInPolygon(CircleSoround.X_Array, CircleSoround.Y_Array, Convert.ToDouble(DriverLat), Convert.ToDouble(DriverLong));

            return IsInDeliverRegion;

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

        public void EraseBuyOrder(CarppiRestaurant_BuyOrders OrdenDeCompra)
        {
            db.CarppiRestaurant_BuyOrders.Remove(OrdenDeCompra);
            db.SaveChanges();
        }

        public void ChangeBuyOrderstateToAttended(CarppiRestaurant_BuyOrders OrdenDeCompra)
        {
            OrdenDeCompra.Stat = (int)GroceryOrderState.RequestBeingAttended;
            db.SaveChanges();
        }
        public bool CaptureFunds(string PaymentIntent, string UserID)
        {
            bool r_turn = false;
            try
            {
                if (ConfirmPaymentIntet(PaymentIntent, UserID))
                {
                    StripeConfiguration.ApiKey = "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";
                    //StripeConfiguration.ApiKey = "sk_test_6P04GsrUgxVEdGIkt43NXflH00awsFl7rR";

                    var service = new PaymentIntentService();

                    var intent = service.Capture(PaymentIntent);
                    r_turn = true;
                }
                else
                {
                    return r_turn;
                }
            }
            catch (Exception ex)
            {
                var ME = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();
                Push(ex.Message, "failed to accep payment:", ME.FirebaseID, "");
                Console.WriteLine(ex.ToString());
            }
            return r_turn;
        }
        public bool ConfirmPaymentIntet(string PaymentIntent, string UserID)
        {
            bool r_turn = false;
            try
            {
                StripeConfiguration.ApiKey = "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";

                var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == UserID).FirstOrDefault();

                // To create a PaymentIntent for confirmation, see our guide at: https://stripe.com/docs/payments/payment-intents/creating-payment-intents#creating-for-automatic
                var options = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = User.PaymentMethod,
                };
                var service = new PaymentIntentService();
                service.Confirm(
                  PaymentIntent,
                  options
                );

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
        public bool RefundOnCancelSolicitud(CarppiRestaurant_BuyOrders Solicitud)
        {
            bool r_turn = false;
            try
            {
                StripeConfiguration.ApiKey = "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";

                var service = new PaymentIntentService();
                var options = new PaymentIntentCancelOptions { };
                var intent = service.Cancel(Solicitud.paymentIntent, options);

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


        public class MesssageData
        {
            public CarppiREstaurant_MensajesClienteRestaurante MessageContext;
            public RoleInCoversation RoleInConversation;
        }
        public class MesssageValue
        {
            public CarppiRestaurant_MensajesRepartidorRestaurante MessageContext;
            public RoleInCoversation RoleInConversation;
        }
        public class ShopItem
        {
            public int ItemID;
            public int Quantity;
        }
        class RestaurantReturn
        {
            public CarppiRestaurant_BuyOrders Orden;
            public List<OrderData> ListaDeProductos;
            public double Costo;
            public string  DeliveryManName;

        }
        class OrderData
        {
            public long Cantidad;
            public string Producto;
        }
        public class ShopListItem
        {
            public int ID;
            public int RegionID;
            public int Cantidad;
            public string Producto;
            public double Costo;
            public byte[] Foto;
            public string Descripcion;
            public List<OptionalChoice> ObligatoryOptions;
            public List<OptionalChoice> PersonalOptions;
        }
        public class OptionalChoice
        {
            public string OptionHash;
            public double CostoExtra;
            public bool Disponible;
            public long ID;

        }

    }
}