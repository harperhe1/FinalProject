using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;//Json
using System.IO;//StringReader
using System.Net;//HttpWebRequest

namespace GameAndChill.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            GetGame1942();
            return View();
        }
        string APIKey = System.Configuration.ConfigurationManager.AppSettings["user-key"];

        public void GetGame1942()
        {
            //make our resquest
            HttpWebRequest request = WebRequest.CreateHttp("https://api-endpoint.igdb.com/games/1942");

            request.Headers.Add("user-key", APIKey);
            request.Accept="application/json";
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

                ViewBag.GameInfo = GameInfo;
                reader.Close();
            }


                
        }
    }
}