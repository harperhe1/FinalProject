using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameAndChill.Models;

namespace GameAndChill.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        // TODO: List all Games
        // TODO: Add Games Page
        // TODO: Do more Css on User Pages
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(int id, int UserID)
        {
            User user = UserMgmt.GetUser(UserID);
            if (user == null)
            {
                ViewBag.Error = "User not found";
                return View("Error");
            }

            Game game = GameMgmt.GetGame(id);
            if (game == null)
            {
                ViewBag.Error = "Game not found";
                return View("Error");
            }

            ViewBag.Game = game;
            ViewBag.User = user;
            return View();
        }
        public ActionResult GameListDetails(int id)
        {
            Game game = GameMgmt.GetGame(id);
            if (game == null)
            {
                ViewBag.Error = "Game not found";
                return View("Error");
            }

            ViewBag.Game = game;
            return View("Details");
        }
        public ActionResult GenreQuestions(int qID, int aID)
        {
            // if either parameters are out of range, error
            // need to change this if we ever have more than 5 questions or more than 5 answers. -David
            if (!QAMgmt.ValidAnswer(qID) || !QAMgmt.ValidAnswer(aID))
            {
                ViewBag.Error = "Index out of range. Try again";
                return View("Error");
            }
            ViewBag.Question = QAMgmt.GetQuestion(qID);
            ViewBag.Genres = GameMgmt.GetGenres();
            ViewBag.Answer = aID;
            ViewBag.IsChecked = QAMgmt.GenreCheckboxes(qID, aID);
            return View();
        }
        public ActionResult SetGenreQuestions(int qID, int aID, IEnumerable<bool> GenreName)
        {
            List<bool> IsGenre = GenreName.ToList();
            QAMgmt.CorrolateQuestions(IsGenre, qID, aID);

            return RedirectToAction("Index");
        }
        

        public ActionResult AddGames()
        {
            return View();
        }
        public ActionResult AddGamesToDb(int startId, int endId)
        {
            if(GameMgmt.AddGame(startId, endId) == false)
            {
                ViewBag.Error = "Invalid startId or endId";
                return View("Error");
            }

            return RedirectToAction("Index","Home");
        }

        // add specific game to DB
        public ActionResult AddGameToDB(int id)
        {
            // if Game is already in DB, don't do anything
            if (GameMgmt.GetGame(id) != null)
            {
                TempData["Added"] = "Game Already Exists";
                return RedirectToAction("Index", "Home");
            }

            GameMgmt.AddGame(id);

            TempData["Added"] = $"Added game to the database";
            return RedirectToAction("Index","Home");

        }


        public ActionResult AddGenresToDB()
        {
            if(GameMgmt.AddAllTheGenres() == false)
            {
                ViewBag.Error = "Server is broke :(";
                return View("Error");
            }

            TempData["Added"] = "Added Genre to Database";
            return RedirectToAction("Index", "Home");
            
        }

        public ActionResult EditGenres()
        {
            ViewBag.Questions = QAMgmt.GetAllQuestions();
            return View();
        }

        public ActionResult ListAllGames(string search)
        {
            ViewBag.ListOfGames = GameMgmt.GetManyGames(search);
            return View();
        }
    
    }
}