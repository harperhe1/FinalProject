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
            if (Validate.GameExists(id, out string Error) == false)
            {
                ViewBag.Error = Error;
                return View("Error");
            }
            if (!Validate.UserExists((int)UserID,out Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }
            ViewBag.Game = GameMgmt.GetGame(id);
            ViewBag.User = UserMgmt.GetUser((int)UserID);
            return View();
        }
        public ActionResult GameListDetails(int id)
        {
            ViewBag.Game = GameMgmt.GetGame(id);
            return View("Details");
        }
        public ActionResult GenreQuestions(int qID, int aID)
        {

            //ToDo:Question Validate & Answer Validate
            ViewBag.Question = ORM.Questions.Find(qID);
            ViewBag.Genres = GameMgmt.GetGenres();
            ViewBag.Answer = aID;
            List<bool> Checked = new List<bool>();
            foreach( var genre in ORM.Genres)
            {
                if(genre.Question_Genre.Where(x => x.QuestionID == qID && x.Answer == aID).Count() != 0)
                {
                    Checked.Add(true);
                }
                else
                {
                    Checked.Add(false);
                }
            }
            ViewBag.IsChecked = Checked;
            return View();
        }
        public ActionResult SetGenreQuestions(int qID, int aID, IEnumerable<bool> GenreName)
        {
            List<bool> IsGenre = GenreName.ToList();
            List<Genre> Genres = ORM.Genres.ToList();
            int temp = 0;
            for(int i =0; i < Genres.Count(); i++)
            {
                Question_Genre gQ = ORM.Question_Genre.Find(  qID,Genres[i].ID, aID );
                if (IsGenre[temp])
                {
                    if(gQ == null)
                    {
                        gQ = new Question_Genre { QuestionID = qID, GenreID = Genres[i].ID, Answer = aID };
                        ORM.Question_Genre.Add(gQ);
                    }
                    temp++;
                }
                else
                {
                    if(gQ != null)
                    {
                        ORM.Question_Genre.Remove(gQ);
                    }
                }
            temp++;
            }
            ORM.SaveChanges();
            return RedirectToAction("Index");
        }
                GameAndChillDBEntities ORM = new GameAndChillDBEntities();

        

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
            ViewBag.Questions = QAMgmt.GetQuestions();
            return View();
        }

        public ActionResult ListAllGames(string search)
        {
            ViewBag.ListOfGames = GameMgmt.GetManyGames(search);
            return View();
        }
    
    }
}