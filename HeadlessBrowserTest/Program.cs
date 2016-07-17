using System;
using Awesomium.Core;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;



namespace HeadlessBrowserTest
{
    class Program
    {
        static void Main(string[] args)
        {
           
            var instagramUser = new InstagramUser()
            {
                Username = ConfigurationManager.AppSettings["username"],
                Password = ConfigurationManager.AppSettings["password"]
            };

            instagramUser.Login();

            //TODO: DO I NEED TO WAIT UNTIL LOGIN HAS FIRED CERTAIN EVENT??
            instagramUser.FollowUsersFollowers(ConfigurationManager.AppSettings["userFollowsToSteal"]);


            WebCore.Shutdown();
            
        }
    }
}
