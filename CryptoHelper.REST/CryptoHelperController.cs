using System;
using Microsoft.AspNetCore.Mvc;

namespace CryptoHelper.REST
{
    public class CryptoHelperController : Controller
    {
        private readonly ICryptoGenerator _cryptoGenerator;
        private readonly ICryptoHelper _cryptoHelper;
        private readonly IClientKeysStore _clientKeysStore;
        private readonly IServerKeysStore _serverKeysStore;

        
        public CryptoHelperController(
            ICryptoGenerator cryptoGenerator,
            ICryptoHelper cryptoHelper,
            IClientKeysStore clientKeysStore,
            IServerKeysStore serverKeysStore
            )
        {
            _cryptoGenerator = cryptoGenerator;
            _cryptoHelper = cryptoHelper;
            _clientKeysStore = clientKeysStore;
            _serverKeysStore = serverKeysStore;
        }
        
        public string GenerateKeys()
        {            
            _serverKeysStore.Update(_cryptoGenerator.GenerateKeyPair());            
            return "Keys were generated";
        }

        [HttpPost]
        public string EnrollKey([FromBody] RequestEnrollKey requestEnrollKey)
        {
            _clientKeysStore.Save(requestEnrollKey.deviceId, requestEnrollKey.publicKey);
            return _cryptoHelper.EncryptMessage(_serverKeysStore.Get().Item1, requestEnrollKey.publicKey);
        }

        public string EncryptedData(string message)
        {
            return _cryptoHelper.DecryptMessage(message, _serverKeysStore.Get().Item2);
        }

        public JsonResult GetData()
        {
            const string data = "Some data";
            var signature = _cryptoHelper.SignMessage(data, _serverKeysStore.Get().Item2);
            
            return Json(new { hash = signature, data = data});
        }

        public JsonResult Verify(string deviceId, string message, string hash)
        {
            return Json(new {Success = _cryptoHelper.VerifyMessage(message, hash, _clientKeysStore.Get(deviceId))});
        }
    }
}