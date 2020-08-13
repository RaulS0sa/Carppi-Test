using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarppiRestaurant.Models;

namespace CarppiRestaurant.Controllers
{
    public class HomeController : Controller
    {
        //PidgeonEntities
        PidgeonEntities db = new PidgeonEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ProcessLoginRequest(string User, string Pass)
        {
            /*
            var obj = db.TutoriUsuarios.Where(a => a.FaceID.Equals(FaceId)).FirstOrDefault();
            String My_newUser = "";
            if (obj == null)
            {
                var Usuario = new TutoriUsuario();
                Usuario.FaceID = FaceId;
                Usuario.EsPrimerUso = true;
                Usuario.IdentidadValidada = false;
                Usuario.Calificacion = 5.0;
                db.TutoriUsuarios.Add(Usuario);
                db.SaveChanges();
                My_newUser = FaceId;

            }
            else
            {
                My_newUser = obj.FaceID;
                //   Session["UserID"] = obj.ID.ToString();
            }
            Session["FaceID"] = My_newUser;
            */
            var restaurant = db.Carppi_IndicesdeRestaurantes.Where(x => x.Correo == User && x.WebsitePasword == Pass).FirstOrDefault();
            if(restaurant == null)
            {
                
                return Json(new { result = "Fail", url = Url.Action("Index", "None") });
            }
            else
            {
                Session["RestaurantID"] = restaurant.CarppiHash;
                return Json(new { result = "Redirect", url = Url.Action("Index", "RestaurantDashBoard") });
            }


            
            return Json(new { result = "Redirect", url = Url.Action("Index", "Tutori") });

        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}