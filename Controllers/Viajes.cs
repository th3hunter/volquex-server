using System;
using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes : Controller
    {
        public Viajes() { }
        
        private VolquexDB db;

        // GET api/Viajes
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Viajes> > Gets(
            [FromQuery] decimal viajeId, 
            [FromQuery] DateTime desde, 
            [FromQuery] DateTime hasta, 
            [FromQuery] string cliNom, 
            [FromQuery] string conducNom, 
            [FromQuery] string volqDsc, 
            [FromQuery] string viaEst,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes(db).Listar(
                    viajeId,
                    desde,
                    hasta,
                    cliNom,
                    conducNom,
                    volqDsc,
                    viaEst,
                    numPagina,
                    numRegistros
                );
        }

        // GET api/Viajes/5
        [HttpGet("{id}")]
        public ActionResult<Models.Viajes> GetById(decimal id)
        {  
            using (db = new VolquexDB())
                return new Services.Viajes(db).Mostrar(id);
        }

        // POST api/Viajes
        [HttpPost("")]
        public ActionResult<Models.Viajes> Post([FromBody] Models.Viajes o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes(db).Insertar(o);
        }

        // PUT api/Viajes
        [HttpPut("")]
        public ActionResult<Models.Viajes> Put([FromBody] Models.Viajes o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes(db).Actualizar(o);
        }

        // POST api/Viajes/Solicitar
        [HttpPost("Solicitar")]
        public ActionResult<RespuestaSimple> Solicitar([FromBody] Models.Viajes o)
        {
            using (db = new VolquexDB())
                return new Services.Viajes(db).Solicitar(o);
        }
        
        // GET api/Viajes/Monitorear
        [HttpGet("Monitorear/{viajeId}")]
        public ActionResult<RespuestaSimple> Monitorear(decimal viajeId)
        { 
            using (db = new VolquexDB())
            {
                // Valido que sea el usuario Cliente
                if (Startup.Usuario.UsuarioId != new Services.Viajes(db).ObtenerCliente(viajeId))
                    return Unauthorized();
                    
                return new Services.Viajes(db).Monitorear(viajeId);
            }
        }

        // GET api/Viajes/CambiarEstado/5/SOL
        [HttpGet("CambiarEstado/{viajeId}/{estado}")]
        public ActionResult<RespuestaSimple> CambiarEstado(decimal viajeId, string estado)
        {
            using (db = new VolquexDB())
            {
                // Valido que sea el usuario Cliente o Conductor
                var clienteId = new Services.Viajes(db).ObtenerCliente(viajeId);
                var conductorId = new Services.Viajes(db).ObtenerConductor(viajeId);
                if (Startup.Usuario.UsuarioId != clienteId && Startup.Usuario.UsuarioId != conductorId)
                    return Unauthorized();

                return new Services.Viajes(db).CambiarEstado(viajeId, estado);
            }
        }

        // GET api/Viajes/ClienteCalifica/5
        [HttpGet("ClienteCalifica/{viajeId}/{calificacion}")]
        public ActionResult<RespuestaSimple> ClienteCalifica(decimal viajeId, int calificacion)
        {
            using (db = new VolquexDB())
            {
                // Valido que sea el usuario Cliente
                if (Startup.Usuario.UsuarioId != new Services.Viajes(db).ObtenerCliente(viajeId))
                    return Unauthorized();

                return new Services.Viajes(db).ClienteCalifica(viajeId, calificacion);
            }
        }

        // GET api/Viajes/conductor-califica/21/5
        [HttpGet("conductor-califica/{viajeId}/{calificacion}")]
        public ActionResult<RespuestaSimple> ConductorCalifica(decimal viajeId, int calificacion)
        {
            using (db = new VolquexDB())
            {
                // Valido que sea el usuario Conductor
                if (Startup.Usuario.UsuarioId != new Services.Viajes(db).ObtenerConductor(viajeId))
                    return Unauthorized();

                return new Services.Viajes(db).ConductorCalifica(viajeId, calificacion);
            }
        }

        // GET api/Viajes/listar-por-cliente/5/true
        [HttpGet("listar-por-cliente/{finalizados}")]
        public ActionResult< DataProvider<Models.Viajes> > ListarPorCliente(
            bool finalizados,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros)
        {
            using (db = new VolquexDB())
                return new Services.Viajes(db).ListarPorCliente(finalizados, numPagina, numRegistros);
        }

        // GET api/Viajes/listar-por-conductor/5/true
        [HttpGet("listar-por-conductor/{estado}")]
        public ActionResult< DataProvider<Models.Viajes> > ListarPorConductor(
            string estado,
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros)
        {
            using (db = new VolquexDB())
                return new Services.Viajes(db).ListarPorConductor(estado, numPagina, numRegistros);
        }

        // GET api/Viajes/en-curso
        [HttpGet("en-curso")]
        public ActionResult<Models.Viajes> EnCurso()
        {
            using (db = new VolquexDB())
                return new Services.Viajes(db).EnCurso();
        }

        // GET api/Viajes/actualizar-posicion/id/lat/lon
        [HttpGet("actualizar-posicion/{viajeId}/{lat}/{lon}")]
        public ActionResult<RespuestaSimple> ActualizarPosicion(
            decimal viajeId,
            string lat,
            string lon
            )
        {
            using (db = new VolquexDB())
            {
                // Valido que sea el usuario Conductor o Admin
                if (Startup.Usuario.UsuarioId != new Services.Viajes(db).ObtenerConductor(viajeId))
                    return Unauthorized();

                return new Services.Viajes(db).ActualizarPosicion(viajeId, lat, lon);
            }
        }
    }
}