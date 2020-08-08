
using System;
using System.Web.Mvc;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using CarppiWebService.Models;
using System.Linq;

namespace CarppiWebService.Controllers
{
    public class HomeController : Controller
    {
        PidgeonEntities db = new PidgeonEntities();
        public ActionResult Index()
        {


            //< h2 >%%% TagForMoney %%%</ h2 >

            //                                < !--Total: $200.18 mxn-- >

            //                                 < p >%%% TagForCard %%%</ p >

            //                                 < !--Ganancias en efectivo: $400 mxn-- >

            //                                  < p >%%% TagForCash %%%</ p >

            //                                  < !--< p > Ganancias en tarjeta: $400 mxn </ p > -->


            //                                      < p >%%% TagForFare %%%</ p >

            var Raul = db.Traveler_Perfil.Where(x=>x.Facebook_profile_id == "10217260473614661").FirstOrDefault();//10217260473614661
            var Raul_Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == "10221568290107381").FirstOrDefault();//10217260473614661
            try
            {
                string path = Server.MapPath("~/App_Data/Sample.jpg");
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;


                var TotalAmount = 15.ToString();
                var TotalCard = 15.ToString();
                var TotalCash = 15.ToString();
                var TotalFare = 15.ToString();


                Push("nicio mensaje", "holi", Raul.FirebaseID, "");
                Push_Repartidor("nicio mensaje", "holi", Raul_Repartidor.FirebaseID, ""); var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new System.Net.Mail.MailAddress("raul.sosa.cortes@gmail.com"));  // replace with valid value 
                message.From = new System.Net.Mail.MailAddress("carppi_mexico@carppi.com.mx");  // replace with valid value
                message.Subject = "MailTest";

                string path2 = Server.MapPath("~/App_Data/Receipt.html");
                var fileContents = System.IO.File.ReadAllText(path2);
                var r1 = fileContents.Replace("%%%TagForMoney%%%", TotalAmount);
                r1 = r1.Replace("%%%TagForCard%%%", TotalCard);
                r1 = r1.Replace("%%%TagForCash%%%", TotalCash);
                r1 = r1.Replace("%%%TagForFare%%%", TotalFare);

                message.Body = r1;
                message.IsBodyHtml = true;

                using (var smtp = new System.Net.Mail.SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "carppi_mexico@carppi.com.mx",  // replace with valid value
                        Password = "THELASTTIMEaround"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp-relay.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                   // smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    smtp.Send(message);
                    //await smtp.SendMailAsync(message);
                  //  return RedirectToAction("Sent");
                }


               // SmtpMail oMail = new SmtpMail("TryIt");
               // EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();

               // // Your gmail email address
               // oMail.From = "carppi_mexico@carppi.com.mx";

               // // Set recipient email address
               // // oMail.To = "raul.sosa.cortes@gmail.com";
               // oMail.To = "raul.sosa.cortes@gmail.com";// "raul.sosa.cortes@gmail.com";
               //                  // Set email subject
               // oMail.Subject = "Tu Recibo de viaje";

               // // Set email body
               // //oMail.TextBody = "this is a test email sent from c# project with gmail.";
               // //            string path = System.Web.HttpContext.Current.Request.MapPath("~\\dataset.csv");
               //// string path = System.Web.HttpContext.Current.Request.MapPath("~/App_Data/Receipt.html");

               // string path2 = Server.MapPath("~/App_Data/Receipt.html");
               // var fileContents = System.IO.File.ReadAllText(path2);
               // var r1 = fileContents.Replace("%%%TagForMoney%%%", TotalAmount);
               // r1 = r1.Replace("%%%TagForCard%%%", TotalCard);
               // r1 = r1.Replace("%%%TagForCash%%%", TotalCash);
               // r1 = r1.Replace("%%%TagForFare%%%", TotalFare);
               // oMail.HtmlBody = r1;//"<h1>Hello!</h1>";
               //                               // Gmail SMTP server address
               // SmtpServer oServer = new SmtpServer("smtp-relay.gmail.com");

               // // Set 465 port
               // oServer.Port = 465;

               // // detect SSL/TLS automatically
               // oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

               // // Gmail user authentication
               // // For example: your email is "gmailid@gmail.com", then the user should be the same
               // oServer.User = "carppi_mexico@carppi.com.mx";
               // oServer.Password = "THELASTTIMEaround";


               // Console.WriteLine("start to send email over SSL ...");
               // oSmtp.SendMail(oServer, oMail);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ep)
            {
                //var Raul_Repartidor = db.CarppiGrocery_Repartidores.Where(x => x.FaceID_Repartidor == "10221568290107381").FirstOrDefault();//10217260473614661
                Push_Repartidor(ep.ToString(), "holi", Raul_Repartidor.FirebaseID, "");
                Push(ep.ToString(), "Error", Raul.FirebaseID, "");
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ep.Message);
            }



