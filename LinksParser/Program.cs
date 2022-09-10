using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace LinksParser
{
    public class Program
    {
        private static readonly OutMethod _writeLine = Console.WriteLine;
        private static readonly InMethod _readLine = Console.ReadLine;

        private delegate void OutMethod(string inputString);

        private delegate string InMethod();

        public static void Main()
        {
            string url = SplitParams(_readLine())["urlInput"];

            url = Uri.UnescapeDataString(url);

            _writeLine("Content-Type: text/html\n\n");
            _writeLine("<html>\n<head>");
            _writeLine("\t<title>LAB_CGI</title>");
            _writeLine("</head>\n");
            _writeLine("<body>\n");
            _writeLine("<p>Все ссылки располагающиеся на указанном сайте: " + url + "</p>\n"); ;
            _writeLine("<ol>\n");

            string html = GetHtmlCode(url);

            foreach (string link in GetAllLinks(html, url))
            {
                _writeLine("<li>\n<a href = \"" + link + "\">\n" + link + "</a>\n</li>\n");
            }

            _writeLine("</ol>\n");
            _writeLine("</body>\n</html>");
        }

        private static Dictionary<string, string> SplitParams(string stringParams)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(stringParams))
            {
                return result;
            }

            string[] arrayParams = stringParams.Split('&');
            string[] arraySplitParams;

            foreach (string parametr in arrayParams)
            {
                arraySplitParams = parametr.Split('=');
                result.Add(arraySplitParams[0], arraySplitParams[1]);
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