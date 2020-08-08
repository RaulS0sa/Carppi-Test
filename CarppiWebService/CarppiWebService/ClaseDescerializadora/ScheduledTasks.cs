using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Exceptions;
using CarppiWebService.Models;

namespace CarppiWebService.ClaseDescerializadora
{
    public class ScheduledTasks : IJob
    {
        PidgeonEntities db = new PidgeonEntities();

        public async Task Execute(IJobExecutionContext context)
        {
            var query = db.Dispositivos.Where(x => x.ID > 0);

            foreach(var elem in query)
            {

                try
                {
                    if (elem.Fecha_Fin_Contrato != null)
                    {
                        int result = DateTime.Compare(Convert.ToDateTime(elem.Fecha_Fin_Contrato), DateTime.UtcNow);

                        if(result >= 0)
                        {
                            elem.Vigente = true;
                           
                            elem.Velocidad = "hola";

                        }
                        else
                        {

                            elem.Vigente = false;

                        }
                       

                    }
                    if(elem.Vigente == null)
                    {
                        elem.Vigente = false;
                    }
                }
                catch (Exception)
                {

                }
            
            }

            db.SaveChanges();
            
            await Console.Out.WriteLineAsync("HelloJob is executing.");
        }


    }
}