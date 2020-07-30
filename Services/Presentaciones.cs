using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Presentaciones
    {
        public Presentaciones(VolquexDB db) 
        {
            this.db = db;
        }

        private VolquexDB db;

        public DataProvider<Models.Presentaciones> Listar(
            decimal presentacionId, 
            string presDsc, 
            string presEst,
            int numPagina,
            int numRegistros
            ) 
        {
            var q = from p in db.Presentaciones
                orderby p.PresDsc
                select new Models.Presentaciones
                {
                    PresentacionId = p.PresentacionId,
                    PresDsc = p.PresDsc,
                    PresEst = p.PresEst
                };

            // Aplica los filtros
            if (presentacionId > 0) 
                q = q.Where(p => p.PresentacionId == presentacionId);
            if (presDsc != null) 
                q = q.Where(p => p.PresDsc.Contains(presDsc));
            if (presEst != null) 
                q = q.Where(p => p.PresEst == presEst);

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Presentaciones>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Presentaciones>(count, q.ToList()) ;
        }

        public Models.Presentaciones Mostrar(decimal id)
        {
            var q = from p in db.Presentaciones
                where p.PresentacionId == id
                select p;

            return q.FirstOrDefault();
        }

        public Models.Presentaciones Insertar(Models.Presentaciones o)
        {
            // Obtengo el siguiente Id a insertar
            o.PresentacionId = Numeracion();
            db.Insert(o);

            return(o);
        }

        public Models.Presentaciones Actualizar(Models.Presentaciones o)
        {
            db.Update(o);
            return(o);
        }

        public int Numeracion() 
        {
            // Obtengo el último registro
            var q = from p in db.Presentaciones
                orderby p.PresentacionId descending
                select new Models.Presentaciones
                {
                    PresentacionId = p.PresentacionId
                };

            // Retorno el último ID + 1
            return q.FirstOrDefault() != null ? q.FirstOrDefault().PresentacionId + 1 : 1;
        }

    }
}