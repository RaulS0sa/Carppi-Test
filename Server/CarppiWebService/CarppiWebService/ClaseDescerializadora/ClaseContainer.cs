using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarppiWebService.ClaseDescerializadora
{
    public class ClaseContainer
    {
        public int ID { get; set; }
        public string Dispositivo { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Hora { get; set; }
        public DateTime Fecha_Activacion { get; set; }
        public DateTime Fecha_Fin_Contrato { get; set; }
        public string Saldo { get; set; }
    }
}