using CarppiWebService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarppiWebService.Controllers
{
    public class TCarppiCarpoolApiController : ApiController
    {
        public enum enumRoles { Pasajero, Conductor, Observador, RideDriver };
        public enum enumEstado_del_usuario { Sin_actividad, PidiendoViajePasajero, ViajeEncontradoPasajero, ViajandoPasajero, EntregadoPasajero, PidiendoViajeConductor, ViajandoConductor, ViajeAceptadoNoRecogido_Conductor, PasajeroSinArribar, PasajeroDejoPlantado, ViajandoPasajeroRideShare, ConductorEsperando, EsperandoCalificar, BusquedaEnProceso, DisponibleParaRideshare, EsperandoCalificarRideshare, Sin_actividadRideShare, ViajeNoEncontradoUsuario, PasajeroEsperandoconductorRideshare, BusquedaEnProcesoRideShare };
        public enum enumEstado_de_los_viajes { EnEspera, Aceptado, EnCurso, EsperaDeAbordaje, aceptado_y_llendo_a_ubicacion, Finalizado, Conduciendo, FinalizoLejosDelObjetivo };
        public enum enumEstado_de_Solicitud { EnEspera, Aceptado, Rechazado, Conduciendo, NoLlego };
        public enum enumTipoDePAgoPreferido { Tarjeta, EnEfectivo, Ambos };

        PidgeonEntities db = new PidgeonEntities();

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo, string inicio, string destino)
        {


            var conductor = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceId).FirstOrDefault();
            var Disponible_wallet = false;

            try
            {
                if (conductor != null)
                {
                    Disponible_wallet = conductor.Stripe_id == "" ? false : true;
                }
            }
            catch (Exception) { }



            var cadena = Base64Decode(Argument);
            var aca = JsonConvert.DeserializeObject<Obj_publicacion>(cadena);

            var Traveler_Viajes_ = new Traveler_Viajes();
            Traveler_Viajes_.Asientos_Totales = aca.plazas;
            Traveler_Viajes_.Conductor_a_asociado_su_tarjeta = Disponible_wallet;
            Traveler_Viajes_.asientos_disponibles = aca.plazas;
            Traveler_Viajes_.Destino = destino;
            Traveler_Viajes_.Disponible = true;
            Traveler_Viajes_.En_curso = false;
            Traveler_Viajes_.Estado_del_viaje = (int)enumEstado_de_los_viajes.EnEspera;
            Traveler_Viajes_.FaceIdDelConductor = FaceId;
            Traveler_Viajes_.Fecha_y_hora = DateTime.UtcNow;
            Traveler_Viajes_.Inicio = inicio;
            Traveler_Viajes_.Kilometros = aca.Kilometros;
            Traveler_Viajes_.Latitud = aca.Lugar_Salida_Lat;
            Traveler_Viajes_.Latitud_destino = aca.Lugar_llegada_Lat;
            Traveler_Viajes_.Longitud = aca.Lugar_Salida_Long;
            Traveler_Viajes_.Longitud_destino = aca.Lugar_llegada_Long;
            Traveler_Viajes_.Tiempo_estimado = aca.Duracion;
            Traveler_Viajes_.TimeData = aca.fecha + "^" + aca.time;
            Traveler_Viajes_.Vehiculo = Vehiculo;
            Traveler_Viajes_.Costo_po_Usuario = Costo;
            Traveler_Viajes_.Viaje_Periodico = aca.Periodico;
            try
            {
                var date1 = new DateTime(Convert.ToInt32(aca.Ano), Convert.ToInt32(aca.Mes), Convert.ToInt32(aca.Dia), 8, 30, 52);
                date1.AddDays(1);
                Traveler_Viajes_.DiaDeViaje = date1.Day;
                Traveler_Viajes_.MesDeViaje = date1.Month;
                Traveler_Viajes_.AnoDeViaje = date1.Year;
                Traveler_Viajes_.Viaje_Periodico = aca.ViajePeriodico ? "1" : "0";
                Traveler_Viajes_.TipoDePago = aca.TipoDePago;
                //ToDo:  Set this accordingly to user preference
                Traveler_Viajes_.Visible = true & conductor.IdentidadComprobada;

            }
            catch (Exception)
            {

            }

            db.Traveler_Viajes.Add(Traveler_Viajes_);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");
        }

        public void SearchUrbanTrip(double latitud, double logitud) 
        {
            List<LatLongObj> latLongObjs = new List<LatLongObj>();
            latLongObjs.Add(new LatLongObj(20.594067, -100.3907431));//Universidad Cuautemoc
            //20.594067,-100.3907431 Central de Autobuses
            //20.594067,-100.3907431 La Alameda
            //20.6133155,-100.4074514 tec ded monterrey
            //20.6133155,-100.4227722  Unitec
            //20.6588188,-100.4338014 Antea

        }
        public static string Base64Decode(string base64EncodedData)
        {
            int mod4 = base64EncodedData.Length % 4;
            //if (mod4 > 0)
            //{
            //    base64EncodedData += new string('=', 4 - mod4);
            //}
            //base64EncodedData += "==";
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            var output = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return output;
        }
    }
    class LatLongObj 
    {
        public double Latitud;
        public double Longitud;
        
        public LatLongObj(double Latitud_arg, double Longitud_arg)
        {
            Latitud = Latitud_arg;
            Longitud = Longitud_arg;
        }
    }
}

