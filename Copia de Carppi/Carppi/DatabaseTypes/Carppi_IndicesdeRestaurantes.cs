using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;


namespace Carppi.DatabaseTypes
{
    public class Carppi_IndicesdeRestaurantes
    {
        [PrimaryKey, AutoIncrement]
        public long TableID { get; set; }

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
        public long? Categoriasbitfield { get; set; }
        public bool IsATestRestaurant { get; set; }
        public string TimeZoneID { get; set; }
        public string SaturdayOpenningSchedule { get; set; }
        public string MondayOpenningSchedule { get; set; }
        public string TuesdayOpenningSchedule { get; set; }
        public string WednesdayOpenningSchedule { get; set; }
        public string ThursDayOpenningSchedule { get; set; }
        public string FridayOpenningSchedule { get; set; }
        public string SunDayOpenningSchedule { get; set; }
        public string StripeAccount { get; set; }
        public string StripeHash { get; set; }
        public long DebtToRestaurant { get; set; }
        public string UpdateTag { get; set; }
        public bool AnyDeliveryMan { get; set; }
        public double calificacion { get; set; }

    }
}