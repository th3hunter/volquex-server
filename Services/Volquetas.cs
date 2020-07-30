using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Volquetas
    {
        public Volquetas(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

        public DataProvider<Models.Volquetas> Listar(
            decimal volquetaId, 
            string volqDsc, 
            string conducNom, 
            string volqEst,
            int numPagina,
            int numRegistros
            ) 
        {
            var tabla = from Estados in db.Estados
                where Estados.TablaId == "VOL"
                select Estados;

            var q = from p in db.Volquetas
                join r in tabla
                    on p.VolqEst equals r.EstadoId
                orderby p.VolqDsc
                select new Models.Volquetas
                {
                    VolquetaId = p.VolquetaId,
                    VolqDsc = p.VolqDsc,
                    ConducNom = p.fkusuarios1.UsuNom,
                    VolqPlaca = p.VolqPlaca,
                    VolqEst = p.VolqEst,
                    EstDsc = r.EstDsc
                };

            // Aplica los filtros
            if (volquetaId > 0) 
                q = q.Where(p => p.VolquetaId == volquetaId);
            if (volqDsc != null) 
                q = q.Where(p => p.VolqDsc.Contains(volqDsc));
            if (conducNom != null) 
                q = q.Where(p => p.fkusuarios1.UsuNom.Contains(conducNom));
            if (volqEst != null) 
                q = q.Where(p => p.VolqEst == volqEst);

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Volquetas>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Volquetas>(count, q.ToList()) ;
        }

        public Models.Volquetas Mostrar(decimal id)
        {
            var q = from a in db.Volquetas
                join b in db.Usuarios on a.ConductorId equals b.UsuarioId
                where a.VolquetaId == id
                select Models.Volquetas.Build(a, b);
            
            // Agrego los atributos adicionales
            var o = q.FirstOrDefault();
            o.ConducNom = o.fkusuarios1.UsuNom;
            o.ConducFoto = o.fkusuarios1.UsuFoto;
            o.ConducTipoFoto = o.fkusuarios1.UsuTipoFoto;
            return o;
        }

        public Models.Volquetas Insertar(Models.Volquetas o)
        {
            // Obtengo el siguiente Id a insertar
            o.VolquetaId = Numeracion();
            db.Insert(o);

            return(o);
        }

        public Models.Volquetas Actualizar(Models.Volquetas o)
        {
            db.Update(o);
            return(o);
        }

        public int Numeracion() 
        {
            // Obtengo el último registro
            var q = from p in db.Volquetas
                orderby p.VolquetaId descending
                select new Models.Volquetas
                {
                    VolquetaId = p.VolquetaId
                };

            // Retorno el último ID + 1
            return q.FirstOrDefault() != null ? q.FirstOrDefault().VolquetaId + 1 : 1;
        }

        public RespuestaSimple ActualizarPosicion(decimal id, string lat, string lon) 
        {
            db.Volquetas
                .Where(p => p.VolquetaId == id)
                .Set(p => p.VolqLat, lat)
                .Set(p => p.VolqLon, lon)
                .Update();

            return new RespuestaSimple();
        }

        public void RecalcularCalificacion(decimal? id)
        {
            // Cuenta y suma las calificaciones de este conductor
            var q = db.Viajes
                .OrderBy(p => p.VolquetaId)
                .Where(p => p.VolquetaId == id)
                .Where(p => p.ViaEst == "FIN");
            
            var count = q.Count();
            var sum = q.Sum(p => p.ViaCalificacionCliente);
            var calificacion = sum / count;

            // Actualiza la calificación
            db.Volquetas
                .Where(p => p.VolquetaId == id)
                .Set(p => p.VolqCalificacion, calificacion)
                .Update();
        }

        public decimal ObtenerUsuario(int volquetaId)
        {
            var q = from p in db.Volquetas
                where p.VolquetaId == volquetaId
                select p.ConductorId;
            
            return q.FirstOrDefault();
        }

        public int ObtenerPorUsuario(decimal usuarioId)
        {
            var q = from p in db.Volquetas
                where p.ConductorId == usuarioId
                select p.VolquetaId;
            
            return q.FirstOrDefault();
        }

    }
}