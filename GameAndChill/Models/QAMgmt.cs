using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class QAMgmt
    {
        static GameAndChillDBEntities ORM = new GameAndChillDBEntities();
        public static void AddAnswer(int UserID, int answer, int qNum)
        {
            // create Question object and define its properties
            Answer q = new Answer();
            q.UserID = UserID;
            q.QuestionID = qNum;
            q.Answer1 = answer;

            // add to DB
            ORM.Answers.Add(q);
        }
        public static void EditAnswer(int UserID, int answer, int qNum)
        {
            // create Question object and define its properties
            Answer q = new Answer();
            q.UserID = UserID;
            q.QuestionID = qNum;
            q.Answer1 = answer;

            // modify entry in DB
            ORM.Entry(q).State = System.Data.Entity.EntityState.Modified;
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
        public static bool AddAnswer(int UserID, int[] answers)
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
                AddAnswer(UserID, answers[i], i + 1);
            }

            ORM.SaveChanges();
            return true;
        }
    }
}