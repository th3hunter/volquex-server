using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using Volquex.Models;

namespace Volquex.Services
{
    public class Usuarios_Disp
    {
        public Usuarios_Disp(VolquexDB db) 
        {
            this.db = db;
        }
        
        private VolquexDB db;

        public void Insertar(decimal usuarioId, string dispositivoId)
        {
            // Elimino los dispositovos expirados
            db.Usuarios_Disp
                .Where(p => p.UsuDispExpira < DateTime.Now)
                .Delete();

            // Actualizo la fecha de expiración del id del dispositivo
            var numRegistros = db.Usuarios_Disp
                .Where(p => p.UsuarioId == usuarioId)
                .Where(p => p.DispositivoId == dispositivoId)
                .Set(p => p.UsuDispExpira, DateTime.Now.AddDays(5))
                .Update();
            
            // Si no se actualizó ningún registro, creo uno nuevo
            if (numRegistros == 0)
            {
                db.Insert(new Models.Usuarios_Disp
                {
                    UsuarioId = usuarioId,
                    DispositivoId = dispositivoId,
                    UsuDispExpira = DateTime.Now.AddDays(5)
                });
            }
        }

        public List<Models.Usuarios_Disp> Listar(decimal usuarioId)
        {
            var q = from p in db.Usuarios_Disp
                where p.UsuarioId == usuarioId
                select p;

            return q.ToList();
        }

    }
}