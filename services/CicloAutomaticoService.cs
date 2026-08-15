using fluid_sim_monitor.models;

namespace fluid_sim_monitor.services
{
    public class CicloAutomaticoService
    {
        private readonly FluidSimService _hardware;

        public CicloAutomaticoService(FluidSimService hardware)
        {
            _hardware = hardware;
        }

        public void ExecutarSequenciaInfinito(CancellationToken cancellationToken, int tempoUsinagemMs = 1500)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_hardware.PecaEstaPresente())
                {
                    Thread.Sleep(200);
                    continue;
                }

                Console.WriteLine("peça detectada! iniciando sequência");

                // A+
                _hardware.AvancarA();
                if (!_hardware.AguardarPosicao(Atuador.AtuadorA, EnumsAtuador.Avancado))
                {
                    Console.WriteLine("falha ao avançar o atuador A");
                    break;
                }

                // B+
                _hardware.AvancarB();
                if (!_hardware.AguardarPosicao(Atuador.AtuadorB, EnumsAtuador.Avancado))
                {
                    Console.WriteLine("falha ao avançar o atuador B");
                    GarantirRetornoSeguro();
                    break;
                }

                Thread.Sleep(tempoUsinagemMs);

                // B-
                _hardware.RecuarB();
                _hardware.AguardarPosicao(Atuador.AtuadorB, EnumsAtuador.Recuado);

                // A-
                _hardware.RecuarA();
                _hardware.AguardarPosicao(Atuador.AtuadorA, EnumsAtuador.Recuado);

                Thread.Sleep(500);
            }

            GarantirRetornoSeguro();
        }

        private void GarantirRetornoSeguro()
        {
            Console.WriteLine("garantindo posição de repouso segura");
            _hardware.RecuarB();
            _hardware.AguardarPosicao(Atuador.AtuadorB, EnumsAtuador.Recuado);

            _hardware.RecuarA();
            _hardware.AguardarPosicao(Atuador.AtuadorA, EnumsAtuador.Recuado);

            _hardware.DesligarTudo();
            Console.WriteLine("todos os atuadores recuados em A0 e B0.");
        }
    }
}