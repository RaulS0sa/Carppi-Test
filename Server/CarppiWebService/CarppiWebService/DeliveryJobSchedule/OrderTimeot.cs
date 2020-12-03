using CarppiWebService.Clase_busqueda;
using CarppiWebService.Models;
using Newtonsoft.Json;
using Quartz;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CarppiWebService.DeliveryJobSchedule
{
    public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };
    public enum enumTipoDePago { Efectivo, Tarjeta };

    public class OrderTimeot : IJob
    {
        PidgeonEntities db = new PidgeonEntities();
        public async Task Execute(IJobExecutionContext context)
        {
            // throw new NotImplementedException();
            //BuyOrder.Stat = (int)GroceryOrderState.RequestCreated;
            var CreatedOrderStat = (int)GroceryOrderState.RequestCreated;
            var AccepteedOrderStat = (int)GroceryOrderState.RequestBeingAttended;
            var CreatedOrders = db.CarppiRestaurant_BuyOrders.Where(x => x.Stat == CreatedOrderStat);
            var AccepteedOrders = db.CarppiRestaurant_BuyOrders.Where(x => x.Stat == AccepteedOrderStat);
            var currentTime= (long)(((DateTime.UtcNow - new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
            foreach (var orden in CreatedOrders)
            {
                var resta = (currentTime - orden.TimeOfCreation);
                var restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == orden.RestaurantHash).FirstOrDefault();
                if( (((currentTime - orden.TimeOfCreation) > 600 && restaurante.AttedsItself == 1)) || (((currentTime - orden.TimeOfCreation) > 900 && restaurante.AttedsItself == 0)))
                {
                    var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == orden.UserID).FirstOrDefault();
                    Push("Prueba intentando mas tarde", "El restaurante rechazo tu orden", cliente.FirebaseID, "");
                    //var restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == orden.RestaurantHash).FirstOrDefault();
                    Push_Restaurante("Una de tus ordenes fue rechazada automaticamente por inactividad","Orden Rechazada", restaurante.FirebaseID, "");
                    if (restaurante.WebsiteFirebaseHash != null)
                    {
                        Push_Restaurante("Una de tus ordenes fue rechazada automaticamente por inactividad", "Orden Rechazada", restaurante.WebsiteFirebaseHash, "");
                        
                    }
                    orden.Stat = (int)GroceryOrderState.RequestRejected;
                    //db.SaveChanges();
                    Task.Delay(4000).ContinueWith(t => EraseBuyOrder(orden));

                }
                else if (((currentTime - orden.TimeOfCreation) > 10 && restaurante.AttedsItself == 0 && (currentTime - orden.TimeOfCreation) < 500))
                {
                    var TempOrder = orden;
                    SetOrderStatus(GroceryOrderState.RequestAccepted, orden.ID, ref TempOrder);
                   // RequestDriver(restaurante.CarppiHash, orden.ID);
                }
            }
            foreach (var orden in AccepteedOrders)
            {
                var resta = (currentTime - orden.TimeOfCreation);
                var restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == orden.RestaurantHash).FirstOrDefault();
                if ( (((currentTime - orden.TimeOfCreation) > 2000 && restaurante.AttedsItself == 0)))
                {
                    var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == orden.UserID).FirstOrDefault();
                    Push("Prueba intentando mas tarde", "El restaurante rechazo tu orden", cliente.FirebaseID, "");
                    //var restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == orden.RestaurantHash).FirstOrDefault();
                    Push_Restaurante("Una de tus ordenes fue rechazada automaticamente por inactividad", "Orden Rechazada", restaurante.FirebaseID, "");
                    if (restaurante.WebsiteFirebaseHash != null)
                    {
                        Push_Restaurante("Una de tus ordenes fue rechazada automaticamente por inactividad", "Orden Rechazada", restaurante.WebsiteFirebaseHash, "");

                    }
                    orden.Stat = (int)GroceryOrderState.RequestRejected;
                    //db.SaveChanges();
                    Task.Delay(4000).ContinueWith(t => EraseBuyOrder(orden));

                }
                else if (((currentTime - orden.TimeOfCreation) > 10 && restaurante.AttedsItself == 0 && (currentTime - orden.TimeOfCreation) < 500))
                {
                    //SetOrderStatus(GroceryOrderState.RequestAccepted, orden.ID);
                    //RequestDriver(restaurante.CarppiHash, orden.ID);
                    Task.Delay(1000).ContinueWith(t => RequestDriver(restaurante.CarppiHash, orden.ID));
                }

            }


            await db.SaveChangesAsync();
        }

        public HttpStatusCode RequestDriver(string restaurantHashRequestingDriver, long NewOrderID)
        {
            var restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == restaurantHashRequestingDriver).FirstOrDefault();
            var Busqueda = new BusquedaRepartidor(Convert.ToDouble(restaurante.Latitud), Convert.ToDouble(restaurante.Longitud), Convert.ToInt64(restaurante.Region));
            CarppiGrocery_Repartidores myrepartidor = Busqueda.SearchForNearestDeliveryBoy();
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.ID == NewOrderID).FirstOrDefault();
            if (Order.FaceIDRepartidor_RepartidorCadena != null)
            {
                return HttpStatusCode.NotAcceptable;// Request.CreateResponse(HttpStatusCode.NotAcceptable, "0repartidores");
            }

            //SearchForNearestDeliveryBoy
            if (myrepartidor == null)
            {
                return HttpStatusCode.Gone;// Request.CreateResponse(HttpStatusCode.Gone, "0repartidores");
            }
            else if (Busqueda.DriverHasToomuchOrders == true)
            {
                return HttpStatusCode.Moved;//Request.CreateResponse(HttpStatusCode.Moved, "0repartidores");
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
                return HttpStatusCode.OK;//Request.CreateResponse(HttpStatusCode.OK, "0repartidores");

            }


        }

        public HttpStatusCode SetOrderStatus(GroceryOrderState Estado, long OrderID, ref CarppiRestaurant_BuyOrders Order_ref)
        {
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.ID == OrderID).FirstOrDefault();
            var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Order.UserID).FirstOrDefault();

            if (Estado == GroceryOrderState.RequestAccepted)
            {
                if (Order.TipoDePago == (int)enumTipoDePago.Efectivo)
                {
                    Push("Tu orden fue aceptada, abre la app para conocer mas detalles", "Tu orden ha sido aceptada", cliente.FirebaseID, "");
                    //Order.Stat = (int)GroceryOrderState.RequestBeingAttended;
                    // ChangeBuyOrderstateToAttended(Order);
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
                       // ChangeBuyOrderstateToAttended(Order);
                        Task.Delay(10000).ContinueWith(t => ChangeBuyOrderstateToAttended(Order));
                    }
                    else
                    {
                        return HttpStatusCode.InternalServerError;//Request.CreateResponse(HttpStatusCode.InternalServerError, "");
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
                        return HttpStatusCode.Forbidden;//Request.CreateResponse(HttpStatusCode.Forbidden, "");
                    }
                }
                else
                {
                    return HttpStatusCode.Conflict;// Request.CreateResponse(HttpStatusCode.Conflict, "");
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
            return HttpStatusCode.Accepted;//Request.CreateResponse(HttpStatusCode.Accepted, "");

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
    }
}