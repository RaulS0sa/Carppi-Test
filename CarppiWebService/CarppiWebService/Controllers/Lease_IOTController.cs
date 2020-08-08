using CarppiWebService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
//using static GeoLoc.Controllers.CarRentController;
using static CarppiWebService.Controllers.CarRentController;

namespace CarppiWebService.Controllers
{

 
    public class Lease_IOTController : ApiController
    {
        PidgeonEntities db = new PidgeonEntities();

        [HttpGet]
        public string Consume_Instruccion_Interruptor(string casa, string cuarto, string indice, string usuario)
        {
            //http.begin("https://geolocale.azurewebsites.net/api/Lease_IOT/Consume_Instruccion_Interruptor?casa=1&cuarto=1&indice=1&usuario=10217507355986566"); //HTTP

            var objeto = db.Lease_Interrutores.Where(x => x.Casa == casa && x.Habitacion == cuarto && x.Indice == indice && x.Usuario == usuario).FirstOrDefault();
            if(objeto == null)
            {
                Lease_Interrutores interruptor = new Lease_Interrutores();
                interruptor.Casa = casa;
                interruptor.Habitacion = cuarto;
                interruptor.Indice = indice;
                interruptor.Alias = "";
                interruptor.Instruccion = false;
                interruptor.Usuario = usuario;

                db.Lease_Interrutores.Add(interruptor);
                db.SaveChanges();
            }
            try
            {
                if (Convert.ToBoolean(objeto.Instruccion))
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception)
            {
                return "0";
            }



        }
        [HttpGet]
        public string Setea_Instruccion_Interruptor(string casa, string habitacion,string indice ,string usuario, string instruccion)
        {
            //  http.begin("https://geolocale.azurewebsites.net/api/Lease_IOT/Setea_Instruccion_Interruptor?casa=1&habitacion=1&indice=1&usuario=10217507355986566&instruccion=1"); //HTTP

            var objeto = db.Lease_Interrutores.Where(x => x.Casa == casa && x.Habitacion == habitacion && x.Indice == indice && x.Usuario == usuario).FirstOrDefault();
            try
            {
                var comando = instruccion == "1" ? true : false;
                objeto.Instruccion = comando;
                db.SaveChanges();
                return "0";
            }
            catch (Exception)
            {
                return "0";
            }


        }



        [HttpGet]
        public string Setea_Instruccion_Dimmer(string Hogar, string habitacion, string Interruptor, string usuario, string instruccion, int Es_dimmer)
        {
            //  http.begin("https://geolocale.azurewebsites.net/api/Lease_IOT/Setea_Instruccion_Dimmer?Hogar=1&habitacion=1&Interruptor=1&usuario=10217507355986566&instruccion=1&Es_dimmer=1"); //HTTP

            bool Dim = Convert.ToBoolean(Es_dimmer);
            var objeto = db.Lease_Interrutores.Where(x => x.Casa == Hogar && x.Indice == Interruptor && x.Es_Dimmer == Dim).FirstOrDefault();
            try
            {
                if(objeto != null)
                {
                    objeto.Porcentaje_dimming = Convert.ToInt32(instruccion);
                }
               // var comando = instruccion == "1" ? true : false;
               // objeto.Instruccion = comando;
                db.SaveChanges();
                return "0";
            }
            catch (Exception)
            {
                return "0";
            }


        }


        [HttpGet]
        public HttpResponseMessage Consume_Instruccion_Dimmer(string Hogar,  string Interruptor, int Es_Dimmer)
        {
            //http.begin("https://geolocale.azurewebsites.net/api/Lease_IOT/Consume_Instruccion_Dimmer?Hogar=1&cuarto=1&Interruptor=80:7D:3A:6E:83:A3&Es_Dimmer=1"); 
            bool dim = Convert.ToBoolean(Es_Dimmer);
            var objeto = db.Lease_Interrutores.Where(x => x.Casa == Hogar && x.Indice == Interruptor && x.Es_Dimmer == dim).FirstOrDefault();
            if (objeto == null)
            {
                Lease_Interrutores interruptor = new Lease_Interrutores();
                interruptor.Casa = Hogar;
                interruptor.Habitacion = "1";
                interruptor.Indice = Interruptor;
                interruptor.Alias = "";
                interruptor.Instruccion = false;
                interruptor.Es_Dimmer = true;
                interruptor.Porcentaje_dimming = 0;
                //interruptor.Usuario = usuario;

                db.Lease_Interrutores.Add(interruptor);
                db.SaveChanges();
            }
            try
            {
                //return Convert.ToString(Convert.ToInt32(objeto.Porcentaje_dimming));
                var response_ = Request.CreateResponse(HttpStatusCode.OK);
                response_.Content = new StringContent(Convert.ToString(Convert.ToInt32(objeto.Porcentaje_dimming)), Encoding.UTF8, "application/json");
                return response_;

                //return response_;
                //return Convert.ToInt32(objeto.Porcentaje_dimming);
                //    if (Convert.ToBoolean(objeto.Instruccion))
                //    {
                //        return "1";
                //    }
                //    else
                //    {
                //        return "0";
                //    }
                //}
            }
            catch (Exception)
            {
                var response_ = Request.CreateResponse(HttpStatusCode.OK);
                response_.Content = new StringContent(Convert.ToString(Convert.ToInt32(objeto.Porcentaje_dimming)), Encoding.UTF8, "application/json");
                return response_;
                //return 0;
            }
           // var response_ = Request.CreateResponse(HttpStatusCode.Accepted, 0);


        }


