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
            int toDecrease = (Genres.Count -3)/2;
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
    public partial class Genre
    {
        public int Priority { get; set; } = 1;
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
        public static List<Game> games;
        
        public static void SetGames()
        {
            GameAndChillDBEntities ORM = new GameAndChillDBEntities();
            games = ORM.Games.ToList();
        }



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
        public List<Game> Result(out string stop)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // TODO: Priority Queue
            List<Game> result = new List<Game>();
            HashSet<Genre> genres = new HashSet<Genre>();
            var answers = User.Answers.ToList();
            foreach (Answer a in answers)
            {
                // get every genre that corrolates with the user's answer
                List<Genre> list = GetGenresFromAnswer(a);
                foreach (Genre g in list)
                {
                    g.Priority++;
                    if(!genres.Contains(g))
                    {
                        genres.Add(g);
                    }
                }
            }
            stop = stopwatch.ElapsedMilliseconds.ToString() + ":POST GENRE ";
            // get every game that has that genre
            int j = 0;
            var temp = genres.OrderByDescending(g => g.Priority).ToList();
            
            foreach (Genre genre in temp)
            {
                j++;
                foreach (Game game in genre.Games)
                {
                    // only add unique games; increment priority every time it shows up
                    game.Priority += genre.Priority;
                    if (game.Priority == genre.Priority)
                    {
                        result.Add(game);
                    }
                }
                if (j >= 3)
                {
                    break;
                }
            }
            stop += stopwatch.ElapsedMilliseconds.ToString() + ":POST GAMES ";
            // run back through our list
            var userGames = ORM.User_Game.ToList();
            for (int i = 0; i < result.Count; i++)
            {
                var remove = result[i];
                 //if user already liked it, take it out
                if (userGames.Where(x => x.UserID ==User.ID && x.GameID == remove.ID).Count() != 0)
                {
                    result.Remove(remove);
                    i--;
                    continue;
                }

                // if game has more than 3 genres, modify priority
                // TODO: decide if it should be 3 or 4
                // TODO: INCREASE EFFICENCY! adds about 12 seconds per 1000 games
                /*if (remove.Genres.Count > 4)
                {
                    remove.DecreasePriority();
                }*/
            }
            Random r = new Random();
            stop += stopwatch.ElapsedMilliseconds.ToString() + ":END";
            stopwatch.Stop();

            // randomize then order by priority
            
            return result.OrderBy(g => r.Next()).OrderByDescending(g => g.Priority).ToList();
        }

        public void MethodName(string x, ref string y)
        {
           
        }
    }
}