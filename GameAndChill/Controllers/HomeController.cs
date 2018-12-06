using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameAndChill.Models;

namespace GameAndChill.Controllers
{
    public class HomeController : Controller
    {

        // TODO: Do more Css on Home Pages
        GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        static bool ValidateUser(int userID)
        {

            return true;
        }
        public ActionResult Index()
        {
            // display list of users

            ViewBag.Users = ORM.Users.ToList();
            if(ORM.Genres.Count() == 0)
            {
                return RedirectToAction("AddGenresToDB", "Game");
            }
            return View();

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