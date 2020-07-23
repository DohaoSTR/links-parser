using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace GetAllLinksFromHtmlPage
{
    public class Program
    {
        private static readonly OutMethod writeLine = Console.WriteLine;
        private static readonly InMethod readLine = Console.ReadLine;

        private delegate void OutMethod(string str);
        private delegate string InMethod();

        public static void Main()
        {
            string url = SplitParams(readLine())["urlInput"];

            url = Uri.UnescapeDataString(url);

            writeLine("Content-Type: text/html\n\n");
            writeLine("<html>\n<head>");
            writeLine("\t<title>LAB_CGI</title>");
            writeLine("</head>\n");
            writeLine("<body>\n");
            writeLine("<p>Все ссылки располагающиеся на указанном сайте: " + url + "</p>\n"); ;
            writeLine("<ol>\n");

            string html = GetHtmlCode(url);

            foreach (string link in GetAllLinks(html, url))
            {
                writeLine("<li>\n<a href = \"" + link + "\">\n" + link + "</a>\n</li>\n");
            }

            writeLine("</ol>\n");
            writeLine("</body>\n</html>");
        }

        private static Dictionary<string, string> SplitParams(string stringWithParams)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(stringWithParams))
            {
                return result;
            }

            string[] ps = stringWithParams.Split('&');
            string[] a;

            foreach (string p in ps)
            {
                a = p.Split('=');
                result.Add(a[0], a[1]);
            }

            return result;
        }

        private static string GetHtmlCode(string url)
        {
            WebClient client = new WebClient
            {
                Credentials = CredentialCache.DefaultNetworkCredentials
            };

            return client.DownloadString(url);
        }

        private static List<string> GetAllLinks(string html, string url)
        {
            List<string> listLinks = new List<string>();

            Regex regexForAllLinks = new Regex("(?: href| src)=[\"|']?(.*?)[\"|'|>]+");

            Regex regexForLinksWithoutProtocol = new Regex("^/");

            if (regexForAllLinks.IsMatch(html))
            {
                foreach (Match matchAllLinks in regexForAllLinks.Matches(html))
                {
                    if (regexForLinksWithoutProtocol.IsMatch(matchAllLinks.Groups[1].Value))
                    {
                        listLinks.Add(url + matchAllLinks.Groups[1].Value);
                    }
                    else
                    {
                        listLinks.Add(matchAllLinks.Groups[1].Value);
                    }
                }
            }
            else
            {
                throw new Exception("На html странице ссылок не найдено!");
            }

            return listLinks;
        }
    }
}

