using System;
using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Viajes_Bit
    {
        public Viajes_Bit() {}

        public DataProvider<Models.Viajes_Bit> Listar(
            decimal viajeId, 
            int numPagina,
            int numRegistros
            ) 
        {
            using (var db = new VolquexDB())
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
        }

        public Models.Viajes_Bit Insertar(decimal viajeId, string estado)
        {
            // Crea el objeto a insertar
            var bitacora = new Models.Viajes_Bit
            {
                ViajeId = viajeId,
                ViajeBitacoraId = Numeracion(),
                ViaBitFchHr = DateTime.Now,
                ViaBitEst = estado
            };

            // Inserto el registro
            using(var db = new VolquexDB())
                db.Insert(bitacora);

            return(bitacora);
        }

        public int Numeracion() {
            using (var db = new VolquexDB())
            {
                // Obtengo el último registro
                var q = from p in db.Viajes_Bit
                    orderby p.ViajeId, p.ViajeBitacoraId descending
                    select new Models.Viajes_Bit
                    {
                        ViajeBitacoraId = p.ViajeBitacoraId
                    };

                // Retorno el último ID + 1
                return q.FirstOrDefault() != null ? q.FirstOrDefault().ViajeBitacoraId + 1 : 1;
            }
        }

    }
}