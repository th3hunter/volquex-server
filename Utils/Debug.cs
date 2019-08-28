using System;

namespace Volquex.Utils
{
    public static class Debug
    {
        public static void Consola(string Variable, Object o)
        {
            Console.WriteLine(">> DEBUG << : " + Variable + " = " + o.ToString());
        }
    }
}