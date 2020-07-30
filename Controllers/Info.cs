using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Version : Controller
    {
        public Version() {}

        // GET api/Version
        [AllowAnonymous]
        [HttpGet("")]
        public ActionResult< string > ObtenerVersion()
        {
            return Startup.Version;
        }
    }
}