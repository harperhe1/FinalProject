using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameAndChill.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult GameFinder()
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View("Home/Index");
        }
    }
}