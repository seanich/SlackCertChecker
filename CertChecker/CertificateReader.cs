using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CertChecker
{
    public class CertificateReader : ICertificateReader
    {
        public async Task<X509Certificate2> GetCertificate(string hostname)
        {
            X509Certificate2 cert = null;
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate, chain, policyErrors) =>
                {
                    cert = new X509Certificate2(certificate);
                    return true;
                }
            };
            var httpClient = new HttpClient(httpClientHandler);
            var uri = new UriBuilder("https", hostname).Uri;
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri));
            return cert;
        }
    }
}