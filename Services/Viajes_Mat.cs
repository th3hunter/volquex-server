using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Viajes_Mat
    {
        public Viajes_Mat(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

        public DataProvider<Models.Viajes_Mat> Listar(
            decimal viajeId, 
            int numPagina,
            int numRegistros
            ) 
        {
            var q = from p in db.Viajes_Mat
                orderby p.ViajeId, p.ViaMatId
                where p.ViajeId == viajeId
                select new Models.Viajes_Mat
                {
                    ViajeId = p.ViajeId,
                    ViaMatId = p.ViaMatId,
                    MaterialId = p.MaterialId,
                    MatDsc = p.fkviajesmatmateriales1.MatDsc,
                    PresDsc = p.fkviajesmatmateriales1.fkpresentaciones1.PresDsc,
                    ViaMatCantidad = p.ViaMatCantidad,
                    ViaMatPrecio = p.ViaMatPrecio,
                    ViaMatImporte = p.ViaMatImporte
                };

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Viajes_Mat>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Viajes_Mat>(count, q.ToList()) ;
        }

        public Models.Viajes_Mat Mostrar(decimal id)
        {
            var q = from p in db.Viajes_Mat
                where p.ViajeId == id
                select p;

            return q.FirstOrDefault();
        }

        public Models.Viajes_Mat Insertar(Models.Viajes_Mat o)
        {
            // Obtengo el siguiente Id a insertar
            o.ViaMatId = Numeracion(o.ViajeId);
            db.Insert(o);

            return(o);
        }

        public Models.Viajes_Mat Actualizar(Models.Viajes_Mat o)
        {
            db.Update(o);
            return(o);
        }

        public int Numeracion(decimal viajeId) {
            // Obtengo el último registro
            var q = from p in db.Viajes_Mat
                orderby p.ViajeId, p.ViaMatId descending
                where p.ViajeId == viajeId
                select new Models.Viajes_Mat
                {
                    ViaMatId = p.ViaMatId
                };

            // Retorno el último ID + 1
            return q.FirstOrDefault() != null ? q.FirstOrDefault().ViaMatId + 1 : 1;
        }

    }
}