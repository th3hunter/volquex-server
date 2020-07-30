using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Estados
    {
        public Estados(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

        public DataProvider<Models.Estados> Listar(
            string tablaId, 
            string estadoId, 
            string estDsc, 
            string estEst,
            int numPagina,
            int numRegistros
            ) 
        {
            var q = from p in db.Estados
                orderby p.TablaId, p.EstOrden
                select new Models.Estados
                {
                    TablaId = p.TablaId,
                    TablaDsc = NombreTabla(p.TablaId),
                    EstadoId = p.EstadoId,
                    EstDsc = p.EstDsc,
                    EstOrden = p.EstOrden,
                    EstEst = p.EstEst
                };

            // Aplica los filtros
            if (tablaId != null) 
                q = q.Where(p => p.TablaId == tablaId);
            if (estadoId != null) 
                q = q.Where(p => p.EstadoId == estadoId);
            if (estDsc != null) 
                q = q.Where(p => p.EstDsc.Contains(estDsc));
            if (estEst != null) 
                q = q.Where(p => p.EstEst.Contains(estEst));

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Estados>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Estados>(count, q.ToList()) ;
        }

        public Models.Estados Mostrar(string tablaId, string estadoId)
        {
            var q = from p in db.Estados
                where p.TablaId == tablaId
                where p.EstadoId == estadoId
                select p;

            return q.FirstOrDefault();
        }

        public Models.Estados Insertar(Models.Estados o)
        {
            db.Insert(o);
            return(o);
        }

        public Models.Estados Actualizar(Models.Estados o)
        {
            db.Update(o);
            return(o);
        }

        public string NombreTabla(string tablaId)
        {
            string nombreModulo = "";

            switch (tablaId)
            {
                case "MAT":
                    nombreModulo = "Materiales";
                    break;
                case "VVO":
                    nombreModulo = "Ofertas";
                    break;
                case "PAR":
                    nombreModulo = "Parámetros";
                    break;
                case "PRE":
                    nombreModulo = "Presentaciones";
                    break;
                case "USU":
                    nombreModulo = "Usuarios";
                    break;
                case "VIA":
                    nombreModulo = "Viajes";
                    break;
                case "VOL":
                    nombreModulo = "Volquetas";
                    break;
            }

            return nombreModulo;
        }

        public DataProvider<Models.Estados> Elegibles(string tablaId)
        {
            var q = from p in db.Estados
                where p.TablaId == tablaId
                where p.EstElegible == "1"
                where p.EstEst == "1"
                orderby p.TablaId, p.EstOrden
                select p;

            return new DataProvider<Models.Estados>(q.ToList());
        }

    }
}