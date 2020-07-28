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
    public class Carppi_ProductosPorRestaurantes
    {
        [PrimaryKey, AutoIncrement]
        public long TableID { get; set; }

        public long ID { get; set; }
        public string IDdRestaurante { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public byte[] Foto { get; set; }
        public double? Costo { get; set; }
        public long? Categoria { get; set; }
        public bool Disponibilidad { get; set; }
        public long ComprasDelProducto { get; set; }

    }
}