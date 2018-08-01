using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.CSOM
{
    public class SPSecurity
    {
        private static SharePointOnlineCredentials sharePointOnlineCredentials;
        private static CookieContainer cookieContainer;

        private static Uri _cookieWebUri;
        private static string _cookieUserName;
        private static string _cookiePassword;

        public SPSecurity() { }
        /// <summary>
        /// Builds a SharePointOnlineCredentials object
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>An object with the provided parameters</returns>
        public SharePointOnlineCredentials GetSharePointOnlineCredentials(string username, string password)
        {
            if (sharePointOnlineCredentials == null)
            {
                var securePassword = new SecureString();
                foreach (char c in password)
                {
                    securePassword.AppendChar(c);
                }

                sharePointOnlineCredentials = new SharePointOnlineCredentials(username, securePassword);
            }
            return sharePointOnlineCredentials;
        }
        /// <summary>
        /// Builds a CookieContainer for SharePoint operations
        /// </summary>
        /// <param name="webUri">The Web URI</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>A CookieContainer with the provided parameters</returns>
        public CookieContainer GetCookieContainer(Uri webUri, string username, string password)
        {
            if (cookieContainer == null || _cookieWebUri.ToString() != webUri.ToString() ||
                _cookieUserName != username || _cookiePassword != password)
            {
                _cookieWebUri = webUri;
                _cookieUserName = username;
                _cookiePassword = password;

                cookieContainer = new CookieContainer();

                var credentials = GetSharePointOnlineCredentials(username, password);
                var authCookie = credentials.GetAuthenticationCookie(webUri);

                cookieContainer.SetCookies(webUri, authCookie);
            }
            return cookieContainer;
        }
    }
}
