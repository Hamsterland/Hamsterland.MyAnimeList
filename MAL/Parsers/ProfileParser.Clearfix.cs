using System;
using System.Linq;
using MAL.Models;
using MAL.Pages;

namespace MAL.Parsers
{
    public partial class ProfileParser
    {
        [ClearfixProperty]
        public static string GetLastOnline(this ProfilePage profilePage)
        {
            return profilePage.ClearfixElements
                .FirstOrDefault()?
                .Children[1]
                .TextContent;
        }
        
        [ClearfixProperty]
        public static Gender GetGender(this ProfilePage profilePage)
        {
            if (!profilePage.IsElementSpecified("Gender"))
            {
                return Gender.Unspecified;
            }

            var gender = profilePage.ClearfixElements[1]
                .Children[1]
                .TextContent;

            return gender switch
            {
                "Non-Binary" => Gender.NonBinary,
                "Female" => Gender.Female,
                "Male" => Gender.Male,
                _ => Gender.Unspecified
            };
        }

        [ClearfixProperty]
        public static string GetBirthday(this ProfilePage profilePage)
        {
            if (!profilePage.IsElementSpecified("Birthday"))
            {
                return null;
            }

            var index = 2;
            
            if (!profilePage.IsElementSpecified("Gender"))
            {
                index = 1;
            }

            return profilePage.ClearfixElements[index]
                .Children[1]
                .TextContent;
        }

        [ClearfixProperty]
        public static string GetLocation(this ProfilePage profilePage)
        {
            if (profilePage.IsElementSpecified("Location"))
            {
                return profilePage.ClearfixElements[^3]
                    .Children[1]
                    .TextContent;
            }

            return null;
        }

        [ClearfixProperty]
        public static DateTime GetDateJoined(this ProfilePage profilePage)
        {
            return DateTime.Parse(profilePage.ClearfixElements[^2]
                .Children[1]
                .TextContent);
        }

        private static bool IsElementSpecified(this ProfilePage profilePage, string name)
        {
            return profilePage.ClearfixElements.Any(x => x.InnerHtml.Contains(name));
        }
    }
}