using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Materiales : Controller
    {
        public Materiales() { }

        // GET api/Materiales
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Materiales> > Gets(
            [FromQuery] decimal materialId, 
            [FromQuery] string matDsc, 
            [FromQuery] string matEst,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Materiales().Listar(
                materialId,
                matDsc,
                matEst,
                numPagina,
                numRegistros
            );
        }

        // GET api/Materiales/5
        [HttpGet("{id}")]
        public ActionResult<Models.Materiales> GetById(decimal id)
        {
            return new Services.Materiales().Mostrar(id);
        }

        // POST api/Materiales
        [HttpPost("")]
        public ActionResult<Models.Materiales> Post([FromBody] Models.Materiales o)
        { 
            return new Services.Materiales().Insertar(o);
        }

        // PUT api/Materiales
        [HttpPut("")]
        public ActionResult<Models.Materiales> Put([FromBody] Models.Materiales o) 
        { 
            return new Services.Materiales().Actualizar(o);
        }
    }
}