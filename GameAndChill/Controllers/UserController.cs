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
        public ActionResult Index(int? id)
        {
            // User Home page. Get information about the user

            // redirect to home if we try to go to user index without selecting a user
            if(id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // if the ID doesn't match a user in the database, return an error
            User currentUser = ORM.Users.Find(id);
            if (!Validate.UserExists((int)id, out string Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }

            // pass user info to the view
            ViewBag.CurrentUser = currentUser;
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult AddUser(User newUser)
        {
            
            if(newUser.Name == null || newUser.Name == "") //used empty string in case it couldn't do null
            {
                ViewBag.Error = "Must have a name";
                return View("Error");
            }

            // add to DB, then get the ID of our new user
            int id = UserMgmt.AddUserReturnID(newUser);

            return RedirectToAction("Questions", new { id });
        }


        
        public ActionResult Questions(int id)
        {
            if (!Validate.UserExists(id, out string Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }

            // get user from DB
            ViewBag.CurrentUser = UserMgmt.GetUser(id);

            return View();
        }
        
        public ActionResult SubmitQuestions(int id, int answer1, int answer2, int answer3, int answer4, int answer5)
        {
            if (!Validate.UserExists(id, out string Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }

            // tell QAMgmt to add entries and not edit them
            bool exists = false;

            // put answers in an array
            int[] answers = new int[] { answer1, answer2, answer3, answer4, answer5 };

            // send data to QAMgmt, go to error page if validation fails
            if (QAMgmt.ManageAnswers(id, answers, exists) == false)
            {
                ViewBag.Error = "One or more of your question submissions was invalid. Try Again.";
                return View("Error");
            }

            return RedirectToAction("Index", new { id });
        }


        public ActionResult EditAnswers(int id)
        {
            if (!Validate.UserExists(id, out string Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }

            // pass current user and their original answers to the view
            ViewBag.CurrentUser = UserMgmt.GetUser(id);
            ViewBag.Answers = UserMgmt.GetAnswers(id); // TODO: use this viewbag
            
            // if user hasn't submitted their answers yet, redirect to Questions method
            if (ViewBag.Answers.Count == 0)
            {
                return RedirectToAction("Questions", new { id });
            }
            
            return View();
        }
        
        public ActionResult SaveQuestionChanges(int id, int answer1, int answer2, int answer3, int answer4, int answer5)
        {
            if (!Validate.UserExists(id, out string Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }

            // tell QAMgmt to edit entries and not add them
            bool exists = true;

            // put answers in an array
            int[] answers = new int[] { answer1, answer2, answer3, answer4, answer5 };

            // send data to QAMgmt, go to error page if validation fails
            if (QAMgmt.ManageAnswers(id, answers, exists) == false)
            {
                ViewBag.Error = "One or more of your question submissions was invalid. Try Again.";
                return View("Error");
            }

            return RedirectToAction("Index", new { id });
        }


        public ActionResult GameFinder(int userID)
        {
            if(!Validate.UserExists(userID,out string Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }
            //Game game = ORM.Games.Find(gameID);
            ConSoulFindGame alg = new ConSoulFindGame(userID);
            List<Game> games = alg.Result();
            if(games.Count != 0)
            {
                Game firstResult = games.First();
                ViewBag.GameDetails = firstResult; // TODO: change to random instead of a First one
            }
            else
            {
                ViewBag.Error = "No games left to find :(";
                return View("Error");
            }
            ViewBag.CurrentUser = alg.User;
            return View();
        }



        public ActionResult RemoveGame(int UserID, int GameID)
        {
            if (UserMgmt.RemoveLike(UserID, GameID) == false)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }
            
            return RedirectToAction("Index", new {id =  UserID });
        }


        public ActionResult LikeGame(bool isLike, int userID, int gameID)
        {
            string Error;
            if (!Validate.UserExists(userID, out Error) || !Validate.GameExists(gameID, out Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }
            // check database if it's liked or disliked by this user
            User_Game found = ORM.User_Game.Find(userID, gameID);
            if (found == null)
            {
                // if not, create object and add to DB
                User_Game userGame = new User_Game();
                userGame.UserID = userID;
                userGame.GameID = gameID;
                userGame.IsLike = isLike;
                ORM.User_Game.Add(userGame);
            }
            else
            {
                // set like status to isLike
                found.IsLike = isLike;
            }
            ORM.SaveChanges();

            // return status message to the user (liked or disliked)
            string like = "";
            if (isLike) { like = "liked"; }
            else { like = "disliked"; }
            TempData["IsLike"] = $"Added to {like} games";

            return RedirectToAction("GameFinder", new { userID });
        }
        public ActionResult DeleteUser(int id) //Delete a user from the database
        {
            if (!Validate.UserExists(id, out string Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }
            //Find user ID
            User userDelete = ORM.Users.Find(id);
            

            //Get their name
            string name = userDelete.Name;

            //Remove the user ID
            var userGames = userDelete.User_Game.ToList();
            foreach (User_Game user_Game in  userGames)
            {
                ORM.User_Game.Remove(user_Game);
            }
            var userAnswers = userDelete.Answers.ToList();
            foreach(Answer answer in userAnswers)
            {
                ORM.Answers.Remove(answer);
            }
            ORM.Users.Remove(userDelete);

            //SaveChanges duhhhhhhhh (Kidding, for real though it does save the changes to the DB)
            ORM.SaveChanges();

            // Send their name back to the view
            TempData["Eulogy"] = $"{name} has been deleted.";

            return RedirectToAction("Index", "Home");
        }
    }
}