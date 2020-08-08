using CarppiWebService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarppiWebService.Controllers
{
    public class TCarppi_MessagesApiController : ApiController
    {
        PidgeonEntities db = new PidgeonEntities();

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage PostMessageInRideShare(String FaceID_Interlocutor, Int64 TripRequest, String Message)
        {
            var REquest_OfTrip = db.CarppiRequestForDrive.Where(x => x.ID == TripRequest).FirstOrDefault();
            if(REquest_OfTrip != null)
            {
                //                --CREATE TABLE Carppi_MensajesRideshare(
                //--ID bigint primary key IDENTITY(1,1) NOT NULL,
                //--Mensaje varchar(max),
                //--FaceID_Sender varchar(450),
                //--RequestID bigint,
                //--DateInSeconds bigint,
                //--Entregado bit not null default 0,
                //--Leido bit not null default 0);
                var NewMessage = new Carppi_MensajesRideshare();
                NewMessage.Mensaje = Message;
                NewMessage.FaceID_Sender = FaceID_Interlocutor;
                NewMessage.RequestID = TripRequest;
                NewMessage.DateInSeconds = (long)(((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) / 1000);
                db.Carppi_MensajesRideshare.Add(NewMessage);
                db.SaveChanges();

                var Driver = db.CarppiRequestForDrive.Where(x => x.FaceIDDriver == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();
                var Passenger = db.CarppiRequestForDrive.Where(x => x.FaceIDPassenger == FaceID_Interlocutor && x.ID == TripRequest).FirstOrDefault();

                //var Token = "";
                //Token = Driver == null ? "" : db.Traveler_Perfil.Where(x=> x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                //Token = Passenger == null ? Token : db.Traveler_Perfil.Where(x => x.Facebook_profile_id == FaceID_Interlocutor).FirstOrDefault().FirebaseID;
                if (Driver != null) {
                    //var intermitentID= 
                    var Token = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == REquest_OfTrip.FaceIDPassenger).FirstOrDefault().FirebaseID;
                    Push(Message, "Mensaje Del Conductor", Token, "");
                }
                else
                {
                    var Token = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == REquest_OfTrip.FaceIDDriver).FirstOrDefault().FirebaseID;
                    Push(Message, "Mensaje Del Pasajero", Token, "");
                }


            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "");

        }

        [HttpGet]
        [ActionName("ApiByAction")]
        public HttpResponseMessage GetAlMessagesFromTheConversation(String FaceID_Interest, Int64 TripRequest)
        {
            var Messages = db.Carppi_MensajesRideshare.Where(x => x.RequestID == TripRequest);
            List<MesssageData> messsageDatas = new List<MesssageData>();
            foreach(var aca in Messages)
            {
                var OldMessage = new MesssageData();
                OldMessage.MessageContext = aca;
                OldMessage.RoleInConversation = aca.FaceID_Sender == FaceID_Interest ? RoleInCoversation.Sender : RoleInCoversation.Receiver;
                messsageDatas.Add(OldMessage);
            }
           
            return Request.CreateResponse(HttpStatusCode.Accepted, messsageDatas);

        }
        public enum RoleInCoversation {Sender, Receiver};
        public class MesssageData
        {
            public Carppi_MensajesRideshare MessageContext;
            public RoleInCoversation RoleInConversation;
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
