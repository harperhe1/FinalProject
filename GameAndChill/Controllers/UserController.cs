using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameAndChill.Models;

namespace GameAndChill.Controllers
{
    public class UserController : Controller
    {
        GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        
        // GET: User
        public ActionResult Index(int id)
        {
            // User Home page. Get information about the user

            // TODO: validate id and display correct error page for out-of-range or bad request

            User currentUser = ORM.Users.Find(id);
            ViewBag.CurrentUser = currentUser;
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