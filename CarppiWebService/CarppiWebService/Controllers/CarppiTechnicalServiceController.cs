using CarppiWebService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;

namespace CarppiWebService.Controllers
{
    public class CarppiTechnicalServiceController : Controller
    {

        PidgeonEntities db = new PidgeonEntities();
        // GET: CarppiTechnicalService
        public ActionResult Index()
        {
            return View();
        }
        public enum ProductSubset { Verduras_hortalizas, Frutas, Carne_Embutidos_Atun, Queso, Tortillas_Pan, BotanasRefresco, Alcohol, ProductosDeLimpieza, Galletas };
        //   url: "/CarppiTechnicalService/SendPushNotification?Tile=" + Titulo + "&Message=" + Mensaje + "&User=" + Cliente,

        [HttpPost]
        public JsonResult SendPushNotification(string Tile, string Message, string User)
        {
            var usuari = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == User).FirstOrDefault();
            Push(Message, Tile, usuari.FirebaseID, "");
            return Json("");
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
        public JsonResult AddBasicProduct(string Name, double Cost, ProductSubset Categoria)
        {
        

            var Photo = Request.Files[0];
            //  var Perfil = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == aca2).FirstOrDefault();
            //  Perfil.FotoDelVehiculo_ByteArray = ConvertToByteArray_InputStream(Photo);
            var asd = new CarppiGrocery_Productos();
            asd.Costo = Cost;
            asd.Foto = ConvertToByteArray_InputStream(Photo);
            asd.RegionID = 1;
            asd.Producto = Name;
            asd.Categoria = (int)Categoria;
            db.CarppiGrocery_Productos.Add(asd);
            db.SaveChanges();
            return Json("");
        }

        [HttpPost]
        public JsonResult QueryTheDriverProfile(string ProfileID)
        {
            //Session["FaceID"]
            var Driver = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == ProfileID).FirstOrDefault();
            if(Driver != null)
            {
                Session["FaceID"] = ProfileID;
            }

            return Json((Driver));
        }


        [HttpPost]
        public JsonResult UpdateCarPhoto()
        {
            var aca2 = Session["FaceID"].ToString();

            var Photo = Request.Files[0];
            var Perfil = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == aca2).FirstOrDefault();
            Perfil.FotoDelVehiculo_ByteArray = ConvertToByteArray_InputStream(Photo);
            db.SaveChanges();
            return Json("");
        }



        [HttpPost]
        public JsonResult UpdateDriverPhoto()
        {
            var aca2 = Session["FaceID"].ToString();

            var Photo = Request.Files[0];
            var Perfil = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == aca2).FirstOrDefault();
            Perfil.Foto_ByteArray = ConvertToByteArray_InputStream(Photo);
            db.SaveChanges();
            return Json("");
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