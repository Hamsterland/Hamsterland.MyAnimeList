using System.Linq;
using System.Text.RegularExpressions;
using MAL.Pages;

namespace MAL.Parsers
{
    public static class CommentsParser
    {
        private static readonly Regex _username = new("(?<Username>(.?)+)'s Comments");
        
        public static string GetUsername(this CommentsPage commentsPage)
        {
            return _username
                .Matches(commentsPage.HtmlDocument
                    .GetElementById("contentWrapper")
                    .FirstChild
                    .TextContent)
                .FirstOrDefault()
                .Groups["Username"]
                .Value;
        }
    }
}