using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarppiWebService.Controllers
{
    public class FirmwareUpdateController : Controller
    {
        // GET: FirmwareUpdate
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public virtual ActionResult Download()
        {
            //https://stackoverflow.com/questions/16670209/download-excel-file-via-ajax-mvc

            // https://stackoverflow.com/questions/3604562/download-file-of-any-type-in-asp-net-mvc-using-fileresult
            //https://stackoverflow.com/questions/47250493/return-excel-file-response-in-asp-net-mvc
            //string fullPath = Path.Combine(Server.MapPath("~/MyFiles"), file);
            //var asd = Server.MapPath("~/Documentos_padron/Book_materias.xlsx");
            string fullPath = Server.MapPath("~/FirmwareUpdate/Test_Blink_esp8266_OTA.ino.nodemcu.bin");
            var fileName = "Test_Blink_esp8266_OTA.ino.nodemcu.bin";
            /*
            switch (indice)
            {
                case "0":
                    fullPath = Server.MapPath("~/Documentos_padron/Subir_alumnos.xlsx");
                    return File(fullPath, "application/vnd.ms-excel", "Ejemplo_subir_alumnos.xlsx");
                    break;
                case "1":
                    fullPath = Server.MapPath("~/Documentos_padron/Book_materias.xlsx");
                    return File(fullPath, "application/vnd.ms-excel", "Ejemplo_subir_profesores_y_materias.xlsx");
            }
            */
            return File(fullPath, System.Web.MimeMapping.GetMimeMapping(fileName), fileName);


        }
    }
}