            //RestClient client = new RestClient();
            //client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            //client.Authenticator =
            //    new HttpBasicAuthenticator("api",
            //                                "898ca80e-30ad6d8b");
            //RestRequest request = new RestRequest();
            //request.AddParameter("domain", "carppi.com.mx", ParameterType.UrlSegment);
            //request.Resource = "carppi.com.mx/messages";
            //request.AddParameter("from", "Excited User <mailgun@carppi.com.mx>");
            //request.AddParameter("to", "raul.sosa.cortes@gmail.com");
            //request.AddParameter("subject", "Hello");
            //request.AddParameter("text", "Testing some Mailgun awesomness!");
            //request.Method = Method.POST;
            //var lkmkl= client.Execute(request);

            //try
            //{
            //    //var smptClient = new SmtpClient("smtp.gmail.com", 587)
            //    //{
            //    //    Credentials = new NetworkCredential("carppi_mexico@carppi.com.mx", "THELASTTIMEaround"),
            //    //    EnableSsl = true
            //    //};
            //    //smptClient.Send("carppi_mexico@carppi.com.mx", "raul.sosa.cortes@gmail.com", "Testing Email", "testing the email");
            //    string to = "raul.sosa.cortes@gmail.com";
            //    string from = "carppi_mexico@carppi.com.mx";
            //    MailMessage message = new MailMessage(from, to);
            //    message.Subject = "Using the new SMTP client.";
            //    message.Body = @"Using this new feature, you can send an email message from an application very easily.";
            //    SmtpClient client = new SmtpClient("smtp.gmail.com");
            //    // Credentials are necessary if the server requires the client 
            //    // to authenticate before it will send email on the client's behalf.
            //    client.UseDefaultCredentials = true;
            //    client.EnableSsl = true;
            //    client.Send(message);



            //}
            //catch (Exception ex)
            //{
            //   // return false;
            //}

            //    RestClient client = new RestClient();
            //    client.BaseUrl = new Uri("https://api.mailgun.net/v3"); 

            //client.Authenticator = 

            //    new HttpBasicAuthenticator("api",
            //        "f4c5d455c9b35563f5108d3ba9b8fdc9-898ca80e-30ad6d8b");
            //    RestRequest request = new RestRequest();
            //    request.AddParameter("domain", "sandbox3f38f24b0ff44cee9bb467a5ff298a68.mailgun.org", ParameterType.UrlSegment);
            //    request.Resource = "sandbox3f38f24b0ff44cee9bb467a5ff298a68.mailgun.org/messages";
            //    request.AddParameter("from", "Excited User <mailgun@sandbox3f38f24b0ff44cee9bb467a5ff298a68.mailgun.org>");
            //    request.AddParameter("to", "raul.sosa.cortes@gmail.com");
            //    request.AddParameter("to", "YOU@sandbox3f38f24b0ff44cee9bb467a5ff298a68.mailgun.org");
            //    request.AddParameter("subject", "Hello");
            //    request.AddParameter("text", "Testing some Mailgun awesomness!");
            //    request.Method = Method.POST;
            //    var aca =  client.Execute(request);

            //// Compose a message
            //MimeMessage mail = new MimeMessage();
            //mail.From.Add(new MailboxAddress("Excited Admin", "foo@carppi.com.mx"));
            //mail.To.Add(new MailboxAddress("Excited User", "raul.sosa.cortes@gmail.com"));
            //mail.Subject = "Hello";
            //mail.Body = new TextPart("plain")
            //{
            //    Text = @"Testing some Mailgun awesomesauce!",
            //};

            //// Send it!
            //using (var client = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    // XXX - Should this be a little different?
            //    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            //    client.Connect("smtp.mailgun.org", 587, false);
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");
            //    client.Authenticate("postmaster@sandbox3f38f24b0ff44cee9bb467a5ff298a68.mailgun.org", "postmaster@sandbox3f38f24b0ff44cee9bb467a5ff298a68.mailgun.org");

            //    client.Send(mail);
            //    client.Disconnect(true);
            //}
            //SmtpClient MyServer = new SmtpClient();
            //MyServer.Host = "";
            //MyServer.Port = 81;

            ////Server Credentials
            //NetworkCredential NC = new NetworkCredential();
            //NC.UserName = "";
            //NC.Password = "";
            ////assigned credetial details to server
            //MyServer.Credentials = NC;

            ////create sender address
            //MailAddress from = new MailAddress("Sender Address", "Name want to display");

            ////create receiver address
            //MailAddress receiver = new MailAddress("raul.sosa.cortes@gmail.com", "Name want to display");

            //MailMessage Mymessage = new MailMessage(from, receiver);
            //Mymessage.Subject = "custom subject";
            //Mymessage.Body = "Custom Body";
            ////sends the email
            //MyServer.Send(Mymessage);



            return View();
            /*
            ViewBag.Title = "Home Page";

            return View();
            */
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

        public void Push_Repartidor(string CuerpoMensaje, string Titulo, string token, string ExtraData)
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
            string serverKey = "AAAAMW1l0Ug:APA91bEyEEAAfz5JR8cyCgdD5CftyMqQ5i9UNuNhzzh_C3vh_GScpP52XQNfL9Tg-PkHLcsJWuv0-ySQbwbjlp5blbuVBppT2M6q4OLcbt_L4iLYBRYlkSZfNb_3Y2B5K4okEBPVGdbk";
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
