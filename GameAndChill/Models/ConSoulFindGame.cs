using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public partial class Game
    {
        public int Priority { get; set; }
        public void DecreasePriority()
        {
            int toDecrease = Genres.Count - 3;
            if (Priority > toDecrease)
            {
                Priority -= toDecrease;
            }
            else
            {
                Priority = 1;
            }
        }
    }

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
                if (answer.Answer1 == qg.Answer)
                {
                    list.Add(qg.Genre);
                }
            }
            return list;
        }

        public List<Game> Result()
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();

            // TODO: Priority Queue
            List<Game> result = new List<Game>();
            
            foreach (Answer a in User.Answers)
            {
                // get every genre that corrolates with the user's answer
                List<Genre> list = GetGenresFromAnswer(a);
                foreach (Genre g in list)
                {
                    // get every game that has that genre
                    foreach (Game game in g.Games)
                    {
                        // only add unique games; increment priority every time it shows up
                        game.Priority++;
                        if (game.Priority == 1)
                        {
                            result.Add(game);
                        }
                    }
                }
            }

            // run back through our list
            for (int i = 0; i < result.Count; i++)
            {
                // if user already liked it, take it out
                if (result[i].User_Game.Where(x => x.UserID == User.ID).Count() != 0)
                {
                    result.Remove(result[i]);
                    i--;
                    continue;
                }

                // if game has more than 3 genres, modify priority
                // TODO: decide if it should be 3 or 4
                if (result[i].Genres.Count > 3)
                {
                    result[i].DecreasePriority();
                }
            }
            Random r = new Random();
            //stopwatch.Stop();

            // randomize then order by priority
            return result.OrderBy(g => r.Next()).OrderByDescending(g => g.Priority).ToList();
        }

    }
}