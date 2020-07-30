using System;

namespace Volquex.Utils
{
    public static class Consola
    {
        public static void Debug(string Variable, Object o)
        {
            Console.WriteLine(">> DEBUG <<-----------------------------------------------------");
            Console.WriteLine(">> " + Variable + " = " + o.ToString());
            Console.WriteLine(">> -------------------------------------------------------------");
        }
        public static void Error(string mensaje)
        {
            Console.WriteLine(">> ERROR <<+++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine(">> " + mensaje);
            Console.WriteLine(">> +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }
    }
}