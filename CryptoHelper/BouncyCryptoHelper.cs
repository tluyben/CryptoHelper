﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.OpenSsl;

namespace CryptoHelper
{
    public class BouncyCryptoHelper : ICryptoHelper
    {
        private const string Algorithm = "RSA";
        private readonly IAsymmetricCipherKeyPairGenerator _keyPairGenerator;
        private readonly int _shaBits;
        
        public BouncyCryptoHelper(): this(4096, 256){}
        public BouncyCryptoHelper(int bits, int shaBits)
        {
            _keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator(Algorithm);
            _keyPairGenerator.Init(
                new RsaKeyGenerationParameters(
                    BigInteger.ValueOf(17), 
                    new SecureRandom(), 
                    bits, 
                    25));
            _shaBits = shaBits;
        }

        public string DecryptMessage(string message, string privateKey)
        {
            var bytesToDecrypt = Convert.FromBase64String(message);
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());
            decryptEngine.Init(false, GetPrivate(privateKey));
            return Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
        }

        public byte[] EncryptMessageAsBytes(string message, AsymmetricKeyParameter publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            encryptEngine.Init(true, publicKey);

            return encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
        }

        public string EncryptMessage(string message, string publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            encryptEngine.Init(true, GetPublic(publicKey));
            return Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
        }
        
        public static string ToPem(object obj)
        {
            using (var mem = new MemoryStream())
            {
                var writer = new StreamWriter(mem);
                var reader = new StreamReader(mem);

                var pem = new PemWriter(writer);
                pem.WriteObject(obj);
                pem.Writer.Flush();
                mem.Position = 0;

                return reader.ReadToEnd();
            }
        }

        

        public Tuple<AsymmetricKeyParameter, AsymmetricKeyParameter> GenerateKeyPairNoPem()
        {
            var keys = _keyPairGenerator.GenerateKeyPair();

            return new Tuple<AsymmetricKeyParameter, AsymmetricKeyParameter>(keys.Public, keys.Private);
        }
        public Tuple<string, string> GenerateKeyPair()
        {
            var keys = _keyPairGenerator.GenerateKeyPair();

            return new Tuple<string, string>(ToPem(keys.Public), ToPem(keys.Private));
        }

        private IDigest GetShaDigest()
        {
            IDigest shaDigest = null;

            if (_shaBits == 1)
            {
                shaDigest = new Sha1Digest();
            }
            else if (_shaBits == 3)
            {
                shaDigest = new Sha3Digest();
            }
            else if (_shaBits == 224)
            {
                shaDigest = new Sha224Digest();
            }
            else if (_shaBits == 256)
            {
                shaDigest = new Sha256Digest();
            }
            else if (_shaBits == 384)
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

            return Convert.ToBase64String(SignMessageAsBytes(message, privateKey));
        }
        
        public byte[] SignMessageAsBytes(string message, string privateKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);
            var signer = new RsaDigestSigner(GetShaDigest());

            signer.Init(true, GetPrivate(privateKey));
            signer.BlockUpdate(bytesToEncrypt, 0, bytesToEncrypt.Length);

            return signer.GenerateSignature(); 
        }

        public bool VerifyMessage(string message, byte[] signature, AsymmetricKeyParameter publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);
            var signer = new RsaDigestSigner(GetShaDigest());
            signer.Init(false, publicKey);

            signer.BlockUpdate(bytesToEncrypt, 0, bytesToEncrypt.Length);

            return signer.VerifySignature(signature);

        }
        public bool VerifyMessage(string message, string signature, string publicKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(message);

            var signer = new RsaDigestSigner(GetShaDigest());
            signer.Init(false, GetPublic(publicKey));

            signer.BlockUpdate(bytesToEncrypt, 0, bytesToEncrypt.Length);

            return signer.VerifySignature(Convert.FromBase64String(signature));
        }

        private AsymmetricKeyParameter GetPrivate(string pem)
        {
            using (var stringReader = new StringReader(pem))
            {
                var chiperPair = (AsymmetricCipherKeyPair)new PemReader(stringReader).ReadObject();
                return chiperPair.Private;
            }

        }

        private AsymmetricKeyParameter GetPublic(string pem)
        {
            if (DoesKeyHaveHeader(pem))
            {
                using (var stringReader = new StringReader(pem))
                {
                    return (AsymmetricKeyParameter)new PemReader(stringReader).ReadObject();
                }
            }

            return PublicKeyFactory.CreateKey(Convert.FromBase64String(pem));
        }

        private bool DoesKeyHaveHeader(string key)
        {
            return key.Contains("BEGIN");
        }
    }
}
