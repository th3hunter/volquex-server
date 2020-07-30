using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Usuarios : Controller
    {
        public Usuarios() {}
        
        private VolquexDB db;

        // GET api/Usuarios
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Usuarios> > Gets(
            [FromQuery] decimal usuarioId, 
            [FromQuery] string usuNom, 
            [FromQuery] string usuTipo,
            [FromQuery] string usuEst,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Usuarios(db).Listar(
                    usuarioId,
                    usuNom,
                    usuTipo,
                    usuEst,
                    numPagina,
                    numRegistros
                );
        }

        // GET api/Usuarios/5
        [HttpGet("{id}")]
        public ActionResult<Models.Usuarios> GetById(decimal id)
        {
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Usuarios(db).Mostrar(id);
        }

        // POST api/Usuarios
        [HttpPost("")]
        public ActionResult<Models.Usuarios> Post([FromBody] Models.Usuarios o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Usuarios(db).Insertar(o);
        }

        // PUT api/Usuarios
        [HttpPut("")]
        public ActionResult<Models.Usuarios> Put([FromBody] Models.Usuarios o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Usuarios(db).Actualizar(o);
        }

        // POST api/Usuarios/login
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<RespuestaSimple> Login( [FromBody] dynamic credenciales )
        {
            string usuario = credenciales.usuario;
            string password = credenciales.password;
            
            using (db = new VolquexDB())
                return new Services.Usuarios(db).Login(usuario, password);
        }

        // POST api/Usuarios/registrar
        [AllowAnonymous]
        [HttpPost("registrar")]
        public Task<ActionResult<RespuestaSimple>> Registrar( [FromBody] dynamic data )
        {
            string token = data.token;
            string type = data.type;
            string tokenDispositivo = data.tokenDispositivo;
            
            return new Services.Usuarios(db).Registrar(token, tokenDispositivo, type);
        }

        // GET api/usuarios/mis-datos
        [HttpGet("mis-datos")]
        public ActionResult<Models.Usuarios> MisDatos()
        {
            using (db = new VolquexDB())
                return new Services.Usuarios(db).MisDatos();
        }

        // POST api/Usuarios/actualizar-datos
        [HttpPost("actualizar-datos")]
        public ActionResult<RespuestaSimple> ActualizarDatos( [FromBody] dynamic data )
        {
            string usuNom = data.usuNom;
            string usuEmail = data.usuEmail;
            string usuCel = data.usuCel;

            using (db = new VolquexDB())
                return new Services.Usuarios(db).ActualizarDatos(usuNom, usuEmail, usuCel);
        }

        // POST api/Usuarios/inicializar/alsdkjf2093845okjldfkjñad
        [AllowAnonymous]
        [HttpGet("inicializar/{dispositivoId}")]
        public ActionResult<RespuestaSimple> Inicializar(string dispositivoId)
        {
            using (db = new VolquexDB())
                return new Services.Usuarios(db).Inicializar(dispositivoId);
        }
    }
}