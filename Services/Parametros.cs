using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Parametros
    {
        private VolquexDB db;

        public Parametros(VolquexDB db) 
        {
            this.db = db;
        }

        public DataProvider<Models.Parametros> Listar(
            string parametroId, 
            string paramDsc, 
            string paramEst,
            int numPagina,
            int numRegistros
            ) 
        {
            var q = from p in db.Parametros
                orderby p.ParamDsc
                select new Models.Parametros
                {
                    ParametroId = p.ParametroId,
                    ParamDsc = p.ParamDsc,
                    ParamVal = p.ParamVal,
                    ParamEst = p.ParamEst
                };

            // Aplica los filtros
            if (parametroId != null) 
                q = q.Where(p => p.ParametroId == parametroId);
            if (paramDsc != null) 
                q = q.Where(p => p.ParamDsc.Contains(paramDsc));
            if (paramEst != null) 
                q = q.Where(p => p.ParamEst == paramEst);

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Parametros>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Parametros>(count, q.ToList()) ;
        }

        public Models.Parametros Mostrar(string id)
        {
            var q = from p in db.Parametros
                where p.ParametroId == id
                select p;

            return q.FirstOrDefault();
        }

        public string ObtenerValor(string id)
        {
            var q = from p in db.Parametros
                where p.ParametroId == id
                select p.ParamVal;

            var valor = q.FirstOrDefault();

            // Si el valor es nulo, pongo mensaje en la consola
            if (valor == null ) Consola.Error("No se ha configurado el parámetro " + id);

            return valor != null ? valor : "";
        }

        public Models.Parametros Insertar(Models.Parametros o)
        {
            db.Insert(o);
            return(o);
        }

        public Models.Parametros Actualizar(Models.Parametros o)
        {
            db.Update(o);
            return(o);
        }

    }
}