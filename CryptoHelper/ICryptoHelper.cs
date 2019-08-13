using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoHelper
{
    public interface ICryptoHelper
    {
        
        string SignMessage(string message, string privateKey);

        bool VerifyMessage(string message, string signature, string publicKey);

        string EncryptMessage(string message, string publicKey);

        string DecryptMessage(string message, string privateKey);
    }
}
