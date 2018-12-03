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
            JObject TheWitcher = GetGameByID(1942);
            ViewBag.GameInfo = TheWitcher;
            return View();
        }
        public ActionResult Details()
        {
            return View();
        }

        string APIKey = System.Configuration.ConfigurationManager.AppSettings["user-key"];
        GameAndChillDBEntities ORM = new GameAndChillDBEntities();

        //Test api with this method. Gets The Witcher 3 and pushes into a viewbag 
        //public void GetGame1942()
        //{
        //    //make our resquest
        //    HttpWebRequest request = WebRequest.CreateHttp("https://api-endpoint.igdb.com/games/1942");

        //    request.Headers.Add("user-key", APIKey);
        //    request.Accept = "application/json";
        //    //make our response
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        //get response stream
        //        StreamReader reader = new StreamReader(response.GetResponseStream());

        //        //read response stream as string
        //        string output = reader.ReadToEnd();

        //        //convert response to JSon
        //        JArray GameInfo = JArray.Parse(output);

        //        ViewBag.GameInfo = GameInfo;
        //        reader.Close();
        //    }
        //}

        // Call this method whenever we want to get a JObject of a specific game
        public JObject GetGameByID(int id)
        {
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{id}?expand=keywords,platforms,genres&fields=name,summary,url,cover,keywords.name,platforms.name,genres.name");

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
            string ids = "";
            for (int i =0; i<id.Length; i++)
            {
                ids = ids + i;
                if (i != id.Length - 1)
                {
                    ids = ids + ",";
                }
            }
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{ids}?expand=keywords,platforms,genres&fields=name,summary,url,cover,keywords.name,platforms.name,genres.name");

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

        // add specific game to DB
        public ActionResult AddGameToDB(int id)
        {
            if(ORM.Games.Find(id) != null)
            {
                TempData["Added"] = "Game Already Exists";
                return RedirectToAction("Index", "Home");
            }
            JObject game = GetGameByID(id);

            // create Game object
            Game g = new Game();
            g.ID = int.Parse(game["id"].ToString());
            g.Name = game["name"].ToString();
            g.Summary = game["summary"].ToString();
            g.URL = game["url"].ToString();
            g.ImageURL = game["cover"]["url"].ToString();

            foreach(JObject keyword in game["keywords"])
            {
                if(ORM.Keywords.Find(keyword["id"].Value<int>()) == null)
                {
                    Keyword newKey = new Keyword();
                    newKey.ID = keyword["id"].Value<int>();
                    newKey.Name = keyword["name"].Value<string>();
                    ORM.Keywords.Add(newKey);
                }
                g.Keywords.Add(ORM.Keywords.Find(keyword["id"].Value<int>()));
            }



            // add to DB
            ORM.Games.Add(g);
            ORM.SaveChanges();

            // result message
            TempData["Added"] = $"Added {g.Name} to the database";

            return RedirectToAction("Index", "Home");

        }
    }
}