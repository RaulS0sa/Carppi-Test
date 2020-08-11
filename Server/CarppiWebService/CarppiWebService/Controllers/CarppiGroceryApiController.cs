using CarppiWebService.Clase_busqueda;
using CarppiWebService.Models;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CarppiWebService.Controllers
{
    public class CarppiGroceryApiController : ApiController
    {

        PidgeonEntities db = new PidgeonEntities();
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
        public enum ProductSubset { Verduras_hortalizas, Frutas, Carne_Embutidos_Atun, Queso, Tortillas_Pan, BotanasRefresco};

        public enum enumTipoDePago { Efectivo, Tarjeta };


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CarppiDeliveryGetArea( double lat, double log)
        {
            var RequestAreaObj = new BusquedaRepartidor(lat, log, 0);
            var Region = RequestAreaObj.GetUsageRegion();
            return Request.CreateResponse(HttpStatusCode.OK, Region);
        }



        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchForPassengerAreaByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea, double lat, double log)
        {
            var New_Area = db.CarppiGrocery_Regiones.Where(x => x.Pais.Contains(Country) && x.Estado.Contains(State) && x.Ciudad.Contains(Town)).FirstOrDefault();

            var IsInRegion = false;
            long Region = 0;
            var ReginObject = new CarppiGrocery_Regiones();

            if (New_Area != null)
            {
                if (FacebookID_UpdateArea != "")
                {
                    var USer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FacebookID_UpdateArea).FirstOrDefault();
                    if (USer != null)
                    {
                        USer.Region_ReparticionDeAlimentos = New_Area.ID;
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
                var NnuevaREgion = new CarppiGrocery_Regiones();
                NnuevaREgion.Ciudad = Town;
                NnuevaREgion.Pais = Country;
                NnuevaREgion.Estado = State;
                NnuevaREgion.Latitud = lat;
                NnuevaREgion.Longitud = log;
                NnuevaREgion.Moneda = "mx";
                db.CarppiGrocery_Regiones.Add(NnuevaREgion);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.BadRequest, NnuevaREgion);
            }

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage PostMessageInRideShare(String FaceID_Interlocutor, Int64 TripRequest, String Message)
        {
            var REquest_OfTrip = db.CarppiRestaurant_BuyOrders.Where(x => x.ID == TripRequest && (x.UserID == FaceID_Interlocutor || x.FaceIDRepartidor_RepartidorCadena == FaceID_Interlocutor) ).FirstOrDefault();
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
                var NewMessage = new CarppiRestaurant_MensajesRepartidorCliente();
                NewMessage.Mensaje = Message;
                NewMessage.FaceID_Sender = FaceID_Interlocutor;
                NewMessage.BuyOrderID = TripRequest;
                NewMessage.DateInSeconds = (long)(((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
                db.CarppiRestaurant_MensajesRepartidorCliente.Add(NewMessage);
                db.SaveChanges();

                //var Driver = db.CarppiRequestForDrives.Where(x => x.FaceIDDriver == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();
                //var Passenger = db.CarppiRequestForDrives.Where(x => x.FaceIDPassenger == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();

                //var Token = "";
                //Token = Driver == null ? "" : db.Traveler_Perfil.Where(x=> x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                //Token = Passenger == null ? Token : db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                if (FaceID_Interlocutor == REquest_OfTrip.UserID)
                {
                    //var intermitentID= 
                    
                    var repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor.Contains( REquest_OfTrip.FaceIDRepartidor_RepartidorCadena)).FirstOrDefault();
                    Push_Repartidor(Message, "Mensaje De la orden " + REquest_OfTrip.ID.ToString(), repartidor.FirebaseID, "");
                  
                }
                else
                {
                    var Token = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == REquest_OfTrip.UserID).FirstOrDefault().FirebaseID;
                    Push(Message, "Mensaje Del Repartidor", Token, "");
                    //Push(Message, "Mensaje Del cliente", Token, "");
                }


            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "");

        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetAlMessagesFromTheConversation(String FaceID_Interest, Int64 TripRequest)
        {
            var Messages = db.CarppiRestaurant_MensajesRepartidorCliente.Where(x => x.BuyOrderID == TripRequest);
            List<MesssageData> messsageDatas = new List<MesssageData>();
            foreach (var aca in Messages)
            {
                var OldMessage = new MesssageData();
                OldMessage.MessageContext = aca;
                OldMessage.RoleInConversation = aca.FaceID_Sender == FaceID_Interest ? RoleInCoversation.Sender : RoleInCoversation.Receiver;
                messsageDatas.Add(OldMessage);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, messsageDatas);

        }
        public enum RoleInCoversation { Sender, Receiver };
        public class MesssageData
        {
            public CarppiRestaurant_MensajesRepartidorCliente MessageContext;
            public RoleInCoversation RoleInConversation;
        }





        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SetOrderStatus(string FaceIDHash_DeliveryBoy, GroceryOrderState Estado, Int32 OrderID )
        {
            var Order = db.CarppiGrocery_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceIDHash_DeliveryBoy && x.ID == OrderID).FirstOrDefault();
            var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Order.UserID).FirstOrDefault();
            Order.Stat = (int)Estado;
            db.SaveChanges();
            if (Estado == GroceryOrderState.RequestAccepted)
            {
                Push("Tu orden fue aceptada, abre la app para conocer mas detalles", "Tu orden ha sido aceptada", cliente.FirebaseID, "");
                Task.Delay(15000).ContinueWith(t => ChangeBuyOrderstateToAttended(Order));
            }
            else if (Estado == GroceryOrderState.RequestGoingToClient)
            {
                Push("Tu repartidor esta llendo a tu ubicación actual, abre la app para conocer mas detalles", "El repartidor corre hacia ti", cliente.FirebaseID, "");
                //Task.Delay(15000).ContinueWith(t => ChangeBuyOrderstateToAttended(Order));
            }
            else if (ConfirmPayment(db.Traveler_Perfil.Where(x=> x.Facebook_profile_id == Order.UserID).FirstOrDefault(),Order.paymentIntent)) 
            { 
                if (Estado == GroceryOrderState.RequestEnded && CaptureFunds(Order.paymentIntent))
                {
                    Order.Stat = (int)Estado;
                    db.SaveChanges();
                    Task.Delay(25000).ContinueWith(t => EraseBuyOrder(Order));
                }
            }
            else if(Estado == GroceryOrderState.RequestEnded)
            {
                Push("Tu orden fue entregada, por el momento no puedes calificar al repartidor :c", "Tu orden fue entregada", cliente.FirebaseID, "");
                var mensajesEnConvo = db.CarppiGrocery_Mensajes.Where(x=> x.BuyOrderID == Order.ID);
                foreach(var mensaje in mensajesEnConvo)
                {
                    db.CarppiGrocery_Mensajes.Remove(mensaje);
                }
                Order.Stat = (int)Estado;
                db.SaveChanges();
                Task.Delay(24000).ContinueWith(t => EraseBuyOrder(Order));
                
            }
            else if (Estado == GroceryOrderState.RequestRejected)
            {
                Push("Prueba intentando mas tarde", "Tu orden fue rechazada", cliente.FirebaseID, "");
                Order.Stat = (int)Estado;
                db.SaveChanges();
                Task.Delay(20000).ContinueWith(t => EraseBuyOrder(Order));

            }

            return Request.CreateResponse(HttpStatusCode.Accepted, "");

        }

        public bool ConfirmPayment(Traveler_Perfil Usuario, string paymentIntentID)
        {
            var r_turn = false;
            try
            {
                StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";

              

                // To create a PaymentIntent for confirmation, see our guide at: https://stripe.com/docs/payments/payment-intents/creating-payment-intents#creating-for-automatic
                var options = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = Usuario.PaymentMethod,
                };
                var service = new PaymentIntentService();
                var payment = service.Get(paymentIntentID);

                service.Confirm(
                  paymentIntentID,
                  options
                );
                r_turn = true;
            }
            catch(Exception)
            { }
            return r_turn;
        }

        public void EraseBuyOrder(CarppiGrocery_BuyOrders OrdenDeCompra)
        {
            db.CarppiGrocery_BuyOrders.Remove(OrdenDeCompra);
            db.SaveChanges();
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
        public void ChangeBuyOrderstateToAttended(CarppiGrocery_BuyOrders OrdenDeCompra)
        {
            OrdenDeCompra.Stat = (int)GroceryOrderState.RequestBeingAttended;
            db.SaveChanges();
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetListOfDeliveryBoyOrder(string FaceIDHash_DeliveryBoy)
        {
            try
            {
                var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceIDHash_DeliveryBoy && x.Stat != (int)GroceryOrderState.RequestEnded);
                List<DeliveryOrderQuery> deliveryOrders = new List<DeliveryOrderQuery>();
                foreach (var ord in Order)
                {
                   if (deliveryOrders.Where(x => x.Orden.RestaurantHash == ord.RestaurantHash).FirstOrDefault() == null)
                    {
                        var Orden = new DeliveryOrderQuery();
                        Orden.Orden = ord;
                        var ProductosDecoded = Base64Decode(ord.ListaDeProductos);
                        var Listanueva = JsonConvert.DeserializeObject<List<ShopItem>>(ProductosDecoded);

                        List<Carppi_ProductosPorRestaurantes> Productos = new List<Carppi_ProductosPorRestaurantes>();
                        foreach (var prod in Listanueva)
                        {
                            var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == prod.ItemID).FirstOrDefault();
                            var ProductoTemporal = new Carppi_ProductosPorRestaurantes();
                            ProductoTemporal.ID = Producto.ID;
                            ProductoTemporal.Costo = Producto.Costo;
                            ProductoTemporal.Nombre = Producto.Nombre;
                            ProductoTemporal.IDdRestaurante = Producto.IDdRestaurante;

                            Productos.Add(ProductoTemporal);
                        }

                        Orden.Productos = Productos;
                        deliveryOrders.Add(Orden);
                    }

                }

                return Request.CreateResponse(HttpStatusCode.Accepted, deliveryOrders);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.ToString());
            }
          
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetListOfDeliveryBoyOrderAndStatus(string FaceIDHash_DeliveryBoyAndStatus)
        {
            try
            {
                var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceIDHash_DeliveryBoyAndStatus && x.Stat != (int)GroceryOrderState.RequestEnded);
                List<DeliveryOrderQuery> deliveryOrders = new List<DeliveryOrderQuery>();
                foreach (var ord in Order)
                {
                    if (deliveryOrders.Where(x => x.Orden.RestaurantHash == ord.RestaurantHash).FirstOrDefault() == null)
                    {

                        var ProductosDecoded = Base64Decode(ord.ListaDeProductos);
                        if (ProductosDecoded.Contains("ItemID") && ProductosDecoded.Contains("Quantity"))
                        {
                            // ShopItem
                            var Orden = new DeliveryOrderQuery();
                            Orden.Orden = ord;
                            var Listanueva = JsonConvert.DeserializeObject<List<ShopItem>>(ProductosDecoded);

                            List<Carppi_ProductosPorRestaurantes> Productos = new List<Carppi_ProductosPorRestaurantes>();
                            foreach (var prod in Listanueva)
                            {
                                var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == prod.ItemID).FirstOrDefault();
                                var ProductoTemporal = new Carppi_ProductosPorRestaurantesWithQuantity();
                                ProductoTemporal.ID = Producto.ID;
                                ProductoTemporal.Costo = Producto.Costo;
                                ProductoTemporal.Nombre = Producto.Nombre;
                                ProductoTemporal.IDdRestaurante = Producto.IDdRestaurante;
                                ProductoTemporal.Cantidad = prod.Quantity;

                                Productos.Add(ProductoTemporal);
                            }

                            Orden.Productos = Productos;
                            deliveryOrders.Add(Orden);
                        }
                        else{
                        // ShopItem
                        var Orden = new DeliveryOrderQuery();
                        Orden.Orden = ord;
                        var Listanueva = JsonConvert.DeserializeObject<List<ShopListItem>>(ProductosDecoded);

                        List<Carppi_ProductosPorRestaurantes> Productos = new List<Carppi_ProductosPorRestaurantes>();
                        foreach (var prod in Listanueva)
                        {
                            var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == prod.ID).FirstOrDefault();
                            var ProductoTemporal = new Carppi_ProductosPorRestaurantesWithQuantity();
                            ProductoTemporal.ID = Producto.ID;
                            ProductoTemporal.Costo = Producto.Costo;
                            ProductoTemporal.Nombre = Producto.Nombre;
                            ProductoTemporal.IDdRestaurante = Producto.IDdRestaurante;
                            ProductoTemporal.Cantidad = prod.Cantidad;

                            Productos.Add(ProductoTemporal);
                        }

                        Orden.Productos = Productos;
                        deliveryOrders.Add(Orden);
                    }
                    }

                }
                var DeliverManObject = db.CarppiGrocery_Repartidores.Where(x=> x.FaceID_Repartidor == FaceIDHash_DeliveryBoyAndStatus).FirstOrDefault();
                var newStatus = new DeliverManStatus();
                newStatus.OrderList = deliveryOrders;
                newStatus.Debt = DeliverManObject.DebtToDeliverMan;
                newStatus.DelivermanStatus = DeliverManObject == null ? false : DeliverManObject.IsAvailableForDeliver;
                return Request.CreateResponse(HttpStatusCode.Accepted, newStatus);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.ToString());
            }

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


        public class Carppi_ProductosPorRestaurantesWithQuantity : Carppi_ProductosPorRestaurantes
        {
            public int Cantidad;
        }


        public class DeliverManStatus
        {
            public List<DeliveryOrderQuery> OrderList;
            public bool DelivermanStatus;
            public long Debt;
        }


        public class DeliveryOrderQuery
        { 
        public CarppiRestaurant_BuyOrders Orden;
            public List<Carppi_ProductosPorRestaurantes> Productos;

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetPendingOrder(string FaceIDHash)
        {
            var Order = db.CarppiGrocery_BuyOrders.Where(x => x.UserID == FaceIDHash).FirstOrDefault();
            if (Order != null)
            {
                var DeliverYBoy = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == Order.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                Order.Latitud_Repartidor = Convert.ToDouble(DeliverYBoy.Latitud);
                Order.Longitud_Repartidor = Convert.ToDouble(DeliverYBoy.Longitud);
                if (Order.Stat == (int)GroceryOrderState.RequestEnded)
                {
                    db.CarppiGrocery_BuyOrders.Remove(Order);
                    Order = null;
                }
            }
          
            //var obj1 = db.CarppiGrocery_Productos.Where(x => x.RegionID == Region);
            db.SaveChanges();
            if (Order != null)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, Order);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, "");
            }
        }
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetExtraDataFromTheBuyOrder(Int32 PendingOrder_ToExtractData)
        {
            var Order = db.CarppiGrocery_BuyOrders.Where(x => x.ID == PendingOrder_ToExtractData).FirstOrDefault();
            var Driver = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == Order.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
            var Comments = db.Carppi_ComentariosHaciaElPerfil.Where(x => x.FaceID_OfComentedFolk == Order.FaceIDRepartidor_RepartidorCadena);
            var Extra = new TripExtraDatacaCommentUtility();
            Extra.DriverName = Driver.Nombre;
            Extra.Rate =Convert.ToDouble( Driver.Rating_double);
            Extra.Marca_Vehiculo = Driver.Marca_Vehiculo;
            //Extra.Color_Vehiculo = Driver.Color_Vehiculo;
            Extra.Modelo_Vehiculo = Driver.Modelo_Vehiculo;
            Extra.Placa_Vehiculo = Driver.Placa;
            try
            {
                Extra.Driverphoto = Driver.Foto_repartidor != null ? Convert.ToBase64String(Driver.Foto_repartidor) : null;
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
                catch (Exception)
                { }

            }
            Extra.ListOfComents = ListOfComents;

            return Request.CreateResponse(HttpStatusCode.Accepted, Extra);


        }
        public class CarppiComentUtility
        {
            public Carppi_ComentariosHaciaElPerfil ComentData;
            public string PhotoOfComenter;

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

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CalculateCostOfTrip(Int32 Region_costo, double LatitudPedido, double LongitudPedido)
        {


            


            return Request.CreateResponse(HttpStatusCode.Accepted, 26);
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CalculateCostOfTripWithRestaurant(string RestaurantHash, double LatitudPedido, double LongitudPedido)
        {

            var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == RestaurantHash).FirstOrDefault();
            var Busqueda = new BusquedaRepartidor(Convert.ToDouble(Restaurante.Latitud), Convert.ToDouble(Restaurante.Longitud), Convert.ToInt64(Restaurante.Region));
            var fee = Busqueda.OperationCost(Convert.ToDouble(LatitudPedido), Convert.ToDouble(LongitudPedido));



            return Request.CreateResponse(HttpStatusCode.Accepted, (fee));
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CalculateCostOfTripWithRestauran_AndLogDatat(string RestaurantHash, double LatitudPedido, double LongitudPedido, string userTag_Log)
        {

            var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == RestaurantHash).FirstOrDefault();
            var Busqueda = new BusquedaRepartidor(Convert.ToDouble(Restaurante.Latitud), Convert.ToDouble(Restaurante.Longitud), Convert.ToInt64(Restaurante.Region));
            var fee = Busqueda.OperationCost(Convert.ToDouble(LatitudPedido), Convert.ToDouble(LongitudPedido));

            
            try
            {
                var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == userTag_Log).FirstOrDefault();
                if (userTag_Log != null)
                {
                    var Tempcliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == userTag_Log).FirstOrDefault();
                    if (Tempcliente != null)
                    {
                        cliente = Tempcliente;
                    }
                }
                /*
                if (cliente.IsFirstOrder == true)
                {
                    fee = 0;
                }
                */
            }
            catch(Exception)
            {
                fee = 15;
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, (fee));
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CalculateCostOfTripWithRestauran_AndRestaurantDetails(string RestaurantHashCode, double LatitudP, double LongitudP, string userTag_Log)
        {

            var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == RestaurantHashCode).FirstOrDefault();
            var Busqueda = new BusquedaRepartidor(Convert.ToDouble(Restaurante.Latitud), Convert.ToDouble(Restaurante.Longitud), Convert.ToInt64(Restaurante.Region));
            var fee = Busqueda.OperationCost(Convert.ToDouble(LatitudP), Convert.ToDouble(LongitudP));

            var cliente = new Traveler_Perfil();
            cliente.StripeClientID = null;
            cliente.PaymentMethod = null;
            cliente.IsUserAJoker = false;
            try
            {
                if(userTag_Log != null)
                {
                    var Tempcliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == userTag_Log).FirstOrDefault();
                    if(Tempcliente != null)
                    {
                        cliente = Tempcliente;
                    }
                }
                
              //  if (cliente.IsFirstOrder == true)
               // {
               //     fee = 0;
              //  }
            }
            catch (Exception)
            {
                fee = 15;
            }

            var r_ = new CostOfTripAndDetails();
            r_.fee = fee;
            r_.AcceptCardPayments = (Restaurante.StripeAccount != null) ;
            r_.UserLoggedCard = (cliente.StripeClientID != null) && (cliente.PaymentMethod != null);
            r_.UserDontShowCashPayment = cliente.IsUserAJoker;
            return Request.CreateResponse(HttpStatusCode.Accepted, (r_));
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CalculateCostOfTripWithRestauran_AndRestaurantDetailsAndPayment(string RestaurantHashCode, double LatitudP, double LongitudP, string userTag_Log, enumTipoDePago TypeOfPayment)
        {

            var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == RestaurantHashCode).FirstOrDefault();
            var Busqueda = new BusquedaRepartidor(Convert.ToDouble(Restaurante.Latitud), Convert.ToDouble(Restaurante.Longitud), Convert.ToInt64(Restaurante.Region));
            var fee = Busqueda.OperationCost(Convert.ToDouble(LatitudP), Convert.ToDouble(LongitudP));

            var cliente = new Traveler_Perfil();
            cliente.StripeClientID = null;
            cliente.PaymentMethod = null;
            cliente.IsUserAJoker = false;
            try
            {
                if(userTag_Log != null)
                {
                    var Tempcliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == userTag_Log).FirstOrDefault();
                    if(Tempcliente != null)
                    {
                        cliente = Tempcliente;
                    }
                }
                
                //  if (cliente.IsFirstOrder == true)
                // {
                //     fee = 0;
                //  }
            }
            catch (Exception)
            {
                fee = 0;
            }

            var r_ = new CostOfTripAndDetails();
            r_.fee = fee;
            r_.AcceptCardPayments = (Restaurante.StripeHash != null);
            r_.UserLoggedCard = (cliente.StripeClientID != null) && (cliente.PaymentMethod != null);
            r_.UserDontShowCashPayment = cliente.IsUserAJoker;
            r_.ComisionPorPagoConTarjeta = TypeOfPayment == enumTipoDePago.Tarjeta ? 1.1 : 1.0;
            r_.TarifaEstaticaStripe = 3.2;

            return Request.CreateResponse(HttpStatusCode.Accepted, (r_));
        }




        public class CostOfTripAndDetails
        {
            public double fee;
            public bool AcceptCardPayments;
            public bool UserLoggedCard;
            public bool UserDontShowCashPayment;
            public double ComisionPorPagoConTarjeta;
            public double TarifaEstaticaStripe;
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetCompleteListOfProductsByProductSubset(Int32 Region, ProductSubset Subset)
        {


            var obj1 = db.CarppiGrocery_Productos.Where(x => x.RegionID == Region && x.Categoria == (int)Subset);


            return Request.CreateResponse(HttpStatusCode.Accepted, obj1);
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetCompleteListOfProducts(Int32 Region)
        {


            var obj1 = db.CarppiGrocery_Productos.Where(x => x.RegionID == Region);
      

            return Request.CreateResponse(HttpStatusCode.Accepted, obj1);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
  
        
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GeneratePurchaseOrder(string FaceIDOfBuyer, string BuyList, double Lat, double Log, long Region, enumTipoDePago tipoDePago)
        {

            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";

            var Buyer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfBuyer).FirstOrDefault();
            if (Buyer == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "NoAutorizado");
            }
          //  else if (Buyer.StripeClientID == null || Buyer.PaymentMethod == null)
          //  {
          //      return Request.CreateResponse(HttpStatusCode.Forbidden, "NoAutorizado");
          //  }
            else
            {
                var Busqueda = new BusquedaRepartidor(Lat, Log,Convert.ToInt64( Buyer.Region));
                if (Busqueda.Repartidor == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Gone, "0repartidores");
                }
                else if(Busqueda.DriverHasToomuchOrders == true)
                {
                    return Request.CreateResponse(HttpStatusCode.Moved, "0repartidores");
                }
                else
                {
                    var buyer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfBuyer).FirstOrDefault();
                    var BuyOrder = new CarppiGrocery_BuyOrders();
                    BuyOrder.Latitud = Lat;
                    BuyOrder.Longitud = Log;
                    BuyOrder.RegionID = Region;
                    BuyOrder.Stat = (int)GroceryOrderState.RequestCreated;
                    BuyOrder.UserID = FaceIDOfBuyer;
                    BuyOrder.ListaDeProductos = BuyList;
                    BuyOrder.FaceIDRepartidor_RepartidorCadena = Busqueda.Repartidor.FaceID_Repartidor;
                    BuyOrder.NombreDelUsuario = buyer.Nombre_usuario;
                    BuyOrder.TipoDePago = ((int)tipoDePago);
                    //BuyOrder.paymentIntent
                    var Decoded = Base64Decode(BuyList);
                    var ListOfItems = JsonConvert.DeserializeObject<List<ShopItem>>(Decoded);
                    var TotalCost = 0.0;
                    
                    foreach (var Item in ListOfItems)
                    {
                        var Produc = db.CarppiGrocery_Productos.Where(x => x.ID == Item.ItemID).FirstOrDefault();
                        TotalCost += (Convert.ToDouble(Produc.Costo) * Item.Quantity);
                    }
                    TotalCost = TotalCost < 10 ? 10 : TotalCost;
                    if (tipoDePago == enumTipoDePago.Tarjeta) 
                    { 
                  
                    var options = new PaymentIntentCreateOptions
                    {

                        Customer = Buyer.StripeClientID,
                        PaymentMethod = Buyer.PaymentMethod,
                        Amount = (int)(TotalCost * 100),
                        Currency = "mxn",
                        CaptureMethod = "manual",
                        PaymentMethodTypes = new List<string> { "card" },
                        SetupFutureUsage = "off_session",
                        ApplicationFeeAmount = 3000,
                        TransferData = new PaymentIntentTransferDataOptions
                        {
                            Destination = "acct_1GJp5qJgvCsnk1R4"
                        }

                    };
                    var service = new PaymentIntentService();
                    PaymentIntent paymentIntent = service.Create(options);

                    BuyOrder.paymentIntent = paymentIntent.Id;
                }
                db.CarppiGrocery_BuyOrders.Add(BuyOrder);
                db.SaveChanges();

                    Push_Repartidor("tienes una nueva orden","Nueva Orden", Busqueda.Repartidor.FirebaseID, "");
                //var Erick = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "849994702134646").FirstOrDefault();
                //Push("Nueva Orden de compra", "Nueva orden, revisa tu celular", "", "");

                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
            }
        }
          


            return Request.CreateResponse(HttpStatusCode.Accepted, "error");
        }
        public class ShopItem
        {
            public int ItemID;
            public int Quantity;
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
