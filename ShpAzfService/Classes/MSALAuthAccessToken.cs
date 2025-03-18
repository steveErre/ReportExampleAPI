using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ShpAzfService.Classes
{
    public static class MSALAuthAccessToken
    {
        // method with cert pfx into store CURRENT USER->personal / webapp directly
        internal static async Task<string> GetApplicationAuthenticatedClient(string clientId, string certThumprint, string[] scopes, string tenantId, ILogger? log)
        {
            try
            {
                X509Certificate2 certificate = GetAppOnlyCertificate(certThumprint); // using this if read from Store/Azure WebApp directly 
                                                                                     // (or for Azure is best use Key Vault..) not implemented..

                IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                .Create(clientId)
                                                .WithCertificate(certificate)
                                                .WithTenantId(tenantId)
                                                .Build();

                AuthenticationResult authResult = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync();
                string accessToken = authResult.AccessToken;
                return accessToken;
            }
            catch(Exception err)
            {
                log?.LogWarning("errore certificate: " + err.Message);
                return null;
            }
            
        }

        // on webapp -> TLS/SSL settings -> private key certiticates (.pdfx) and upload with password..
        // then in application settings add: WEBSITE_LOAD_CERTIFICATES with value * for load all certificates, the thumbrint if want load only specific..
        // https://docs.microsoft.com/it-it/azure/app-service/configure-ssl-certificate-in-code
        // https://azure.microsoft.com/en-us/blog/using-certificates-in-azure-websites-applications/ 
        private static X509Certificate2 GetAppOnlyCertificate(string thumbPrint)
        {
            X509Certificate2 appOnlyCertificate = null;
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, false);
                if (certCollection.Count > 0)
                {
                    appOnlyCertificate = certCollection[0];
                }
                certStore.Close();
                return appOnlyCertificate;
            }
        }
    }
}
