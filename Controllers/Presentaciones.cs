using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Presentaciones : Controller
    {
        public Presentaciones() { }

        // GET api/Presentaciones
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Presentaciones> > Gets(
            [FromQuery] decimal presentacionId, 
            [FromQuery] string presDsc, 
            [FromQuery] string presEst,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Presentaciones().Listar(
                presentacionId,
                presDsc,
                presEst,
                numPagina,
                numRegistros
            );
        }

        // GET api/Presentaciones/5
        [HttpGet("{id}")]
        public ActionResult<Models.Presentaciones> GetById(decimal id)
        {
            return new Services.Presentaciones().Mostrar(id);
        }

        // POST api/Presentaciones
        [HttpPost("")]
        public ActionResult<Models.Presentaciones> Post([FromBody] Models.Presentaciones o)
        { 
            return new Services.Presentaciones().Insertar(o);
        }

        // PUT api/Presentaciones
        [HttpPut("")]
        public ActionResult<Models.Presentaciones> Put([FromBody] Models.Presentaciones o) 
        { 
            return new Services.Presentaciones().Actualizar(o);
        }
    }
}