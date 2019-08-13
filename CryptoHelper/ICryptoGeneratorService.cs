using System;

namespace CryptoHelper
{
    public interface ICryptoGenerator
    {
        Tuple<string, string> GenerateKeyPair();
    }
}