using CarppiWebService.Clase_busqueda;
using CarppiWebService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarppiWebService.Controllers
{
    public class CarppiRepartidorApiController : ApiController
    {
        PidgeonEntities db = new PidgeonEntities();

        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded };

        public enum enumTipoDePago { Efectivo, Tarjeta };
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public class ShopItem
        {
            public int ItemID;
            public int Quantity;
        }
        public enum WorkState { StopWork, StartWork };

       

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ChangeDeliverymanJourney(string FaceIDHash_Deliveryman, WorkState workState )
        {
            var deliveryPerson = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == FaceIDHash_Deliveryman).FirstOrDefault();
            if(workState == WorkState.StartWork)
            {
                var time = (long)(((DateTime.UtcNow - new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
                deliveryPerson.LastTimeOfLogin = time;
                deliveryPerson.IsAvailableForDeliver = true;
            }
            else
            {
                deliveryPerson.IsAvailableForDeliver = false;
            }

            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Accepted, "");
        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetRestaurantDetailedData(string FaceIDHash_DeliveryMan, long OrderID_Restaurant)
        {
            var firstOrder = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceIDHash_DeliveryMan && x.Stat != (int)GroceryOrderState.RequestEnded && x.ID == OrderID_Restaurant).FirstOrDefault();
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceIDHash_DeliveryMan && x.Stat != (int)GroceryOrderState.RequestEnded && x.RestaurantHash == firstOrder.RestaurantHash);
            List<DeliveryOrderQuery> deliveryOrders = new List<DeliveryOrderQuery>();
            var CostoParcial = 0.0;
            foreach (var ord in Order)
            {
                var Orden = new DeliveryOrderQuery();
                Orden.Orden = ord;
                var ProductosDecoded = Base64Decode(ord.ListaDeProductos);
                if (ProductosDecoded.Contains("ItemID") && ProductosDecoded.Contains("Quantity"))
                {
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
                    CostoParcial += Convert.ToDouble(Producto.Costo * prod.Quantity);

                    Productos.Add(ProductoTemporal);
                }

                Orden.Productos = Productos;
                deliveryOrders.Add(Orden);
            }
                else
                {
                    var Listanueva = JsonConvert.DeserializeObject<List<ShopListItem>>(ProductosDecoded);

                    List<Carppi_ProductosPorRestaurantes> Productos = new List<Carppi_ProductosPorRestaurantes>();
                    foreach (var prod in Listanueva)
                    {
                        var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == prod.ID).FirstOrDefault();
                        var ProductoTemporal = new Carppi_ProductosPorRestaurantes();
                        ProductoTemporal.ID = Producto.ID;
                        ProductoTemporal.Costo = Producto.Costo;
                        ProductoTemporal.Nombre = Producto.Nombre;
                        ProductoTemporal.IDdRestaurante = Producto.IDdRestaurante;
                        var CostoNormal =Convert.ToDouble( Producto.Costo);
                        try
                        {
                            foreach (var c in prod.PersonalOptions)
                            {
                                var Adicional = db.OptionalChoice.Where(x => x.ID == c.ID).FirstOrDefault();
                                CostoNormal += Convert.ToDouble(Adicional.CostoExtra);
                            }
                        }
                        catch (Exception)
                        { }
                        CostoParcial += Convert.ToDouble(CostoNormal * prod.Cantidad);

                        Productos.Add(ProductoTemporal);
                    }

                    Orden.Productos = Productos;
                    deliveryOrders.Add(Orden);
                }

            }

            var Contexto = new DeliveryContext();
            Contexto.deliveryOrderQuery = deliveryOrders;
            Contexto.CostoTotal = CostoParcial;
            var TypeOfPaymentAppend = Order.FirstOrDefault().TipoDePago == (int)enumTipoDePago.Efectivo ? " TipoDePago: Efectivo" : " TipoDePago: Tarjeta";
            Contexto.cliente = Order.FirstOrDefault().NombreDelRestaurante+ "   ," + TypeOfPaymentAppend;

            return Request.CreateResponse(HttpStatusCode.Accepted, Contexto);

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetOrderProductList(string FaceIDHash_DeliveryBoy, long OrderID)
        {
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == FaceIDHash_DeliveryBoy && x.Stat != (int)GroceryOrderState.RequestEnded && x.ID == OrderID);
            List<DeliveryOrderQuery> deliveryOrders = new List<DeliveryOrderQuery>();
            var CostoParcial = 0.0;
            foreach (var ord in Order)
            {
                var Orden = new DeliveryOrderQuery();
                Orden.Orden = ord;
                var ProductosDecoded = Base64Decode(ord.ListaDeProductos);
                if (ProductosDecoded.Contains("ItemID") && ProductosDecoded.Contains("Quantity"))
                {
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
                    CostoParcial += Convert.ToDouble(Producto.Costo * prod.Quantity);

                    Productos.Add(ProductoTemporal);
                }

                Orden.Productos = Productos;
                deliveryOrders.Add(Orden);
            }
                else
                {
                    var Listanueva = JsonConvert.DeserializeObject<List<ShopListItem>>(ProductosDecoded);

                    List<Carppi_ProductosPorRestaurantes> Productos = new List<Carppi_ProductosPorRestaurantes>();
                    foreach (var prod in Listanueva)
                    {
                        var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == prod.ID).FirstOrDefault();
                        var ProductoTemporal = new Carppi_ProductosPorRestaurantes();
                        ProductoTemporal.ID = Producto.ID;
                        ProductoTemporal.Costo = Producto.Costo;
                        ProductoTemporal.Nombre = Producto.Nombre;
                        ProductoTemporal.IDdRestaurante = Producto.IDdRestaurante;
                        var CostoNormal = Convert.ToDouble(Producto.Costo);
                        try
                        {
                            foreach (var c in prod.PersonalOptions)
                            {
                                var Adicional = db.OptionalChoice.Where(x => x.ID == c.ID).FirstOrDefault();
                                CostoNormal += Convert.ToDouble(Adicional.CostoExtra);
                            }
                        }
                        catch(Exception)
                        { }
                        CostoParcial += Convert.ToDouble(CostoNormal * prod.Cantidad);

                        Productos.Add(ProductoTemporal);
                    }

                    Orden.Productos = Productos;
                    deliveryOrders.Add(Orden);

                }

            }
            var TypeOfPaymentAppend = Order.FirstOrDefault().TipoDePago == (int)enumTipoDePago.Efectivo ? " TipoDePago: Efectivo" :  " TipoDePago: Tarjeta";
            var Contexto = new DeliveryContext();
            Contexto.deliveryOrderQuery = deliveryOrders;
            Contexto.CostoTotal = Convert.ToDouble(Order.FirstOrDefault().Precio);// CostoParcial;
            Contexto.cliente = Order.FirstOrDefault().NombreDelUsuario + "  ," + TypeOfPaymentAppend;
            Contexto.TarifaDelEnvio = Order.FirstOrDefault().TarifaDelServicio;

            return Request.CreateResponse(HttpStatusCode.Accepted, Contexto);

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
        class DeliveryContext
        {
           public List<DeliveryOrderQuery> deliveryOrderQuery;
            public double CostoTotal;
            public string cliente;
            public double TarifaDelEnvio;
        }

        class DeliveryOrderQuery
        {
            public CarppiRestaurant_BuyOrders Orden;
            public List<Carppi_ProductosPorRestaurantes> Productos;

        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage RegisterUserProfile(string Face_identifier_2, string Nombre_usuario, string FirstName, string LastName, string FirebaseToken)
        {
            //ToDo: aqui faltan atributos al tipo del usuario, por favor, actualiza la db

            var uri = "https://graph.facebook.com/" + Face_identifier_2 + "/picture?type=large";

            //System.Net.WebRequest request = System.Net.WebRequest.Create(uri);  //  System.Net.WebRequest.CreateDefault();
            //System.Net.WebResponse response = request.GetResponse();
            //System.IO.Stream responseStream = response.GetResponseStream();
            //var arra = ReadFully(responseStream);

            //WebResponse myResp = response.GetResponse();
            var Validation = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor.Contains(Face_identifier_2)).FirstOrDefault();
            if (Validation == null)
            {
                var UserType = new CarppiGrocery_Repartidores();
                UserType.FaceID_Repartidor = Face_identifier_2;
                UserType.Latitud = 0.0;
                UserType.Longitud = 0.0;
                UserType.BuyOrderID = 0;
                UserType.Nombre = FirstName;
                UserType.Apellido = LastName;
                UserType.FirebaseID = FirebaseToken;
                //// UserType.FirebaseID = "";
                // UserType.StripeDeliverID = "";
                ///UserType.Foto_repartidor = "";

                db.CarppiGrocery_Repartidores.Add(UserType);
                db.SaveChanges();
            }
            else
            {
                Validation.FaceID_Repartidor = Face_identifier_2;
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
        public HttpResponseMessage UpdateeDeliveryBoyByStateAndCountry(string Town, string Country, string State, string FacebookID_UpdateArea)
        {
            var New_Area = db.CarppiGrocery_Regiones.Where(x => x.Pais.Contains(Country) && x.Estado.Contains(State) && x.Ciudad.Contains(Town)).FirstOrDefault();

            var IsInRegion = false;
            long Region = 0;
            var ReginObject = new CarppiGrocery_Regiones();

            if (New_Area != null)
            {
                if (FacebookID_UpdateArea != "")
                {
                    var USer = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == FacebookID_UpdateArea).FirstOrDefault();
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

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ActualizaLocalizacionYToken(string user5, double Latitud, double Longitud, string token)
        {


            var obj1 = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == user5).FirstOrDefault();
            obj1.Latitud = Latitud;
            obj1.Longitud = Longitud;
            obj1.FirebaseID = token;
            db.SaveChanges();

            var ordenes = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == user5);
            var ClienteMasCercano = new CarppiRestaurant_BuyOrders();
            var RestauranteMasCercano = new CarppiRestaurant_BuyOrders();
            double distanciacliente = 10000;
            double distanciaRestaureante = 10000;
            foreach (var orden in ordenes)
            {
                var distClienteTemporal = ManhattanDisane(Latitud, Longitud, Convert.ToDouble(orden.LatitudPeticion), Convert.ToDouble(orden.LongitudPeticion));
                var distRestaurantTemporal = ManhattanDisane(Latitud, Longitud, Convert.ToDouble(orden.LatitudRestaurante), Convert.ToDouble(orden.LongitudRestaurante));

                if (distClienteTemporal < distanciacliente)
                {
                    ClienteMasCercano = orden;
                    distanciacliente = distClienteTemporal;
                }

                if (distRestaurantTemporal < distanciaRestaureante)
                {
                    RestauranteMasCercano = orden;
                    distanciaRestaureante = distRestaurantTemporal;
                }

            }

            var nuevabusquedarepartidora = new BusquedaRepartidor(Latitud, Longitud, 2);
            if (nuevabusquedarepartidora.IsInThisRegion(Convert.ToDouble(ClienteMasCercano.LatitudPeticion), Convert.ToDouble(ClienteMasCercano.LongitudPeticion)))
            {
                if (ClienteMasCercano.ReparidorEnElAreaDelCliente == false && ClienteMasCercano.Stat == (int)GroceryOrderState.RequestGoingToClient)
                {
                    var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == ClienteMasCercano.UserID).FirstOrDefault();
                    Push("El repartidor esta llegando hacia ti", "Corre a recibir el pedido", cliente.FirebaseID, "");
                    ClienteMasCercano.ReparidorEnElAreaDelCliente = true;
                    db.SaveChanges();
                }
            }
            if (nuevabusquedarepartidora.IsInThisRegion(Convert.ToDouble(RestauranteMasCercano.LatitudRestaurante), Convert.ToDouble(RestauranteMasCercano.LongitudRestaurante)))
            {
                if (RestauranteMasCercano.ReparidorEnElAreaDelRestaurante == false && RestauranteMasCercano.Stat == (int)GroceryOrderState.RequestBeingAttended)
                {
                    var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == RestauranteMasCercano.RestaurantHash).FirstOrDefault();
                    Push_Restaurante("El repartidor ha llegado", "El repartidor va a llegar, ten lista sus ordenes", Restaurante.FirebaseID, "");
                    RestauranteMasCercano.ReparidorEnElAreaDelRestaurante = true;
                    db.SaveChanges();
                }

            }

            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }

        //  public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected, RequestDeliverManInRestaurantSpot };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage ActualizaLocalizacion(string user5, double Latitud, double Longitud)
        {


            var obj1 = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == user5).FirstOrDefault();
            obj1.Latitud = Latitud;
            obj1.Longitud = Longitud;
            db.SaveChanges();

            var ordenes = db.CarppiRestaurant_BuyOrders.Where(x=> x.FaceIDRepartidor_RepartidorCadena == user5);
            var ClienteMasCercano = new CarppiRestaurant_BuyOrders();
            var RestauranteMasCercano = new CarppiRestaurant_BuyOrders();
            double distanciacliente = 10000;
            double distanciaRestaureante = 10000;
            foreach (var orden in ordenes)
            {
                var distClienteTemporal = ManhattanDisane(Latitud, Longitud,Convert.ToDouble( orden.LatitudPeticion), Convert.ToDouble(orden.LongitudPeticion));
                var distRestaurantTemporal = ManhattanDisane(Latitud, Longitud, Convert.ToDouble(orden.LatitudRestaurante), Convert.ToDouble(orden.LongitudRestaurante));

                if(distClienteTemporal < distanciacliente)
                {
                    ClienteMasCercano = orden;
                    distanciacliente = distClienteTemporal;
                }

                if (distRestaurantTemporal < distanciaRestaureante)
                {
                    RestauranteMasCercano = orden;
                    distanciaRestaureante = distRestaurantTemporal;
                }

            }

            var nuevabusquedarepartidora =new BusquedaRepartidor(Latitud, Longitud, 2);
           if( nuevabusquedarepartidora.IsInThisRegion(Convert.ToDouble(ClienteMasCercano.LatitudPeticion), Convert.ToDouble(ClienteMasCercano.LongitudPeticion)))
            {
                if (ClienteMasCercano.ReparidorEnElAreaDelCliente == false && ClienteMasCercano.Stat == (int)GroceryOrderState.RequestGoingToClient)
                {
                    var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == ClienteMasCercano.UserID).FirstOrDefault();
                    Push("El repartidor esta llegando hacia ti", "Corre a recibir el pedido", cliente.FirebaseID, "");
                    ClienteMasCercano.ReparidorEnElAreaDelCliente = true;
                    db.SaveChanges();
                }
            }
            if (nuevabusquedarepartidora.IsInThisRegion(Convert.ToDouble(RestauranteMasCercano.LatitudRestaurante), Convert.ToDouble(RestauranteMasCercano.LongitudRestaurante)))
            {
                if (RestauranteMasCercano.ReparidorEnElAreaDelRestaurante == false && RestauranteMasCercano.Stat == (int)GroceryOrderState.RequestBeingAttended)
                {
                    var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == RestauranteMasCercano.RestaurantHash).FirstOrDefault();
                    Push_Restaurante("El repartidor ha llegado", "El repartidor va a llegar, ten lista sus ordenes", Restaurante.FirebaseID, "");
                    RestauranteMasCercano.ReparidorEnElAreaDelRestaurante = true;
                    db.SaveChanges();
                }

            }

            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
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
        public Double ManhattanDisane(double LatOrige, double longOriges, double LatDestino, double LongDestiono)
        {
            return Math.Abs(LatOrige - LatDestino) + Math.Abs(longOriges - LongDestiono);
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



    }
}
