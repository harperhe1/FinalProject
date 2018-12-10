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
        public static bool RemoveLike(int userID, int gameID)
        {
            // check if user exists
            if (GetUser(userID) == null || GameMgmt.GetGame(gameID) == null)
            {
                return false;
            }

            // get entry in DB where the user liked or disliked a game, then remove it
            User_Game ug = ORM.User_Game.Find(userID, gameID);
            if (ug != null)
            {
                ORM.User_Game.Remove(ug);
            }

            ORM.SaveChanges();
            return true;
        }

        public static bool LikeDislikeGame(int userID, int gameID, bool isLike, out string status)
        {
            if (GetUser(userID) == null)
            {
                status = "User not found";
                return false;
            }
            if(GameMgmt.GetGame(gameID) == null)
            {
                status = "Game not found";
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

            string like = "";
            if (isLike) { like = "liked"; }
            else { like = "disliked"; }

            status = $"Added to {like} games";
            return true;
        }

        public static bool DeleteUser(int id, out string status)
        {
            //Find user ID
            User userDelete = GetUser(id);

            if (userDelete == null)
            {
                status = "User not found";
                return false;
            }

            //Get their name
            string name = userDelete.Name;

            //Remove the user ID
            var userGames = userDelete.User_Game.ToList();
            foreach (User_Game user_Game in userGames)
            {
                ORM.User_Game.Remove(user_Game);
            }
            var userAnswers = userDelete.Answers.ToList();
            foreach (Answer answer in userAnswers)
            {
                ORM.Answers.Remove(answer);
            }
            ORM.Users.Remove(userDelete);

            //SaveChanges duhhhhhhhh (Kidding, for real though it does save the changes to the DB)
            ORM.SaveChanges();

            status = $"{name} has been deleted.";

            return true;
        }
    }
}