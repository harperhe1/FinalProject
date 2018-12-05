using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class ConSoulFindGame
    {
        public ConSoulFindGame() { }
        public ConSoulFindGame(int userID)
        {
            User = ORM.Users.Find(userID);
        }
        public ConSoulFindGame(User user)
        {
            User = user;
        }

        GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        public User User { get; set; }
        public List<Game> Result
        {
            get
            {
                return result();
            }
            private set
            {
                value = result();
            }
        }


        private List<Game> result()
        {
            // placeholder. This will return our list of suggested games
            return null;
        }
        
    }
}