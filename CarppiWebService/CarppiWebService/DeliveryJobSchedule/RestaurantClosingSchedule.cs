using CarppiWebService.Models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace CarppiWebService.DeliveryJobSchedule
{
    public class RestaurantClosingSchedule : IJob
    {
        PidgeonEntities db = new PidgeonEntities();
        public async Task Execute(IJobExecutionContext context)
        {
            // throw new NotImplementedException();

            //  var Timeee = new TimeOutType();
            //  Timeee.OpeningTime = "15:00";
            //  Timeee.ClossingTime = "16:00";
            //  var Encoded = JsonConvert.SerializeObject(Timeee);
            /*

             DateTime utc = DateTime.UtcNow;



             TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
             DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, zone);
             var dia = localDateTime.DayOfWeek;
             var tiempo = localDateTime.ToString("HH:mm"); // for 24hr format


             var SturdayNigthFever = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID == 2).FirstOrDefault();
             var moment = JsonConvert.DeserializeObject<TimeOutType>(SturdayNigthFever.SaturdayOpenningSchedule);
             */

            var Restaurantes = db.Carppi_IndicesdeRestaurantes.Where(x => x.IsATestRestaurant == false);
            foreach(var rest in Restaurantes)
            {
                SetOpenOrClosedRestaurant(rest.ID);
            }

            
            //Hora estándar central (México)
            //TimeZoneInfo mex = TimeZoneInfo.FindSystemTimeZoneById("Hora estándar central (México)");
            //DateTime MexDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, mex);
            //await Console.Out.WriteLineAsync("HelloJob is executing.");

            await db.SaveChangesAsync();
        }
        void SetOpenOrClosedRestaurant(long restaurantID )
        {
            DateTime utc = DateTime.UtcNow;
            var Restaurant = db.Carppi_IndicesdeRestaurantes.Where(x => x.ID == restaurantID).FirstOrDefault();
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(Restaurant.TimeZoneID);
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utc, zone);
            var dia = localDateTime.DayOfWeek;
            var Hour = localDateTime.ToString("HH:mm"); // for 24hr format

            //string Hour
            switch (localDateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    {

                        var moment = JsonConvert.DeserializeObject<TimeOutType>(Restaurant.MondayOpenningSchedule);
                        if (moment.OpeningTime == Hour)
                        {
                            if (Restaurant.EstaAbierto == false)
                            {
                                Push_Restaurante("Tu restaurante se ha abierto automaticamente de acuerdo al horario especificado", "Restaurant abierto", Restaurant.FirebaseID, "");
                                Restaurant.EstaAbierto = true;
                            }
                        }
                        else if (moment.ClossingTime == Hour)
                        {
                            Restaurant.EstaAbierto = false; ;
                        }
                    }
                    break;
                case DayOfWeek.Tuesday:
                    {

                        var moment = JsonConvert.DeserializeObject<TimeOutType>(Restaurant.TuesdayOpenningSchedule);
                        if (moment.OpeningTime == Hour)
                        {
                            if (Restaurant.EstaAbierto == false)
                            {
                                Push_Restaurante("Tu restaurante se ha abierto automaticamente de acuerdo al horario especificado", "Restaurant abierto", Restaurant.FirebaseID, "");
                                Restaurant.EstaAbierto = true;
                            }
                        }
                        else if (moment.ClossingTime == Hour)
                        {
                            Restaurant.EstaAbierto = false; ;
                        }
                    }
                    break;
                case DayOfWeek.Wednesday:
                    {

                        var moment = JsonConvert.DeserializeObject<TimeOutType>(Restaurant.WednesdayOpenningSchedule);
                        if (moment.OpeningTime == Hour)
                        {
                            if (Restaurant.EstaAbierto == false)
                            {
                                Push_Restaurante("Tu restaurante se ha abierto automaticamente de acuerdo al horario especificado", "Restaurant abierto", Restaurant.FirebaseID, "");
                                Restaurant.EstaAbierto = true;
                            }
                        }
                        else if (moment.ClossingTime == Hour)
                        {
                            Restaurant.EstaAbierto = false; ;
                        }
                    }
                    break;
                case DayOfWeek.Thursday:
                    {

                        var moment = JsonConvert.DeserializeObject<TimeOutType>(Restaurant.ThursDayOpenningSchedule);
                        if (moment.OpeningTime == Hour)
                        {
                            if (Restaurant.EstaAbierto == false)
                            {
                                Push_Restaurante("Tu restaurante se ha abierto automaticamente de acuerdo al horario especificado", "Restaurant abierto", Restaurant.FirebaseID, "");
                                Restaurant.EstaAbierto = true;
                            }
                        }
                        else if (moment.ClossingTime == Hour)
                        {
                            Restaurant.EstaAbierto = false; ;
                        }
                    }
                    break;
                case DayOfWeek.Friday:
                    {

                        var moment = JsonConvert.DeserializeObject<TimeOutType>(Restaurant.FridayOpenningSchedule);
                        if (moment.OpeningTime == Hour)
                        {
                            if (Restaurant.EstaAbierto == false)
                            {
                                Push_Restaurante("Tu restaurante se ha abierto automaticamente de acuerdo al horario especificado", "Restaurant abierto", Restaurant.FirebaseID, "");
                                Restaurant.EstaAbierto = true;
                            }
                        }
                        else if (moment.ClossingTime == Hour)
                        {
                            Restaurant.EstaAbierto = false; ;
                        }
                    }
                    break;
                case DayOfWeek.Saturday:
                    {
                        
                        var moment = JsonConvert.DeserializeObject<TimeOutType>(Restaurant.SaturdayOpenningSchedule);
                        if (moment.OpeningTime == Hour)
                        {
                            if (Restaurant.EstaAbierto == false)
                            {
                                Push_Restaurante("Tu restaurante se ha abierto automaticamente de acuerdo al horario especificado", "Restaurant abierto", Restaurant.FirebaseID, "");
                                Restaurant.EstaAbierto = true;
                            }
                        }
                        else if(moment.ClossingTime == Hour)
                        {
                            Restaurant.EstaAbierto = false; ;
                        }
                    }
                    break;
                case DayOfWeek.Sunday:
                    {

                        var moment = JsonConvert.DeserializeObject<TimeOutType>(Restaurant.SunDayOpenningSchedule);
                        if (moment.OpeningTime == Hour)
                        {
                            if (Restaurant.EstaAbierto == false)
                            {
                                Push_Restaurante("Tu restaurante se ha abierto automaticamente de acuerdo al horario especificado", "Restaurant abierto", Restaurant.FirebaseID, "");
                                Restaurant.EstaAbierto = true;
                            }
                        }
                        else if (moment.ClossingTime == Hour)
                        {
                            Restaurant.EstaAbierto = false; ;
                        }
                    }
                    break;
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


    }
    public class TimeOutType
    {
        public string OpeningTime;
        public string ClossingTime;
    }
}