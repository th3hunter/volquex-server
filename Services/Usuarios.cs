using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinqToDB;
using Newtonsoft.Json;
using Volquex.Models;
using Volquex.Utils;
	
namespace Volquex.Services
{
    public class Usuarios
    {

        public Usuarios() {}

        public DataProvider<Models.Usuarios> Listar(
            decimal usuarioId, 
            string usuNom, 
            string usuTipo,
            string usuEst,
            int numPagina,
            int numRegistros
            ) 
        {
            using (var db = new VolquexDB())
            {
                var q = from p in db.Usuarios
                    orderby p.UsuNom
                    select new Models.Usuarios
                    {
                        UsuarioId = p.UsuarioId,
                        UsuNom = p.UsuNom,
                        UsuEmail = p.UsuEmail,
                        UsuCel = p.UsuCel,
                        UsuTipo = p.UsuTipo,
                        UsuFoto = p.UsuFoto,
                        UsuTipoFoto = p.UsuTipoFoto,
                        UsuEst = p.UsuEst
                    };

                // Aplica los filtros
                if (usuarioId > 0) 
                    q = q.Where(p => p.UsuarioId == usuarioId);
                if (usuNom != null) 
                    q = q.Where(p => p.UsuNom.Contains(usuNom));
                if (usuTipo != null) 
                    q = q.Where(p => p.UsuTipo == usuTipo);
                if (usuEst != null) 
                    q = q.Where(p => p.UsuEst == usuEst);

                // Obtiene el total de registros antes de aplicar el paginado
                var count = q.Count();

                // Aplica el paginado
                q = Query<Models.Usuarios>.Paginar(q, numPagina, numRegistros);

                // Retorna el DTO (Data Transfer Object)
                return new DataProvider<Models.Usuarios>(count, q.ToList()) ;
            }
        }

        public Models.Usuarios Mostrar(decimal id)
        {
            using (var db = new VolquexDB())
            {
                var q = from p in db.Usuarios
                    where p.UsuarioId == id
                    select p;

                return q.FirstOrDefault();
            }
        }

        public Models.Usuarios Insertar(Models.Usuarios o)
        {
            using(var db = new VolquexDB())
            {
                // Obtengo el siguiente Id a insertar
                o.UsuarioId = Numeracion();
                db.Insert(o);
            }

            return(o);
        }

        public Models.Usuarios Actualizar(Models.Usuarios o)
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
                var q = from p in db.Usuarios
                    orderby p.UsuarioId descending
                    select new Models.Usuarios
                    {
                        UsuarioId = p.UsuarioId
                    };

