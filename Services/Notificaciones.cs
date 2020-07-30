using System.Collections.Generic;
using System.Net.Http.Headers;
using System;
using System.Linq;
using System.Net.Http;
using LinqToDB;
using Newtonsoft.Json;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Notificaciones
    {
        private VolquexDB db;

        public Notificaciones(VolquexDB db) 
        {
            this.db = db;
        }

        public DataProvider<Models.Notificaciones> ListarPorViaje(
            decimal viajeId, 
            int numPagina,
            int numRegistros
            ) 
        {
            var q = from p in db.Notificaciones
                where p.ViajeId == viajeId
                orderby p.ViajeId, p.NotFchHr
                select new Models.Notificaciones
                {
                    NotificacionId = p.NotificacionId,
                    NotFchHr = p.NotFchHr,
                    ViajeId = p.ViajeId,
                    UsuarioId = p.UsuarioId,
                    DispositivoId = p.DispositivoId,
                    NotMensaje = p.NotMensaje,
                    NotRespuesta = p.NotRespuesta,
                    NotEst = p.NotEst
                };

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Notificaciones>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Notificaciones>(count, q.ToList()) ;
        }

        public DataProvider<Models.Notificaciones> ListarPorUsuario(
            decimal usuarioId, 
            int numPagina,
            int numRegistros
            ) 
        {
            var q = from p in db.Notificaciones
                where p.UsuarioId == usuarioId
                orderby p.UsuarioId, p.NotFchHr
                select new Models.Notificaciones
                {
                    NotificacionId = p.NotificacionId,
                    NotFchHr = p.NotFchHr,
                    ViajeId = p.ViajeId,
                    UsuarioId = p.UsuarioId,
                    DispositivoId = p.DispositivoId,
                    NotMensaje = p.NotMensaje,
                    NotRespuesta = p.NotRespuesta,
                    NotEst = p.NotEst
                };

            // Obtiene el total de registros antes de aplicar el paginado
            var count = q.Count();

            // Aplica el paginado
            q = Query<Models.Notificaciones>.Paginar(q, numPagina, numRegistros);

            // Retorna el DTO (Data Transfer Object)
            return new DataProvider<Models.Notificaciones>(count, q.ToList()) ;
        }

        public Models.Notificaciones Mostrar(decimal id)
        {
            var q = from p in db.Notificaciones
                where p.NotificacionId == id
                select p;

            return q.FirstOrDefault();
        }

        public decimal Insertar(Models.Notificaciones o)
        {
            return db.InsertWithDecimalIdentity(o);
        }

        public Models.Notificaciones Actualizar(Models.Notificaciones o)
        {
            db.Update(o);
            return(o);
        }

        public void EnviarNotificacion(decimal[] listaUsuarios, decimal viajeId, string titulo, string mensaje, Accion accion)
        {
            var listaDispositivos = new Models.Usuarios_Disp[0];

            // Recorre la lista de usuarios
            foreach(var usuarioId in listaUsuarios)
            {
                // Obtiene los dispositivos de este usuario
                listaDispositivos.Concat(new Services.Usuarios_Disp(db).Listar(usuarioId));
            }

            // Envía la notificación
            EnviarNotificacion(listaDispositivos, viajeId, titulo, mensaje, accion);
        }

        public void EnviarNotificacion(decimal usuarioId, decimal viajeId, string titulo, string mensaje, Accion accion)
        {
            // Obtiene los dispositivos de este usuario
            var dispositivos = new Services.Usuarios_Disp(db).Listar(usuarioId);

            // Envía la notificación
            EnviarNotificacion(dispositivos.ToArray(), viajeId, titulo, mensaje, accion);
        }

        public async void EnviarNotificacion(Models.Usuarios_Disp[] dispositivos, decimal viajeId, string titulo, string mensaje, Accion accion)
        {
            // Inicializo los objetos para hacer el Request
            var client = new HttpClient();
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            
            // Arma la URL para enviar la notificación
            request.RequestUri = new Uri("https://fcm.googleapis.com/fcm/send");
            request.Headers.TryAddWithoutValidation("Authorization", "key=" + Startup.Firebase.CloudMessagingKey);

            // Arma la lista de tokens
            var tokens = new List<string>();
            foreach (var d in dispositivos)
                tokens.Add(d.DispositivoId);

            // Crea la instancia de la notificación
            Notificacion notificacion = new Notificacion(tokens.ToArray(), viajeId, titulo, mensaje, accion);

            // Adjunta el body
            var json = JsonConvert.SerializeObject(notificacion);

            Consola.Debug("json", json);
            
            request.Content = new StringContent(json);

            // Adjunta los headers
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Envía el request al Firebase
            var response = await client.SendAsync(request);

            // Crea los datos del registro a insertar
            var r = new Models.Notificaciones
            {
                NotFchHr = DateTime.Now,
                ViajeId = viajeId,
                NotMensaje = titulo + " - " + mensaje + " - Data: " + json,
                NotRespuesta = response.Content.ReadAsStringAsync().Result,
                NotEst = response.StatusCode.ToString()
            };

            // Uso una nueva conexión porque este método se llama asíncronamente
            using (var db2 = new VolquexDB())
                // Creo un registro con cada token
                foreach(var disp in dispositivos)
                {
                    r.UsuarioId = disp.UsuarioId;
                    r.DispositivoId = disp.DispositivoId;

                    // Hago el insert
                    db2.InsertWithDecimalIdentity(r);
                }

            client.Dispose();

        }

    }
}