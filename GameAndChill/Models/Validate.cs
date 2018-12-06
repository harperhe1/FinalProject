using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public static class Validate
    {
        static GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        public static bool UserExists(int userID, out string Error)
        {
            if(ORM.Users.Find(userID) == null)
            {
                Error = "User Not Found";
                return false;
            }
            Error = "";
            return true;
        }
        public static bool GameExists(int gameID,out string Error)
        {
            if (ORM.Games.Find(gameID) == null)
            {
                Error = "Game Not Found";
                return false;
            }
            Error = "";
            return true;
        }
        //public static bool
    }
}