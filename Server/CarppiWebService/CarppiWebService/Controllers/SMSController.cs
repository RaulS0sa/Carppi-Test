using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.Http;
using Twilio.TwiML;
using CarppiWebService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace CarppiWebService.Controllers
{
    public class SMSController : TwilioController
    // GET: SMS
    
    {

        PidgeonEntities db = new PidgeonEntities();
        [HttpPost]
        public ActionResult Index()
        {
            var requestBody1 = Request.Form["body"];
            byte[] data = Convert.FromBase64String(requestBody1);
            string requestBody = Encoding.UTF8.GetString(data);
            //uys8FeMDGygTNTlmQNq6e7HuognzfllS4Tj9uu868dhBsceubqctDEtzQAJx
            try
            {
                string[] miembros = requestBody.Split(',');
                string dispositivo = miembros[0];//.Split(':')[1];
                //string hora = miembros[1];//.Split(':')[1];
                string lat = miembros[1];//.Split(':')[1];
                string log = miembros[2];//.Split(':')[1];
                string saldo = miembros[3];//.Split(':')[1];
                //var bob = JObject.Parse(requestBody);
                // ClaseDescerializadora.ClaseContainer account = JsonConvert.DeserializeObject<ClaseDescerializadora.ClaseContainer>(requestBody);
                //{"Dispositivo":"123456789","Hora":"11:11","Latitud":"20.09048","Longitud":"-98.369092","Saldo":"10"}

                //string device = bob["Dispositivo"].ToString();// account.Dispositivo;
                var query = db.Dispositivos.Where(x => x.Dispositivo == dispositivo).FirstOrDefault();
                string device = dispositivo;
                if (!String.IsNullOrEmpty(device))
                {
                    if (query == null)
                    {
                        Dispositivos disp = new Dispositivos();
                        disp.Dispositivo = device;
                        disp.Fecha_Activacion = DateTime.Now;
                        disp.Fecha_Fin_Contrato = DateTime.Now.AddDays(365);
                        disp.Hora = "";
                        disp.Latitud = lat;
                        disp.Longitud = log;
                        disp.Saldo = saldo;
                        db.Dispositivos.Add(disp);
                        db.SaveChanges();
                    }
                    else
                    {
                        query.Hora = "";
                        query.Latitud = lat;
                        query.Longitud = log;
                        query.Saldo = saldo;
                        db.SaveChanges();

                    }

                }
                var response1 = new MessagingResponse();
                response1.Message("");
                return TwiML(response1);
            }
            catch (Exception ex) {
                var response2 = new MessagingResponse();
                response2.Message(ex.ToString());
                return TwiML(response2);
            }
            // Console.WriteLine(account.Latitud);
            //const string requestBody = "";//Request.Form["Body"];

            /*
            if (requestBody == "hello")
            {
                response.Message("Hi!");
            }
            else if (requestBody == "bye")
            {
                response.Message("Goodbye");
            }
            */
            var response = new MessagingResponse();
            response.Message("");

           return TwiML(response);

        }
    }
}