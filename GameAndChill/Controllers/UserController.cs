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
        public ActionResult SubmitQuestions(int id, int question1, int question2, int question3, int question4, int question5)
        {
            //Code will go here. Commented out to be able to build without errors
            Question q1 = new Question();
            q1.UserID = id;
            q1.ID = 1;
            q1.Answer = question1;
            ORM.Questions.Add(q1);

            Question q2 = new Question();
            q2.UserID = id;
            q2.ID = 2;
            q2.Answer = question2;
            ORM.Questions.Add(q2);

            Question q3 = new Question();
            q3.UserID = id;
            q3.ID = 3;
            q3.Answer = question3;
            ORM.Questions.Add(q3);

            Question q4 = new Question();
            q4.UserID = id;
            q4.ID = 4;
            q4.Answer = question4;
            ORM.Questions.Add(q4);

            Question q5 = new Question();
            q5.UserID = id;
            q5.ID = 5;
            q5.Answer = question5;
            ORM.Questions.Add(q5);

            ORM.SaveChanges();

            return RedirectToAction("Index", new { id });
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult GameFinder(int gameID, int userID)
        {
            Game game = ORM.Games.Find(gameID);
            User user = ORM.Users.Find(userID);
            ViewBag.GameDetails = game;
            ViewBag.CurrentUser = user;
            return View();
        }
        public ActionResult LikeGame(bool isLike, int userID, int gameID)
        {
            User_Game found = ORM.User_Game.Find(userID, gameID);
            if (found == null)
            {
                User_Game userGame = new User_Game();
                userGame.UserID = userID;
                userGame.GameID = gameID;
                userGame.IsLike = isLike;
                ORM.User_Game.Add(userGame);
            }
            else
            {
                found.IsLike = isLike;
            }
            ORM.SaveChanges();

            string like = "";
            if (isLike) { like = "liked"; }
            else { like = "disliked"; }
            TempData["IsLike"] = $"Added to {like} games";

            return RedirectToAction("GameFinder", new { gameID, userID });
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