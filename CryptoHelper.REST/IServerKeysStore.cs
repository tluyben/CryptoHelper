using System;

namespace CryptoHelper.REST
{
    public interface IServerKeysStore
    {
        void Update(Tuple<string, string> keys);

        Tuple<string, string> Get();
    }
}