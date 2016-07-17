using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Core;
using System.Configuration;
using System.Diagnostics;


namespace HeadlessBrowserTest
{
    class InstagramUser
    {
        private WebSession session;
        private WebView view;
        private string loginURL = "https://www.instagram.com/accounts/login/";


        public InstagramUser()
        {
            WebCore.Initialize(new WebConfig()
            {
                LogPath = Environment.CurrentDirectory + ".awesomium.log",
                LogLevel = LogLevel.Verbose,
            });

            session = WebCore.CreateWebSession(new WebPreferences());
            view = WebCore.CreateWebView(1100, 600, session);
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool isUserLoggedIn { get; private set; }


        public void Login()
        {
            if (!isUserLoggedIn)
            {
                view.Source = new Uri(loginURL);
                view.LoadingFrameComplete += View_LoadingFrameComplete_LoginScreenLoaded;

                if (WebCore.UpdateState == WebCoreUpdateState.NotUpdating)
                {
                    WebCore.Run();
                }
            }
        }

        public void FollowUsersFollowers(string username)
        {
            if(isUserLoggedIn)
            {


            }

           // session.

            /*TODO: 
            * 1. Go to user
            * 2. Get Users follows & filter
            * 3. Send request for each user wanting to follow
            */


        }


        private void CompleteLoginFormAndSubmit()
        {
            MimicKeyboardInputIntoInputField("username", Username);

            MimicKeyboardInputIntoInputField("password", Password);

            dynamic submit = (JSObject)view.ExecuteJavascriptWithResult(
                "document.getElementsByClassName('_aj7mu _taytv _ki5uo _o0442')[0].getBoundingClientRect()");

            MimicMouseMovementAndClick((int)submit.left + 1, (int)submit.top + 1);

            view.LoadingFrameComplete -= View_LoadingFrameComplete_LoginScreenLoaded;
            view.LoadingFrameComplete += View_LoadingFrameComplete_LoggedIn;

        }



        #region EventHandlers


        private void View_LoadingFrameComplete_LoggedIn(object sender, FrameEventArgs e)
        {
            TakeSnapshot();

            isUserLoggedIn = true; //TODO: set true if save session and has cookie already.
        }

        private void View_LoadingFrameComplete_LoginScreenLoaded(object sender, FrameEventArgs e)
        {
            if (!e.IsMainFrame)
                return;

            CompleteLoginFormAndSubmit();
        }


        #endregion


        #region UserInteractions

        /// <summary>
        /// Types the supplied inputText into the designated inputField
        /// </summary>
        /// <param name="view">Webview containing the destination input field</param>
        /// <param name="inputFieldName">The name of the input field to type the string into</param>
        /// <param name="inputText">The text to be typed into the input field</param>
        private void MimicKeyboardInputIntoInputField(string inputFieldName, string inputText)
        {
            //bring focus to the destination field
            view.ExecuteJavascriptWithResult("document.getElementsByName('" + inputFieldName + "')[0].focus()");

            foreach (char c in inputText.ToCharArray())
            {
                var wke = new WebKeyboardEvent()
                {
                    Type = WebKeyboardEventType.KeyDown,
                    Text = c.ToString(),
                    UnmodifiedText = c.ToString(),
                    KeyIdentifier = c.ToString()
                };

                view.InjectKeyboardEvent(wke);
                wke.Type = WebKeyboardEventType.Char;
                view.InjectKeyboardEvent(wke);
                wke.Type = WebKeyboardEventType.KeyUp;
                view.InjectKeyboardEvent(wke);

            }

        }

        private void MimicMouseMovementAndClick( int x, int y)
        {
            view.InjectMouseMove((int)x, (int)y);
            view.InjectMouseDown(MouseButton.Left);
            view.InjectMouseUp(MouseButton.Left);
        }

        private void TakeSnapshot()
        {

            int docHeight = (int)view.ExecuteJavascriptWithResult("(function() { " +
           "var bodyElmnt = document.body; var html = document.documentElement; " +
           "var height = Math.max( bodyElmnt.scrollHeight, bodyElmnt.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight ); " +
           "return height; })();");

            view.Resize(view.Width, docHeight);

            BitmapSurface surface = (BitmapSurface)view.Surface;

            surface.SaveToJPEG("result.jpg", 100);

            Process.Start("result.jpg");
        }

        #endregion

    }
}
