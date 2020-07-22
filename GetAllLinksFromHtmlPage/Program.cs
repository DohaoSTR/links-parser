using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace GetAllLinksFromHtmlPage
{
    class Program
    {
        static Dictionary<string, string> SplitParams(string pstr)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(pstr)) return res;
            string[] ps = pstr.Split('&');
            string[] a;
            foreach (string p in ps)
            {
                a = p.Split('=');
                res.Add(a[0], a[1]);
            }
            return res;
        }

        delegate void OutMethod(string str);
        delegate string InMethod();

        static OutMethod wl = Console.WriteLine;
        static InMethod rl = Console.ReadLine;

        static void Main()
        {
            string url = SplitParams(rl())["urlInput"];
            url = Uri.UnescapeDataString(url);
            wl("Content-Type: text/html\n\n");
            wl("<html>\n<head>");
            wl("\t<title>LAB_CGI</title>");
            wl("</head>\n");
            wl("<body>\n");
            wl("<p>Все ссылки располагающиеся на указанном сайте: " + url + "</p>\n"); ;
            wl("<ol>\n");
            string html = GetHtmlCode(url);
            foreach (string link in GetAllLinks(html, url))
                wl("<li>\n<a href = \"" + link + "\">\n" + link + "</a>\n</li>\n");
            wl("</ol>\n");
            wl("</body>\n</html>");
        }

        public static string GetHtmlCode(string url)
        {
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
            return client.DownloadString(url);
        }

        public static List<string> GetAllLinks(string html, string url)
        {
            List<string> listLinks = new List<string>();
            Regex regexForAllLinks = new Regex("(?: href| src)=[\"|']?(.*?)[\"|'|>]+");
            Regex regexForLinksWithoutProtocol = new Regex("^/");
            if (regexForAllLinks.IsMatch(html))
            {
                foreach (Match matchAllLinks in regexForAllLinks.Matches(html))
                {
                    if (regexForLinksWithoutProtocol.IsMatch(matchAllLinks.Groups[1].Value))
                        listLinks.Add(url + matchAllLinks.Groups[1].Value);
                    else
                        listLinks.Add(matchAllLinks.Groups[1].Value);
                }
            }
            return listLinks;
        }
    }
}

