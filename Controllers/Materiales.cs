using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Materiales : Controller
    {
        private VolquexDB db;

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
            using (db = new VolquexDB())
                return new Services.Materiales(db).Listar(
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
            using (db = new VolquexDB())
                return new Services.Materiales(db).Mostrar(id);
        }

        // POST api/Materiales
        [HttpPost("")]
        public ActionResult<Models.Materiales> Post([FromBody] Models.Materiales o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Materiales(db).Insertar(o);
        }

        // PUT api/Materiales
        [HttpPut("")]
        public ActionResult<Models.Materiales> Put([FromBody] Models.Materiales o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Materiales(db).Actualizar(o);
        }
    }
}