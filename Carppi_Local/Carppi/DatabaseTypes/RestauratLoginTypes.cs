using System;
using SQLite;
namespace Carppi.DatabaseTypes
{
    public class RestauratLoginTypes
    {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Nombre { get; set; }
        public long? Region { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public byte[] Foto { get; set; }
        public string FacebookId { get; set; }
        public string FirebaseID { get; set; }
        public int? contextualVailidationNumber { get; set; }
        public int? TypeOfStore { get; set; }
        public string CarppiHash { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public bool? VerificacionDecuenta { get; set; }
        public bool? RegistroValidado { get; set; }
        public bool? EstaAbierto { get; set; }
        public string Categorias { get; set; }
        public int? TypeOfFood { get; set; }
    }
}
