using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes_Bit : Controller
    {
        public Viajes_Bit() { }

        // GET api/Viajes_Bit
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Viajes_Bit> > Gets(
            [FromQuery] decimal viajeId, 
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Viajes_Bit().Listar(
                viajeId,
                numPagina,
                numRegistros
            );
        }
    }
}