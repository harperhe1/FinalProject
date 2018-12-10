using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class GameInfo
    {
        private static GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        public static Game FindGame (int id)
        {
            return ORM.Games.Find(id);
        }
        public static List<Genre> GetGenres()
        {
            return ORM.Genres.ToList();
        }
    }
}