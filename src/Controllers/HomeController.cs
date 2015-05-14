using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Button.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace Button.Controllers
{
    public class HomeController : Controller
    {
        private const string KlifaLt = "klifa.lt";
        private const string DKey = "key";
        private const string DCount = "count";
        private static readonly object Mutex = new object();

        public ActionResult Index()
        {
            var cookie = Request.Cookies[KlifaLt];
            var dict = GetDict();

            if (!VotedYet(cookie, dict))
                return View("Button");

            return View("Index", (object)dict[DCount]);
        }

        public ActionResult Button()
        {
            return View();
        }

        public ActionResult Click()
        {

            var cookie = Request.Cookies[KlifaLt];
            var dict = GetDict();

            if (!VotedYet(cookie, dict))
            {
                dict[DCount] = (ToInt(dict[DCount]) + 1).ToString(CultureInfo.InvariantCulture);
                SaveDict(dict);

                Response.Cookies.Set(new HttpCookie(KlifaLt, dict[DKey])
                {
                    Expires = DateTime.Now.AddYears(1)
                });

                var context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
                context.Clients.All.hello(dict[DCount]);
            }


            return RedirectToAction("Index");
        }

        public ActionResult Reset()
        {
            InitDict();

            return RedirectToAction("Button");
        }

        private Dictionary<string, string> InitDict()
        {
            var dict = new Dictionary<string, string>();
            dict[DKey] = Guid.NewGuid().ToString("N");
            dict[DCount] = "0";
            SaveDict(dict);
            return dict;
        }

        private static int ToInt(string s)
        {
            int number;
            return int.TryParse(s, out number) ? number : 0;
        }

        private static bool VotedYet(HttpCookie cookie, IDictionary<string, string> dict)
        {
            return cookie != null
                   && !string.IsNullOrEmpty(cookie.Value)
                   && dict.ContainsKey(DKey)
                   && dict[DKey] == cookie.Value;
        }

        private string FilePath
        {
            get { return Server.MapPath("~/data.txt"); }
        }

        public void Serialize(IDictionary<string, string> dictionary, Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
            writer.Flush();
        }

        public Dictionary<string, string> Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var count = reader.ReadInt32();
            var dictionary = new Dictionary<string, string>(count);
            for (var n = 0; n < count; n++)
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        private IDictionary<string, string> GetDict()
        {
            lock (Mutex)
            {
                try
                {
                    using (var fs = System.IO.File.OpenRead(FilePath))
                    {
                        return Deserialize(fs);
                    }
                }
                catch
                {
                    return InitDict();
                }
            }
        }

        private void SaveDict(IDictionary<string, string> dict)
        {
            lock (Mutex)
            {
                using (var fs = System.IO.File.OpenWrite(FilePath))
                {
                    Serialize(dict, fs);
                }
            }
        }
    }

}