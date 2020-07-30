using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes_Bit : Controller
    {
        public Viajes_Bit() { }
        
        private VolquexDB db;

        // GET api/Viajes_Bit
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Viajes_Bit> > Gets(
            [FromQuery] decimal viajeId, 
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Bit(db).Listar(
                    viajeId,
                    numPagina,
                    numRegistros
                );
        }
    }
}