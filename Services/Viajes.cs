using System.Xml.Linq;
using System;
using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Viajes
    {
        public Viajes(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

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

        public Models.Viajes Mostrar(decimal id)
        {
            var q = from a in db.Viajes
                join b in db.Usuarios on a.ClienteId equals b.UsuarioId
                from c in db.Usuarios.LeftJoin(p => p.UsuarioId == a.ConductorId)
                from d in db.Volquetas.LeftJoin(p => p.VolquetaId == a.VolquetaId)
                where a.ViajeId == id
                select Models.Viajes.Build(a, b, c, d);

            // Agrego los atributos adicionales
            var o = q.FirstOrDefault();
            o.CliNom = o.fkcliente1.UsuNom;
            o.ConducNom = o.fkconductor1.UsuNom;
            o.ConducFoto = o.fkconductor1.UsuFoto;
            o.ConducTipoFoto = o.fkconductor1.UsuTipoFoto;
            o.ConducCalificacion = o.fkconductor1.UsuCalificacion;
            o.VolqDsc = o.fkvolquetas1.VolqDsc;
            o.VolqLat = o.fkvolquetas1.VolqLat;
            o.VolqLon = o.fkvolquetas1.VolqLon;
            o.VolqPlaca = o.fkvolquetas1.VolqPlaca;
            o.VolqCapacidad = o.fkvolquetas1.VolqCapacidad;

            // Cargo la lista de materiales
            o.Materiales = new Services.Viajes_Mat(db).Listar(id, 0, 0).Listado.ToArray();

            // Cargo la lista de la bitácora
            o.Bitacora = new Services.Viajes_Bit(db).Listar(id, 0, 0).Listado.ToArray();
            
            return o;
        }

        public Models.Viajes Insertar(Models.Viajes o)
        {
            // Obtengo el siguiente Id a insertar
            o.ViajeId = Numeracion();
            db.Insert(o);

            return(o);
        }

        public Models.Viajes Actualizar(Models.Viajes o)
        {
            db.Update(o);
            return(o);
        }

        public decimal Numeracion() 
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
		
		public RespuestaSimple Solicitar(Models.Viajes o)
		{
            // Obtengo el siguiente Id a insertar
            o.ViajeId = Numeracion();
            
            // Obtengo el cliente de la sesión
            o.ClienteId = Startup.Usuario.UsuarioId;
            o.ConductorId = null;
            
            // Seteo los valores default
            o.ViaFch = DateTime.Now;
            o.ViaFchHr = DateTime.Now;
            o.ViaEst = "SOL";
            
            db.Insert(o);
            
            var viajesMat = new Services.Viajes_Mat(db);

            // Recorre los materiales para insertarlos
            foreach (var material in o.Materiales)
            {
                material.ViajeId = o.ViajeId;
                viajesMat.Insertar(material);
            }

            // Genera la bitácora
            new Services.Viajes_Bit(db).Insertar(o.ViajeId, o.ViaEst, "");

            // Obtiene las volquetas disponibles junto con sus dispositivos
            var dispositivos = from volq in db.Volquetas
                from disp in db.Usuarios_Disp.InnerJoin(p => p.UsuarioId == volq.ConductorId)
                where volq.VolqEst == "LIB"
                select disp;

            var listaDispositivos = dispositivos.ToArray();

            // Si hay conductores disponibles
            if (listaDispositivos.Length > 0)
            {
                // Envía la notificación a las volquetas
                var mensaje = new Services.Parametros(db).ObtenerValor("MSG-NUEVO-VIAJE");
                var titulo = new Services.Parametros(db).ObtenerValor("MSG-NUEVO-VIAJE-TITULO");

                new Services.Notificaciones(db).EnviarNotificacion(listaDispositivos, o.ViajeId, titulo, mensaje, Accion.NuevoViaje);
            }

            return(new RespuestaSimple(200, "", o.ViajeId));
		}

        public RespuestaSimple Monitorear(decimal id)
        {
            // Obtiene el estado del viaje y la posición de la volqueta
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

        public RespuestaSimple CambiarEstado(decimal viajeId, string estado)
        {
            var q = db.Viajes
                .Where(p => p.ViajeId == viajeId)
                .Set(p => p.ViaEst, estado);

            // Si es FINALIZADO, actualizo la hora de llegada también
            if (estado == "FIN")
                q = q.Set(p => p.ViaFchHrLlegada, DateTime.Now);

            q.Update();

            // Cambia el estado a las ofertas
            var r = db.Viajes_Volq
                .Where(p => p.ViajeId == viajeId)
                .Set(p => p.ViaVolqEstViaje, estado);
            r.Update();
            
            string obs = "";

            // Si el estado es FINALIZADO
            if (estado == "FIN" || estado == "CAN")
            {
                // Cambia el estado a la volqueta también
                CambiarEstadoVolqueta(viajeId, "LIB");

                // Obtengo los datos del viaje
                var viaje = Mostrar(viajeId);

                // Si es FINALIZADO notifico al cliente
                if (estado == "FIN")
                {
                    var mensaje = new Services.Parametros(db).ObtenerValor("MSG-VIAJE-FINALIZADO");
                    var titulo = new Services.Parametros(db).ObtenerValor("MSG-VIAJE-FINALIZADO-TITULO");
                    new Services.Notificaciones(db).EnviarNotificacion(viaje.ClienteId, viajeId, titulo, mensaje, Accion.ViajeFinalizado);
                }

                // Si es CANCELADO
                if (estado == "CAN")
                {
                    // Obtengo el tipo de usuario que cancela
                    var usuarioCancela = new Services.Sesiones(db).Mostrar().fkusuarios1;

                    // Si es CLIENTE, notifico al conductor
                    if (usuarioCancela.UsuTipo == "CLI")
                    {
                        var mensaje = new Services.Parametros(db).ObtenerValor("MSG-CLIENTE-CANCELA");
                        var titulo = new Services.Parametros(db).ObtenerValor("MSG-CLIENTE-CANCELA-TITULO");
                        new Services.Notificaciones(db).EnviarNotificacion(viaje.ConductorId.GetValueOrDefault(), viajeId, titulo, mensaje, Accion.ViajeCancelado);
                        obs = "Cancelado por el cliente";
                    }
                    else
                    {
                        var mensaje = new Services.Parametros(db).ObtenerValor("MSG-CONDUCTOR-CANCELA");
                        var titulo = new Services.Parametros(db).ObtenerValor("MSG-CONDUCTOR-CANCELA-TITULO");
                        new Services.Notificaciones(db).EnviarNotificacion(viaje.ClienteId, viajeId, titulo, mensaje, Accion.ViajeCancelado);
                        obs = "Cancelado por el conductor";
                    }
                }
            }

            // Genera la bitácora
            new Services.Viajes_Bit(db).Insertar(viajeId, estado, obs);

            return new RespuestaSimple();
        }

        public void CambiarEstadoVolqueta(decimal viajeId, string estado)
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

        public RespuestaSimple ClienteCalifica(
            decimal viajeId, 
            int calificacion)
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
            new Services.Usuarios(db).RecalcularCalificacionConductor(conductorId);
            new Services.Volquetas(db).RecalcularCalificacion(volquetaId);

            return new RespuestaSimple();
        }

        public RespuestaSimple ConductorCalifica(
            decimal viajeId, 
            int calificacion)
        {
            db.Viajes
                .Where(p => p.ViajeId == viajeId)
                .Set(p => p.ViaCalificacionConductor, calificacion)
                .Update();

            // Obtiene el Id del cliente
            var q = from p in db.Viajes
                where p.ViajeId == viajeId
                select p.ClienteId;
            var clienteId = q.FirstOrDefault();

            // Recalcula las calificaciones del conductor y de la volqueta
            new Services.Usuarios(db).RecalcularCalificacionCliente(clienteId);

            return new RespuestaSimple();
        }

        public DataProvider<Models.Viajes> ListarPorCliente(
            bool finalizados,
            int numPagina,
            int numRegistros)
        {
            // Obtiene el id del cliente de la sesión
            decimal idCliente = Startup.Usuario.UsuarioId;

            // Hace query a la tabla de Estados
            var estados = from Estados in db.Estados
                where Estados.TablaId == "VIA"
                select Estados;

            var q = from p in db.Viajes
                join r in estados
                    on p.ViaEst equals r.EstadoId
                where p.ClienteId == idCliente
                orderby p.ClienteId, p.ViaFchHr descending
                select new Models.Viajes
                {
                    ViajeId = p.ViajeId,
                    ViaFchHr = p.ViaFchHr,
                    ViaNombreDestino = p.ViaNombreDestino,
                    ViaTotal = p.ViaTotal,
                    ViaEst = p.ViaEst,
                    EstDsc = r.EstDsc,
                };
            
            // Si es sólo los finalizados
            if (finalizados)
                q = q.Where(p => p.ViaEst == "FIN" || p.ViaEst == "CAN" || p.ViaEst == "CAC");
            else
                // Si no es finalizados, busco en cualquier estado menos FINALIZADO ni ANULADO
                q = q.Where(p => p.ViaEst != "FIN" && p.ViaEst != "CAN" && p.ViaEst != "CAC" && p.ViaEst != "X");

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Viajes>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            var dp = new DataProvider<Models.Viajes>(count, q.ToList()) ;
    
            // Recorre la lista para agregar los materiales de este viaje
            dp.Listado.ForEach(i =>
                i.Materiales = new Services.Viajes_Mat(db).Listar(i.ViajeId, 0, 0).Listado.ToArray()
            );

            return dp;
        }

        public DataProvider<Models.Viajes> ListarPorConductor(
            string estado,
            int numPagina,
            int numRegistros)
        {
            // Obtiene el id del cliente de la sesión
            decimal conductorId = Startup.Usuario.UsuarioId;

            // Hace query a la tabla de Estados
            var estados = from Estados in db.Estados
                where Estados.TablaId == "VIA"
                select Estados;

            IQueryable<Models.Viajes> q = null;

            // estado = DISPONIBLES
            // Trae todos los registros en estado = SOL y que no tengan una oferta
            // de éste conductor
            if (estado == "disponibles")
                q = from p in db.Viajes
                    join r in estados
                        on p.ViaEst equals r.EstadoId
                    from s in db.Viajes_Volq
                        .LeftJoin(ofe => p.ViajeId == ofe.ViajeId && 
                            ofe.ConductorId == conductorId)
                    where p.ViaEst == "SOL"
                    where s == null
                    orderby p.ViaEst, p.ViaFchHr descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId,
                        ViaFchHr = p.ViaFchHr,
                        ViaNombreDestino = p.ViaNombreDestino,
                        ViaTotal = p.ViaTotal,
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc,
                        fkcliente1 = p.fkcliente1
                    };

            // estado = OFERTADOS
            // Trae todos los registros en estado = SOL y que sí tengan una oferta
            // de éste conductor
            if (estado == "ofertados")
                q = from p in db.Viajes
                    join r in estados
                        on p.ViaEst equals r.EstadoId
                    from s in db.Viajes_Volq
                        .LeftJoin(ofe => p.ViajeId == ofe.ViajeId && 
                            ofe.ConductorId == conductorId)
                    where p.ViaEst == "SOL"
                    where s.ConductorId == conductorId
                    orderby p.ViaEst, p.ViaFchHr descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId,
                        ViaFchHr = p.ViaFchHr,
                        ViaNombreDestino = p.ViaNombreDestino,
                        ViaTotal = s.ViaVolqOferta + s.ViaVolqTotalMateriales,
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc,
                        // Incluye el detalle del cliente
                        fkcliente1 = p.fkcliente1,
                        // Incluye el detalle de ofertas
                        fkvolq1 = s
                    };
            
            // estado = ENCURSO
            if (estado == "enCurso")
                q = from p in db.Viajes
                    join r in estados
                        on p.ViaEst equals r.EstadoId
                    where p.ConductorId == conductorId
                    where p.ViaEst != "SOL"
                    where p.ViaEst != "FIN"
                    where p.ViaEst != "CAN"
                    where p.ViaEst != "CAC"
                    where p.ViaEst != "X"
                    orderby p.ConductorId, p.ViaFchHr descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId,
                        ViaFchHr = p.ViaFchHr,
                        ViaNombreDestino = p.ViaNombreDestino,
                        ViaTotal = p.ViaTotal,
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc,
                        fkcliente1 = p.fkcliente1
                    };
            
            // estado = FINALIZADOS
            if (estado == "finalizados")
                q = from p in db.Viajes
                    join r in estados
                        on p.ViaEst equals r.EstadoId
                    where p.ConductorId == conductorId
                    where p.ViaEst == "FIN" || p.ViaEst == "CAN" || p.ViaEst == "CAC"
                    orderby p.ConductorId, p.ViaFchHr descending
                    select new Models.Viajes
                    {
                        ViajeId = p.ViajeId,
                        ViaFchHr = p.ViaFchHr,
                        ViaNombreDestino = p.ViaNombreDestino,
                        ViaTotal = p.ViaTotal,
                        ViaEst = p.ViaEst,
                        EstDsc = r.EstDsc,
                        fkcliente1 = p.fkcliente1
                    };

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Viajes>.Paginar(q, numPagina, numRegistros);

            // Crea el DTO (Data Transfer Object)
            var dp = new DataProvider<Models.Viajes>(count, q.ToList()) ;

            // Recorre la lista para agregarle los materiales
            dp.Listado.ForEach(i =>
            {
                i.Materiales = new Services.Viajes_Mat(db).Listar(i.ViajeId, 0, 0).Listado.ToArray();
            });

            return dp;
        }

        public Models.Viajes EnCurso() 
        {
            // Obtiene el id del usuario de la sesión
            var conductorId = Startup.Usuario.UsuarioId;

            // Hace query a la tabla de Estados
            var estados = from Estados in db.Estados
                where Estados.TablaId == "VIA"
                select Estados;

            // Verifica si hay algún viaje en curso con este conductor
            var q = from p in db.Viajes
                join r in estados
                    on p.ViaEst equals r.EstadoId
                where p.ConductorId == conductorId
                where p.ViaEst == "ACE"
                orderby p.ConductorId, p.ViaEst
                select new Models.Viajes
                {
                    ViajeId = p.ViajeId,
                    ViaFchHr = p.ViaFchHr,
                    ViaDestinoLat = p.ViaDestinoLat,
                    ViaDestinoLon = p.ViaDestinoLon,
                    ViaNombreDestino = p.ViaNombreDestino,
                    ViaValorFlete = p.ViaValorFlete,
                    ViaTotal = p.ViaTotal,
                    ViaEst = p.ViaEst,
                    EstDsc = r.EstDsc,
                    // Inlcuye el detalle del cliente
                    fkcliente1 = p.fkcliente1
                };

            var viaje = q.FirstOrDefault();

            // Si encontró un viaje
            if (viaje != null)
            {
                // Agrega el detalle de materiales
                viaje.Materiales = new Services.Viajes_Mat(db).Listar(viaje.ViajeId, 0, 0).Listado.ToArray();

                // Cambia el estado al viaje indicando de que ya se notificó al conductor
                CambiarEstado(viaje.ViajeId, "NOT");
            }

            return viaje;
        }

        public RespuestaSimple ActualizarPosicion(decimal viajeId, string lat, string lon)
        {
            // Obtiene la volqueta del viaje y el estado del mismo
            var via = from p in db.Viajes
                where p.ViajeId == viajeId
                select new Models.Viajes 
                {
                    VolquetaId = p.VolquetaId,
                    ViaEst = p.ViaEst
                };
            
            var viaje = via.FirstOrDefault();

            // Actualiza la posición de la volqueta
            db.Volquetas
                .Where(p => p.VolquetaId == viaje.VolquetaId)
                .Set(p => p.VolqLat, lat)
                .Set(p => p.VolqLon, lon)
                .Update();

            return new RespuestaSimple(viaje.ViaEst);
        }

        public decimal ObtenerCliente(decimal viajeId) {
            var q = from p in db.Viajes
                where p.ViajeId == viajeId
                select p.ClienteId;

            return q.FirstOrDefault();
        }

        public decimal ObtenerConductor(decimal viajeId) {
            var q = from p in db.Viajes
                where p.ViajeId == viajeId
                select p.ConductorId;

            return q.FirstOrDefault().GetValueOrDefault(0);
        }

    }
}