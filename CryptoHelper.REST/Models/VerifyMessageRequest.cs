namespace CryptoHelper.REST.Models
{
    public class VerifyMessageRequest
    {
        public string message;
        public string signature;
        public string publicKey;
    }
}