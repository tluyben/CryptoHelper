using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoHelper
{
    interface ICryptoHelper
    {
        Tuple<string, string> GenerateKeyPair();

        string SignMessage(string message, string privateKey);

        bool VerifyMessage(string message, string signature, string publicKey);

        string EncryptMessage(string message, string publicKey);

        string DecryptMessage(string message, string privateKey);
    }
}
