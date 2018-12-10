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
    }
}