using CarppiWebService.Models;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace GeoLoc.Controllers
{
    public class SMSNexmoController : Controller
    {

        PidgeonEntities db = new PidgeonEntities();
        // GET: SMSNexmo
        public ActionResult Index()
        {
            return View();
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult Receive([FromUri]SMS.SMSInbound response)
        {
            byte[] data = Convert.FromBase64String(response.text);
            string requestBody = Encoding.UTF8.GetString(data);
            if (requestBody.Contains("Localizacion"))
            {
                try
                {
                    string[] miembros = requestBody.Split(',');

                    string dispositivo = miembros[0];//.Split(':')[1];

                    dispositivo = response.msisdn;
                    //string hora = miembros[1];//.Split(':')[1];
                    string lat = miembros[1];//.Split(':')[1];
                    string log = miembros[2];//.Split(':')[1];
                    string saldo = miembros[3];//.Split(':')[1];
                    string Hora = miembros[3];
                    string velocidad = "";
                    try
                    {
                        velocidad = miembros[4];
                    }
                    catch (Exception)
                    {

                    }
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
                            disp.Fecha_Activacion = DateTime.UtcNow;
                            disp.Fecha_Fin_Contrato = DateTime.UtcNow;
                            disp.Hora = Hora;
                            disp.Latitud = lat;
                            disp.Longitud = log;
                            disp.Saldo = saldo;
                            db.Dispositivos.Add(disp);
                            db.SaveChanges();
                        }
                        else
                        {
                            if(query.Vigente == true)
                            {

                                query.Hora = Hora;
                                query.Latitud = lat;
                                query.Longitud = log;
                                query.Velocidad = velocidad;


                                Trayectorias_geoloc traye = new Trayectorias_geoloc();

                                traye.Latitud = lat;
                                traye.Longitud = log;
                                traye.fecha = DateTime.UtcNow;
                                traye.Dispositivo = dispositivo;
                                db.Trayectorias_geoloc.Add(traye);

                                ///////////////
                                //borra extras
                                /////////////////
                                var trap = db.Trayectorias_geoloc.Where(x => x.Dispositivo.Contains(dispositivo)).ToArray();

                                if (trap.Count() > 50)
                                {
                                    int conteo_al_revez = 0;
                                    for (int cuenta = trap.Count(); cuenta > 0; cuenta--)
                                    {
                                        if (conteo_al_revez > 50)
                                        {
                                            db.Trayectorias_geoloc.Remove(trap[cuenta]);
                                        }
                                        conteo_al_revez++;
                                    }
                                }


                                db.SaveChanges();

                            }
                            
                        }

                    }
                    return new HttpStatusCodeResult(200);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(200);
                }

            }
            else if (requestBody.Contains("$"))
            {
                try
                {
                    string[] miembros = requestBody.Split('$');


                    var semi_cade = miembros[1];
                    var cad = semi_cade.Split(' ')[0];


                    var dispositivo = response.msisdn;
              
                    var query = db.Dispositivos.Where(x => x.Dispositivo == dispositivo).FirstOrDefault();
                    query.Saldo = cad;
                    db.SaveChanges();
                    return new HttpStatusCodeResult(200);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(200);
                }
            }
            else
            {
                return new HttpStatusCodeResult(200);
            }
            //uys8FeMDGygTNTlmQNq6e7HuognzfllS4Tj9uu868dhBsceubqctDEtzQAJx
         
         
        }

    }
}