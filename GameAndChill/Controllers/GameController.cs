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
        public ActionResult Index()
        {
            GetGame1942();
            return View();
        }
        public ActionResult Details()
        {
            return View();
        }

        string APIKey = System.Configuration.ConfigurationManager.AppSettings["user-key"];
        GameAndChillDBEntities ORM = new GameAndChillDBEntities();

        //Test api with this method. Gets The Witcher 3 and pushes into a viewbag 
        public void GetGame1942()
        {
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp("https://api-endpoint.igdb.com/games/1942");

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

                ViewBag.GameInfo = GameInfo;
                reader.Close();
            }
        }
        // Call this method whenever we want to get a JObject of a specific game
        public JObject GetGameByID(int id)
        {
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{id}");

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

        // add specific game to DB
        public ActionResult AddGameToDB(int id)
        {
            JObject game = GetGameByID(id);

            // create Game object
            Game g = new Game();
            g.ID = int.Parse(game["id"].ToString());
            g.Name = game["name"].ToString();
            g.Summary = game["summary"].ToString();
            g.URL = game["url"].ToString();
            g.ImageURL = game["cover"]["url"].ToString();

            // add to DB
            ORM.Games.Add(g);
            ORM.SaveChanges();

            // result message
            TempData["Added"] = $"Added {g.ID} to the database";

            return RedirectToAction("Index", "Home");

        }
    }
}