using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Text;

namespace CryptoHelper
{
    public class BouncyCryptoHelper : ICryptoHelper
    {

        public string DecryptMessage(string message, string privateKey)
        {
            var bytesToDecrypt = Convert.FromBase64String(message);
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            decryptEngine.Init(false, PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)));
            return Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
        }

        public string EncryptMessage(string message, string publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            encryptEngine.Init(true, PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey)));
            return Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
        }

        private IDigest GetShaDigest()
        {            
            return new Sha512Digest();
        }

        public string SignMessage(string message, string privateKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);
            var signer = new RsaDigestSigner(GetShaDigest());
            signer.Init(true, PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)));
            signer.BlockUpdate(bytesToEncrypt, 0, bytesToEncrypt.Length);

            return Convert.ToBase64String(signer.GenerateSignature());
        }

        public bool VerifyMessage(string message, string signature, string publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);

            var signer = new RsaDigestSigner(GetShaDigest());
            signer.Init(false, PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey)));
            signer.BlockUpdate(bytesToEncrypt, 0, bytesToEncrypt.Length);

            return signer.VerifySignature(Convert.FromBase64String(signature));
        }
    }
}
