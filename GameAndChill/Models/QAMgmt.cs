using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class QAMgmt
    {
        static GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        public static List<Question> GetQuestions()
        {
            return ORM.Questions.ToList();
        }
        public static bool ValidAnswer(int q)
        {
            // Check if the answer passed in is between 1 and 5. If not, we don't want that in our database and messing anything up!
            if (q <= 5 && q >= 1)
            {
                return true;
            }
            return false;
        }
        public static bool ManageAnswers(int UserID, int[] answers, bool exists)
        {
            for (int i = 0; i < 5; i++)
            {
                // Validation
                if (!ValidAnswer(answers[i]))
                {
                    return false;
                }
            }
            
            for (int i = 0; i < 5; i++)
            {
                // create Question object and define its properties
                Answer q = new Answer();
                q.UserID = UserID;
                q.QuestionID = i+1;
                q.Answer1 = answers[i];

                if (exists)
                {
                    // modify entry in DB
                    ORM.Entry(q).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    // add to DB
                    ORM.Answers.Add(q);
                }
            }

            ORM.SaveChanges();
            return true;
        }
    }
}