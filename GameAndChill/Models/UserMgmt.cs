using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class UserMgmt
    {
        private static GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        public static int AddUserReturnID(User newUser)
        {
            // add to DB
            ORM.Users.Add(newUser);
            ORM.SaveChanges();

            // get the ID of our new user
            int id = ORM.Users.Where(x => x.Name == newUser.Name).ToList().Last().ID;

            return id;
        }
        public static User GetUser(int id)
        {
            return ORM.Users.Find(id);
        }
        public static List<Answer> GetAnswers(int id)
        {
            User currentUser = GetUser(id);
            return currentUser.Answers.ToList();
        }
        public static bool RemoveLike(int UserID, int GameID)
        {
            User user = GetUser(UserID);

            // check if user exists
            if (user == null)
            {
                return false;
            }

            // get entry in DB where the user liked or disliked a game, then remove it
            User_Game ug = user.User_Game.Where(x => x.GameID == GameID).First();
            if (ug != null)
            {
                ORM.User_Game.Remove(ug);
            }

            ORM.SaveChanges();
            return true;
        }

        public static bool LikeDislikeGame(int userID, int gameID, bool isLike)
        {
            if (GetUser(userID) == null)
            {
                return false;
            }
            
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
            return true;
        }
    }
}