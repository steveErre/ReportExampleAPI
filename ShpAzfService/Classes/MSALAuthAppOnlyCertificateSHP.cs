using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ShpAzfService.Classes
{
    class MSALAuthAppOnlyCertificateSHP
    {
        public static async Task<ClientContext> sampleInvocationAppOnly(string nameSite, ILogger? log) // passare come argomento "ILogger log" per azure function
        {
            /*
             * Register Azure AD App 
             * Upload certificate 
             * Add permission APPLICATION -> SharePoint -> Sites.FullControl.All 
             * Give admin consent
            */

            string tenantName = Environment.GetEnvironmentVariable("TenantName", EnvironmentVariableTarget.Process);
            log.LogInformation("tenant: " + tenantName);

            string siteUrl = $"https://{tenantName}.sharepoint.com";
            if (!string.IsNullOrWhiteSpace(nameSite))
            {
                siteUrl += $"/sites/{nameSite}";
            }
            log.LogInformation("site url: " + siteUrl);

            string clientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Process);
            log.LogInformation("clientId: " + clientId);
            string certThumprint = Environment.GetEnvironmentVariable("Thumbprint", EnvironmentVariableTarget.Process);
            log.LogInformation("certThumprint: " + certThumprint);

            //For SharePoint app only auth, the scope will be the SharePoint tenant name followed by /.default
            var scopes = new string[] { $"https://{tenantName}.sharepoint.com/.default" };

            //Tenant id can be the tenant domain or it can also be the GUID found in Azure AD properties.
            string tenantId = $"{tenantName}.onmicrosoft.com";

            // metodo con path e pwd: var accessToken = await GetApplicationAuthenticatedClient(clientId, certThumprint, scopes, tenantId, path, pwd);
            string accessToken = await MSALAuthAccessToken.GetApplicationAuthenticatedClient(clientId, certThumprint, scopes, tenantId,log);

            var clientContext = GetClientContextWithAccessToken(siteUrl, accessToken);

            
            return clientContext;

        }

        //internal static async Task<string> GetApplicationAuthenticatedClient(string clientId, string certThumprint, string[] scopes, string tenantId, string path, string pwd)
        //{
        //    X509Certificate2 certificate = GetAppOnlyCertificateFromPath(certThumprint, path, pwd); // read from disk to test it
        //    IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
        //                                    .Create(clientId)
        //                                    .WithCertificate(certificate)
        //                                    .WithTenantId(tenantId)
        //                                    .Build();

        //    AuthenticationResult authResult = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync();
        //    string accessToken = authResult.AccessToken;
        //    return accessToken;
        //}

        public static ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
        {
            ClientContext clientContext = new ClientContext(targetUrl);
            clientContext.ExecutingWebRequest +=
                delegate (object oSender, WebRequestEventArgs webRequestEventArgs)
                {
                    webRequestEventArgs.WebRequestExecutor.RequestHeaders["Authorization"] =
                        "Bearer " + accessToken;
                };
            return clientContext;
        }


        //private static X509Certificate2 GetAppOnlyCertificateFromPath(string thumbPrint, string path, string pwd)
        //{
        //    string certPath = System.IO.Path.GetFullPath(path);
        //    X509Certificate2 certificate = new X509Certificate2(certPath, pwd, X509KeyStorageFlags.MachineKeySet);
        //    return certificate;
        //}
    }
}
