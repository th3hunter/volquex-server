using System;

namespace Volquex.Models
{
    [Serializable]
    public class RespuestaSimple
    {
        public int Codigo;
        public string Texto;
        public dynamic Contenido;

        public RespuestaSimple() 
        {
            this.Codigo = 200;
            this.Texto = "";
        }

        public RespuestaSimple(int codigo)
        {
            this.Codigo = codigo;
            this.Texto = "";
        }

        public RespuestaSimple(int codigo, string texto)
        {
            this.Codigo = codigo;
            this.Texto = texto;
        }

        public RespuestaSimple(int codigo, string texto, Object contenido)
        {
            this.Codigo = codigo;
            this.Texto = texto;
            this.Contenido = contenido;
        }

        public RespuestaSimple(Object contenido)
        {
            this.Codigo = 200;
            this.Texto = "";
            this.Contenido = contenido;
        }
    }
}