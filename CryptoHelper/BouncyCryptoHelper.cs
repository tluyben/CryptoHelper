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
        private readonly IAsymmetricCipherKeyPairGenerator _keyPairGenerator;

        public BouncyCryptoHelper(string name) : this(name, 4096, 512){}

        private readonly int _shaImpl;

        public BouncyCryptoHelper(string name, int bits, int shaImpl)
        {
            _keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator(name);
            _keyPairGenerator.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(17), new SecureRandom(), bits, 25));
            _shaImpl = shaImpl; 
        }

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

        public Tuple<string, string> GenerateKeyPair()
        {
            var keys =_keyPairGenerator.GenerateKeyPair();
            return  new Tuple<string, string>(PublicConvert(keys.Public), PrivateConvert(keys.Private));
        }

        private IDigest GetShaDigest()
        {
            IDigest shaDigest = null;

            if (_shaImpl == 1)
            {
                shaDigest = new Sha1Digest();
            }
            else if (_shaImpl == 3)
            {
                shaDigest = new Sha3Digest();
            }
            else if (_shaImpl == 224)
            {
                shaDigest = new Sha224Digest();
            }
            else if (_shaImpl == 256)
            {
                shaDigest = new Sha256Digest();
            }
            else if (_shaImpl == 384)
            {
                shaDigest = new Sha384Digest();
            }
            else
            {
                shaDigest = new Sha512Digest(); 
            }
            return shaDigest;
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

        private string PublicConvert(AsymmetricKeyParameter key)
        {
            var privateKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPrivateBytes);
        }        
         
        private string PrivateConvert(AsymmetricKeyParameter key)
        {
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(key);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPrivateBytes);
        }

    }
}
