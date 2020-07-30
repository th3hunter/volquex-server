using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;
using Volquex.Models;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes_Volq : Controller
    {
        public Viajes_Volq() { }
        
        private VolquexDB db;

        // GET api/Viajes_Volq
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Viajes_Volq> > Gets(
            [FromQuery] decimal viajeId, 
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).Listar(
                    viajeId,
                    numPagina,
                    numRegistros
                );
        }

        // GET api/Viajes_Volq/5
        [HttpGet("{id}")]
        public ActionResult<Models.Viajes_Volq> GetById(decimal id)
        {
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).Mostrar(id);
        }

        // POST api/Viajes_Volq
        [HttpPost("")]
        public ActionResult<Models.Viajes_Volq> Post([FromBody] Models.Viajes_Volq o)
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).Insertar(o);
        }

        // PUT api/Viajes_Volq
        [HttpPut("")]
        public ActionResult<Models.Viajes_Volq> Put([FromBody] Models.Viajes_Volq o) 
        { 
            // Valida que el usuario sea administrador
            if (Startup.Usuario.UsuTipo != "ADM")
                return Unauthorized();
                
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).Actualizar(o);
        }

        // GET api/Viajes_Volq/ofertas/5
        [HttpGet("ofertas/{viajeId}")]
        public ActionResult< DataProvider<Models.Viajes_Volq> > Ofertas(decimal viajeId)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).Ofertas(viajeId);
        }

        // GET api/Viajes_Volq/guardar-oferta/5/10.22
        [HttpGet("Guardar-Oferta/{viajeId}/{valorOferta}")]
        public ActionResult<RespuestaSimple> GuardarOferta(decimal viajeId, decimal valorOferta)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).GuardarOferta(viajeId, valorOferta);
        }

        // GET api/Viajes_Volq/guardar-oferta-materiales
        [HttpPost("Guardar-Oferta-Materiales")]
        public ActionResult<RespuestaSimple> GuardarOfertaMateriales(
            [FromBody] Models.Viajes_Volq oferta)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).GuardarOfertaMateriales(oferta);
        }

        // GET api/Viajes_Volq/cancelar-oferta/5/10.22
        [HttpGet("Cancelar-Oferta/{viajeId}")]
        public ActionResult<RespuestaSimple> CancelarOferta(decimal viajeId)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).CancelarOferta(viajeId);
        }

        // GET api/Viajes_Volq/consultar-oferta/5
        [HttpGet("Consultar-Oferta/{viajeId}")]
        public ActionResult<Models.Viajes> ConsultarOferta(decimal viajeId, 
                [FromQuery] string estado)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).ConsultarOferta(viajeId, estado);
        }

        // GET api/Viajes_Volq/consultar-oferta-materiales/5/1
        [HttpGet("Consultar-Oferta-Materiales/{viajeId}/{viajeVolquetaId}")]
        public ActionResult<DataProvider<Models.Viajes_Volq_Mat>> 
            ConsultarOfertaMateriales(decimal viajeId, decimal viajeVolquetaId)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).ConsultarOfertaMateriales(viajeId, viajeVolquetaId);
        }

        // GET api/Viajes_Volq/AceptarOferta/5/2
        [HttpGet("AceptarOferta/{viajeId}/{volquetaId}")]
        public ActionResult<RespuestaSimple> AceptarOferta(decimal viajeId, int volquetaId)
        {
            using (db = new VolquexDB())
            {
                // Valido que sea el usuario Cliente
                if (Startup.Usuario.UsuarioId != new Services.Viajes(db).ObtenerCliente(viajeId))
                    return Unauthorized();

                return new Services.Viajes_Volq(db).AceptarOferta(viajeId, volquetaId);
            }
        }

        // GET api/Viajes_Volq/listar-ofertados/2
        [HttpGet("listar-ofertados/{conductorId}")]
        public ActionResult<DataProvider<Models.Viajes_Volq>> ListarOfertados(decimal conductorId)
        {
            using (db = new VolquexDB())
                return new Services.Viajes_Volq(db).ListarOfertados(conductorId);
        }
    }
}