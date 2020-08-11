using CarppiWebService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CarppiWebService.Controllers
{

    public class CarRentController : ApiController
    {
        PidgeonEntities db = new PidgeonEntities();
        [HttpGet]
        public HttpResponseMessage Nueva_publicacion(string Usuario, string Latitud, string Longitud, string pueblo, string Latmayor, string longmayor, string latmenor, string longmenor, string modelo, string costo, string descripcion, string clave_de_publicacion, string disponibilidad)
        {//{PlaceEntity{id=ChIJwTcYaEFTn4YRsnI88arEpGI, placeTypes=[1009, 1012], locale=null, name=Mazatlán, 
            //address =Mazatlán, Sin., México, phoneNumber=, latlng=lat/lng: (23.2494148,-106.4111425), viewport=LatLngBounds{southwest=lat/lng: (23.175891699999998,-106.4941638), northeast=lat/lng: (23.317595999999998,-106.35039049999999)}, websiteUri=null, isPermanentlyClosed=false, priceLevel=-1}}

            var query = db.Coches_en_renta.Where(x => x.Arrendatario == clave_de_publicacion).FirstOrDefault();
            int disp = 0;
            if (disponibilidad == "true")
            {
                disp = 1;
            }
            if (query == null)
            {
                Coches_en_renta publicacion = new Coches_en_renta();
                publicacion.Arrendatario = clave_de_publicacion;
                publicacion.Costo = costo;
                publicacion.Disponibilidad = disp;//Convert.ToInt32(disponibilidad);
                publicacion.Latitud =Double.Parse(Latitud);
                publicacion.Longitud = Double.Parse(Longitud);
                publicacion.Modelo = modelo;
                publicacion.Relacion_base_fotografias = clave_de_publicacion;

                db.Coches_en_renta.Add(publicacion);
                db.SaveChanges();
                var query23 = db.Coches_en_renta.Where(x => x.Arrendatario == clave_de_publicacion).FirstOrDefault();


                dynamic jsonObject = new JObject();
                jsonObject.ID_de_publicacion = clave_de_publicacion;
                string yourJson_ = JsonConvert.SerializeObject(jsonObject);// jsonObject;
                var response_ = Request.CreateResponse(HttpStatusCode.Accepted);
                response_.Content = new StringContent(yourJson_, Encoding.UTF8, "application/json");
                return response_;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Ya Existente");
            }
            // return Request.CreateResponse(HttpStatusCode.Unauthorized, "Ya Existente");
        }

        [HttpGet]
        public HttpResponseMessage Subida_de_imagen(string publicacion, string numero_de_foto, string Indicie_de_seccion, string cantidad_de_secciones, string cadena_de_seccion)
        {
            // a♫adir funcion sin subir
            try
            {
                int Indicie = Convert.ToInt32(Indicie_de_seccion);
                int secciones = Convert.ToInt32(cantidad_de_secciones);
                int ID = Convert.ToInt32(publicacion);
                if (Indicie < secciones && Indicie != 1)
                {
                    var queer = db.Fotos_de_coches.Where(x => x.Id_de_publicacion == publicacion).FirstOrDefault();
                    var texto = queer.Foto;
                    texto += cadena_de_seccion;
                    queer.Foto = texto.Replace(" ", string.Empty);
                    db.SaveChanges();

                }
                else if (Indicie == 1)
                {
                    Fotos_de_coches foto = new Fotos_de_coches();
                    foto.Id_de_publicacion = publicacion;
                    foto.Foto = cadena_de_seccion;
                    db.Fotos_de_coches.Add(foto);
                    db.SaveChanges();
                }


                return Request.CreateResponse(HttpStatusCode.Accepted, "Ya Existente");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, ex.ToString());
            }
        }


        public async Task<HttpResponseMessage> Post()
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
                var result =await Request.Content.ReadAsMultipartAsync(provider);

                var cccc = result.Contents.FirstOrDefault().ReadAsAsync<Info>().Result;
                Coches_en_renta publica = new Coches_en_renta();
                publica.Latitud = cccc.Latitud;
                publica.Longitud = cccc.Longitud;
                publica.Arrendatario = cccc.fullname;
                publica.Relacion_base_fotografias = cccc.Id_de_publicacion;
                db.Coches_en_renta.Add(publica);
                db.SaveChanges();

                    

               

                foreach (MultipartFileData file in provider.FileData)
                {
                    
                    files.Add(Path.GetFileName(file.LocalFileName));
                    Fotos_de_coches foto = new Fotos_de_coches();
                    foto.Id_de_publicacion = cccc.Id_de_publicacion;
                    foto.Foto = file.LocalFileName;
                    
                    db.Fotos_de_coches.Add(foto);
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
        public HttpResponseMessage Busca_vehiculo(string Latitud, string Longitud)
        {
            try
            {
                var lat_temp = Double.Parse(Latitud);
                var long_temp = Double.Parse(Longitud);
                var query = db.Coches_en_renta.Where(x => x.Latitud == lat_temp && x.Longitud == long_temp);
                List<Retorno_de_publicaciones> retorno = new List<Retorno_de_publicaciones>();

                //List<String> Lista_arrendatario = new List<string>();
                foreach (var coche in query)
                {
                    var query_imagenes = db.Fotos_de_coches.Where(x => x.Id_de_publicacion == coche.Relacion_base_fotografias);
                    List<string> bases64 = new List<string>();
                    foreach (var direccion in query_imagenes)
                    {
                        var cadena = direccion.Foto.Split('/');

                       // var cadena_nueva = "~/" + cadena[cadena.Length - 2] + "/" + cadena[cadena.Length - 1];
                        //var path = System.Web.Hosting.HostingEnvironment.MapPath(cadena_nueva);
                        //string path = Server.MapPath("~/App_Data/Sample.jpg");
                        byte[] imageByteData = System.IO.File.ReadAllBytes(direccion.Foto);
                        string imageBase64Data = Convert.ToBase64String(imageByteData);
                        string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                        bases64.Add(imageDataURL);

                    }
                    Retorno_de_publicaciones ret = new Retorno_de_publicaciones
                    {
                        Arrendaror = coche.Arrendatario,
                        Ciudad = coche.Arrendatario,
                        Costo = coche.Costo,
                        Imagenes = bases64
                    };
                    retorno.Add(ret);
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

        [HttpGet]
        public HttpResponseMessage descarga_imagen()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Sample.jpg");
            //string path = Server.MapPath("~/App_Data/Sample.jpg");
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            dynamic jsonObject = new JObject();
            jsonObject.Resultados = imageDataURL;
            string yourJson_ = JsonConvert.SerializeObject(jsonObject);// jsonObject;
            var response_ = Request.CreateResponse(HttpStatusCode.Accepted);
            response_.Content = new StringContent(yourJson_, Encoding.UTF8, "application/json");
            return response_;

        }



        // We implement MultipartFormDataStreamProvider to override the filename of File which
        // will be stored on server, or else the default name will be of the format like Body-
        // Part_{GUID}. In the following implementation we simply get the FileName from 
        // ContentDisposition Header of the Request Body.
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
    }
    public class Info
    {
        public String fullname { get; set; }
        public String Id_de_publicacion { get; set; }
        public Double Latitud { get; set; }
        public Double Longitud { get; set; }
    }

    public class Retorno_de_publicaciones
    {
        public String Arrendaror { get; set; }
        public String Costo { get; set; }
        public String Ciudad { get; set; }
        public List<string> Imagenes { get; set; }
    }
}

