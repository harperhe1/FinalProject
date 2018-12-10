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

            // get user from DB, put in a viewbag
            ViewBag.CurrentUser = UserMgmt.GetUser((int)id);

            // if the ID doesn't match a user in the database, return an error
            if (ViewBag.CurrentUser == null)
            {
                ViewBag.Error = "User does not exist";
                return View("Error");
            }

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
            // get user from DB, put in a viewbag
            ViewBag.CurrentUser = UserMgmt.GetUser(id);
            
            // if the ID doesn't match a user in the database, return an error
            if (ViewBag.CurrentUser == null)
            {
                ViewBag.Error = "User does not exist";
                return View("Error");
            }

            return View();
        }
        public ActionResult SubmitQuestions(int id, int answer1, int answer2, int answer3, int answer4, int answer5)
        {
            // validate
            if (UserMgmt.GetUser(id) == null)
            {
                ViewBag.Error = "User not found";
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
            //if (ViewBag.Answers.Count == 0)
            //{
            //    return RedirectToAction("Questions", new { id });
            //}
            
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
            if(UserMgmt.GetUser(userID) == null)
            {
                ViewBag.Error = "User not found";
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
            string status;
            if (UserMgmt.LikeDislikeGame(userID, gameID, isLike, out status) == false)
            {
                ViewBag.Error = status;
                return View("Error");
            }
            
            // return status message to the user (liked or disliked)
            TempData["IsLike"] = status;

            return RedirectToAction("GameFinder", new { userID });
        }

        public ActionResult DeleteUser(int id) //Delete a user from the database
        {
            if (UserMgmt.DeleteUser(id, out string status) == false)
            {
                ViewBag.Error = status;
                return View("Error");
            }

            // Send their name back to the view
            TempData["Eulogy"] = status;

            return RedirectToAction("Index", "Home");
        }
    }
}