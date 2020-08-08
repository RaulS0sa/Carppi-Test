using CarppiWebService.Models;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
//using System.Web.Mail;
//using System.Net.Mail.SmtpClient;

namespace CarppiWebService.Controllers
{
    public class CarppiIOSRestaurantApiController : ApiController
    {

        PidgeonEntities db = new PidgeonEntities();
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

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SeekForHashInService(string CarppiHashToSeek, string FirebaseTag)
        {//CarppiRestaurant_BuyOrderx.
            var HashUnscaped = Regex.Unescape(CarppiHashToSeek.Replace("\"", ""));
            var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == HashUnscaped).FirstOrDefault();
            if (products != null)
            {
                products.FirebaseID = FirebaseTag;
                db.SaveChanges();
                if (products.RegistroValidado == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Moved, "");
                }


            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "");
            }


            return Request.CreateResponse(HttpStatusCode.NotFound, "");


        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SeekForHashInService(string CarppiHashToSeek)
        {//CarppiRestaurant_BuyOrderx.
            var HashUnscaped = Regex.Unescape(CarppiHashToSeek.Replace("\"", ""));
            var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == HashUnscaped).FirstOrDefault();
          if(products != null)
            { 
                if(products.RegistroValidado == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Moved, "");
                }
            
            
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "");
            }


            return Request.CreateResponse(HttpStatusCode.NotFound, "");


        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage LogError(string ErrorResponse, string EncodedQueryString, string HttpError,string facebookID)
        {
            var message = new MailMessage();
            message.To.Add(new System.Net.Mail.MailAddress("raul.sosa.cortes@gmail.com"));  // replace with valid value 
            message.From = new System.Net.Mail.MailAddress("carppi_mexico@carppi.com.mx");  // replace with valid value
            message.Subject = HttpError;//"MailTest";



            message.Body = "<h4>" + EncodedQueryString + "  Response:"+ ErrorResponse + "   FacebookID:" + facebookID +"</h4>";
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
                
                smtp.Send(message);

            }
            var Raul_Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == "10221568290107381").FirstOrDefault();//10217260473614661
            Push_Repartidor(HttpError, ErrorResponse + "    "  + facebookID,  Raul_Repartidor.FirebaseID, "");
            return Request.CreateResponse(HttpStatusCode.Created, "");

        }

        public enum TypeOfStore { Restaurant, Grocery, Pharmacy };
        [System.Web.Http.AcceptVerbs("GET", "POST", "PUT")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddNewRestaurant(int RegionID,
            double Latitud, 
            double Longitud, 
            TypeOfStore TipoDeTienda, 
            string FaceID,
            string NombreDelNegocio,
            string CiudadArg,
            string EstadoArg,
            string PaisArg,
            string FirebaseHash,
            string Correo,
            string contrasena,
            string Categorias,
            int FoodCategory)
        {
            //FirebaseHash
            RegionID = 1;

            var Restaurant = db.Carppi_IndicesdeRestaurantes.Where(x => x.Correo == Correo).FirstOrDefault();
            if (Restaurant == null)
            {
                try
                {
                    var NewHome = new Carppi_IndicesdeRestaurantes();
                    var Rnd = new Random();
                    var RndNmbr = (Rnd.NextDouble() * (999999 - 100000)) + 100000;
                    try
                    {
                        var httpRequest = HttpContext.Current.Request;
                        var fileCollection = httpRequest.Files;
                        var keycollection = httpRequest.Files.AllKeys;


                        var MMfile = fileCollection[keycollection.FirstOrDefault()];

                        NewHome.Foto = ToByteArray(MMfile.InputStream);
                    }
                    catch(Exception)
                    {  
                    
                    }
                    NewHome.Ciudad = CiudadArg;
                    NewHome.Estado = EstadoArg;
                    NewHome.Pais = PaisArg;
                    NewHome.Nombre = NombreDelNegocio;
                    NewHome.Region = RegionID;
                    NewHome.Latitud = Latitud;
                    NewHome.Longitud = Longitud;
                    //NewHome.CarppiHash;
                    NewHome.contextualVailidationNumber = (int)RndNmbr;
                    NewHome.Contraseña = contrasena;
                    NewHome.Correo = Correo;

                    NewHome.FacebookId = FaceID;
                    NewHome.FirebaseID = FirebaseHash;
                    NewHome.RegistroValidado = false;
                    NewHome.TypeOfStore = (int)TipoDeTienda;
                    NewHome.VerificacionDecuenta = false;
                    NewHome.EstaAbierto = false;
                    NewHome.Categorias = Categorias;
                    NewHome.IsATestRestaurant = true;
                    NewHome.Categoriasbitfield = ReturnRealBit(FoodCategory);

                    db.Carppi_IndicesdeRestaurantes.Add(NewHome);
                    db.SaveChanges();
                    NewHome.CarppiHash = ComputeSha256Hash(JsonConvert.SerializeObject(NewHome));

                    Envio_DeCorreo("Identificacion", "Clave: " + (int)RndNmbr, Correo);
                    Push_Restaurante("Identificacion", "Clave: " + (int)RndNmbr, FirebaseHash, "");


                    db.SaveChanges();


                    return Request.CreateResponse(HttpStatusCode.Created, NewHome.CarppiHash);
                }
                catch (DbEntityValidationException e)
                {
                    var ErrorTexto = "Usuario " + Correo + '\n';
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        ErrorTexto += String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ErrorTexto += String.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    Envio_DeCorreo("Error", ErrorTexto, "Raul.sosa.cortes@gmail.com");
                    throw;
                }
                catch (Exception es)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, es.ToString());

                }

            }
            else
            {
                var Rnd = new Random();
                var RndNmbr = (Rnd.NextDouble() * (999999 - 100000)) + 100000;
                Restaurant.FirebaseID = FirebaseHash;
                Restaurant.contextualVailidationNumber = (int)RndNmbr;
                db.SaveChanges();
                Envio_DeCorreo("Identificacion", "Clave: " + (int)RndNmbr, Correo);
                return Request.CreateResponse(HttpStatusCode.OK,Restaurant.CarppiHash);
            }


            return Request.CreateResponse(HttpStatusCode.Conflict, "");

        }
        
        public int ReturnRealBit(int Regularbit)
        {
            switch (Regularbit)
            {
                case 0:
                    return (Int16)AvailableFoodListing.Hamburgesas;
                case 1:
                    return (Int16)AvailableFoodListing.Guajolotes;
                case 2:
                    return (Int16)AvailableFoodListing.Postres;
                case 3:
                    return (Int16)AvailableFoodListing.Pollo;
                case 4:
                    return (Int16)AvailableFoodListing.Indio;
                case 5:
                    return (Int16)AvailableFoodListing.Americano;
                case 6:
                    return (Int16)AvailableFoodListing.Pizza;
                case 7:
                    return (Int16)AvailableFoodListing.Saludable;
                case 8:
                    return (Int16)AvailableFoodListing.Vegetariano;
                case 9:
                    return (Int16)AvailableFoodListing.Chino;
                case 10:
                    return (Int16)AvailableFoodListing.Continental;
                case 11:
                    return (Int16)AvailableFoodListing.Pastes;
                case 12:
                    return (Int32)AvailableFoodListing.Tacos;
                case 13:
                    return (Int32)AvailableFoodListing.Antojitos;
                case 14:
                    return (Int32)AvailableFoodListing.ComidaCorrida;
                case 15:
                    return (Int32)AvailableFoodListing.SushiYJapones;
                case 16:
                    return (Int32)AvailableFoodListing.BebidasSinAlcohol;
                case 17:
                    return (Int32)AvailableFoodListing.BebidasConAlcohol;
                case 18:
                    return (Int32)AvailableFoodListing.Desayuno;
                case 19:
                    return (Int32)AvailableFoodListing.Chilaquiles;
                default:
                    return 0;


            }
        }
        [HttpGet]
        public HttpResponseMessage RateDeliiverMan(long Order, double Rating, string Coment)
        {
            var Orden = db.CarppiRestaurant_BuyOrders.Where(x => x.ID == Order).FirstOrDefault();
            var repartidor = db.CarppiGrocery_Repartidores.Where(x=> x.FaceID_Repartidor == Orden.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
            repartidor.Rating_double = repartidor.Rating_double == null ? Rating : ((0.65 * repartidor.Rating_double) + (0.35 * Rating));
            repartidor.UltimoComentario = Coment;
            db.SaveChanges();
            db.CarppiRestaurant_BuyOrders.Remove(Orden);
            db.SaveChanges();




            return Request.CreateResponse(HttpStatusCode.OK, "");

        }



        [HttpGet]
        public HttpResponseMessage ValidateSecurityCode_login(int LoginCode, string CarppiHashloginVerification)
        {

            //      var User = db.TutoriUsuarios.Where(x => x.FaceID == FaceBookIDTutoriToAddHomework).FirstOrDefault();
            try
            {


                var restaurant = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == CarppiHashloginVerification).FirstOrDefault();
                if(restaurant.contextualVailidationNumber == LoginCode)
                {
                    restaurant.RegistroValidado = true;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Created, CarppiHashloginVerification);
                }

                
            }
            catch (Exception)
            {

            }



            return Request.CreateResponse(HttpStatusCode.BadRequest, "");

        }


        [HttpGet]
        public HttpResponseMessage ValidateSecurityCode_loginfacebook(int LoginCode, string facebookhash)
        {

            //      var User = db.TutoriUsuarios.Where(x => x.FaceID == FaceBookIDTutoriToAddHomework).FirstOrDefault();
            try
            {


                var restaurant = db.Carppi_IndicesdeRestaurantes.Where(x => x.FacebookId == facebookhash).FirstOrDefault();
                if (restaurant.contextualVailidationNumber == LoginCode)
                {
                    restaurant.RegistroValidado = true;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Created, restaurant.CarppiHash);
                }


            }
            catch (Exception)
            {

            }



            return Request.CreateResponse(HttpStatusCode.BadRequest, "");

        }

        static string ComputeSha256Hash(string rawData)
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
        void Envio_DeCorreo(string Tema, string texto, string CorreoDestinatario)
        {
            var Raul = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == "10217260473614661").FirstOrDefault();//10217260473614661
            var Raul_Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == "10221568290107381").FirstOrDefault();//10217260473614661
            try
            {
              //  string path = Server.MapPath("~/App_Data/Sample.jpg");
             //   byte[] imageByteData = System.IO.File.ReadAllBytes(path);
             //   string imageBase64Data = Convert.ToBase64String(imageByteData);
             //   string imageDataURL = string.Format("data:im  age/png;base64,{0}", imageBase64Data);
              //  ViewBag.ImageData = imageDataURL;


                var TotalAmount = 15.ToString();
                var TotalCard = 15.ToString();
                var TotalCash = 15.ToString();
                var TotalFare = 15.ToString();


                Push("nicio mensaje", "holi", Raul.FirebaseID, "");
                Push_Repartidor("nicio mensaje", "holi", Raul_Repartidor.FirebaseID, ""); var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new System.Net.Mail.MailAddress(CorreoDestinatario));  // replace with valid value 
                message.From = new System.Net.Mail.MailAddress("carppi_mexico@carppi.com.mx");  // replace with valid value
                message.Subject = Tema;

                //string path2 = Server.MapPath("~/App_Data/Receipt.html");
                //var fileContents = System.IO.File.ReadAllText(path2);
                //var r1 = fileContents.Replace("%%%TagForMoney%%%", TotalAmount);
                //r1 = r1.Replace("%%%TagForCard%%%", TotalCard);
                //r1 = r1.Replace("%%%TagForCash%%%", TotalCash);
                //r1 = r1.Replace("%%%TagForFare%%%", TotalFare);

                message.Body = "<h4>" + texto +"</h4>";
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
                Push_Repartidor(ep.ToString(), "holi", Raul_Repartidor.FirebaseID, "");
                Push(ep.ToString(), "Error", Raul.FirebaseID, "");
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
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

        [HttpPost]
        public HttpResponseMessage AddRestaurantItem(int ProductCategory, double CostTotal, string nombre, string descriptcion, string restaurante)
        {

            //      var User = db.TutoriUsuarios.Where(x => x.FaceID == FaceBookIDTutoriToAddHomework).FirstOrDefault();
            try
            {

                var httpRequest = HttpContext.Current.Request;
                var fileCollection = httpRequest.Files;
                var keycollection = httpRequest.Files.AllKeys;


                var MMfile = fileCollection[keycollection.FirstOrDefault()];
                var NewHome = new Carppi_ProductosPorRestaurantes();
                NewHome.Foto = ToByteArray(MMfile.InputStream);
                NewHome.Categoria = ProductCategory;
                NewHome.Costo = CostTotal;
                NewHome.Descripcion = descriptcion;
                NewHome.Nombre = nombre;
                NewHome.IDdRestaurante = restaurante;
                db.Carppi_ProductosPorRestaurantes.Add(NewHome);




                db.SaveChanges();


                return Request.CreateResponse(HttpStatusCode.Created, NewHome.ID);
            }
            catch (Exception)
            {

            }



            return Request.CreateResponse(HttpStatusCode.BadRequest, "");

        }


        [HttpPost]
        public HttpResponseMessage AddNewProduct(int CategoriaDelProducto, double CostTotal, string nombre, string descriptcion, string restaurante)
        {
      
      //      var User = db.TutoriUsuarios.Where(x => x.FaceID == FaceBookIDTutoriToAddHomework).FirstOrDefault();
            try
            {

                var httpRequest = HttpContext.Current.Request;
                var fileCollection = httpRequest.Files;
                var keycollection = httpRequest.Files.AllKeys;

               
                var MMfile = fileCollection[keycollection.FirstOrDefault()];
                var NewHome = new Carppi_ProductosPorRestaurantes();
                NewHome.Foto = ToByteArray(MMfile.InputStream);
                NewHome.Categoria = CategoriaDelProducto;
                NewHome.Costo = CostTotal;
                NewHome.Descripcion = descriptcion;
                NewHome.Nombre = nombre;
                NewHome.IDdRestaurante = restaurante;
                db.Carppi_ProductosPorRestaurantes.Add(NewHome);




                db.SaveChanges();
             

                return Request.CreateResponse(HttpStatusCode.Created, NewHome.ID);
            }
            catch (Exception)
            {

            }



            return Request.CreateResponse(HttpStatusCode.BadRequest, "");

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
            catch(Exception)
            {
                return new byte[] { };
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

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage UpdateFirebaseToken(string FaceID, string FirebaseID, string CarppiHash)
        {
            var usuario = new Carppi_IndicesdeRestaurantes();
            if (FaceID != null)
            {
                 usuario = db.Carppi_IndicesdeRestaurantes.Where(x => x.FacebookId == FaceID).FirstOrDefault();
            }
            else if (CarppiHash != null)
            {
                var HashUnscaped = Regex.Unescape(CarppiHash.Replace("\"", ""));
                usuario = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == HashUnscaped).FirstOrDefault();
            }
            if (usuario.ID > 0) {
                usuario.FirebaseID = FirebaseID;
                db.SaveChanges();
                
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");

        }
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchByTextTest(Int64 ServiceAreaText_Test, string TextMeantToesearched_Test)
        {//CarppiRestaurant_BuyOrderx.
            ServiceAreaText_Test = 1;
            var pop = new FirstResponseToCarppiFood();
            pop.BitfieldForProducts = 5;

            var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.Region == ServiceAreaText_Test && (x.Nombre.ToLower().Contains(TextMeantToesearched_Test) || x.Categorias.ToLower().Contains(TextMeantToesearched_Test))).ToArray();
            List<Carppi_IndicesdeRestaurantes> PromotedProducts = new List<Carppi_IndicesdeRestaurantes>();

            pop.PromotedProducts = products.ToList();


            return Request.CreateResponse(HttpStatusCode.OK, pop);


        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchByText(Int64 ServiceAreaText, string TextMeantToesearched)
        {//CarppiRestaurant_BuyOrderx.
            ServiceAreaText = 1;
            var pop = new FirstResponseToCarppiFood();
            pop.BitfieldForProducts = 5;
            
            var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.Region == ServiceAreaText && (x.Nombre.ToLower().Contains(TextMeantToesearched) || x.Categorias.ToLower().Contains(TextMeantToesearched)) && x.IsATestRestaurant == false && x.EstaAbierto == true).ToArray();
            List<Carppi_IndicesdeRestaurantes> PromotedProducts = new List<Carppi_IndicesdeRestaurantes>();

            pop.PromotedProducts = products.ToList();


            return Request.CreateResponse(HttpStatusCode.OK, pop);


        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchByTag(Int64 ServiceArea, AvailableFoodListing availableFoodListing)
        {//CarppiRestaurant_BuyOrderx.
            ServiceArea = 1;
            var pop = new FirstResponseToCarppiFood();
            pop.BitfieldForProducts = 5;
            var TagToSearch = (int)availableFoodListing;
            var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.Region == ServiceArea && (x.Categoriasbitfield & TagToSearch) == TagToSearch && x.IsATestRestaurant == false && x.EstaAbierto == true).ToArray();
            List<Carppi_IndicesdeRestaurantes> PromotedProducts = new List<Carppi_IndicesdeRestaurantes>();
           
            pop.PromotedProducts = products.ToList();


            return Request.CreateResponse(HttpStatusCode.OK, pop);


        }
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchByTagTest(Int64 ServiceAreaTest, AvailableFoodListing availableFoodListingTest)
        {//CarppiRestaurant_BuyOrderx.
            ServiceAreaTest = 1;
            var pop = new FirstResponseToCarppiFood();
            pop.BitfieldForProducts = 5;
            var TagToSearch = (int)availableFoodListingTest;
            var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.Region == ServiceAreaTest && (x.Categoriasbitfield & TagToSearch) == TagToSearch).ToArray();
            List<Carppi_IndicesdeRestaurantes> PromotedProducts = new List<Carppi_IndicesdeRestaurantes>();

            pop.PromotedProducts = products.ToList();


            return Request.CreateResponse(HttpStatusCode.OK, pop);


        }



        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CarppiRestaurantExistanceDeterminationTest(string CadenadelUsuarioRestaurant_TOTEst)
        {//CarppiRestaurant_BuyOrders
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.UserID == CadenadelUsuarioRestaurant_TOTEst).FirstOrDefault();
            if (Order != null)
            {
                var DeliverYBoy = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == Order.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                //Order.LatitudRepartidor = Convert.ToDouble(DeliverYBoy.Latitud);
                //Order.LongitudRepartidor = Convert.ToDouble(DeliverYBoy.Longitud);
                //if (Order.Stat == (int)GroceryOrderState.RequestEnded)
                //{
                //    db.CarppiRestaurant_BuyOrders.Remove(Order);
                //    Order = null;
                //}
            }

            //var obj1 = db.CarppiGrocery_Productos.Where(x => x.RegionID == Region);
            db.SaveChanges();
            if (Order != null)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, Order);
            }
            else
            {
                var pop = new FirstResponseToCarppiFood();
                pop.BitfieldForProducts = 4101;

                var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID > 0 ).ToArray();
                List<Carppi_IndicesdeRestaurantes> PromotedProducts = new List<Carppi_IndicesdeRestaurantes>();
                ///PromotedProducts.Add(products[0]);
                for (var i = 0; i < products.LongLength && i < 8;i++)
                {
                    PromotedProducts.Add(products[i]);
                }
                pop.PromotedProducts = PromotedProducts;


                return Request.CreateResponse(HttpStatusCode.OK, pop);
                // return Request.CreateResponse(HttpStatusCode.Conflict, "");
            }


        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CarppiRestaurantExistanceDetermination(string CadenadelUsuarioRestaurant)
        {//CarppiRestaurant_BuyOrders
            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.UserID == CadenadelUsuarioRestaurant).FirstOrDefault();
            if (Order != null)
            {
                try
                {
                    var DeliverYBoy = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == Order.FaceIDRepartidor_RepartidorCadena).FirstOrDefault();
                    Order.LatitudRepartidor = Convert.ToDouble(DeliverYBoy.Latitud);
                    Order.LongitudRepartidor = Convert.ToDouble(DeliverYBoy.Longitud);
                }
                catch (Exception)
                { }
                //if (Order.Stat == (int)GroceryOrderState.RequestEnded)
                //{
                //    db.CarppiRestaurant_BuyOrders.Remove(Order);
                //    Order = null;
                //}
            }

            //var obj1 = db.CarppiGrocery_Productos.Where(x => x.RegionID == Region);
            db.SaveChanges();
            if (Order != null)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, Order);
            }
            else
            {
                var pop = new FirstResponseToCarppiFood();
                pop.BitfieldForProducts = 8199;

                var products = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID > 0 && x.IsATestRestaurant == false && x.EstaAbierto == true).ToArray();
                List<Carppi_IndicesdeRestaurantes> PromotedProducts = new List<Carppi_IndicesdeRestaurantes>();
                ///PromotedProducts.Add(products[0]);
                ///
                long Oring = 0;
                for (var i = 0; i < products.LongLength && i < 12; i++)
                {
                    PromotedProducts.Add(products[i]);

                    Oring |= Convert.ToInt64(products[i].Categoriasbitfield);
                }
                pop.BitfieldForProducts = Oring;
                pop.PromotedProducts = PromotedProducts;


                return Request.CreateResponse(HttpStatusCode.OK, pop);
                // return Request.CreateResponse(HttpStatusCode.Conflict, "");
            }


        }
        class FirstResponseToCarppiFood
        {
            public long BitfieldForProducts;
            public List<Carppi_IndicesdeRestaurantes> PromotedProducts;
        }
        public enum enumTipoDePago { Efectivo, Tarjeta };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GeneratePurchaseOrder(string FaceIDOfBuyer, string BuyList, double Lat, double Log, long Region, enumTipoDePago tipoDePago)
        {
            Region = 1;

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
                //    var Busqueda = new BusquedaRepartidor(Lat, Log, Convert.ToInt64(Buyer.Region));
                //    if (Busqueda.Repartidor == null)
                //    {
                //        return Request.CreateResponse(HttpStatusCode.Gone, "0repartidores");
                //    }
                //    else if (Busqueda.DriverHasToomuchOrders == true)
                //    {
                //        return Request.CreateResponse(HttpStatusCode.Moved, "0repartidores");
                //    }
                //if

                var repartidores = db.CarppiGrocery_Repartidores.Where(x => x.Region == Region && x.IsAvailableForDeliver == true).FirstOrDefault();
                if (repartidores == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Gone, "0repartidores");
                }
                {
                    var buyer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfBuyer).FirstOrDefault();
                    var BuyOrder = new CarppiRestaurant_BuyOrders();
                    BuyOrder.LatitudPeticion = Lat;
                    BuyOrder.LongitudPeticion = Log;
                    BuyOrder.RegionID = Region;
                    BuyOrder.Stat = (int)GroceryOrderState.RequestCreated;
                    BuyOrder.UserID = FaceIDOfBuyer;
                    BuyOrder.ListaDeProductos = BuyList;
                    //BuyOrder.FaceIDRepartidor_RepartidorCadena = Busqueda.Repartidor.FaceID_Repartidor;
                    BuyOrder.NombreDelUsuario = buyer.Nombre_usuario;
                    BuyOrder.TipoDePago = ((int)tipoDePago);
                    //BuyOrder.paymentIntent
                    var Decoded = Base64Decode(BuyList);
                    var ListOfItems = JsonConvert.DeserializeObject<List<ShopItem>>(Decoded);
                    var TotalCost = 0.0;

                    foreach (var Item in ListOfItems)
                    {
                        var Produc = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == Item.ItemID).FirstOrDefault();
                        TotalCost += (Convert.ToDouble(Produc.Costo) * Item.Quantity);
                    }
                    TotalCost = TotalCost < 10 ? 10 : TotalCost;

                    BuyOrder.Precio = TotalCost;
                    var firstProductID = ListOfItems.FirstOrDefault().ItemID;
                    var FirstProduc = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == firstProductID).FirstOrDefault();
                    var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == FirstProduc.IDdRestaurante).FirstOrDefault();
                    BuyOrder.RestaurantHash = Restaurante.CarppiHash;
                    BuyOrder.NombreDelRestaurante = Restaurante.Nombre;




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
                    db.CarppiRestaurant_BuyOrders.Add(BuyOrder);
                    db.SaveChanges();

                    //Push_Repartidor("tienes una nueva orden", "Nueva Orden", Busqueda.Repartidor.FirebaseID, "");

                    Push_Restaurante("Tienes una nueva Orden", "Nueva Orden", Restaurante.FirebaseID, "");
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
            }



            return Request.CreateResponse(HttpStatusCode.Accepted, "error");
        }

        public enum enumGender { Male, Female, Genderless };
        public enum enumRoles { Pasajero, Conductor, Observador, RideDriver };
        public enum enumEstado_del_usuario { Sin_actividad, PidiendoViajePasajero, ViajeEncontradoPasajero, ViajandoPasajero, EntregadoPasajero, PidiendoViajeConductor, ViajandoConductor, ViajeAceptadoNoRecogido_Conductor, PasajeroSinArribar, PasajeroDejoPlantado, ViajandoPasajeroRideShare, ConductorEsperando, EsperandoCalificar, BusquedaEnProceso, DisponibleParaRideshare, EsperandoCalificarRideshare, Sin_actividadRideShare, ViajeNoEncontradoUsuario, PasajeroEsperandoconductorRideshare, BusquedaEnProcesoRideShare };
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GeneratePurchaseOrder_ComentariosAndLogin(string FaceIDOfBuyer, string BuyList, double Lat, double Log, long Region, enumTipoDePago tipoDePago, string Comentario, string Correo,string FirebaseID)
        {
            Region = 1;

            StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";

            var Buyer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfBuyer).FirstOrDefault();
            if (Buyer == null)
            {
                var UserType = new Traveler_Perfil();
                UserType.Correo = Correo;
                UserType.Edad = "";
                UserType.Facebook_profile_id ="iOS_" +  FaceIDOfBuyer;
                UserType.Fecha = DateTime.UtcNow;
                UserType.Latitud = "";
                UserType.Latitud_un_decimal = "";
                UserType.Longitud = "";
                UserType.Longitud_un_decimal = "";
                UserType.Nombre_usuario = "";
                UserType.Numero_de_telefono = "";
                UserType.pidiendo_viaje = (int)(enumEstado_del_usuario.Sin_actividad);
                UserType.Resenas = "";
                UserType.Rol = (int)(enumRoles.Pasajero);
                UserType.Stripe_id = "";
                UserType.Telefono_verificado = false;
                UserType.Viaje_asociado = "";
                UserType.TarjetaValidada = false;
                UserType.IdentidadComprobada = false;
                UserType.Telefono = "";
                UserType.Rating_double = 4.0;
                UserType.Gender = (int)enumGender.Male;
                UserType.hasConfirmedGender = false;
                //UserType.FirstName = FirstName;
                //UserType.LastName = LastName;
                UserType.FirebaseID = FirebaseID;

                db.Traveler_Perfil.Add(UserType);
                db.SaveChanges();
                // return Request.CreateResponse(HttpStatusCode.Unauthorized, "NoAutorizado");
            }
            //  else if (Buyer.StripeClientID == null || Buyer.PaymentMethod == null)
            //  {
            //      return Request.CreateResponse(HttpStatusCode.Forbidden, "NoAutorizado");
            //  }
            ///else
            {
                Buyer.FirebaseID = FirebaseID;
                db.SaveChanges();
                //    var Busqueda = new BusquedaRepartidor(Lat, Log, Convert.ToInt64(Buyer.Region));
                var repartidores = db.CarppiGrocery_Repartidores.Where(x => x.Region == Region && x.IsAvailableForDeliver == true).FirstOrDefault();
                var Decoded = Base64Decode(BuyList);
                var ListOfItems = JsonConvert.DeserializeObject<List<ShopItem>>(Decoded);
                var firstProductID = ListOfItems.FirstOrDefault().ItemID;
                var FirstProduc = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == firstProductID).FirstOrDefault();
                var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == FirstProduc.IDdRestaurante).FirstOrDefault();
                var LucianoRestaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == "185e301b73e780a45d769680a3a23dc03cd8c928c5ab884c68f62e24ad4691f0").FirstOrDefault();//10217260473614661
                Push_Repartidor("Pedido a " + Restaurante.Nombre, "Pedido", "drHqtJimmNE:APA91bEj62GnTfWXegWgyEz4pKcl8GzsQmWqULWtjxGCu2EpTHU7KmHevFj2EjYg3Vem836OO96HXBArn5oGMxuP3KedHs5rg_Hseg7KHSYYnww4lcvyC348xSCLjI5qSmU9nW5eZiXX", "");


                
                var FarDistance = (Math.Sqrt(Math.Pow(Convert.ToDouble(Restaurante.Latitud) - Lat, 2) + Math.Pow(Convert.ToDouble(Restaurante.Longitud) - Log, 2)) / 0.00909) > 15;
                if (repartidores == null || FarDistance)
                {
                    return Request.CreateResponse(HttpStatusCode.Gone, "0repartidores");
                }
                else if (Restaurante.EstaAbierto == false)
                {
                    return Request.CreateResponse(HttpStatusCode.Gone, "0repartidores");
                }
                //    else if (Busqueda.DriverHasToomuchOrders == true)
                //    {
                //        return Request.CreateResponse(HttpStatusCode.Moved, "0repartidores");
                //    }
                //if
                {
                    var buyer = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceIDOfBuyer).FirstOrDefault();
                    //buyer.IsFirstOrder = false;
                    var BuyOrder = new CarppiRestaurant_BuyOrders();
                    BuyOrder.LatitudPeticion = Lat;
                    BuyOrder.LongitudPeticion = Log;
                    BuyOrder.RegionID = Region;
                    BuyOrder.Stat = (int)GroceryOrderState.RequestCreated;
                    BuyOrder.UserID = FaceIDOfBuyer;
                    BuyOrder.ListaDeProductos = BuyList;
                    //BuyOrder.FaceIDRepartidor_RepartidorCadena = Busqueda.Repartidor.FaceID_Repartidor;
                    BuyOrder.NombreDelUsuario = buyer.Nombre_usuario;
                    BuyOrder.TipoDePago = ((int)tipoDePago);
                    BuyOrder.Comentario = Comentario;
                    //BuyOrder.paymentIntent
                    //var Decoded = Base64Decode(BuyList);
                    //var ListOfItems = JsonConvert.DeserializeObject<List<ShopItem>>(Decoded);
                    var TotalCost = 0.0;

                    foreach (var Item in ListOfItems)
                    {
                        var Produc = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == Item.ItemID).FirstOrDefault();
                        TotalCost += (Convert.ToDouble(Produc.Costo) * Item.Quantity);
                    }
                    TotalCost = TotalCost < 10 ? 10 : TotalCost;

                    BuyOrder.Precio = TotalCost;
                   // var firstProductID = ListOfItems.FirstOrDefault().ItemID;
                   // var FirstProduc = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == firstProductID).FirstOrDefault();
                   // var Restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == FirstProduc.IDdRestaurante).FirstOrDefault();
                    BuyOrder.RestaurantHash = Restaurante.CarppiHash;
                    BuyOrder.NombreDelRestaurante = Restaurante.Nombre;
                    BuyOrder.TimeOfCreation = (long)(((DateTime.UtcNow - new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);




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
                    db.CarppiRestaurant_BuyOrders.Add(BuyOrder);
                    db.SaveChanges();

                    //Push_Repartidor("tienes una nueva orden", "Nueva Orden", Busqueda.Repartidor.FirebaseID, "");

                    Push_Restaurante("Tienes una nueva Orden", "Nueva Orden", Restaurante.FirebaseID, "");
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
            }



            return Request.CreateResponse(HttpStatusCode.Accepted, "error");
        }

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
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CarppiProductDetailedView(int ProductDetailID)
        {
            var Producto = db.Carppi_ProductosPorRestaurantes.Where(x => x.ID == ProductDetailID).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, Producto);
        }
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage UserUIStateDetermination(string UserChain)
        {

            var Order = db.CarppiRestaurant_BuyOrders.Where(x => x.UserID == UserChain).FirstOrDefault();
            if (Order != null)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, Order);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }

            //var obj1 = db.CarppiGrocery_Productos.Where(x => x.RegionID == Region);
           


        }




        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CarppiRestaurantDetailedViewWithCategories(int RestaurantDetailID_ForCategories)
        {
            var new_rest = new DetailedProductViewFromRestaurantWithCategories();

            var Retaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID == RestaurantDetailID_ForCategories).FirstOrDefault();
            //new_rest.RestaurantData = Retaurante;
            List<long> Indice_deProductos = new List<long>();
            var AlltheProductFromTheRestaurantIndexes = db.Carppi_ProductosPorRestaurantes.Where(x => x.IDdRestaurante == Retaurante.CarppiHash && x.Disponibilidad == true);
            long BitField = 0;
            foreach (var Producto in AlltheProductFromTheRestaurantIndexes)
            {
                //Indice_deProductos.Add(Producto.ID);
                BitField = BitField | Convert.ToInt64(Producto.Categoria);
            }

            var query = AlltheProductFromTheRestaurantIndexes.OrderByDescending(x => x.ComprasDelProducto).ToArray();
            for (var i=0;i< query.Count() && i< 15; i++ )
            {
                Indice_deProductos.Add(query[i].ID);
                
            }



            new_rest.Products = Indice_deProductos;
            new_rest.FoodCategories = BitField;

            return Request.CreateResponse(HttpStatusCode.Accepted, new_rest);

        }
        class DetailedProductViewFromRestaurantWithCategories
        {
            public Carppi_IndicesdeRestaurantes RestaurantData;
            public List<long> Products;
            public long FoodCategories;
        }
        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage SearchInsideRestaurantProducts(int RestaurantDetailID_Regularseller, AvailableFoodListing Bit)
        {
            long BitTOQuery = Convert.ToInt64(Bit);
            var Retaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID == RestaurantDetailID_Regularseller).FirstOrDefault();
            var AlltheProductFromTheRestaurantIndexes = db.Carppi_ProductosPorRestaurantes.Where(x => x.IDdRestaurante == Retaurante.CarppiHash && x.Disponibilidad == true && ((x.Categoria & BitTOQuery)== BitTOQuery) );

            List<long> Indice_deProductos = new List<long>();
            foreach(var producto in AlltheProductFromTheRestaurantIndexes)
            {
                Indice_deProductos.Add(producto.ID);

            }
            return Request.CreateResponse(HttpStatusCode.Accepted, Indice_deProductos);
        }


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage BestSellersRestaurant(int RestaurantDetailID_Bestseller)
        {
            var new_rest = new DetailedProductViewFromRestaurantWithCategories();

            var Retaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID == RestaurantDetailID_Bestseller).FirstOrDefault();
            new_rest.RestaurantData = Retaurante;
            List<long> Indice_deProductos = new List<long>();
            var AlltheProductFromTheRestaurantIndexes = db.Carppi_ProductosPorRestaurantes.Where(x => x.IDdRestaurante == Retaurante.CarppiHash && x.Disponibilidad == true);
          

            var query = AlltheProductFromTheRestaurantIndexes.OrderByDescending(x => x.ComprasDelProducto).ToArray();
            for (var i = 0; i < query.Count() && i < 15; i++)
            {
                Indice_deProductos.Add(query[i].ID);

            }




            return Request.CreateResponse(HttpStatusCode.Accepted, Indice_deProductos);

        }
      


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage CarppiRestaurantDetailedView(int RestaurantDetailID)
        {
            var new_rest = new DetailedProductViewFromRestaurant();

            var Retaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID== RestaurantDetailID).FirstOrDefault();
            new_rest.RestaurantData = Retaurante;
            List<long> Indice_deProductos = new List<long>();
            var AlltheProductFromTheRestaurantIndexes = db.Carppi_ProductosPorRestaurantes.Where(x => x.IDdRestaurante == Retaurante.CarppiHash && x.Disponibilidad == true);
            foreach(var Producto in AlltheProductFromTheRestaurantIndexes)
            {
                Indice_deProductos.Add(Producto.ID);
            }
            new_rest.Products = Indice_deProductos;

            return Request.CreateResponse(HttpStatusCode.Accepted, new_rest);

        }
        class DetailedProductViewFromRestaurant
        {
            public Carppi_IndicesdeRestaurantes RestaurantData;
            public List<long> Products;
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





    }
}