        public async Task<HttpResponseMessage> Post_PublicacionRenta()
        {
            // Check whether the POST operation is MultiPart?
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
            // data will be loaded.
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();


            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                var result = await Request.Content.ReadAsMultipartAsync(provider);

                var cccc = result.Contents.FirstOrDefault().ReadAsAsync<Publicacion>().Result;
                Lease_Publicaciones publica = new Lease_Publicaciones();
                publica.Perfil_de_publicacion = cccc.Perfil_de_publicacion;//Clases.ClasContainer.Identidad_de_usuario;
                publica.Direccion = cccc.Direccion;
                publica.Precio = cccc.Precio;
                publica.Cuartos = cccc.Cuartos;
                publica.Banos = cccc.Banos;


                //info.Id_de_publicacion = Math.Round(rand.NextDouble() * 10000).ToString();
                publica.Latitud = cccc.Latitud;
                publica.Longitud = cccc.Longitud;

                db.Lease_Publicaciones.Add(publica);
                db.SaveChanges();
                int ID = publica.ID;

                for (int i = 1; i < result.Contents.Count; i++)
                {
                    MultipartFileData file = provider.FileData[i];
                    var acapulco = File.ReadAllBytes(file.LocalFileName);
                    //files.Add(Path.GetFileName(file.LocalFileName));
                    Lease_FotosPublicacion foto = new Lease_FotosPublicacion();

                    //foto.Id_de_publicacion = cccc.Id_de_publicacion;
                    //foto.Foto = file.LocalFileName;
                    foto.Foto = acapulco;
                    foto.Id_de_la_publicacion = ID;
                    db.Lease_FotosPublicacion.Add(foto);
                    db.SaveChanges();
                }

                
                



                // Send OK Response along with saved file names to the client.
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet]
        public HttpResponseMessage Busca_Casa(string Latitud, string Longitud)
        {
            try
            {
               
                var query = db.Lease_Publicaciones.Where(x => x.Latitud.Contains(Latitud) && x.Longitud.Contains(Longitud));
                List<Publicacion_Retorno> retorno = new List<Publicacion_Retorno>();

                //List<String> Lista_arrendatario = new List<string>();
                foreach (var casa in query)
                {
                    var query_imagenes = db.Lease_FotosPublicacion.Where(x => x.Id_de_la_publicacion == casa.ID);
                    List<string> bases64 = new List<string>();
                    foreach (var direccion in query_imagenes)
                    {
                        var foto = Convert.ToBase64String(direccion.Foto);
                        //var cadena = direccion.Foto.Split('/');

                        // var cadena_nueva = "~/" + cadena[cadena.Length - 2] + "/" + cadena[cadena.Length - 1];
                        //var path = System.Web.Hosting.HostingEnvironment.MapPath(cadena_nueva);
                        //string path = Server.MapPath("~/App_Data/Sample.jpg");
                        //byte[] imageByteData = System.IO.File.ReadAllBytes(direccion.Foto);
                        //string imageBase64Data = Convert.ToBase64String(imageByteData);
                        string imageDataURL = string.Format("data:image/png;base64,{0}", foto);
                        bases64.Add(imageDataURL);


                        Publicacion_Retorno ret = new Publicacion_Retorno
                        {
                            Direccion = casa.Direccion,
                            Precio = casa.Precio,
                            Imagen = string.Format("data:image/png;base64,{0}", foto)
                        };
                    retorno.Add(ret);
                    }
                    //Lista_arrendatario.Add(coche.Arrendatario);
                }
                dynamic jsonObject = new JObject();
                //jsonObject.Resultados = String.Join("|", Lista_arrendatario.ToArray());
                string yourJson_ = JsonConvert.SerializeObject(retorno, Formatting.Indented);
                //string yourJson_ = JsonConvert.SerializeObject(jsonObject);// jsonObject;
                var response_ = Request.CreateResponse(HttpStatusCode.Accepted);
                response_.Content = new StringContent(yourJson_, Encoding.UTF8, "application/json");
                return response_;
            }
            catch (Exception e)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }



    }
    class Publicacion
    {
        
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
	    public string Precio { get; set; }
	    public string Cuartos { get; set; }
        public string Banos { get; set; }
	    public bool Ocupacion { get; set; }
        public string Perfil_de_publicacion { get; set; }
    }
    class Publicacion_Retorno
    {

        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Precio { get; set; }
        public string Cuartos { get; set; }
        public string Banos { get; set; }
        public bool Ocupacion { get; set; }
        public string Perfil_de_publicacion { get; set; }
        public string Imagen { get; set; }
    }
}
