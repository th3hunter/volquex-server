using System;
using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Viajes_Bit
    {
        public Viajes_Bit(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

        public DataProvider<Models.Viajes_Bit> Listar(
            decimal viajeId, 
            int numPagina,
            int numRegistros
            ) 
        {
            // Primero hago un SELECT a la tabla Estados para luego hacerle
            // el join
            var tabla = from Estados in db.Estados
                where Estados.TablaId == "VIA"
                select Estados;

            var q = from p in db.Viajes_Bit
                join r in tabla
                    on p.ViaBitEst equals r.EstadoId
                where p.ViajeId == viajeId
                orderby p.ViaBitFchHr
                select new Models.Viajes_Bit
                {
                    ViajeId = p.ViajeId,
                    ViaBitFchHr = p.ViaBitFchHr,
                    ViaBitEst = p.ViaBitEst,
                    EstDsc = r.EstDsc
                };

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Viajes_Bit>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Viajes_Bit>(count, q.ToList()) ;
        }

        public Models.Viajes_Bit Insertar(decimal viajeId, string estado, string obs)
        {
            // Crea el objeto a insertar
            var bitacora = new Models.Viajes_Bit
            {
                ViajeId = viajeId,
                ViajeBitacoraId = Numeracion(viajeId),
                ViaBitFchHr = DateTime.Now,
                ViaBitEst = estado,
                ViaBitInfo = obs
            };

            // Inserto el registro
            db.Insert(bitacora);

            return(bitacora);
        }

        public int Numeracion(decimal viajeId) {
            // Obtengo el último registro
            var q = from p in db.Viajes_Bit
                where p.ViajeId == viajeId
                orderby p.ViajeId, p.ViajeBitacoraId descending
                select p.ViajeBitacoraId;

            // Retorno el último ID + 1
            var ultimoId = q.FirstOrDefault();
            return ultimoId > 0 ? ultimoId + 1 : 1;
        }

    }
}