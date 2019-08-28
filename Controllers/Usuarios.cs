using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Http;
using System.Net.Http;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Usuarios : Controller
    {

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
            return new Services.Usuarios().Listar(
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
            return new Services.Usuarios().Mostrar(id);
        }

        // POST api/Usuarios
        [HttpPost("")]
        public ActionResult<Models.Usuarios> Post([FromBody] Models.Usuarios o)
        { 
            return new Services.Usuarios().Insertar(o);
        }

        // PUT api/Usuarios
        [HttpPut("")]
        public ActionResult<Models.Usuarios> Put([FromBody] Models.Usuarios o) 
        { 
            return new Services.Usuarios().Actualizar(o);
        }

        // POST api/Usuarios/login
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<RespuestaSimple> Login( [FromBody] dynamic credenciales )
        {
            string usuario = credenciales.usuario;
            string password = credenciales.password;
            
            return new Services.Usuarios().Login(usuario, password);
        }

        // POST api/Usuarios/registrar
        [AllowAnonymous]
        [HttpPost("registrar")]
        public Task<ActionResult<RespuestaSimple>> Registrar( [FromBody] dynamic data )
        {
            string token = data.token;
            string type = data.type;
            
            return new Services.Usuarios().Registrar(token, type);
        }

        // GET api/usuarios/mis-datos
        [HttpGet("mis-datos")]
        public ActionResult<Models.Usuarios> MisDatos()
        {
            return new Services.Usuarios().MisDatos();
        }

        // POST api/Usuarios/actualizar-datos
        [HttpPost("actualizar-datos")]
        public ActionResult<RespuestaSimple> ActualizarDatos( [FromBody] dynamic data )
        {
            string usuNom = data.usuNom;
            string usuEmail = data.usuEmail;
            string usuCel = data.usuCel;

            return new Services.Usuarios().ActualizarDatos(usuNom, usuEmail, usuCel);
        }
    }
}