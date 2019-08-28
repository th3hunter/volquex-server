using System;
using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Viajes
    {
        public Viajes() {}

        public DataProvider<Models.Viajes> Listar(
            decimal viajeId, 
            DateTime desde, 
            DateTime hasta, 
            string cliNom,
            string conducNom,
            string volqDsc,
            string viaEst,
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

                // Hago el SELECT a la tabla Viajes
                var q = from p in db.Viajes
                    join r in tabla
                        on p.ViaEst equals r.EstadoId
                    orderby p.ViaFchHr descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId,
                        ViaFch = p.ViaFch,
                        ViaFchHr = p.ViaFchHr,
                        CliNom = p.fkcliente1.UsuNom,
                        ConducNom = p.fkconductor1.UsuNom,
                        ConducFoto = p.fkconductor1.UsuFoto,
                        VolqDsc = p.fkvolquetas1.VolqDsc,
                        ViaNombreDestino = p.ViaNombreDestino,
                        ViaTotal = p.ViaTotal,
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc
                    };

                // Aplica los filtros
                if (viajeId > 0) 
                    q = q.Where(p => p.ViajeId == viajeId);
                if (desde != null) 
                    q = q.Where(p => p.ViaFch >= desde);
                if (hasta != null) 
                    q = q.Where(p => p.ViaFch <= hasta);
                if (cliNom != null)
                    q = q.Where(p => p.fkcliente1.UsuNom.Contains(cliNom));
                if (conducNom != null)
                    q = q.Where(p => p.fkconductor1.UsuNom.Contains(conducNom));
                if (volqDsc != null)
                    q = q.Where(p => p.fkvolquetas1.VolqDsc.Contains(volqDsc));
                if (viaEst != null) 
                    q = q.Where(p => p.ViaEst == viaEst);

                // Obtiene el total de registros antes de aplicar el paginado
                var count = q.Count();

                // Aplica el paginado
                q = Query<Models.Viajes>.Paginar(q, numPagina, numRegistros);

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Viajes>(count, q.ToList()) ;
            }
        }

        public Models.Viajes Mostrar(decimal id)
        {
            using (var db = new VolquexDB())
            {
                var q = from a in db.Viajes
                    join b in db.Usuarios on a.ClienteId equals b.UsuarioId
                    join c in db.Usuarios on a.ConductorId equals c.UsuarioId
                    join d in db.Volquetas on a.VolquetaId equals d.VolquetaId
                    where a.ViajeId == id
                    select Models.Viajes.Build(a, b, c, d);

                // Agrego los atributos adicionales
                var o = q.FirstOrDefault();
                o.CliNom = o.fkcliente1.UsuNom;
                o.ConducNom = o.fkconductor1.UsuNom;
                o.ConducFoto = o.fkconductor1.UsuFoto;
                o.ConducTipoFoto = o.fkconductor1.UsuTipoFoto;
                o.VolqDsc = o.fkvolquetas1.VolqDsc;
                o.VolqLat = o.fkvolquetas1.VolqLat;
                o.VolqLon = o.fkvolquetas1.VolqLon;
                o.VolqPlaca = o.fkvolquetas1.VolqPlaca;
                o.VolqCapacidad = o.fkvolquetas1.VolqCapacidad;

                // Cargo la lista de materiales
                o.Materiales = new Services.Viajes_Mat().Listar(id, 0, 0).Listado.ToArray();
                return o;
            }
        }

        public Models.Viajes Insertar(Models.Viajes o)
        {
            using(var db = new VolquexDB())
            {
                // Obtengo el siguiente Id a insertar
                o.ViajeId = Numeracion();
                db.Insert(o);
            }

            return(o);
        }

        public Models.Viajes Actualizar(Models.Viajes o)
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
                var q = from p in db.Viajes
                    orderby p.ViajeId descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId
                    };

                // Retorno el último ID + 1
                return q.FirstOrDefault() != null ? q.FirstOrDefault().ViajeId + 1 : 1;
            }
        }
		
		public RespuestaSimple Solicitar(Models.Viajes o)
		{
            using(var db = new VolquexDB())
            {
                // Obtengo el siguiente Id a insertar
                o.ViajeId = Numeracion();
				
				// Obtengo el cliente de la sesión
				o.ClienteId = new Services.Sesiones().ObtenerUsuario();
				
				// Seteo los valores default
				o.ViaFch = DateTime.Now;
				o.ViaFchHr = DateTime.Now;
				o.ViaEst = "SOL";

                Debug.Consola("Viaje", o);
				
                db.Insert(o);
				
                var viajesMat = new Services.Viajes_Mat();

                // Recorre los materiales para insertarlos
                foreach (var material in o.Materiales)
                {
                    material.ViajeId = o.ViajeId;
                    viajesMat.Insertar(material);
                }

                // Genera la bitácora
                new Services.Viajes_Bit().Insertar(o.ViajeId, o.ViaEst);

                // ToDo
                // Envía notificación a las volquetas disponibles
            }

            return(new RespuestaSimple(200, "", o.ViajeId));
		}

        public RespuestaSimple Monitorear(decimal id)
        {
            // Obtiene el estado del viaje y la posición de la volqueta
            using (var db = new VolquexDB())
            {
                var tabla = from Estados in db.Estados
                    where Estados.TablaId == "VIA"
                    select Estados;
					
                var q = from p in db.Viajes
                    join r in tabla
                        on p.ViaEst equals r.EstadoId
                    where p.ViajeId == id
                    select new
                    {
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc,
						VolquetaId = p.VolquetaId,
                        VolqLat = p.fkvolquetas1.VolqLat,
                        VolqLon = p.fkvolquetas1.VolqLon
                    };

                return new RespuestaSimple(q.FirstOrDefault());
            }
        }

        public RespuestaSimple CambiarEstado(decimal id, string estado)
        {
            // Actualiza el estado
            using (var db = new VolquexDB())
            {
                var q = db.Viajes
                    .Where(p => p.ViajeId == id)
                    .Set(p => p.ViaEst, estado);

                // Si es FINALIZADO, actualizo la hora de llegada también
                if (estado == "FIN")
                    q = q.Set(p => p.ViaFchHrLlegada, DateTime.Now);

                q.Update();

                // Cambia el estado a las ofertas
                var r = db.Viajes_Volq
                    .Where(p => p.ViajeId == id)
                    .Set(p => p.ViaVolqEstViaje, estado);
                r.Update();

                // Si el estado es FINALIZADO
                if (estado == "FIN" || estado == "CAN")
                {
                    // Cambia el estado a la volqueta también
                    CambiarEstadoVolqueta(id, "LIB");

                    // ToDo
                    // Envia notificación a la volqueta del viaje Finalizado/Cancelado
                }

                // Genera la bitácora
                new Services.Viajes_Bit().Insertar(id, estado);

                return new RespuestaSimple();
            }
        }

        public void CambiarEstadoVolqueta(decimal viajeId, string estado)
        {
            using(var db = new VolquexDB())
            {
                // Obtiene el Id de la volqueta
                var q = from p in db.Viajes
                    where p.ViajeId == viajeId
                    select p;
                
                int? volquetaId = q.FirstOrDefault().VolquetaId;

                // Actualiza el estado de la volqueta
                db.Volquetas
                    .Where(p => p.VolquetaId == volquetaId)
                    .Set(p => p.VolqEst, estado)
                    .Update();
            }
        }

        public RespuestaSimple ClienteCalifica(
            decimal viajeId, 
            int calificacion)
        {
            // Actualiza las calificaciones
            using (var db = new VolquexDB())
            {
                db.Viajes
                    .Where(p => p.ViajeId == viajeId)
                    .Set(p => p.ViaCalificacionCliente, calificacion)
                    .Update();

                // Obtiene el Id del conductor y de la volqueta
                var q = from p in db.Viajes
                    where p.ViajeId == viajeId
                    select new
                    {
                        p.ConductorId,
                        p.VolquetaId
                    };
                var r = q.FirstOrDefault();

                var conductorId = r.ConductorId;
                var volquetaId = r.VolquetaId;

                // Recalcula las calificaciones del conductor y de la volqueta
                new Services.Usuarios().RecalcularCalificacionConductor(conductorId);
                new Services.Volquetas().RecalcularCalificacion(volquetaId);

            }

            return new RespuestaSimple();
        }

        public DataProvider<Models.Viajes> ListarPorCliente(
            decimal idCliente,
            bool finalizados,
            int numPagina,
            int numRegistros)
        {
            using (var db = new VolquexDB())
            {

                // Hace query a la tabla de Estados
                var estados = from Estados in db.Estados
                    where Estados.TablaId == "VIA"
                    select Estados;

                var q = from p in db.Viajes
                    join r in estados
                        on p.ViaEst equals r.EstadoId
                    where p.ClienteId == idCliente
                    orderby p.ClienteId, p.ViaEst, p.ViaFchHr descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId,
                        ViaFchHr = p.ViaFchHr,
                        ViaNombreDestino = p.ViaNombreDestino,
                        ViaTotal = p.ViaTotal,
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc,
                        // Retorna los materiales de este viaje
                        Materiales = new Services.Viajes_Mat().Listar(p.ViajeId, 0, 0).Listado.ToArray()
                    };
                
                // Si es sólo los finalizados
                if (finalizados)
                    q = q.Where(p => p.ViaEst == "FIN");
                else
                    // Si no es finalizados, busco en cualquier estado menos FINALIZADO ni ANULADO
                    q = q.Where(p => p.ViaEst != "FIN" && p.ViaEst != "X");

                // Obtiene el total de registros antes de aplicar el paginado
                var count = q.Count();

                // Aplica el paginado
                q = Query<Models.Viajes>.Paginar(q, numPagina, numRegistros);

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Viajes>(count, q.ToList()) ;
            }
        }

        public DataProvider<Models.Viajes> ListarPorConductor(
            decimal idConductor,
            bool finalizados,
            int numPagina,
            int numRegistros)
        {
            using (var db = new VolquexDB())
            {
                // Hace query a la tabla de Estados
                var estados = from Estados in db.Estados
                    where Estados.TablaId == "VIA"
                    select Estados;

                var q = from p in db.Viajes
                    join r in estados
                        on p.ViaEst equals r.EstadoId
                    where p.ConductorId == idConductor
                    orderby p.ConductorId, p.ViaEst, p.ViaFchHr descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId,
                        ViaFchHr = p.ViaFchHr,
                        ViaNombreDestino = p.ViaNombreDestino,
                        ViaTotal = p.ViaTotal,
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc,
                        // Retorna los materiales de este viaje
                        Materiales = new Services.Viajes_Mat().Listar(p.ViajeId, 0, 0).Listado.ToArray()
                    };
                
                // Si es sólo los finalizados
                if (finalizados)
                    q = q.Where(p => p.ViaEst == "FIN");
                else
                    // Si no es finalizados, busco los disponibles
                    q = q.Where(p => p.ViaEst == "SOL");

                // Obtiene el total de registros antes de aplicar el paginado
                var count = q.Count();

                // Aplica el paginado
                q = Query<Models.Viajes>.Paginar(q, numPagina, numRegistros);

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Viajes>(count, q.ToList()) ;
            }
        }

    }
}