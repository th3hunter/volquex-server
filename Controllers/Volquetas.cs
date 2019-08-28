using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Volquetas : Controller
    {
        public Volquetas() { }

        // GET api/Volquetas
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Volquetas> > Gets(
            [FromQuery] decimal volquetaId, 
            [FromQuery] string volqDsc, 
            [FromQuery] string conducNom, 
            [FromQuery] string volqEst,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Volquetas().Listar(
                volquetaId,
                volqDsc,
                conducNom,
                volqEst,
                numPagina,
                numRegistros
            );
        }

        // GET api/Volquetas/5
        [HttpGet("{id}")]
        public ActionResult<Models.Volquetas> GetById(decimal id)
        {
            return new Services.Volquetas().Mostrar(id);
        }

        // POST api/Volquetas
        [HttpPost("")]
        public ActionResult<Models.Volquetas> Post([FromBody] Models.Volquetas o)
        { 
            return new Services.Volquetas().Insertar(o);
        }

        // PUT api/Volquetas
        [HttpPut("")]
        public ActionResult<Models.Volquetas> Put([FromBody] Models.Volquetas o) 
        { 
            return new Services.Volquetas().Actualizar(o);
        }
        
        // GET api/Volquetas/ActualizarPosicion
        [HttpGet("ActualizarPosicion/{id}/{lat}/{lon}")]
        public RespuestaSimple ActualizarPosicion(decimal id, string lat, string lon)
        { 
            return new Services.Volquetas().ActualizarPosicion(id, lat, lon);
        }
    }
}