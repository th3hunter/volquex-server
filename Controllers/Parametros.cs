using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Parametros : Controller
    {
        public Parametros() { }

        // GET api/Parametros
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Parametros> > Gets(
            [FromQuery] string parametroId, 
            [FromQuery] string paramDsc, 
            [FromQuery] string paramEst,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Parametros().Listar(
                parametroId,
                paramDsc,
                paramEst,
                numPagina,
                numRegistros
            );
        }

        // GET api/Parametros/5
        [HttpGet("{id}")]
        public ActionResult<Models.Parametros> GetById(string id)
        {
            return new Services.Parametros().Mostrar(id);
        }

        // POST api/Parametros
        [HttpPost("")]
        public ActionResult<Models.Parametros> Post([FromBody] Models.Parametros o)
        { 
            return new Services.Parametros().Insertar(o);
        }

        // PUT api/Parametros
        [HttpPut("")]
        public ActionResult<Models.Parametros> Put([FromBody] Models.Parametros o) 
        { 
            return new Services.Parametros().Actualizar(o);
        }
    }
}