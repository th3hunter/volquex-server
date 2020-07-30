using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Estados : Controller
    {
        public Estados() {}
        
        private VolquexDB db;

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
            using (db = new VolquexDB())
                return new Services.Estados(db).Listar(
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
            using (db = new VolquexDB())
                return new Services.Estados(db).Mostrar(tablaId, estadoId);
        }

        // POST api/Estados
        [HttpPost("")]
        public ActionResult<Models.Estados> Post([FromBody] Models.Estados o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();

            using (db = new VolquexDB())
                return new Services.Estados(db).Insertar(o);
        }

        // PUT api/Estados
        [HttpPut("")]
        public ActionResult<Models.Estados> Put([FromBody] Models.Estados o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Estados(db).Actualizar(o);
        }
        
        // GET api/Estados/elegibles/tablaId
        [HttpGet("elegibles/{tablaId}")]
        public ActionResult< DataProvider<Models.Estados> > Elegibles(string tablaId)
        {
            using (db = new VolquexDB())
                return new Services.Estados(db).Elegibles(tablaId);
        }
    }
}