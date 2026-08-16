using System;
using System.Threading;
using System.Threading.Tasks;
using fluid_sim_monitor.models;
using fluid_sim_monitor.services;
using fluid_sim_monitor.utils;

namespace fluid_sim_monitor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var service = new FluidSimService();

            try
            {
                string comando = "";
                ConsoleEstilo.EscreverColorido("=== PAINEL DE CONTROLE INDUSTRIAL ===", ConsoleColor.Cyan);
                Console.WriteLine("1 - avançar A\n2 - recuar A");
                Console.WriteLine("3 - avançar B\n4 - recuar B");
                Console.WriteLine("5 - desligar tudo\n6 - ciclo automático");
                Console.WriteLine("7 - sair");
                ConsoleEstilo.EscreverColorido("---------------------------------------------------", ConsoleColor.Cyan);

                ExibirStatusPainel(service);

                while (comando != "7")
                {
                    Console.Write(">> ");
                    comando = Console.ReadLine()?.Trim() ?? "";

                    switch (comando)
                    {
                        case "1":
                            service.AvancarA();
                            service.AguardarPosicao(Atuador.AtuadorA, EnumsAtuador.Avancado);
                            ExibirStatusPainel(service);
                            break;

                        case "2":
                            service.RecuarA();
                            service.AguardarPosicao(Atuador.AtuadorA, EnumsAtuador.Recuado);
                            ExibirStatusPainel(service);
                            break;

                        case "3":
                            service.AvancarB();
                            service.AguardarPosicao(Atuador.AtuadorB, EnumsAtuador.Avancado);
                            ExibirStatusPainel(service);
                            break;

                        case "4":
                            service.RecuarB();
                            service.AguardarPosicao(Atuador.AtuadorB, EnumsAtuador.Recuado);
                            ExibirStatusPainel(service);
                            break;

                        case "5":
                            service.DesligarTudo();
                            ConsoleEstilo.EscreverColorido("atuadores desligados", ConsoleColor.DarkRed);
                            ExibirStatusPainel(service);
                            break;

                        case "6":
                            bool pecaPresente = service.PecaEstaPresente();
                            
                            if(pecaPresente == true)
                            {
                                ConsoleEstilo.EscreverColorido("ciclo automatico iniciado (ENTER para interromper)", ConsoleColor.Cyan);
                            }
                            else
                            {
                                ConsoleEstilo.EscreverColorido("ciclo automatico iniciado, aguardando peça S1 (ENTER para interromper)", ConsoleColor.Cyan);
                            }

                            var ciclo = new CicloAutomaticoService(service);

                            using (var cts = new CancellationTokenSource())
                            {
                                var tarefaCiclo = Task.Run(() => ciclo.ExecutarSequenciaInfinito(cts.Token, 1500));

                                while (!tarefaCiclo.IsCompleted)
                                {
                                    if (Console.KeyAvailable)
                                    {
                                        var tecla = Console.ReadKey(intercept: true);

                                        if (tecla.Key == ConsoleKey.Enter)
                                        {
                                            cts.Cancel();
                                            break;
                                        }
                                        else
                                        {
                                            ConsoleEstilo.EscreverColorido("impossível alterar estado ou recuar atuadores em funcionamento!!!", ConsoleColor.Yellow);
                                            ConsoleEstilo.EscreverColorido("dê ENTER para desativar o ciclo automático primeiro!!!", ConsoleColor.DarkYellow);
                                        }
                                    }
                                    Thread.Sleep(100);
                                }

                                tarefaCiclo.Wait();
                            }

                            ConsoleEstilo.EscreverColorido("ciclo automatico finalizado", ConsoleColor.DarkRed);
                            ExibirStatusPainel(service);
                            break;

                        case "7":
                            service.DesligarTudo();
                            ConsoleEstilo.EscreverColorido("programa encerrado", ConsoleColor.DarkCyan);
                            break;

                        default:
                            ConsoleEstilo.EscreverColorido("comando inválido!!!", ConsoleColor.Red);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"erro: {ex.Message}");
            }
        }

        private static void ExibirStatusPainel(FluidSimService service)
        {
            var posA = service.ObterPosicao(Atuador.AtuadorA);
            var posB = service.ObterPosicao(Atuador.AtuadorB);
            bool peca = service.PecaEstaPresente();

            ExibirCorAtuador("Atuador A", posA);
            ExibirCorAtuador("Atuador B", posB);

            if (peca)
                ConsoleEstilo.EscreverColorido("Peça: PRESENTES (S1=1)", ConsoleColor.Green);
            else
                Console.WriteLine("Peça: AUSENTE (S1=0)");
        }

        private static void ExibirCorAtuador(string nome, EnumsAtuador posicao)
        {
            Console.Write($"{nome}: ");
            if (posicao == EnumsAtuador.Avancado)
                ConsoleEstilo.EscreverColorido($"{posicao}", ConsoleColor.Green);
            else if (posicao == EnumsAtuador.Recuado)
                ConsoleEstilo.EscreverColorido($"{posicao}", ConsoleColor.Yellow);
            else
                ConsoleEstilo.EscreverColorido($"{posicao}", ConsoleColor.DarkGray);
        }
    }
}