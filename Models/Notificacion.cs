	using System;

namespace Volquex.Models
{
    [Serializable]
    public class Notificacion
    {
        public string[] registration_ids ;
        public Data data;
        public Notification notification;

		[Serializable]
        public class Data
        {
            public decimal viajeId;
            public Accion accion; 
        }

		[Serializable]
        public class Notification
        {
            public string title;
            public string body;
        }

        public Notificacion(string[] tokens, decimal viajeId, string titulo, string mensaje, Accion accion)
        {
            this.registration_ids = tokens;
            this.data = new Data
            {
                viajeId = viajeId,
                accion = accion
            };
            this.notification = new Notification
            {
                title = titulo,
                body = mensaje
            };
        }
    }

    public enum Accion
    {
        NuevoViaje,
        OfertaAceptada,
        ViajeFinalizado,
        ViajeCancelado
    }
}
