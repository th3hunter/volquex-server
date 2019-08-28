using System;
using System.Linq;
using LinqToDB;
using Volquex.Models;

namespace Volquex.Services
{
    public class Sesiones
    {
        public Sesiones () {}

        public Models.Sesiones Insertar(Models.Sesiones o)
        {
            using(var db = new VolquexDB())
            {
                db.Insert(o);
            }

            return o;
        }

		// Elimina las sesiones expiradas
		public void EliminarExpiradas()
		{
			using (var db = new VolquexDB())
			{
				db.Sesiones
					.Where(p => p.SesionExpira < Sql.CurrentTimestamp)
					.Delete();
			}
		}

		// Verifica si el token es válido
		public string Autorizar(string Token)
		{
			using (var db = new VolquexDB())
			{
				var q = from p in db.Sesiones
					where p.SesionId == Token
					select p;

				var r = q.FirstOrDefault();

				// Si no existe la sesión
				if (r == null)
					return "No ha iniciado sesión.";
				
				// Si el token existe pero está expirado
				if(r.SesionExpira < DateTime.Now)
					return "Su sesión expiró.";
				
				// Si todo está OK, aumento la expiración de la sesión a 30 min.
                // pero sólo si es del tipo LOCAL
                if (r.SesionTipo == "local")
                    db.Sesiones
                        .Where(p => p.SesionId == Token)
                        .Set(p => p.SesionExpira, p => System.DateTime.Now.AddMinutes(30))
                        .Update();

                // Retorno vacío para indicar que está OK
				return "";
			}
		}
		
        // Obtiene el usuario de la tabla Sesiones
		public decimal ObtenerUsuario()
		{	
			// Lee la sesión y obtiene el usuario asociado
			using (var db = new VolquexDB())
			{
				var q = from p in db.Sesiones
					where p.SesionId == Startup.Token
					select p;

				return(q.FirstOrDefault().UsuarioId);
			}
		}

        // Obtiene el Token del header en el request
        public string ObtenerToken() {
            Startup.Request.Headers.TryGetValue("Authorization", out var token);
            return token;
        }
	}
}