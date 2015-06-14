using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackendWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult Reset()
        {
            Scoreboard.Instance.Users = new Dictionary<string, Score>();
            return View("Index");
        }
    }
}