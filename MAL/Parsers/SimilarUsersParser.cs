using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

namespace MAL.Parsers
{
    public static class SimilarUsersParser
    {
        public static IEnumerable<string> GetSimilarUsernames(this IHtmlDocument htmlDocument)
        {
            var borderClasses = htmlDocument
                .All
                .Where(x => x.ClassName == "borderClass")
                .ToList();

            return borderClasses.Count == 0 
                ? Enumerable.Empty<string>() 
                : borderClasses.Select(x => x.Children[0].TextContent);
        }
    }
}