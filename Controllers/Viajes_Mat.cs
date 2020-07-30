using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes_Mat : Controller
    {
        public Viajes_Mat() { }
        
        private VolquexDB db;

        // GET api/Viajes_Mat
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Viajes_Mat> > Gets(
            [FromQuery] decimal viajeId, 
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Mat(db).Listar(
                    viajeId,
                    numPagina,
                    numRegistros
                );
        }

        // GET api/Viajes_Mat/5
        [HttpGet("{id}")]
        public ActionResult<Models.Viajes_Mat> GetById(decimal id)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Mat(db).Mostrar(id);
        }

        // POST api/Viajes_Mat
        [HttpPost("")]
        public ActionResult<Models.Viajes_Mat> Post([FromBody] Models.Viajes_Mat o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes_Mat(db).Insertar(o);
        }

        // PUT api/Viajes_Mat
        [HttpPut("")]
        public ActionResult<Models.Viajes_Mat> Put([FromBody] Models.Viajes_Mat o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes_Mat(db).Actualizar(o);
        }
    }
}