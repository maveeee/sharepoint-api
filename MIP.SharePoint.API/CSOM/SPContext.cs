using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.CSOM
{
    public enum SharePointType
    {
        Online,
        OnPrem,
    }
    /// <summary>
    /// Provides a ClientContext model, which can be used to access the SharePoint
    /// </summary>
    public class SPContext
    {
        private static ClientContext clientContext;

        private static string _url;
        private static string _password;
        private static string _username;

        public SPContext() { }

        public void ForceClientContextToReload()
        {
            clientContext = null;
        }

        /// <summary>
        /// A ClientContext object to query the SharePoint
        /// </summary>
        /// <param name="url">The URL parameter of the SharePoint</param>
        /// <param name="username">The Username of a SharePoint User</param>
        /// <param name="password">The Password of a Sharepoint User</param>
        /// <returns>A ClientContext Instance</returns>
        public ClientContext GetInstance(string url, string username, string password, SharePointType sharePointType)
        {
            if (clientContext == null || (url != _url) || (username != _username) || (password != _password))
            {
                clientContext = GetContext(url, username, password, sharePointType);
                _url = url;
                _username = username;
                _password = password;
            }

            return clientContext;

        }
        /// <summary>
        /// Creates a ClientContext object
        /// </summary>
        /// <param name="url">The URL parameter of the SharePoint</param>
        /// <param name="username">The Username of a SharePoint User</param>
        /// <param name="password">The Password of a Sharepoint User</param>
        /// <returns>A ClientContext Instance</returns>
        private ClientContext GetContext(string url, string username, string password, SharePointType sharePointType)
        {
            if (sharePointType == SharePointType.Online)
            {
                return new ClientContext(url)
                {
                    Credentials = new SPSecurity().GetSharePointOnlineCredentials(username, password)
                };
            }
            else if (sharePointType == SharePointType.OnPrem)
            {
                return new ClientContext(url)
                {
                    Credentials = new NetworkCredential(username, password)
                };
            }
            throw new ArgumentException("SharePoint Type was not specified!");
        }
    }
}
