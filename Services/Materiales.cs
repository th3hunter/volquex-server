using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Materiales
    {
        public Materiales() {}

        public DataProvider<Models.Materiales> Listar(
            decimal materialId, 
            string matDsc, 
            string matEst,
            int numPagina,
            int numRegistros
            ) 
        {
            using (var db = new VolquexDB())
            {
                var q = from p in db.Materiales
                    orderby p.MatDsc
                    select new Models.Materiales
                    {
                        MaterialId = p.MaterialId,
                        MatDsc = p.MatDsc,
                        PresDsc = p.fkpresentaciones1.PresDsc,
                        MatPrecio = p.MatPrecio,
                        MatEst = p.MatEst
                    };

                // Aplica los filtros
                if (materialId > 0) 
                    q = q.Where(p => p.MaterialId == materialId);
                if (matDsc != null) 
                    q = q.Where(p => p.MatDsc.Contains(matDsc));
                if (matEst != null) 
                    q = q.Where(p => p.MatEst == matEst);

                // Obtiene el total de registros antes de aplicar el paginado
                var count = q.Count();

                // Aplica el paginado
                q = Query<Models.Materiales>.Paginar(q, numPagina, numRegistros);

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Materiales>(count, q.ToList()) ;
            }
        }

        public Models.Materiales Mostrar(decimal id)
        {
            using (var db = new VolquexDB())
            {
                var q = from p in db.Materiales
                    where p.MaterialId == id
                    select p;

                return q.FirstOrDefault();
            }
        }

        public Models.Materiales Insertar(Models.Materiales o)
        {
            using(var db = new VolquexDB())
            {
                // Obtengo el siguiente Id a insertar
                o.MaterialId = Numeracion();
                db.Insert(o);
            }

            return(o);
        }

        public Models.Materiales Actualizar(Models.Materiales o)
        {
            using (var db = new VolquexDB())
            {
                db.Update(o);
            }

            return(o);
        }

        public decimal Numeracion() {
            using (var db = new VolquexDB())
            {
                // Obtengo el último registro
                var q = from p in db.Materiales
                    orderby p.MaterialId descending
                    select new Models.Materiales
                    {
                        MaterialId = p.MaterialId
                    };

                // Retorno el último ID + 1
                return q.FirstOrDefault() != null ? q.FirstOrDefault().MaterialId + 1 : 1;
            }
        }

    }
}