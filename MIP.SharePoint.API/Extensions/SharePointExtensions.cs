using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Extensions
{
    public static class SharePointExtensions
    {
        public static void ExecuteQueryWithIncrementalRetry(this ClientContext context, int retryCount = 5, int delay = 30000)
        {
            int retryAttempts = 0;
            var backOffInterval = delay;

            if (retryCount <= 0 || delay <= 0)
                throw new ArgumentException("Provide a valid retry count and/or delay.");

            while(retryAttempts < retryCount)
            {
                try
                {
                    context.ExecuteQuery();
                    return;
                }
                catch(WebException wex)
                {
                    var response = wex.Response as HttpWebResponse;
                    if (response != null || response.StatusCode == (HttpStatusCode)429)
                    {
                        System.Threading.Thread.Sleep(backOffInterval);
                        retryAttempts++;
                        backOffInterval *= 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            throw new Exception($"Maximum retry attempts {retryCount} have been attempted.");

        }
    }
}
