using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalRHub.Contracts;
using System.Collections.Generic;

namespace SignalRHub.Controllers
{
    [Route("api/[controller]")]
    public class UtilController : Controller
    {

        private readonly ISignalRMainHub _signalR;

        public UtilController (ISignalRMainHub signalR)
        {
            _signalR = signalR;
        }

        [HttpPost]
        [Route("TestSignalR")]
        [AllowAnonymous]
        public ActionResult TestSignalR(string message)
        {
            _signalR.EchoAll(message);
            return Ok();
        }

        [HttpGet]
        [Route("GetSignalRClientIds")]
        [AllowAnonymous]
        public IEnumerable<string> GetSignalRClientIds()
        {
            return _signalR.GetAllConnectionIds();
        }

    }
}
