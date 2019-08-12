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
        
        [HttpPost]
        public JsonResult EnrollKey([FromBody] RequestEnrollKey requestEnrollKey)
        {
            var result = _cryptoHelper.EncryptMessage("hello", requestEnrollKey.publicKey);
            return Json(new { message = result});
        }
    }
}