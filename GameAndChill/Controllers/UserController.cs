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
            if(newUser.Name == null || newUser.Name == "")
            {
                ViewBag.Error = "Must have a name";
                return View("Error");
            }
            // add to DB
            ORM.Users.Add(newUser);
            ORM.SaveChanges();

            // get the ID of our new user; send to viewbag
            List<User> users = ORM.Users.Where(x => x.Name == newUser.Name).ToList();
            id = users[users.Count - 1].ID;
            ViewBag.id = id;

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
        public ActionResult SubmitQuestions(int id, int question1, int question2, int question3, int question4, int question5)
        {
            // Validation
            if (!ValidAnswer(question1) || !ValidAnswer(question2) || !ValidAnswer(question3) || !ValidAnswer(question4) || !ValidAnswer(question5))
            {
                ViewBag.Error = "One or more of your question submissions was invalid. Try Again.";
                return View("Error");
            }
            
            // run the same method for different questions
            AddQuestionToDB(id, question1, 1);
            AddQuestionToDB(id, question2, 2);
            AddQuestionToDB(id, question3, 3);
            AddQuestionToDB(id, question4, 4);
            AddQuestionToDB(id, question5, 5);

            
            // Original Code. Now generalized in the method "AddQuestionToDB". -David

            /*Question q1 = new Question();
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
            ORM.Questions.Add(q5);*/

            ORM.SaveChanges();

            return RedirectToAction("Index", new { id });
        }


        public ActionResult EditAnswers(int id)
        {
            // pull users original answers
            List<Answer> found = ORM.Answers.Where(x=>x.UserID==id).ToList();

            // if user hasn't submitted their answers yet, redirect to Questions method
            if(found.Count == 0)
            {
                return RedirectToAction("Questions", new { id });
            }
            
            // pass current user and their original answers to the view
            User currentUser = ORM.Users.Find(id);
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
        public ActionResult SaveQuestionChanges(int id, int question1, int question2, int question3, int question4, int question5)
        {
            // Validation
            if (!ValidAnswer(question1) || !ValidAnswer(question2) || !ValidAnswer(question3) || !ValidAnswer(question4) || !ValidAnswer(question5))
            {
                ViewBag.Error = "One or more of your question submissions was invalid. Try Again.";
                return View("Error");
            }

            // modify each question
            EditQuestionInDB(id, question1, 1);
            EditQuestionInDB(id, question2, 2);
            EditQuestionInDB(id, question3, 3);
            EditQuestionInDB(id, question4, 4);
            EditQuestionInDB(id, question5, 5);


            // Original Code. Now generalized in the method "EditQuestionInDB". -David

            /*Question editQ1 = new Question();
            editQ1.UserID = id;
            editQ1.ID = 1;
            editQ1.Answer = question1;
            ORM.Entry(editQ1).State = System.Data.Entity.EntityState.Modified;

            Question editQ2 = new Question();
            editQ2.UserID = id;
            editQ2.ID = 2;
            editQ2.Answer = question2;
            ORM.Entry(editQ2).State = System.Data.Entity.EntityState.Modified;

            Question editQ3 = new Question();
            editQ3.UserID = id;
            editQ3.ID = 3;
            editQ3.Answer = question3;
            ORM.Entry(editQ3).State = System.Data.Entity.EntityState.Modified;

            Question editQ4 = new Question();
            editQ4.UserID = id;
            editQ4.ID = 4;
            editQ4.Answer = question4;
            ORM.Entry(editQ4).State = System.Data.Entity.EntityState.Modified;

            Question editQ5 = new Question();
            editQ5.UserID = id;
            editQ5.ID = 5;
            editQ5.Answer = question5;
            ORM.Entry(editQ5).State = System.Data.Entity.EntityState.Modified;*/

            ORM.SaveChanges();

            return RedirectToAction("Index", new { id });
        }


        public ActionResult GameFinder(int gameID, int userID)
        {
            //Game game = ORM.Games.Find(gameID);
            ConSoulFindGame alg = new ConSoulFindGame(userID);
            ViewBag.GameDetails = alg.Result().First();
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