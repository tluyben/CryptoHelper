using System;
using Xunit;

namespace CryptoHelper.Test
{
    public class BouncyCryptoHelperTest
    {
        [Fact]
        public void RsaDecryptEncryptTest()
        {
            var bouncyCryptoHelper = new BouncyCryptoHelper("RSA");
            var keys = bouncyCryptoHelper.GenerateKeyPair();

            var input = "Example message for decryption/encription! With spec symbol";
            var publicKey = keys.Item1;
            var privateKey = keys.Item2;

            var encryptedMessage = bouncyCryptoHelper.EncryptMessage(input, publicKey);
            var output = bouncyCryptoHelper.DecryptMessage(encryptedMessage, privateKey);

            Assert.Equal(input,output);
        }

        [Fact]
        public void RsaSignTest()
        {
            var bouncyCryptoHelper = new BouncyCryptoHelper("RSA");
            var keys = bouncyCryptoHelper.GenerateKeyPair();

            var input = "Example message for decryption/encription! With spec symbol";
            var publicKey = keys.Item1;
            var privateKey = keys.Item2;

            var encryptedMessage = bouncyCryptoHelper.SignMessage(input, privateKey);
            Assert.True(bouncyCryptoHelper.VerifyMessage(input, encryptedMessage, publicKey));
        }
    }
}
