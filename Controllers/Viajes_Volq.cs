using Microsoft.AspNetCore.Mvc;
using Volquex.Utils;

namespace Volquex.Controllers
{
    [Route("api/[controller]")]
    public class Viajes_Volq : Controller
    {
        public Viajes_Volq() { }

        // GET api/Viajes_Volq
        [HttpGet("")]
        public ActionResult< DataProvider<Models.Viajes_Volq> > Gets(
            [FromQuery] decimal viajeId, 
            [FromQuery] int numPagina,
            [FromQuery] int numRegistros
            )
        {
            return new Services.Viajes_Volq().Listar(
                viajeId,
                numPagina,
                numRegistros
            );
        }

        // GET api/Viajes_Volq/5
        [HttpGet("{id}")]
        public ActionResult<Models.Viajes_Volq> GetById(decimal id)
        {
            return new Services.Viajes_Volq().Mostrar(id);
        }

        // POST api/Viajes_Volq
        [HttpPost("")]
        public ActionResult<Models.Viajes_Volq> Post([FromBody] Models.Viajes_Volq o)
        { 
            return new Services.Viajes_Volq().Insertar(o);
        }

        // PUT api/Viajes_Volq
        [HttpPut("")]
        public ActionResult<Models.Viajes_Volq> Put([FromBody] Models.Viajes_Volq o) 
        { 
            return new Services.Viajes_Volq().Actualizar(o);
        }

        // GET api/Viajes_Volq/ofertas/5
        [HttpGet("ofertas/{viajeId}")]
        public ActionResult< DataProvider<Models.Viajes_Volq> > Ofertas(decimal viajeId)
        {
            return new Services.Viajes_Volq().Ofertas(viajeId);
        }

        // GET api/Viajes_Volq/AceptarOferta/5/2
        [HttpGet("AceptarOferta/{viajeId}/{volquetaId}")]
        public ActionResult<RespuestaSimple> AceptarOferta(decimal viajeId, int volquetaId)
        {
            return new Services.Viajes_Volq().AceptarOferta(viajeId, volquetaId);
        }

        // GET api/Viajes_Volq/listar-ofertados/5/2
        [HttpGet("listar-ofertados/{conductorId}")]
        public ActionResult<DataProvider<Models.Viajes_Volq>> ListarOfertados(int conductorId)
        {
            return new Services.Viajes_Volq().ListarOfertados(conductorId);
        }
    }
}