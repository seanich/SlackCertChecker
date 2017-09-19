using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CertChecker
{
    public interface ICertificateReader
    {
        Task<X509Certificate2> GetCertificate(string hostname);
    }
}