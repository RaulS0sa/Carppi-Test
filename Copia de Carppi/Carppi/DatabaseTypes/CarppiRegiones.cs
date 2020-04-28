using System;
namespace Carppi.DatabaseTypes
{
    public class CarppiRegiones
    {
        public int ID { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public double Latitud { get; set; }
        public double Longitud{ get; set; }
        public double LatitudCentroide { get; set; }
        public double LongitudCentroide { get; set; }
        public long ServicioRegional { get; set; }
        public double Radio { get; set; }
        public string Moneda { get; set; }
    }
}
