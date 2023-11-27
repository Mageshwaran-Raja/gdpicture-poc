using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GDPicture.POC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        [HttpGet("create")]
        public IActionResult CreateSession()
        {
            // Generate a unique session ID
            string sessionId = HttpContext.Session.GetString("SessionId");

            // Store the session ID in the session
            if (sessionId == null) {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("SessionId", sessionId);
            }

            return Ok(sessionId);
        }
    }
}