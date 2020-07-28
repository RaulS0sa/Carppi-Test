using System;
using SQLite;

namespace Carppi_Cliente.DatabaseTypes
{
    public class Log_info
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public string Photo { get; set; }

        public string locacionrecogida { get; set; }

        public string locacionLevantada { get; set; }

        public string Viajesrelizados { get; set; }
        public string Calificacion { get; set; }
        public string TUsuario { get; set; }
        public string ProfileId { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }

        public bool tarjetaRegistrada { get; set; }
        public string Correo { get; set; }
        public Int64? Region { get; set; }
        public Int64? ServiciosRegionales { get; set; }

        public Int64? Region_Delivery { get; set; }
        //FirebaseID
        public string FirebaseID { get; set; }

    }
}
