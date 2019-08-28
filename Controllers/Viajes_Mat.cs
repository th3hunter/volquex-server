using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes_Mat : Controller
    {
        public Viajes_Mat() { }

        // GET api/Viajes_Mat
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Viajes_Mat> > Gets(
            [FromQuery] decimal viajeId, 
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Viajes_Mat().Listar(
                viajeId,
                numPagina,
                numRegistros
            );
        }

        // GET api/Viajes_Mat/5
        [HttpGet("{id}")]
        public ActionResult<Models.Viajes_Mat> GetById(decimal id)
        {
            return new Services.Viajes_Mat().Mostrar(id);
        }

        // POST api/Viajes_Mat
        [HttpPost("")]
        public ActionResult<Models.Viajes_Mat> Post([FromBody] Models.Viajes_Mat o)
        { 
            return new Services.Viajes_Mat().Insertar(o);
        }

        // PUT api/Viajes_Mat
        [HttpPut("")]
        public ActionResult<Models.Viajes_Mat> Put([FromBody] Models.Viajes_Mat o) 
        { 
            return new Services.Viajes_Mat().Actualizar(o);
        }
    }
}