using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Volquetas : Controller
    {
        public Volquetas() { }
        
        private VolquexDB db;

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
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Volquetas(db).Listar(
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
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Volquetas(db).Mostrar(id);
        }

        // POST api/Volquetas
        [HttpPost("")]
        public ActionResult<Models.Volquetas> Post([FromBody] Models.Volquetas o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Volquetas(db).Insertar(o);
        }

        // PUT api/Volquetas
        [HttpPut("")]
        public ActionResult<Models.Volquetas> Put([FromBody] Models.Volquetas o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Volquetas(db).Actualizar(o);
        }
        
        // GET api/Volquetas/ActualizarPosicion
        [HttpGet("ActualizarPosicion/{id}/{lat}/{lon}")]
        public ActionResult<RespuestaSimple> ActualizarPosicion(decimal id, string lat, string lon)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Volquetas(db).ActualizarPosicion(id, lat, lon);
        }
    }
}