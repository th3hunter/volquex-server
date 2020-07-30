using System.Data.Common;
using System;
using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Viajes_Volq
    {
        public Viajes_Volq(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

        public DataProvider<Models.Viajes_Volq> Listar(
            decimal viajeId, 
            int numPagina,
            int numRegistros
            ) 
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

        public Models.Viajes_Volq Mostrar(decimal id)
        {
            var q = from p in db.Viajes_Volq
                where p.ViajeId == id
                select p;

            return q.FirstOrDefault();
        }

        public Models.Viajes_Volq Insertar(Models.Viajes_Volq o)
        {
            // Obtiene la última numeración
            o.ViajeVolquetaId = Numeracion(o.ViajeId);

            // Asigna el conductor actual de la volqueta
            o.ConductorId = new Services.Volquetas(db).ObtenerUsuario(o.VolquetaId);

            // Inserta el registro
            db.Insert(o);

            return(o);
        }

        public Models.Viajes_Volq Actualizar(Models.Viajes_Volq o)
        {
            db.Update(o);
            return(o);
        }

        public int Numeracion(decimal viajeId)
        {
            // Obtengo el último registro
            var q = from p in db.Viajes_Volq
                where p.ViajeId == viajeId
                orderby p.ViajeId, p.ViajeVolquetaId descending
                select p.ViajeVolquetaId;

            // Retorno el último ID + 1
            var ultimo = q.FirstOrDefault();
            return ultimo > 0 ? ultimo + 1 : 1;
        }

        public DataProvider<Models.Viajes_Volq> Ofertas(decimal viajeId)
        {
            var q = from p in db.Viajes_Volq
                orderby p.ViajeId, p.VolquetaId
                where p.ViajeId == viajeId
                select new Models.Viajes_Volq
                {
                    ViajeVolquetaId = p.ViajeVolquetaId,
                    VolquetaId = p.VolquetaId,
                    VolqDsc = p.fkviajesvolqvolquetas1.VolqDsc,
                    VolqPlaca = p.fkviajesvolqvolquetas1.VolqPlaca,
                    VolqCapacidad = p.fkviajesvolqvolquetas1.VolqCapacidad,
                    ConducNom = p.fkviajesvolqconductor.UsuNom,
                    ConducFoto = p.fkviajesvolqconductor.UsuFoto,
                    ConducTipoFoto = p.fkviajesvolqconductor.UsuTipoFoto,
                    ConducCalificacion = p.fkviajesvolqconductor.UsuCalificacion,
                    ViaVolqTipoDscto = p.ViaVolqTipoDscto,
                    ViaVolqDscto = p.ViaVolqDscto,
                    ViaVolqOferta = p.ViaVolqOferta + p.ViaVolqTotalMateriales,
                    ViaVolqEst = p.ViaVolqEst
                };

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Viajes_Volq>(q.ToList()) ;
        }

        public RespuestaSimple GuardarOferta(decimal viajeId, decimal valorOferta) 
        {
            // Obtengo el usuario actual
            var usuarioId = Startup.Usuario.UsuarioId;

            // Obtengo la volqueta actual de este usuario
            var volquetaId = new Services.Volquetas(db).ObtenerPorUsuario(usuarioId);

            // Si no está asignado a ninguna volqueta
            if (volquetaId == 0)
            {
                return new RespuestaSimple(500, "El usuario actual no está asignado a ninguna volqueta");
            }

            // Crea una nueva oferta
            var oferta = new Models.Viajes_Volq
            {
                ViajeId = viajeId,
                // Obtiene la última numeración
                ViajeVolquetaId = Numeracion(viajeId),
                VolquetaId = volquetaId,
                ConductorId = usuarioId,
                ViaVolqOferta = valorOferta,
                ViaVolqEst = "OFE",
                ViaVolqEstViaje = "SOL"
            };
            
            db.Insert(oferta);
            return new RespuestaSimple();
        }

        public RespuestaSimple GuardarOfertaMateriales(Models.Viajes_Volq oferta) 
        {
            // Obtengo el usuario actual
            var usuarioId = Startup.Usuario.UsuarioId;

            // Obtengo la volqueta actual de este usuario
            var volquetaId = new Services.Volquetas(db).ObtenerPorUsuario(usuarioId);

            // Si no está asignado a ninguna volqueta
            if (volquetaId == 0)
            {
                return new RespuestaSimple(500, "El usuario actual no está asignado a ninguna volqueta");
            }
            
            // Obtiene la última numeración
            oferta.ViajeVolquetaId = Numeracion(oferta.ViajeId);

            // Configura la oferta con los datos faltantes
            oferta.VolquetaId = volquetaId;
            oferta.ConductorId = usuarioId;
            oferta.ViaVolqEst = "OFE";
            oferta.ViaVolqEstViaje = "SOL";

            decimal totalMateriales = 0;

            // Obtiene el total de los materiales
            foreach (var material in oferta.Materiales)
                totalMateriales += material.ViaVolqMatCantidad * material.ViaVolqMatPrecio;

            // Graba el total de la oferta
            oferta.ViaVolqTotalMateriales = totalMateriales;
            
            // Inserta la nueva oferta
            db.Insert(oferta);

            // Inserta los materiales
            foreach (var material in oferta.Materiales)
            {
                material.ViajeId = oferta.ViajeId;
                material.ViajeVolquetaId = oferta.ViajeVolquetaId;
                db.Insert(material);

                // Guarda el precio más reciente de cada material en el usuario
                var historial = new Models.Usuarios_Mat
                {
                    UsuarioId = usuarioId,
                    MaterialId = material.MaterialId,
                    UsuMatPrecio = material.ViaVolqMatPrecio
                };

                db.InsertOrReplace(historial);
            }

            return new RespuestaSimple();
        }

        public RespuestaSimple CancelarOferta(decimal viajeId)
        {
            // Obtiene el usuario de la sesión
            var usuarioId = Startup.Usuario.UsuarioId;

            // Obtiene la volqueta asociada a este usuario
            var volquetaId = new Services.Volquetas(db).ObtenerPorUsuario(usuarioId);

            // Si no encontró volqueta, retorno error
            if (volquetaId == 0) return new RespuestaSimple(500, "El usuario no está asociado a ninguna volqueta.");

            // Primero verifica que la oferta no haya sido aceptada
            var q = from p in db.Viajes_Volq
                where p.ViajeId == viajeId
                where p.VolquetaId == volquetaId
                select p.ViaVolqEst;

            if (q.FirstOrDefault() == "ACE") 
                return new RespuestaSimple(500, "No se puede cancelar la oferta porque ya fue aceptada.");
            
            // Elimina la oferta
            db.Viajes_Volq
                .Where(p => p.ViajeId == viajeId)
                .Where(p => p.VolquetaId == volquetaId)
                .Delete();

            return new RespuestaSimple();
        }

        public Models.Viajes ConsultarOferta(decimal viajeId, string estado)
        {
            // Obtiene el usuario actual
            var conductorId = Startup.Usuario.UsuarioId;

            var q = from p in db.Viajes
                from r in db.Viajes_Volq
                    .LeftJoin(ofe => p.ViajeId == ofe.ViajeId && 
                    ofe.ConductorId == conductorId)
                where p.ViajeId == viajeId   
                select new Models.Viajes
                {
                    ViajeId = p.ViajeId,
                    ViaFchHr = p.ViaFchHr,
                    ViaEst = p.ViaEst,
                    ViaDestinoLat = p.ViaDestinoLat,
                    ViaDestinoLon = p.ViaDestinoLon,
                    ViaNombreDestino = p.ViaNombreDestino,
                    ViaCalificacionCliente = p.ViaCalificacionCliente,
                    ViaValorFlete = p.ViaValorFlete,
                    ViaTotal = p.ViaTotal,
                    // Incluye el detalle del cliente
                    fkcliente1 = p.fkcliente1,
                    // Incluye el detalle de ofertas
                    fkvolq1 = new Models.Viajes_Volq
                    {
                        ViajeId = r.ViajeId,
                        ViajeVolquetaId = r.ViajeVolquetaId,
                        VolquetaId = r.VolquetaId,
                        ViaVolqOferta = r.ViaVolqOferta,
                        ViaVolqEst = r.ViaVolqEst,
                        ConductorId = r.ConductorId,
                        ViaVolqEstViaje = r.ViaVolqEstViaje,
                        ViaVolqTotalMateriales = r.ViaVolqTotalMateriales
                    }
                }; 

            var viaje = q.FirstOrDefault();

            // Carga los materiales de la oferta
            var query = from s in db.Viajes_Volq_Mat
                where s.ViajeId == viaje.ViajeId
                    && s.ViajeVolquetaId == viaje.fkvolq1.ViajeVolquetaId
                select new Models.Viajes_Volq_Mat
                {
                    ViaVolqMatCantidad = s.ViaVolqMatCantidad,
                    ViaVolqMatPrecio = s.ViaVolqMatPrecio,
                    Material = new Models.Materiales
                    {
                        MatDsc = s.Material.MatDsc,
                        fkpresentaciones1 = s.Material.fkpresentaciones1
                    }
                };

            viaje.fkvolq1.Materiales = query.ToArray();

            // Si el estado es SOLICITADO
            if (viaje.ViaEst == "SOL")
                // Obtiene el valor base del flete de los parámetros
                viaje.ViaValorFlete = Decimal.Parse(new Services.Parametros(db).ObtenerValor("FLETE"));

            // Incluye los materiales de este viaje
            viaje.Materiales = new Services.Viajes_Mat(db).Listar(viajeId, 0, 0).Listado.ToArray();

            // Si el estado es DISPONIBLES, pongo el último precio de cada material
            if (estado == "disponibles")
            {
                foreach (var material in viaje.Materiales)
                {
                    var precio = 
                        from viajesMat in db.Viajes_Mat
                        from usuariosMat in db.Usuarios_Mat
                            .LeftJoin(hist => hist.MaterialId == viajesMat.MaterialId
                                && hist.UsuarioId == conductorId)
                        where viajesMat.ViajeId == viajeId
                            && viajesMat.MaterialId == material.MaterialId
                        select usuariosMat.UsuMatPrecio;

                    // Obtiene el precio y recalcula el importe
                    material.ViaMatPrecio = precio.FirstOrDefault();
                    material.ViaMatImporte = material.ViaMatPrecio * material.ViaMatCantidad;
                }
            }

            // Cargo la lista de la bitácora
            viaje.Bitacora = new Services.Viajes_Bit(db).Listar(viajeId, 0, 0).Listado.ToArray();

            // Si se está consultando un viaje desde la pestaña EN CURSO
            if (estado == "enCurso")
                // Si el viaje recién ha sido ACEPTADO
                if (viaje.ViaEst == "ACE")
                {
                    // Actualizo el estado a CONDUCTOR NOTIFICADO
                    viaje.ViaEst = "NOT";
                    new Services.Viajes(db).CambiarEstado(viajeId, "NOT");
                }
                
            return viaje;
        }

        public DataProvider<Models.Viajes_Volq_Mat> ConsultarOfertaMateriales(decimal viajeId, decimal viajeVolquetaId)
        {
            var q = from p in db.Viajes_Volq_Mat
                where p.ViajeId == viajeId
                where p.ViajeVolquetaId == viajeVolquetaId
                select new Models.Viajes_Volq_Mat
                {
                    ViajeId = p.ViajeId,
                    ViajeVolquetaId = p.ViajeVolquetaId,
                    MaterialId = p.MaterialId,
                    ViaVolqMatPrecio = p.ViaVolqMatPrecio,
                    ViaVolqMatCantidad = p.ViaVolqMatCantidad,
                    Oferta = p.Oferta,
                    Material = new Models.Materiales {
                        MaterialId = p.Material.MaterialId,
                        MatDsc = p.Material.MatDsc,
                        Presentacion = p.Material.Presentacion,
                        MatPrecio = p.Material.MatPrecio,
                        fkpresentaciones1 = p.Material.fkpresentaciones1
                    }
                };

            return new DataProvider<Models.Viajes_Volq_Mat>(q.ToList()) ;
        }

        public RespuestaSimple AceptarOferta(decimal viajeId, int volquetaId)
        {
            db.BeginTransaction();

            // Verifica si la volqueta está libre
            var lib = from p in db.Volquetas
                where p.VolquetaId == volquetaId
                where p.VolqEst == "LIB"
                select p.VolquetaId;
            
            // Si no retornó ningún registro
            if (lib.FirstOrDefault() == 0)
                // Retorno error
                return new RespuestaSimple(500, "La volqueta se encuentra en otro viaje. Intente más tarde.");

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
                    ViajeVolquetaId = p.ViajeVolquetaId,
                    ConductorId = p.fkviajesvolqvolquetas1.ConductorId,
                    ViaVolqOferta = p.ViaVolqOferta
                };

            var r = a.FirstOrDefault();
            var conductorId = r.ConductorId;
            var viaVolqOferta = r.ViaVolqOferta;

            // Recorre los materiales del viaje
            var materiales =
                from mat in db.Viajes_Mat
                where mat.ViajeId == viajeId
                select mat;

            foreach (var material in materiales.ToArray())
            {
                // Obtiene el precio ofertado de la volqueta
                var oferta =
                    from ofe in db.Viajes_Volq_Mat
                    where ofe.ViajeId == viajeId
                        && ofe.ViajeVolquetaId == r.ViajeVolquetaId
                        && ofe.MaterialId == material.MaterialId
                    select ofe.ViaVolqMatPrecio;

                var precio = oferta.FirstOrDefault();

                // Actualiza el precio y el importe en Viajes_Mat
                db.Viajes_Mat
                    .Where(p => p.ViajeId == viajeId)
                    .Where(p => p.MaterialId == material.MaterialId)
                    .Set(p => p.ViaMatPrecio, precio)
                    .Set(p => p.ViaMatImporte, imp => precio * imp.ViaMatCantidad)
                    .Update();
            }

            // Totaliza los materiales
            var sum =  
                from viajesMat in db.Viajes_Mat
                where viajesMat.ViajeId == viajeId
                select viajesMat.ViaMatImporte;

            var total = sum.Sum();

            // Actualiza el estado del viaje
            // Asigna la volqueta, el conductor y el precio aceptado
            // Actualiza el total del viaje
            db.Viajes
                .Where(p => p.ViajeId == viajeId)
                .Set(p => p.ViaEst, "ACE")
                .Set(p => p.VolquetaId, volquetaId)
                .Set(p => p.ConductorId, conductorId)
                .Set(p => p.ViaValorFlete, viaVolqOferta)
                .Set(p => p.ViaTotal, total + viaVolqOferta)
                .Update();

            // Actualiza el estado de la volqueta
            db.Volquetas
                .Where(p => p.VolquetaId == volquetaId)
                .Set(p => p.VolqEst, "VIA")
                .Update();

            db.CommitTransaction();

            // Envía notificación a la volqueta
            var mensaje = new Services.Parametros(db).ObtenerValor("MSJ-ACEPTA-OFERTA");
            var titulo = new Services.Parametros(db).ObtenerValor("MSJ-ACEPTA-OFERTA-TITULO");
            new Services.Notificaciones(db).EnviarNotificacion(conductorId, viajeId, titulo, mensaje, Accion.OfertaAceptada);

            // Retorna el DTO (Data Transfer Object)
            return new RespuestaSimple();
        }

        public DataProvider<Models.Viajes_Volq> ListarOfertados(decimal conductorId) 
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