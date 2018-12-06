using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class ConSoulFindGame
    {
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



        private List<Genre> GetGenresFromAnswer(Answer answer)
        {
            List<Genre> list = new List<Genre>();
            foreach (Question_Genre qg in answer.Question.Question_Genre)
            {
                if(answer.Answer1 == qg.Answer)
                {
                    list.Add(qg.Genre);
                }
            }
            return list;
        }

        public List<Game> Result()
        {
            // TODO: Fix this at somepoint. Also possibly put the info into a Cookie
            // skeleton of the path we take to get games from the user's answers
            List<Game> result = new List<Game>();
            
            foreach (Answer a in User.Answers)
            {
                List<Genre> list = GetGenresFromAnswer(a);
                foreach(Genre g in list)
                {
                    foreach(Game game in g.Games)
                    {
                        if (game.User_Game.Where(x => x.UserID == User.ID).Count() == 0)
                        {
                            result.Add(game);
                        }
                    }
                }
            }
            return result;
        }
        
    }
}