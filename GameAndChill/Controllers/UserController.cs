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
            if(currentUser == null)
            {
                ViewBag.Error = "User does not exist.";
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
            int id;
            if(newUser.Name == null || newUser.Name == "") //used empty string in case it couldn't do null
            {
                ViewBag.Error = "Must have a name";
                return View("Error");
            }
            // add to DB
            ORM.Users.Add(newUser);
            ORM.SaveChanges();

            // get the ID of our new user; send to viewbag
            id = ORM.Users.Where(x => x.Name == newUser.Name).Last().ID;

            return RedirectToAction("Questions", new { id });
        }


        public bool ValidAnswer(int q)
        {
            // Check if the answer passed in is between 1 and 5. If not, we don't want that in our database and messing anything up!
            if (q <= 5 && q >= 1)
            {
                return true;
            }
            return false;
        }
        public ActionResult Questions(int id)
        {
            User currentUser = ORM.Users.Find(id);
            ViewBag.CurrentUser = currentUser;
            return View();
        }
        public void AddQuestionToDB(int id, int answer, int qNum)
        {
            // create Question object and define its properties
            Answer q = new Answer();
            q.UserID = id;
            q.QuestionID = qNum;
            q.Answer1 = answer;

            // add to DB
            ORM.Answers.Add(q);
        }
        public ActionResult SubmitQuestions(int id, int answer1, int answer2, int answer3, int answer4, int answer5)
        {
            // Validation
            if (!ValidAnswer(answer1) || !ValidAnswer(answer2) || !ValidAnswer(answer3) || !ValidAnswer(answer4) || !ValidAnswer(answer5))
            {
                ViewBag.Error = "One or more of your question submissions was invalid. Try Again.";
                return View("Error");
            }
            
            // run the same method for different questions
            AddQuestionToDB(id, answer1, 1);
            AddQuestionToDB(id, answer2, 2);
            AddQuestionToDB(id, answer3, 3);
            AddQuestionToDB(id, answer4, 4);
            AddQuestionToDB(id, answer5, 5);

            ORM.SaveChanges();

            return RedirectToAction("Index", new { id });
        }


        public ActionResult EditAnswers(int id)
        {
            // pull users original answers
            User currentUser = ORM.Users.Find(id);
            List<Answer> found = currentUser.Answers.ToList();

            // if user hasn't submitted their answers yet, redirect to Questions method
            if (found.Count == 0)
            {
                return RedirectToAction("Questions", new { id });
            }
            
            // pass current user and their original answers to the view
            ViewBag.CurrentUser = currentUser;
            ViewBag.Answers = found; // TODO: use this viewbag
            
            return View();
        }
        public void EditQuestionInDB(int id, int answer, int qNum)
        {
            // create Question object and define its properties
            Answer q = new Answer();
            q.UserID = id;
            q.QuestionID = qNum;
            q.Answer1 = answer;

            // modify entry in DB
            ORM.Entry(q).State = System.Data.Entity.EntityState.Modified;
        }
        public ActionResult SaveQuestionChanges(int id, int answer1, int answer2, int answer3, int answer4, int answer5)
        {
            // Validation
            if (!ValidAnswer(answer1) || !ValidAnswer(answer2) || !ValidAnswer(answer3) || !ValidAnswer(answer4) || !ValidAnswer(answer5))
            {
                ViewBag.Error = "One or more of your question submissions was invalid. Try Again.";
                return View("Error");
            }

            // modify each question
            EditQuestionInDB(id, answer1, 1);
            EditQuestionInDB(id, answer2, 2);
            EditQuestionInDB(id, answer3, 3);
            EditQuestionInDB(id, answer4, 4);
            EditQuestionInDB(id, answer5, 5);

            ORM.SaveChanges();

            return RedirectToAction("Index", new { id });
        }


        public ActionResult GameFinder(int userID)
        {
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
        public ActionResult LikeGame(bool isLike, int userID, int gameID)
        {
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

            return RedirectToAction("GameFinder", new { gameID, userID });
        }


        public ActionResult DeleteUser(int id) //Delete a user from the database
        {
            //Find user ID
            User userDelete = ORM.Users.Find(id);

            //Get their name
            string name = userDelete.Name;

            //Remove the user ID
            ORM.Users.Remove(userDelete);

            //SaveChanges duhhhhhhhh (Kidding, for real though it does save the changes to the DB)
            ORM.SaveChanges();

            // Send their name back to the view
            TempData["Eulogy"] = $"{name} has been deleted.";

            return RedirectToAction("Index", "Home");
        }
    }
}