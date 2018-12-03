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

        public ActionResult AddUser(User newUser)
        {
            int id;
            ORM.Users.Add(newUser);
            ORM.SaveChanges();
            List<User> users = ORM.Users.Where(x => x.Name == newUser.Name).ToList();
            id = users[users.Count - 1].ID;
            ViewBag.id = id;
            //return RedirectToAction("Index", new { id });
            return RedirectToAction("Questions", new { id });
        }
        public ActionResult Questions(int id)
        {
            User currentUser = ORM.Users.Find(id);
            ViewBag.CurrentUser = currentUser;
            return View();
        }
        //public ActionResult SubmitQuestions(int id, int question1, int question2, int question3, int question4, int questions5)
        //{
            //Code will go here. Commented out to be able to build without errors
        //}
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult GameFinder()
        {
            return View();
        }
        //public ActionResult Delete()   Commented this line out for the time being. I don't think we need it -SR
        //{
        //    return View("Index", "Home");
        //}
        public ActionResult DeleteUser(int id) //Delete a user from the database
        {
            //Find user ID
            User userDelete = ORM.Users.Find(id);
            //Remove the user ID
            ORM.Users.Remove(userDelete);
            //SaveChanges duhhhhhhhh (Kidding, for real though it does save the changes to the DB)
            ORM.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}