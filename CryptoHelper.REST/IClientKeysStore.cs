namespace CryptoHelper.REST
{
    public interface IClientKeysStore
    {
        void Save(string deviceId, string publicKey);
        string Get(string deviceId);
    }
}