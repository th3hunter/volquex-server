using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Parametros : Controller
    {
        private VolquexDB db;

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
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Parametros(db).Listar(
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
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Parametros(db).Mostrar(id);
        }

        // POST api/Parametros
        [HttpPost("")]
        public ActionResult<Models.Parametros> Post([FromBody] Models.Parametros o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Parametros(db).Insertar(o);
        }

        // PUT api/Parametros
        [HttpPut("")]
        public ActionResult<Models.Parametros> Put([FromBody] Models.Parametros o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Parametros(db).Actualizar(o);
        }

        // GET api/Parametros/link-apps
        [AllowAnonymous]
        [HttpGet("link-apps")]
        public ActionResult<RespuestaSimple> LinkApps() 
        { 
            using (db = new VolquexDB())
            {
                // Obtiene los links de las apps de los parámetros
                var paramService = new Services.Parametros(db);
                string android = paramService.ObtenerValor("APP-ANDROID");
                string ios = paramService.ObtenerValor("APP-ANDROID");

                // Construye la respuesta
                return new RespuestaSimple(new
                {
                    android,
                    ios
                });
            }
        }
    }
}