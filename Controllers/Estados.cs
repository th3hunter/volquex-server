using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Estados : Controller
    {
        public Estados() { }

        // GET api/Estados
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Estados> > Gets(
            [FromQuery] string tablaId, 
            [FromQuery] string estadoId, 
            [FromQuery] string estDsc, 
            [FromQuery] string estEst,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Estados().Listar(
                tablaId,
                estadoId,
                estDsc,
                estEst,
                numPagina,
                numRegistros
            );
        }

        // GET api/Estados/VIA/ACE
        [HttpGet("{tablaId}/{estadoId}")]
        public ActionResult<Models.Estados> GetById(string tablaId, string estadoId)
        {
            return new Services.Estados().Mostrar(tablaId, estadoId);
        }

        // POST api/Estados
        [HttpPost("")]
        public ActionResult<Models.Estados> Post([FromBody] Models.Estados o)
        { 
            return new Services.Estados().Insertar(o);
        }

        // PUT api/Estados
        [HttpPut("")]
        public ActionResult<Models.Estados> Put([FromBody] Models.Estados o) 
        { 
            return new Services.Estados().Actualizar(o);
        }
    }
}