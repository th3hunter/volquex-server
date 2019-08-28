using System.Data.Common;
using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Viajes_Volq
    {
        public Viajes_Volq() {}

        public DataProvider<Models.Viajes_Volq> Listar(
            decimal viajeId, 
            int numPagina,
            int numRegistros
            ) 
        {
            using (var db = new VolquexDB())
            {
                var q = from p in db.Viajes_Volq
                    orderby p.ViajeId, p.VolquetaId
                    select new Models.Viajes_Volq
                    {
                        ViajeId = p.ViajeId,
                        VolqDsc = p.fkviajesvolqvolquetas1.VolqDsc,
                        ConducNom = p.fkviajesvolqconductor.UsuNom,
                        ViaVolqTipoDscto = p.ViaVolqTipoDscto,
                        ViaVolqDscto = p.ViaVolqDscto,
                        ViaVolqOferta = p.ViaVolqOferta,
                        ViaVolqEst = p.ViaVolqEst
                    };

                // Aplica los filtros
                if (viajeId > 0) 
                    q = q.Where(p => p.ViajeId == viajeId);

                // Obtiene el total de registros antes de aplicar el paginado
                var count = q.Count();

                // Aplica el paginado
                q = Query<Models.Viajes_Volq>.Paginar(q, numPagina, numRegistros);

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Viajes_Volq>(count, q.ToList()) ;
            }
        }

        public Models.Viajes_Volq Mostrar(decimal id)
        {
            using (var db = new VolquexDB())
            {
                var q = from p in db.Viajes_Volq
                    where p.ViajeId == id
                    select p;

                return q.FirstOrDefault();
            }
        }

        public Models.Viajes_Volq Insertar(Models.Viajes_Volq o)
        {
            using(var db = new VolquexDB())
            {
                // Obtiene la última numeración
                o.ViajeVolquetaId = Numeracion(o.ViajeId);

                // Asigna el conductor actual de la volqueta
                o.ConductorId = o.fkviajesvolqvolquetas1.fkusuarios1.UsuarioId;

                // Inserta el registro
                db.Insert(o);
            }

            return(o);
        }

        public Models.Viajes_Volq Actualizar(Models.Viajes_Volq o)
        {
            using (var db = new VolquexDB())
            {
                db.Update(o);
            }

            return(o);
        }

        public int Numeracion(decimal viajeId)
        {
            using (var db = new VolquexDB())
            {
                // Obtengo el último registro
                var q = from p in db.Viajes_Volq
                    orderby p.ViajeId, p.ViajeVolquetaId descending
                    select new Models.Viajes_Volq
                    {
                        ViajeVolquetaId = p.ViajeVolquetaId
                    };

                // Retorno el último ID + 1
                return q.FirstOrDefault() != null ? q.FirstOrDefault().ViajeVolquetaId + 1 : 1;
            }
        }

        public DataProvider<Models.Viajes_Volq> Ofertas(decimal viajeId)
        {
            // Lee las ofertas del viaje indicado
            using (var db = new VolquexDB())
            {
                var q = from p in db.Viajes_Volq
                    orderby p.ViajeId, p.VolquetaId
                    where p.ViajeId == viajeId
                    select new Models.Viajes_Volq
                    {
                        VolquetaId = p.VolquetaId,
                        VolqDsc = p.fkviajesvolqvolquetas1.VolqDsc,
                        VolqPlaca = p.fkviajesvolqvolquetas1.VolqPlaca,
                        VolqCapacidad = p.fkviajesvolqvolquetas1.VolqCapacidad,
                        ConducNom = p.fkviajesvolqconductor.UsuNom,
                        ConducFoto = p.fkviajesvolqconductor.UsuFoto,
                        ConducTipoFoto = p.fkviajesvolqconductor.UsuTipoFoto,
                        ViaVolqTipoDscto = p.ViaVolqTipoDscto,
                        ViaVolqDscto = p.ViaVolqDscto,
                        ViaVolqOferta = p.ViaVolqOferta,
                        ViaVolqEst = p.ViaVolqEst
                    };

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Viajes_Volq>(q.ToList()) ;
            }
        }

        public RespuestaSimple AceptarOferta(decimal viajeId, int volquetaId)
        {
            using (var db = new VolquexDB())
            {
                db.BeginTransaction();

                // Actualiza el estado de la oferta
                db.Viajes_Volq
                    .Where(p => p.ViajeId == viajeId)
                    .Where(p => p.VolquetaId == volquetaId)
                    .Set(p => p.ViaVolqEst, "ACE")
                    .Set(p => p.ViaVolqEstViaje, "ACE")
                    .Update();

                // Obtiene el conductor y el precio aceptado
                var a = from p in db.Viajes_Volq
                    where p.ViajeId == viajeId
                    where p.VolquetaId == volquetaId
                    select new Models.Viajes_Volq
                    {
                        ConductorId = p.fkviajesvolqvolquetas1.ConductorId,
                        ViaVolqOferta = p.ViaVolqOferta
                    };

                var r = a.FirstOrDefault();
                var conductorId = r.ConductorId;
                var viaVolqOferta = r.ViaVolqOferta;

                // Actualiza el estado del viaje
                // Asigna la volqueta, el conductor y el precio aceptado
				// Actualiza el total del viaje
                db.Viajes
                    .Where(p => p.ViajeId == viajeId)
                    .Set(p => p.ViaEst, "ACE")
                    .Set(p => p.VolquetaId, volquetaId)
                    .Set(p => p.ConductorId, conductorId)
                    .Set(p => p.ViaValorFlete, viaVolqOferta)
					.Set(p => p.ViaTotal, p => p.ViaTotal + viaVolqOferta)
                    .Update();

                // Actualiza el estado de la volqueta
                db.Volquetas
                    .Where(p => p.VolquetaId == volquetaId)
                    .Set(p => p.VolqEst, "VIA")
                    .Update();

                // ToDo
                // Envía notificación a la volqueta

                db.CommitTransaction();

                // Retorna el DTO (Data Transfer Object)
                return new RespuestaSimple();
            }
        }

        public DataProvider<Models.Viajes_Volq> ListarOfertados(decimal conductorId) {
            using (var db = new VolquexDB())
            {
                var q = from p in db.Viajes_Volq
                    where p.ConductorId == conductorId
                    where p.ViaVolqEstViaje == "SOL"
                    where p.ViaVolqEst == "OFE"
                    select p;

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Viajes_Volq>(q.ToList()) ;
            }
        }

    }
}