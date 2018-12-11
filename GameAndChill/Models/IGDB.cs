using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;//Json
using System.IO;//StringReader
using System.Net;//HttpWebRequest

namespace GameAndChill.Models
{
    public class IGDB
    {
        static string APIKey = System.Configuration.ConfigurationManager.AppSettings["user-key"];
        static string expander = "?expand=keywords,platforms,genres&fields=name,summary,url,cover,keywords.name,platforms.name,genres.name";
        
        // Call this method whenever we want to get a JObject of a specific game
        public static JObject GetGameByID(int id)
        {
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{id}{expander}");


            request.Headers.Add("user-key", APIKey);
            request.Accept = "application/json";
            //make our response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as string
                string output = reader.ReadToEnd();

                //convert response to JSon
                JArray GameInfo = JArray.Parse(output);
                reader.Close();

                // since our array is only one jObject we can return that jObject like so
                foreach (JObject game in GameInfo)
                {
                    return game;
                }
            }
            // return null if something goes wrong with the request/response
            return null;
        }

        // For Multiple Games
        public static JArray GetMultipleGamesByID(int[] id)
        {
            // takes all the ids in the array and puts it into a string that can be used in the url
            string ids = "";
            for (int i = 0; i < id.Length; i++)
            {
                ids = ids + id[i];
                if (i != id.Length - 1)
                {
                    ids = ids + ",";
                }
            }

            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{ids}{expander}");

            request.Headers.Add("user-key", APIKey);
            request.Accept = "application/json";
            //make our response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as string
                string output = reader.ReadToEnd();

                //convert response to JSon
                JArray GameInfo = JArray.Parse(output);
                reader.Close();

                return GameInfo;
            }
            // return null if something goes wrong with the request/response
            return null;
        }

        public static JArray GetAllTheGenres()
        {
            string ids = "";
            for (int i = 0; i < 34; i++)
            {
                ids = ids + i;
                if (i != 33)
                {
                    ids = ids + ",";
                }
            }
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/genres/{ids}?fields=id,name");

            request.Headers.Add("user-key", APIKey);
            request.Accept = "application/json";
            //make our response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as string
                string output = reader.ReadToEnd();

                //convert response to JSon
                JArray GenreInfo = JArray.Parse(output);
                reader.Close();

                return GenreInfo;
            }
            // return false if something goes wrong with the request/response
            return null;
        }


        // WIP
        public static JArray Get50GamesByGenre(int genreID)
        {
            string newExpander = "";

            //make our resquest
            // TODO: Create the correct request
            // We need the top 50 games from a genre, ordered by rating. Possibly older than 2000 if we want to stick to retro.
            HttpWebRequest request = WebRequest.CreateHttp($"https://api-endpoint.igdb.com/games/{newExpander}");

            request.Headers.Add("user-key", APIKey);
            request.Accept = "application/json";
            //make our response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //get response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                //read response stream as string
                string output = reader.ReadToEnd();

                //convert response to JSon
                JArray GameInfo = JArray.Parse(output);
                reader.Close();

                return GameInfo;
            }
            // return null if something goes wrong with the request/response
            return null;
        }
    }
}