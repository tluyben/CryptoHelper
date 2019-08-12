using System;

namespace CryptoHelper.REST
{
    public class ServerKeysStore : IServerKeysStore
    {
        private Tuple<string, string> _keys;
        public void Update(Tuple<string, string> keys)
        {
            _keys = keys;
        }

        public Tuple<string, string> Get()
        {
            return _keys;
        }
    }
}