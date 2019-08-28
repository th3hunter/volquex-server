using System;
using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes : Controller
    {
        public Viajes() { }

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
            return new Services.Viajes().Listar(
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
            return new Services.Viajes().Mostrar(id);
        }

        // POST api/Viajes
        [HttpPost("")]
        public ActionResult<Models.Viajes> Post([FromBody] Models.Viajes o)
        { 
            return new Services.Viajes().Insertar(o);
        }

        // PUT api/Viajes
        [HttpPut("")]
        public ActionResult<Models.Viajes> Put([FromBody] Models.Viajes o) 
        { 
            return new Services.Viajes().Actualizar(o);
        }

        // POST api/Viajes/Solicitar
        [HttpPost("Solicitar")]
        public ActionResult<RespuestaSimple> Solicitar([FromBody] Models.Viajes o)
        {
            return new Services.Viajes().Solicitar(o);
        }
        
        // GET api/Viajes/Monitorear
        [HttpGet("Monitorear/{id}")]
        public RespuestaSimple Monitorear(decimal id)
        { 
            return new Services.Viajes().Monitorear(id);
        }

        // GET api/Viajes/CambiarEstado/5/SOL
        [HttpGet("CambiarEstado/{id}/{estado}")]
        public ActionResult<RespuestaSimple> CambiarEstado(decimal id, string estado)
        {
            return new Services.Viajes().CambiarEstado(id, estado);
        }

        // GET api/Viajes/ClienteCalifica/5
        [HttpGet("ClienteCalifica/{id}/{calificacion}")]
        public ActionResult<RespuestaSimple> ClienteCalifica(decimal id, int calificacion)
        {
            return new Services.Viajes().ClienteCalifica(id, calificacion);
        }

        // GET api/Viajes/listar-por-cliente/5/true
        [HttpGet("listar-por-cliente/{idCliente}/{finalizados}")]
        public ActionResult< DataProvider<Models.Viajes> > ListarPorCliente(
            decimal idCliente,
            bool finalizados,
            int numPagina,
            int numRegistros)
        {
            return new Services.Viajes().ListarPorCliente(idCliente, finalizados, numPagina, numRegistros);
        }

        // GET api/Viajes/listar-por-conductor/5/true
        [HttpGet("listar-por-conductor/{idConductor}/{finalizados}")]
        public ActionResult< DataProvider<Models.Viajes> > ListarPorConductor(
            decimal idConductor,
            bool finalizados,
            int numPagina,
            int numRegistros)
        {
            return new Services.Viajes().ListarPorConductor(idConductor, finalizados, numPagina, numRegistros);
        }
    }
}