using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;//Json
using System.IO;//StringReader
using System.Net;//HttpWebRequest
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
        public ActionResult Details(int id, int? UserID)
        {
            if (Validate.GameExists(id, out string Error) == false)
            {
                ViewBag.Error = Error;
                return View("Error");
            }
            if (UserID.HasValue && !Validate.UserExists((int)UserID,out Error))
            {
                ViewBag.Error = Error;
                return View("Error");
            }
            ViewBag.Game = GameInfo.FindGame(id);
            ViewBag.User = UserMgmt.GetUser((int)UserID);
            return View();
        }
        public ActionResult GenreQuestions(int qID, int aID)
        {
            //ToDo:Question Validate & Answer Validate
            ViewBag.Question = ORM.Questions.Find(qID);
            ViewBag.Genres = GameInfo.GetGenres();
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
        

        string APIKey = System.Configuration.ConfigurationManager.AppSettings["user-key"];
        GameAndChillDBEntities ORM = new GameAndChillDBEntities();

        string expander = "?expand=keywords,platforms,genres&fields=name,summary,url,cover,keywords.name,platforms.name,genres.name";


        // Call this method whenever we want to get a JObject of a specific game
        public JObject GetGameByID(int id)
        {
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{id}{expander}");


            request.Headers.Add("user-key", APIKey);
            request.Accept = "application/json";
            //make our response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as string
                string output = reader.ReadToEnd();

                //convert response to JSon
                JArray GameInfo = JArray.Parse(output);
                reader.Close();

                // since our array is only one jObject we can return that jObject like so
                foreach (JObject game in GameInfo)
                {
                    return game;
                }
            }
            // return null if something goes wrong with the request/response
            return null;
        }

        // For Multiple Games
        public JArray GetMultipleGamesByID(int[] id)
        {
            // takes all the ids in the array and puts it into a string that can be used in the url
            string ids = "";
            for (int i =0; i<id.Length; i++)
            {
                ids = ids + id[i];
                if (i != id.Length - 1)
                {
                    ids = ids + ",";
                }
            }

            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{ids}{expander}");

            request.Headers.Add("user-key", APIKey);
            request.Accept = "application/json";
            //make our response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as string
                string output = reader.ReadToEnd();

                //convert response to JSon
                JArray GameInfo = JArray.Parse(output);
                reader.Close();

                return GameInfo;
            }
            // return null if something goes wrong with the request/response
            return null;
        }

        public ActionResult AddGames()
        {
            return View();
        }
        public ActionResult AddGamesToDb(int startId, int endId)
        {
            if(startId < 1 || endId < 1 || startId > endId)
            {
                ViewBag.Error = "Invalid startId or endId";
                return View("Error");
            }
            List<int> ids = new List<int>();
            for(int i =startId; i < endId +1; i++)
            {
                ids.Add(i);
            }
            JArray games = GetMultipleGamesByID(ids.ToArray());
            
            foreach(JObject game in games)
            {
                if (ORM.Games.Find(game["id"].Value<int>()) == null)
                {
                    GameToDB(game);
                }
            }

            return RedirectToAction("Index","Home");
        }

        // add specific game to DB
        public ActionResult AddGameToDB(int id)
        {
            // if Game is already in DB, don't do anything
            if (ORM.Games.Find(id) != null)
            {
                TempData["Added"] = "Game Already Exists";
                return RedirectToAction("Index", "Home");
            }

            GameToDB(GetGameByID(id));
            
            return RedirectToAction("Index","Home");

        }
        public void GameToDB(JObject game)
        {
            // create Game object

            Game g = new Game();
            g.ID = int.Parse(game["id"].ToString());
            g.Name = game["name"].ToString();
            if (game["summary"] != null)
            {
                if (game["summary"].ToString().Length > 255)
                    g.Summary = game["summary"].ToString().Substring(0, 255);
                else
                    g.Summary = game["summary"].ToString();
            }
            g.URL = game["url"].ToString();
            if(game["cover"] != null)
            {
                g.ImageURL = game["cover"]["url"].ToString();
            }
            if (game["keyword"] != null)
            {
                foreach (JObject keyword in game["keywords"])
                {
                    if (keyword["name"].ToString().Length > 50)
                    {
                        continue;
                    }
                    // check if keyword is in DB
                    if (ORM.Keywords.Find(keyword["id"].Value<int>()) == null)
                    {
                        // create Keyword object and add to DB
                        Keyword newKey = new Keyword();
                        newKey.ID = keyword["id"].Value<int>();
                        newKey.Name = keyword["name"].Value<string>();
                        ORM.Keywords.Add(newKey);
                    }
                    // pair the Keyword to the Game in the Keyword_Game table 
                    g.Keywords.Add(ORM.Keywords.Find(keyword["id"].Value<int>()));
                }
            }
            if (game["platforms"] != null)
            {
                foreach (JObject platform in game["platforms"])
                {
                    if (ORM.Platforms.Find(platform["id"].Value<int>()) == null)
                    {
                        Platform newPlatform = new Platform();
                        newPlatform.ID = platform["id"].Value<int>();
                        newPlatform.Name = platform["name"].Value<string>();
                        ORM.Platforms.Add(newPlatform);
                    }
                    g.Platforms.Add(ORM.Platforms.Find(platform["id"].Value<int>()));
                }
            }
            if (game["genres"] != null)
            {
                foreach (JObject genre in game["genres"])
                {
                    if (ORM.Genres.Find(genre["id"].Value<int>()) == null)
                    {
                        Genre newGenre = new Genre();
                        newGenre.ID = genre["id"].Value<int>();
                        newGenre.Name = genre["name"].Value<string>();
                        ORM.Genres.Add(newGenre);
                    }
                    g.Genres.Add(ORM.Genres.Find(genre["id"].Value<int>()));
                }
            }
            // add to DB
            ORM.Games.Add(g);
            ORM.SaveChanges();
            // result message
            TempData["Added"] = $"Added {g.Name} to the database";
        }

        public ActionResult AddGenresToDB()
        {
            string ids = "";
            for (int i = 0; i < 34; i++)
            {
                ids = ids + i;
                if (i != 33)
                {
                    ids = ids + ",";
                }
            }
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/genres/{ids}?fields=id,name");

            request.Headers.Add("user-key", APIKey);
            request.Accept = "application/json";
            //make our response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as string
                string output = reader.ReadToEnd();

                //convert response to JSon
                JArray GenreInfo = JArray.Parse(output);
                reader.Close();

                foreach (JObject genre in GenreInfo)
                {
                    if(ORM.Genres.Find(genre["id"].Value<int>()) == null)
                    {
                        if(genre["name"] == null)
                        {
                            continue;
                        }
                        Genre newGenre = new Genre();
                        newGenre.ID = genre["id"].Value<int>();
                        newGenre.Name = genre["name"].Value<string>();
                        ORM.Genres.Add(newGenre);
                    }
                }

                ORM.SaveChanges();
                TempData["Added"] = "Added Genre to Database";
                return RedirectToAction("Index", "Home");
            }
            // return null if something goes wrong with the request/response
            return View("Error");
        }

        public ActionResult EditGenres()
        {
            ViewBag.Questions = ORM.Questions.ToList();
            return View();
        }

        public ActionResult ListAllGames(string search)
        {
            List<Game> listOfGames;
            if (search == null || search == "")
            {
                listOfGames = ORM.Games.ToList();
            }
            else
            {
                listOfGames = ORM.Games.Where(x => x.Name.Contains(search)).ToList();
            }

            ViewBag.ListOfGames = listOfGames;
            return View();
        }
    
    }
}