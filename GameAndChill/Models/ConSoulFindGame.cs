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
            
            /*Ans1 = ORM.Answers.Find(new { QuestionID = 1, UserID = user.ID });
            Ans2 = ORM.Answers.Find(new { QuestionID = 2, UserID = user.ID });
            Ans3 = ORM.Answers.Find(new { QuestionID = 3, UserID = user.ID });
            Ans4 = ORM.Answers.Find(new { QuestionID = 4, UserID = user.ID });
            Ans5 = ORM.Answers.Find(new { QuestionID = 5, UserID = user.ID });*/

        }

        GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        public User User { get; set; }
        /*public Answer Ans1 { get; set; }
        public Answer Ans2 { get; set; }
        public Answer Ans3 { get; set; }
        public Answer Ans4 { get; set; }
        public Answer Ans5 { get; set; }*/

        /*public List<Game> Result
        {
            get
            {
                return result();
            }
            private set
            {
                value = result();
            }
        }*/


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
            // skeleton of the path we take to get games from the user's answers
            List<Game> result = new List<Game>();
            
            foreach (Answer a in User.Answers)
            {
                List<Genre> list = GetGenresFromAnswer(a);
                foreach(Genre g in list)
                {
                    foreach(Game game in g.Games)
                    {
                        if (ORM.User_Game.Where(x => x.UserID == User.ID && x.GameID == game.ID).Count() == 0)
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