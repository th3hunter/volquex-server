using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Presentaciones : Controller
    {
        public Presentaciones() { }
        
        private VolquexDB db;

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
            using (db = new VolquexDB())
                return new Services.Presentaciones(db).Listar(
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
            using (db = new VolquexDB())
                return new Services.Presentaciones(db).Mostrar(id);
        }

        // POST api/Presentaciones
        [HttpPost("")]
        public ActionResult<Models.Presentaciones> Post([FromBody] Models.Presentaciones o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Presentaciones(db).Insertar(o);
        }

        // PUT api/Presentaciones
        [HttpPut("")]
        public ActionResult<Models.Presentaciones> Put([FromBody] Models.Presentaciones o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Presentaciones(db).Actualizar(o);
        }
    }
}