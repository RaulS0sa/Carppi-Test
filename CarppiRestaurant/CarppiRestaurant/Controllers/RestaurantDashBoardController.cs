using CarppiRestaurant.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        PidgeonEntities db = new PidgeonEntities();
        // GET: RestaurantDashBoard
        public ActionResult Index()
        {
            //https://dashboard.stripe.com/express/oauth/authorize?redirect_uri=https://carppirestaurant.azurewebsites.net&response_type=code&client_id=ca_HefGcaqsN1stYmIBiakDDfvPcpEIfEk6&scope=read_write#/
            //http://metropolitanhost.com/themes/themeforest/angular/costic/order
            Session["RestaurantID"] = "4501fa592738def70c450dcd5320e613bd6811bff9cef49eeb872f5da9c2d13c";
            return View();
        }
        [HttpGet]
        [ActionName("ApiByAction")]
        public JsonResult CarppiProductDetailedView_Compresed(int ProductDetailID_CompressedData)
        {
            var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == ProductDetailID_CompressedData).FirstOrDefault();

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