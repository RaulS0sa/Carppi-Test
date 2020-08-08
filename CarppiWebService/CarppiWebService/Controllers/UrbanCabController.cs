using CarppiWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarppiWebService.Controllers
{

    public class UrbanCabController : ApiController
    {
        PidgeonEntities db = new PidgeonEntities();
        public enum EstradosDeLosConductores { SinActividad, ViajandoHaciaPasajero, ViajandoADestino };
        public enum EstradosDeLosPasajeros { SinActividad, DecidiendoPrecio, buscandoViaje, Esperandoconductor, Viajando };


        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage RegisterUserProfile(string Face_identifier_2, bool EsConductor, string Nombre_usuario)
        {
            //ToDo: aqui faltan atributos al tipo del usuario, por favor, actualiza la db

            //var uri = "https://graph.facebook.com/" + Face_identifier_2 + "/picture?type=large";

            //System.Net.WebRequest request = System.Net.WebRequest.Create(uri);  //  System.Net.WebRequest.CreateDefault();
            //System.Net.WebResponse response = request.GetResponse();
            //System.IO.Stream responseStream = response.GetResponseStream();
            //var arra = ReadFully(responseStream);

            //WebResponse myResp = response.GetResponse();
            // var Validation = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Face_identifier_2).FirstOrDefault();

            if (EsConductor == true)
            {
                var Validation = db.UrbanCabDrivers.Where(x => x.FaceID == Face_identifier_2).FirstOrDefault();
                if (Validation == null)
                {
                    var NewDriver = new UrbanCabDrivers();
                    NewDriver.Calificacion = "4";
                  //  NewDriver.clabe = 0;
                   // NewDriver.ClienteID = 0;
                   // NewDriver.Correo = 0;
                   // NewDriver.Correo_validado = 0;
                    NewDriver.Estado = (int)(EstradosDeLosConductores.SinActividad);
                    NewDriver.FaceID = Face_identifier_2;
                   // NewDriver.Foto = 0;
                    NewDriver.Identidad_validada = false;
                //    NewDriver.Latitud = 0;
                //    NewDriver.Longitud = 0;
                //    NewDriver.Marca = 0;
                //    NewDriver.Matricula = 0;
                //    NewDriver.Modelo = 0;
                    NewDriver.Nombre = Nombre_usuario;
              //      NewDriver.Numero_validado = 0;
              //      NewDriver.OpenPayID = 0;
              //      NewDriver.StripeID = 0;
              //      NewDriver.Telefono = 0;
                    NewDriver.AvailableToDrive = false;
                    db.UrbanCabDrivers.Add(NewDriver);


                }
                else
                {
                    Validation.AvailableToDrive = false;
                    Validation.Identidad_validada = false;
                }
                db.SaveChanges();
            }
            else
            {

                var Validation = db.UrbanCabPassengers.Where(x => x.FaceID == Face_identifier_2).FirstOrDefault();
                if (Validation == null)
                {
                    var NewDriver = new UrbanCabPassengers();
                    NewDriver.Calificacion = "4";
              //      NewDriver.clabe = 0;
              //      NewDriver.ClienteID = 0;
                  //  NewDriver.Correo = 0;
                  //  NewDriver.Correo_validado = 0;
                    NewDriver.Estado = (int)(EstradosDeLosPasajeros.SinActividad);
                    NewDriver.FaceID = Face_identifier_2;
                  //  NewDriver.Foto = 0;
                    NewDriver.Identidad_validada = false;
                //    NewDriver.Latitud = 0;
                //    NewDriver.Longitud = 0;
                   // NewDriver.Marca = 0;
                   // NewDriver.Matricula = 0;
                   // NewDriver.Modelo = 0;
                    NewDriver.Nombre = Nombre_usuario;
                    //    NewDriver.Numero_validado = 0;
                    //    NewDriver.OpenPayID = 0;
                    //    NewDriver.StripeID = 0;
                    //    NewDriver.Telefono = 0;
                    //  NewDriver.AvailableToDrive = 0;
                    db.UrbanCabPassengers.Add(NewDriver);


                }
                db.SaveChanges();
            }
            
            //StreamReader reader = new StreamReader(response.GetResponseStream());
            //var aa = reader.ReadToEnd();

            //var cadena =Convert.ToBase64String(GetStreamWithAuthAsync(Face_identifier_2));

            return Request.CreateResponse(HttpStatusCode.Accepted, "Aceptado");


        }




    }
}
