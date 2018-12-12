using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class UserMgmt
    {
        public static int AddUserReturnID(User newUser)
        {
            // add to DB
            GameAndChillDBEntities ORM = new GameAndChillDBEntities();
            ORM.Users.Add(newUser);
            ORM.SaveChanges();

            // get the ID of our new user
            int id = ORM.Users.Where(x => x.Name == newUser.Name).ToList().Last().ID;

            return id;
        }
        public static User GetUser(int id)
        {
            GameAndChillDBEntities ORM = new GameAndChillDBEntities();
            return ORM.Users.Find(id);
        }
        public static List<User> GetAllTheUsers()
        {
            GameAndChillDBEntities ORM = new GameAndChillDBEntities();
            return ORM.Users.ToList();
        }
        public static List<Answer> GetAnswers(int id)
        {
            User currentUser = GetUser(id);
            return currentUser.Answers.ToList();
        }
        public static bool RemoveLike(int userID, int gameID)
        {
            // check if user exists
            GameAndChillDBEntities ORM = new GameAndChillDBEntities();
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
            //By doing it this way it will save the changes and then updated the ORM
            ORM.SaveChanges();
            return true;
        }

        public static bool LikeDislikeGame(int userID, int gameID, bool isLike, out string status)
        {
            //Check for if user exists
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
            GameAndChillDBEntities ORM = new GameAndChillDBEntities();
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
            GameAndChillDBEntities ORM = new GameAndChillDBEntities();
            User userDelete = ORM.Users.Find(id);

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