                // Retorno el último ID + 1
                return q.FirstOrDefault() != null ? q.FirstOrDefault().UsuarioId + 1 : 1;
            }
        }

        public RespuestaSimple Login(string usuario, string password)
        {
            using (var db = new VolquexDB())
            {
                // Ubico el usuario
                var q = from p in db.Usuarios
                    where p.UsuEmail == usuario
                    orderby p.UsuEmail
                    select new Models.Usuarios
                    {
                        UsuarioId = p.UsuarioId,
                        UsuEmail = p.UsuEmail,
                        UsuNom = p.UsuNom,
                        UsuPassword = p.UsuPassword
                    };
                
                // Obtiene el primer registro
                var r = q.FirstOrDefault();

                // Si no encontró, retorno
                if (r == null) return new RespuestaSimple(401, "Usuario y/o contraseña incorrectos.");

                Debug.Consola("UsuPassword", r.UsuPassword);
                Debug.Consola("password", password);
                Debug.Consola("UsuEmail", r.UsuEmail);

                // Desencripto la contraseña usando la clave
                // Si no coinciden, retorno
                // También permito entrar si la contraseña es igual al usuario (inicialmente)
                if (Encriptacion.Desencriptar(r.UsuPassword) != password &&
                    (r.UsuPassword != r.UsuEmail || r.UsuPassword != password))
                    return new RespuestaSimple(401, "Usuario y/o contraseña incorrectos.");

                // Usuario correcto. Creo un token JWT y lo devuelvo
                var token = Encriptacion.Encriptar(Guid.NewGuid().ToString());

                // Elimina las sesiones expiradas
                new Services.Sesiones().EliminarExpiradas();

                // Creo la sesión en la tabla y almaceno el token
                new Services.Sesiones().Insertar( new Models.Sesiones {
                    SesionId = token,
                    SesionExpira = DateTime.Now.AddHours(1),
                    UsuarioId = r.UsuarioId,
                    SesionTipo = "local"
                });

                // Retorno OK y el Token
                return new RespuestaSimple(200, token);
            }
        }

        public async Task<ActionResult<RespuestaSimple>> Registrar(string token, string type)
        {
            var res = new RespuestaSimple();
            string id = "", name = "", email = "", photo = "";

            // Inicializo los objetos para hacer el Request
            var client = new HttpClient();
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;

            // Para FACEBOOK
            if (type == "facebook")
                // Envío el token en el query string
                request.RequestUri = new Uri("https://graph.facebook.com/me?fields=id,name,email&access_token=" + token);
            else
            {
                // Para GOOGLE
                // Envío el token en el Header (OAUTH 2.0)
                request.RequestUri = new Uri("https://www.googleapis.com/oauth2/v3/userinfo");
                request.Headers.Add("Authorization", "Bearer " + token);
            }

            // Envía el Request al proveedor respectivo
            var response = await client.SendAsync(request);

            // Si el token es válido
            if (response.IsSuccessStatusCode)
            {
                // Obtengo los datos del usuario
                dynamic data = JsonConvert.DeserializeObject( 
                    response.Content.ReadAsStringAsync().Result
                );

                id = data.id;
                name = data.name;
                email = data.email;

                // Si es FACEBOOK
                if (type == "facebook")
                    // Armo la URL para obtener la foto
                    photo = "https://graph.facebook.com/" + id + "/picture?type=large";
                else
                    // Si es GOOGLE
                    // La foto viene en el JSON directamente
                    photo = data.picture;
            }
            else
            {
                // Si el token no es válido, retorno error
                res.Codigo = 500; 
                res.Texto = "No se pudo iniciar sesión en " + type;
            }

            // Si se pudo iniciar sesión
            if (res.Codigo == 200)
            {
                // Verifico si el usuario ya existe por el email
                using (var db = new VolquexDB())
                {
                    var q = from p in db.Usuarios
                        where p.UsuEmail == email
                        orderby p.UsuEmail
                        select p;

                    res.Contenido = q.FirstOrDefault();

                    // Si no existe, inserto uno nuevo
                    if (res.Contenido == null)
                        res.Contenido = Insertar( new Models.Usuarios {
                            UsuNom = name,
                            UsuEmail = email,
                            UsuCel = "",
                            UsuTipo = "CLI",
                            UsuEst = "ACT",
                            UsuFoto = photo,
                            UsuTipoFoto = "R"
                        } );
                }

                // Inserto la sesión
                new Services.Sesiones().Insertar(new Models.Sesiones {
                    SesionId = token,
                    SesionExpira = DateTime.Now.AddYears(1000),
                    UsuarioId = res.Contenido.UsuarioId,
                    SesionTipo = type
                });
            }

            client.Dispose();
            return res;
        }

        public Models.Usuarios MisDatos() 
        {
            // Obtengo el token del header
            string token = new Services.Sesiones().ObtenerToken();

            using (var db = new VolquexDB())
            {
                // Leo el usuario de la sesión
                var usuarioId = new Services.Sesiones().ObtenerUsuario();

                // Obtengo los datos
                var q = from p in db.Usuarios
                    where p.UsuarioId == usuarioId
                    select p;
                
                return q.FirstOrDefault();
            }
        }

        public RespuestaSimple ActualizarDatos(string usuNom, string usuEmail, string usuCel)
        {
            // Obtengo el token del header
            string token = new Services.Sesiones().ObtenerToken();

            using (var db = new VolquexDB())
            {
                // Leo el usuario de la sesión
                var usuarioId = new Services.Sesiones().ObtenerUsuario();

                // Si no encontró, retorno error
                if (usuarioId == 0)
                    return new RespuestaSimple(500, "El usuario no existe");
                    
                // Verifico que el Email no esté repetido
                var q02 = from u in db.Usuarios
                    where u.UsuarioId != usuarioId
                    where u.UsuEmail == usuEmail
                    select u;

                // Si retorna algo, mando error
                if (q02.FirstOrDefault() != null)
                    return new RespuestaSimple(500, "El Email ya está asignado a otro usuario");

                // Si no hubo errores, grabo
                db.Usuarios
                    .Where(p => p.UsuarioId == usuarioId)
                    .Set(p => p.UsuNom, usuNom)
                    .Set(p => p.UsuEmail, usuEmail)
                    .Set(p => p.UsuCel, usuCel)
                    .Update();

                return new RespuestaSimple(200, "Se actualizó tu información personal");
            }

        }

        public void RecalcularCalificacionCliente(decimal? id)
        {
            using(var db = new VolquexDB())
            {
                // Cuenta y suma las calificaciones de este conductor
                var q = db.Viajes
                    .OrderBy(p => p.ClienteId)
                    .Where(p => p.ClienteId == id)
                    .Where(p => p.ViaEst == "FIN");
                
                var count = q.Count();
                var sum = q.Sum(p => p.ViaCalificacionConductor);
                var calificacion = sum / count;

                // Actualiza la calificación
                db.Usuarios
                    .Where(p => p.UsuarioId == id)
                    .Set(p => p.UsuCalificacion, calificacion)
                    .Update();
            }
        }

        public void RecalcularCalificacionConductor(decimal? id)
        {
            using(var db = new VolquexDB())
            {
                // Cuenta y suma las calificaciones de este conductor
                var q = db.Viajes
                    .OrderBy(p => p.ConductorId)
                    .Where(p => p.ConductorId == id)
                    .Where(p => p.ViaEst == "FIN");
                
                var count = q.Count();
                var sum = q.Sum(p => p.ViaCalificacionCliente);
                var calificacion = sum / count;

                // Actualiza la calificación
                db.Usuarios
                    .Where(p => p.UsuarioId == id)
                    .Set(p => p.UsuCalificacion, calificacion)
                    .Update();
            }
        }

    }
}