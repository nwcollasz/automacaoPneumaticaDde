using System;

namespace fluid_sim_monitor.utils
{
    internal class ConsoleEstilo
    {
        public static void EscreverColorido(string texto, ConsoleColor cor)
        {
            Console.ForegroundColor = cor;
            Console.WriteLine(texto);
            Console.ResetColor();
        }
    }
}
