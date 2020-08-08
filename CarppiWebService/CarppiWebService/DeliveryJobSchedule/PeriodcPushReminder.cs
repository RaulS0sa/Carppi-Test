using CarppiWebService.Models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;


namespace CarppiWebService.DeliveryJobSchedule
{
    
    public class PeriodcPushReminder : IJob
    {
        PidgeonEntities db = new PidgeonEntities();
        public async Task Execute(IJobExecutionContext context)
        {

            DateTime utc = DateTime.UtcNow;
            
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, zone);
            var Hour = localDateTime.ToString("HH:mm"); // for 24hr format
            var repartidores = db.CarppiGrocery_Repartidores.Where(x => x.Region == 2 && x.IsAvailableForDeliver == true).FirstOrDefault();
          
            if ( (Hour == "17:20" || Hour == "19:35"))
            {
                
                var TextToGenerate = new GeneratePushText().GenerateText();
                var Clientes = db.Traveler_Perfil.Where(x => x.Region == 2);
              ///   PushItAll(TextToGenerate.Texto, TextToGenerate.Titulo, "");
                
                // foreach(var cliente in Clientes)
                // {
                //Push(TextToGenerate, "Carppi", cliente.FirebaseID, "");
                // }
            }

            

            
            // throw new NotImplementedException();
            //BuyOrder.Stat = (int)GroceryOrderState.RequestCreated;
            //var CreatedOrderStat = (int)GroceryOrderState.RequestCreated;
            //var CreatedOrders = db.CarppiRestaurant_BuyOrders.Where(x => x.Stat == CreatedOrderStat);
            //var currentTime = (long)(((DateTime.UtcNow - new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
            //foreach (var orden in CreatedOrders)
            //{
            //    var resta = (currentTime - orden.TimeOfCreation);
            //    if ((currentTime - orden.TimeOfCreation) > 600)
            //    {
            //        var cliente = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == orden.UserID).FirstOrDefault();
            //        Push("Prueba intentando mas tarde", "Tu orden fue rechazada", cliente.FirebaseID, "");
            //        var restaurante = db.Carppi_IndicesdeRestaurantes.Where(x => x.CarppiHash == orden.RestaurantHash).FirstOrDefault();
            //        Push_Restaurante("Una de tus ordenes fue rechazada automaticamente por inactividad", "Orden Rechazada", restaurante.FirebaseID, "");
            //        orden.Stat = (int)GroceryOrderState.RequestRejected;
            //        //db.SaveChanges();
            //        Task.Delay(4000).ContinueWith(t => EraseBuyOrder(orden));

            //    }
            //}

            //await db.SaveChangesAsync();
        }


        class GeneratePushText
        { 
        
            public  GeneratePushText()
            {


                
            }

