using System.Collections.Generic;
using System;
using System.Linq;

namespace Volquex.Models
{
    public static class Query<T>
    {
        public static IQueryable<T> Paginar(IQueryable<T> q, int NumPagina, int NumRegistros)
        {
            var r = q;

            if (NumPagina > 0 && NumRegistros > 0) 
                r = q.Skip( (NumPagina - 1) * NumRegistros ).Take(NumRegistros);

            return r;
        }
    }

    [Serializable]
    public class DataProvider<T>
    {
        public int TotalRegistros;
        public List<T> Listado;

        public DataProvider(int TotalRegistros, List<T> Listado)
        {
            this.TotalRegistros = TotalRegistros;
            this.Listado = Listado;
        }

        public DataProvider(List<T> Listado)
        {
            this.TotalRegistros = 0;
            this.Listado = Listado;
        }
    }
}