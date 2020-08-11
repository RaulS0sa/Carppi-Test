using CarppiWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarppiWebService.Controllers
{
    public class GeoLocController : Controller
    {
        // GET: GeoLoc

        PidgeonEntities db = new PidgeonEntities();
        public ActionResult Index()
        {
            return View();
        }
       
    }
}