            public Frase GenerateText()
            {
                var rnd = new Random();
                PidgeonEntities db = new PidgeonEntities();
                var Restaurant = db.Carppi_IndicesdeRestaurantes.Where(x =>  x.EstaAbierto == true && x.IsATestRestaurant == false).ToArray();
                var RandRestindex = (int)(rnd.NextDouble() *( Restaurant.Length -1));
                var RandRestaurantName = Restaurant[RandRestindex].Nombre;
                var RandRestaurantID = Restaurant[RandRestindex].CarppiHash;

                var ProductArray = db.Carppi_ProductosPorRestaurantes.Where(x=> x.IDdRestaurante == RandRestaurantID && x.Disponibilidad == true).ToArray();

                var RandProducindex = (int)(rnd.NextDouble() * (ProductArray.Length - 1));
                var RandRestaurantProduct = ProductArray[RandProducindex].Nombre;

                Frase TextToReturn = new Frase();
                var RandVar = rnd.NextDouble();
                if (RandVar < 0.33)
                {
                    TextToReturn =  FraseSoloRestaurante(RandRestaurantName);

                }
                else if (RandVar >= 0.33 && RandVar < 0.50)
                {
                    TextToReturn = FraseRestauranteProducto(RandRestaurantName, RandRestaurantProduct);

                }
                else
                {
                    TextToReturn = FrassesCarppi();
                }
                return TextToReturn;
            }
            private Frase FrassesCarppi()
            {
                List<Frase> newfoundglory = new List<Frase>();
                newfoundglory.Add(new Frase("Tenemos que hablar...", "No nos hemos visto en mucho tiempo, ve los restaurantes que tenemos para ti"));
                newfoundglory.Add(new Frase("Hay que tomar una decisión..", "Y esa desición es ¿que vas a cenar con Carppi? 😜😜"));
                newfoundglory.Add(new Frase("Estas en los huesos! 😱", "Nos preocupamos mucho por ti, tienes que comer algo 😋😋"));
                newfoundglory.Add(new Frase("Estudios demuestran...", "Que pedir por Carppi puede mejorar tu humor 100%, pide ahora 😱😋😋"));
                newfoundglory.Add(new Frase("Hoy es nuestro dia especial 😍😍", "Pide algo por Carppi para festejar 😍😍😍"));
                newfoundglory.Add(new Frase("Tienes que admitirlo 😒😒", "Hoy no quieres cocinar y nosotros queremos llevarte algo a las puertas de tu hogar, pide ya! 😱😋"));
                newfoundglory.Add(new Frase("Es uno de esos dias ☹️☹️🙁😞", "Pero todo mejora cuando recibes tu comida a domicilio, pide ya! 😱😋"));
                newfoundglory.Add(new Frase("Dicen que las penas...", "Con pan son buenas!, asi que pidete algo 😍😍"));
                newfoundglory.Add(new Frase("Estoy muy triste 🙁😞", "No has pedido nada en un rato y tengo mucha hambre 😱😋"));
                newfoundglory.Add(new Frase("Algo pasa entre nosotros 😞", "Que no se que es pero cuando pides por Carppi ME ENCANTA!! 😍😍"));
                newfoundglory.Add(new Frase("Ya no te quiero 🙁😞", "Pero te amo, y mas cuando pides algo por Carppi 😍😍"));
                //🤖👾👽🤡
                newfoundglory.Add(new Frase("Podre ser solo una app 🤖, pero...", "Soy la app mas feliz cuando pides algo por aqui 😍😍"));
                newfoundglory.Add(new Frase("Nos invaden 😱👽👽", "Los varatisimos precios y la gran calidad de nuestros restaurantes 👾😍😍"));
                newfoundglory.Add(new Frase("El fin se acerca 👽😱👽", "Ó quiza no, pero no quisieras que llegue con el estomago vacio 👾👾😍"));
                newfoundglory.Add(new Frase("Estas muy flaquito/a 👻", "No queremos que desaparezcas 👻, pide algo ahora! 👻😍"));
                newfoundglory.Add(new Frase("Tenemos la vacuna 💊💉🧬", "Pero para tu hambre, el secreto es pedir ahora 😍💊"));
                newfoundglory.Add(new Frase("Sabes...", "Ni tu ex te dio las alitas que que tenemos para ti, pidete unas 😍😍"));
                newfoundglory.Add(new Frase("Ya es hora, ¿no?", "Hora de que pidas una de las hamburguesas que tenemos para ti 😍😍"));
                newfoundglory.Add(new Frase("Tienes un nuevo Match 😍😍", "Con la seleccion de pizzas que tenemos para ti 😍😍"));
                newfoundglory.Add(new Frase("El amor esta en el aire 😍😍", "En el aire que respiras cuando hueles la pizza que tiene Carppi para ti 😍😍"));
                // newfoundglory.Add(new Frase("Si no comes ☠️😵☠️😵", "No queremos que desaparezcas 👻, pide algo ahora! 👻😍"));
                var rnd = new Random();
                var ndex = (int)(rnd.NextDouble() * (newfoundglory.Count - 1));

                return newfoundglory[ndex];

            }
            private Frase FraseSoloRestaurante(string Restaurante)
            {
                string p1 = Restaurante + " tiene muchas cosas que te podrían gustar 😱, atrevete a probar algo nuevo con Carppi!";
                string p2 = Restaurante + " tiene los mejores platillos a un super precio, pidete alguno! 😜";
                string p3 = "Cuando cansado estes " +  Restaurante + " te levantara, todo lo que venden es genial! 😜";
                string p4 = Restaurante + " tiene todos sus platillos a un super precio!, pidete algo 😱😜";

                List<string> ArrToSelect = new List<string>() { p1, p2, p3, p4 };
                var rnd = new Random();
                var ndex = (int)(rnd.NextDouble() * (ArrToSelect.Count-1));

                // return ArrToSelect[ndex];
                var nuevoretorno = new Frase();
                nuevoretorno.Texto = ArrToSelect[ndex];
                nuevoretorno.Titulo = "Carppi";
                return nuevoretorno;


            }
            private Frase FraseRestauranteProducto(String Restaurante, string Producto)
            {
                string p1 = "Disfruta de " + Producto + "Traido a ti por " + Restaurante + " solo en Carppi";
                string p2 = "Disfruta de " + Producto + " de " + Restaurante + " hasta tu puerta en minutos, solo en Carppi!";
                string p3 = "Que te pareceria un " + Producto + " de " + Restaurante + " Calientito y fresquesito 😋 😋 😋";
                string p4 = "Pidete un " + Producto + " de " + Restaurante + " Esta de lujo! 😱😱😱";
                string p5 = "Si hoy no quieres cocinar, quiza un " + Producto + " de " + Restaurante + " Te gustara 😜";
                string p6 = Restaurante + " tiene el mejor " + Producto + ", pidelo que se acaba! 😱😱😱";


                List<string> ArrToSelect = new List<string>() {p1,p2,p3,p4,p5,p6 };
                var rnd = new Random();
                var ndex =(int)( rnd.NextDouble() * (ArrToSelect.Count - 1));

                var nuevoretorno = new Frase();
                nuevoretorno.Texto = ArrToSelect[ndex];
                nuevoretorno.Titulo = "Carppi";
                return nuevoretorno;

            }



        }
        
        public class Frase
        {
            public string Titulo;
            public string Texto;
            public Frase() { }
            public Frase(String title, string text)
            {
                Titulo = title;
                Texto = text;
            }
        }
        public void EraseBuyOrder(CarppiRestaurant_BuyOrders OrdenDeCompra)
        {
            db.CarppiRestaurant_BuyOrders.Remove(OrdenDeCompra);
            db.SaveChanges();
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

        public void PushItAll(string CuerpoMensaje, string Titulo, string ExtraData)
        {
            string json = "";
            //jsonObject.materias = informacion.Materias;
            var condition_ = "!('motin' in topics)";
            var data_ = new
            {
              

                notification = new
                {
                    body = CuerpoMensaje,
                    title = Titulo,
                    sound = ExtraData,
                    message = " "
                },
                condition = condition_,
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
    }
}