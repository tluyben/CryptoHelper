using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;

namespace CryptoHelper.REST
{
    public class ClientClientKeysStore :IClientKeysStore
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        
        public ClientClientKeysStore(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        
        public void Save(string deviceId, string publicKey)
        {
            using (var writer = File.CreateText(_hostingEnvironment.WebRootPath + "/" + deviceId))
            {
                writer.WriteLine(publicKey);
            }
        }

        public string Get(string deviceId)
        {
            return File.ReadAllText(_hostingEnvironment.WebRootPath + deviceId);
        }
    }
}