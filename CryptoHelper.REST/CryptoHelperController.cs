using System;
using CryptoHelper.REST.Models;
using Microsoft.AspNetCore.Mvc;

namespace CryptoHelper.REST
{
    public class CryptoHelperController : Controller
    {
        private readonly ICryptoHelper _cryptoHelper;
        
        public CryptoHelperController(ICryptoHelper cryptoHelper)
        {
            _cryptoHelper = cryptoHelper;
        }
        
        [HttpPost]
        public JsonResult DecryptedMessage([FromBody] DecryptMessageRequest decryptMessageRequest)
        {
            var result = _cryptoHelper.EncryptMessage("Message from the server which was decrypted!", decryptMessageRequest.publicKey);
            return Json(new { message = result});
        }
        
        [HttpPost]
        public JsonResult VerifyMessage([FromBody] VerifyMessageRequest request)
        {
            var result = _cryptoHelper.VerifyMessage(request.message, request.signature, request.publicKey);
            return Json(new { message = result});
        }
    }
}