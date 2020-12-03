using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CarppiRestaurant.Models;
using Newtonsoft.Json;

namespace CarppiRestaurant.Controllers
{
    public class NewRestaurantController : Controller
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
        public ActionResult Index()
        {
            return View ();
        }

        //url: "/NewRestaurant/Upload_files?Title=" + Title + "&EmailOfRestaurant=" + EmailOfRestaurant + "&PAssWrodOfRestaurant=" + PAssWrodOfRestaurant + "&CategoriesOfRestaurant=" + CategoriesOfRestaurant + "&CategoriesOfBussines=" + CategoriesOfBussines + "",
        [HttpPost]
        public JsonResult Upload_files(string Title, string EmailOfRestaurant, string PAssWrodOfRestaurant, long CategoriesOfRestaurant, string CategoriesOfBussines, string LAtitud, String Logitud, string FirebaseHash)
        {
            var Restaurant = db.Carppi_IndicesdeRestaurantes.Where(x => x.Correo == EmailOfRestaurant).FirstOrDefault();
            if (Restaurant == null)
            {
                try
                {
                    var NewHome = new Carppi_IndicesdeRestaurantes();
                    var Rnd = new Random();
                    var RndNmbr = (Rnd.NextDouble() * (999999 - 100000)) + 100000;
                    try
                    {
                        var Photo = Request.Files[0];
                        NewHome.Foto = ConvertToByteArray_InputStream(Photo);
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
                    var Region = GetUsageRegion(Convert.ToDouble(LAtitud), Convert.ToDouble(Logitud));
                    if(Region == null)
                    {
                        return Json(new { result = "La region de registro no es valida", url = Url.Action("Index", "RestaurantDashBoard") });
                    }
                    NewHome.Ciudad = Region.Ciudad;
                    NewHome.Estado = Region.Estado;
                    NewHome.Pais = Region.Pais;
                    NewHome.Nombre = Title;
                    NewHome.Region = Region.ID;
                    NewHome.Latitud = Convert.ToDouble(LAtitud);
                    NewHome.Longitud = Convert.ToDouble(Logitud);
                    //NewHome.CarppiHash;
                    NewHome.contextualVailidationNumber = (int)RndNmbr;
                    NewHome.Contraseña = PAssWrodOfRestaurant;
                    NewHome.WebsitePasword = PAssWrodOfRestaurant;
                    NewHome.Correo = EmailOfRestaurant;

                    NewHome.FacebookId = "";
                    NewHome.FirebaseID = FirebaseHash;
                    NewHome.RegistroValidado = false;
                    NewHome.TypeOfStore = (int)0;
                    NewHome.VerificacionDecuenta = false;
                    NewHome.EstaAbierto = false;
                    NewHome.Categorias = CategoriesOfBussines;
                    NewHome.IsATestRestaurant = true;
                    NewHome.Categoriasbitfield = ReturnRealBit(CategoriesOfRestaurant);
                    NewHome.TimeZoneID = "Central Standard Time";
                    NewHome.SaturdayOpenningSchedule = "";

                    db.Carppi_IndicesdeRestaurantes.Add(NewHome);
                    db.SaveChanges();
                    NewHome.CarppiHash = ComputeSha256Hash(JsonConvert.SerializeObject(NewHome));

                    Envio_DeCorreo("Identificacion", "Clave: " + (int)RndNmbr, EmailOfRestaurant);
                    Push_Restaurante("Identificacion", "Clave: " + (int)RndNmbr, FirebaseHash, "");


                    db.SaveChanges();
                    Session["RestaurantID"] = NewHome.CarppiHash;

                    return Json(new { result = "Redirect", url = Url.Action("Index", "RestaurantDashBoard") });
                    
                }
                catch (DbEntityValidationException e)
                {
                    var ErrorTexto = "Usuario " + EmailOfRestaurant + '\n';
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
                    return Json(new { result = "Error", url = Url.Action("Index", "RestaurantDashBoard") });
                    //return Request.CreateResponse(HttpStatusCode.NotFound, es.ToString());

                }

            }
            else
            {
                var Rnd = new Random();
                var RndNmbr = (Rnd.NextDouble() * (999999 - 100000)) + 100000;
                Restaurant.FirebaseID = FirebaseHash;
                Restaurant.contextualVailidationNumber = (int)RndNmbr;
                db.SaveChanges();
                Envio_DeCorreo("Identificacion", "Clave: " + (int)RndNmbr, EmailOfRestaurant);
                return Json(new { result = "Cuenta ya creada", url = Url.Action("Index", "RestaurantDashBoard") });
                //return Request.CreateResponse(HttpStatusCode.OK, Restaurant.CarppiHash);
            }


           // return Request.CreateResponse(HttpStatusCode.Conflict, "");


            /*
            try
            {

                var aca2 = Session["FaceID"].ToString();

                var Photo = Request.Files[0];
                var NewHome = new TutoriaTarea();
                NewHome.FaceIdOwner = aca2;
                NewHome.ALaVenta = OnSale;
                NewHome.Calificacion = 5.0;
                NewHome.Descargas = 0;
                NewHome.Descripcion = Description;
                NewHome.Imagen = ConvertToByteArray_InputStream(Photo);
                //request.Amount = new Decimal(Math.Round(cobro, 2,
                //                                 MidpointRounding.ToEven));
                NewHome.Precio = new Decimal(Math.Round(PriceOf, 2,
                                                 MidpointRounding.ToEven));
                NewHome.Tags = TagArray;
                NewHome.Titulo = Title;

                db.TutoriaTareas.Add(NewHome);
                db.SaveChanges();
                for (var i = 1; i < Request.Files.Count; i++)
                {
                    var NewFileToStore = new TutoriaTareasListaDeArchivo();
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    NewFileToStore.Archivo = ConvertToByteArray_InputStream(file); ;
                    NewFileToStore.ExtensionDelArchivo = file.ContentType; ;
                    NewFileToStore.IDdeTarea = NewHome.ID;
                    NewFileToStore.NombreDelArchivo = file.FileName;
                    db.TutoriaTareasListaDeArchivos.Add(NewFileToStore);
                    //db.TutoriTareasListaDeArchivos.Add(NewFileToStore);


                }
                db.SaveChanges();
            }
            catch (Exception)
            {

            }

            try
            {
                //Todo :add Close modal
                var ArrayOfTags = TagArray.Split(',');
                var T_TextUtility = new TutoriTextUtility();
                T_TextUtility.Texto = Title;
                T_TextUtility.ListadeCategorias = TagArray;
                List<TutoriTextUtility> BrandNewList = new List<TutoriTextUtility>() { T_TextUtility };
                var Train = new SearchEngineClass();

                Train.Entrenar(BrandNewList);
                // foreach (var tag in ArrayOfTags)
                //{
                //    var ExistanceOfTag = db.TutoriTagUsuarios.Where(x => x.Categoria == tag).FirstOrDefault();

                //}
            }
            catch (Exception)
            {

            }
            */
            return Json(new { result = "Redirect", url = Url.Action("Index", "RestaurantDashBoard") });

          //  return Json("Uploaded " +0+ " files");

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

        public int ReturnRealBit(long Regularbit)
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
        public CarppiGrocery_Regiones GetUsageRegion(double LatAtribute, double LongAtribute)
        {
            //double TemporalLatitude = Convert.ToDouble(x.Latitud);
            // IEnumerable<double> result =sequence.Where(x => x.HasValue).Select(x => x.Value);
            var RegionenelArea = db.CarppiGrocery_Regiones.Where(x => (Math.Pow(Math.Pow((x.Latitud) - LatAtribute, 2) + Math.Pow((x.Longitud) - LongAtribute, 2), 0.5) / 0.00909) < 15).FirstOrDefault();

           // double? templong = (double?)LongAtribute; 
            //var RegionenelArea = db.CarppiGrocery_Regiones.Where(x => x.ID == 1).FirstOrDefault();
            // var RegionenelArea2 = db.CarppiGrocery_Regiones.Where(x => (Math.Pow(Math.Pow((x.Latitud) - LatAtribute, 2) + Math.Pow((x.Longitud) - templong, 2), 0.5) /0.00909   )  < 15);
            if (RegionenelArea != null)
            {
                return RegionenelArea;
                //return 0;
            }
            else
            {
                return null;
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

                message.Body = "<h4>" + texto + "</h4>";
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


    }
}
