using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;//Json

namespace GameAndChill.Models
{
    public class GameMgmt
    {
        private static GameAndChillDBEntities ORM = new GameAndChillDBEntities();

        // GET
        public static Game GetGame (int id)
        {
            return ORM.Games.Find(id);
        }
        public static List<Game> GetManyGames(string search)
        {
            if (search == null || search == "")
            {
                return ORM.Games.ToList();
            }
            else
            {
                return ORM.Games.Where(x => x.Name.Contains(search)).ToList();
            }
        }
        public static List<Genre> GetGenres()
        {
            return ORM.Genres.ToList();
        }


        // POST
        public static void AddGame(int gameId)
        {
            // add one game
            GameToDB(IGDB.GetGameByID(gameId));
            SaveAndRefresh();
        }
        public static bool AddGame(int startId, int endId)
        {
            // multiple games

            // validate ids
            if (startId < 1 || endId < 1 || startId > endId)
            {
                return false;
            }

            List<int> ids = new List<int>();
            for (int i = startId; i < endId + 1; i++)
            {
                ids.Add(i);
            }
            JArray games = IGDB.GetMultipleGamesByID(ids.ToArray());

            foreach (JObject game in games)
            {
                if (GetGame(game["id"].Value<int>()) == null)
                {
                    GameToDB(game);
                }
            }
            SaveAndRefresh();

            return true;
        }
        public static bool AddAllTheGenres()
        {
            JArray genres = IGDB.GetAllTheGenres();

            if (genres == null)
            {
                return false;
            }

            foreach(JObject genre in genres)
            {
                GenreToDB(genre);
            }
            SaveAndRefresh();

            return true;
        }

        static void GameToDB(JObject game)
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
            if (game["cover"] != null)
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
                    GenreToDB(genre);
                    g.Genres.Add(ORM.Genres.Find(genre["id"].Value<int>()));
                }
            }
            // add to DB
            ORM.Games.Add(g);
        }
        static void GenreToDB(JObject genre)
        {
            if (ORM.Genres.Find(genre["id"].Value<int>()) == null)
            {
                Genre newGenre = new Genre();
                newGenre.ID = genre["id"].Value<int>();
                newGenre.Name = genre["name"].Value<string>();
                ORM.Genres.Add(newGenre);
            }
        }

        static void SaveAndRefresh()
        {
            ORM.SaveChanges();
            ORM = new GameAndChillDBEntities();
        }
    }
}