﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndChill.Models
{
    public class QAMgmt
    {
        static GameAndChillDBEntities ORM = new GameAndChillDBEntities();

        // GET
        public static Question GetQuestion(int id)
        {
            return ORM.Questions.Find(id);
        }
        public static List<Question> GetAllQuestions()
        {
            return ORM.Questions.ToList();
        }
        public static List<bool> GenreCheckboxes(int qID, int aID)
        {
            List<bool> Checked = new List<bool>();
            foreach (var genre in ORM.Genres)
            {
                if (genre.Question_Genre.Where(x => x.QuestionID == qID && x.Answer == aID).Count() != 0)
                {
                    Checked.Add(true);
                }
                else
                {
                    Checked.Add(false);
                }
            }
            return Checked;
        }

        // POST
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
                SaveAndRefresh();
            }
            return true;
        }
        public static void CorrolateQuestions(List<bool> IsGenre, int qID, int aID)
        {
            List<Genre> Genres = ORM.Genres.ToList();
            int temp = 0;
            for (int i = 0; i < Genres.Count(); i++)
            {
                Question_Genre gQ = ORM.Question_Genre.Find(qID, Genres[i].ID, aID);
                if (IsGenre[temp])
                {
                    if (gQ == null)
                    {
                        gQ = new Question_Genre { QuestionID = qID, GenreID = Genres[i].ID, Answer = aID };
                        ORM.Question_Genre.Add(gQ);
                    }
                    temp++;
                }
                else
                {
                    if (gQ != null)
                    {
                        ORM.Question_Genre.Remove(gQ);
                    }
                }
                temp++;
            }
            SaveAndRefresh();
        }
        static void SaveAndRefresh()
        {
            ORM.SaveChanges();
            ORM = new GameAndChillDBEntities();
        }
    }
}