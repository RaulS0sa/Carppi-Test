using CarppiWebService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CarppiWebService.Controllers
{
    public class PositionRequestController : ApiController
    {

        PidgeonEntities db = new PidgeonEntities();
        [HttpGet]
        public HttpResponseMessage Peticion(string numero)
        {
            var query = db.Dispositivos.Where(x=> x.Dispositivo.Contains(numero)).FirstOrDefault();
            if (query != null)
            {
                dynamic jsonObject = new JObject();
                jsonObject.Latitud = query.Latitud;
                jsonObject.Longitud = query.Longitud;
                jsonObject.Hora = query.Hora;
                jsonObject.Tipo = "Normal";
                jsonObject.Vigente = query.Vigente;
                jsonObject.Fin_Vigencia = query.Fecha_Fin_Contrato;
                jsonObject.Saldo = query.Saldo;
                jsonObject.Velocida = query.Velocidad;
                string yourJson_ = JsonConvert.SerializeObject(jsonObject);// jsonObject;
                var response_ = Request.CreateResponse(HttpStatusCode.Accepted);
                response_.Content = new StringContent(yourJson_, Encoding.UTF8, "application/json");
                return response_;
              
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Ya Existente");
            }
        }

        [HttpGet]
        public HttpResponseMessage Trayectorias(string numero1)
        {

            var query = db.Trayectorias_geoloc.Where(x => x.Dispositivo.Contains(numero1));

            List<Trayectorias_geoloc> traye = new List<Trayectorias_geoloc>();

           
            if (query != null)
            {

                foreach (var elem in query)
                {
                    traye.Add(elem);
                }
                dynamic jsonObject = new JObject();

                string yourJson_ = JsonConvert.SerializeObject(traye);// jsonObject;
                var response_ = Request.CreateResponse(HttpStatusCode.Accepted);
                response_.Content = new StringContent(yourJson_, Encoding.UTF8, "application/json");
                return response_;

            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Null");
            }
        }

        [HttpGet]
        public HttpResponseMessage Actualiza_pago(string Device)
        {

            var query = db.Dispositivos.Where(x => x.Dispositivo.Contains(Device)).FirstOrDefault();
            query.Vigente = true;
            DateTime dap = DateTime.UtcNow;
            try
            {
                dap = Convert.ToDateTime(query.Fecha_Fin_Contrato);
            }
            catch (Exception) { }

          
                dap = dap.AddMonths(1);
                query.Fecha_Fin_Contrato = dap;
                db.SaveChanges();
           // }
                return Request.CreateResponse(HttpStatusCode.Accepted, "Null");
            
        }


    }
}
