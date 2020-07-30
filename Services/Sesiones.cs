using System;
using System.Linq;
using LinqToDB;
using Volquex.Models;
using Volquex.Utils;

namespace Volquex.Services
{
    public class Sesiones
    {
        public Sesiones(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

        // Obtiene el registro de la sesión y del usuario asociado
        public Models.Sesiones Mostrar()
        {
			var q = from p in db.Sesiones
				where p.SesionId == Startup.TokenSesion
				select new Models.Sesiones
				{
					SesionId = p.SesionId,
					SesionExpira = p.SesionExpira,
					UsuarioId = p.UsuarioId,
					SesionTipo = p.SesionTipo,
					fkusuarios1 = p.fkusuarios1
				};

			return(q.FirstOrDefault());
        }

        public Models.Sesiones Insertar(Models.Sesiones o)
        {
			db.InsertOrReplace(o);
            return o;
        }

		// Elimina las sesiones expiradas
		public void EliminarExpiradas()
		{
			db.Sesiones
				.Where(p => p.SesionExpira < Sql.CurrentTimestamp)
				.Delete();
		}

		// Verifica si el token es válido
		public RespuestaSimple Autorizar(string Token)
		{
			var q = from p in db.Sesiones
				where p.SesionId == Token
				select new Models.Sesiones
                {
                    SesionTipo = p.SesionTipo,
                    SesionExpira = p.SesionExpira,
                    fkusuarios1 = p.fkusuarios1
                };

			var r = q.FirstOrDefault();

			// Si no existe la sesión
			if (r == null)
				return new RespuestaSimple(500, "No ha iniciado sesión.");
			
			// Si el token existe pero está expirado
			if(r.SesionExpira < DateTime.Now)
				return new RespuestaSimple(500, "Su sesión expiró.");
			
			// Si todo está OK, aumento la expiración de la sesión a 30 min.
			// pero sólo si es del tipo LOCAL
			if (r.SesionTipo == "local")
				db.Sesiones
					.Where(p => p.SesionId == Token)
					.Set(p => p.SesionExpira, p => System.DateTime.Now.AddMinutes(30))
					.Update();

			// Si la sesión no es local, aumento a 30 días
			if (r.SesionTipo != "local")
				db.Sesiones
					.Where(p => p.SesionId == Token)
					.Set(p => p.SesionExpira, p => System.DateTime.Now.AddDays(30))
					.Update();

			// Retorno vacío para indicar que está OK
			return new RespuestaSimple(new 
            {
                Usuario = r.fkusuarios1
            });
		}
	}
}