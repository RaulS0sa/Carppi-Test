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
        public string FirebaseID { get; set; }

    }
}