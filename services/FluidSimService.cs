using System;
using System.Threading;
using fluid_sim_monitor.models;
using NDde.Client;

namespace fluid_sim_monitor.services
{
    public class FluidSimService
    {
        private readonly DdeClient _client;
        private int _estadoSaidas = 0;

        public FluidSimService()
        {
            _client = new DdeClient("FLSIMP", "IOPanel");
            _client.Connect();

            DesligarTudo();
        }

        public bool SensorA1() => LerBitGet(0);         
        public bool SensorA2() => LerBitGet(1);         
        public bool PecaEstaPresente() => LerBitGet(2); 
        public bool SensorB1() => LerBitGet(3);         
        public bool SensorB2() => LerBitGet(4);        

        public bool SensorA0() => SensorA1();
        public bool SensorB0() => SensorB1();

        public void AvancarA() => EscreverBitSet(0, true);
        public void RecuarA() => EscreverBitSet(0, false);
        public void AvancarB() => EscreverBitSet(1, true);
        public void RecuarB() => EscreverBitSet(1, false);

        public void DesligarTudo()
        {
            _estadoSaidas = 0;
            try
            {
                _client.Poke("set_0", "0", 60000);
            }
            catch { }
        }

        public EnumsAtuador ObterPosicao(Atuador atuador)
        {
            if (atuador == Atuador.AtuadorA)
            {
                if (SensorA1() && !SensorA2()) return EnumsAtuador.Recuado;
                if (!SensorA1() && SensorA2()) return EnumsAtuador.Avancado;
            }
            else if (atuador == Atuador.AtuadorB)
            {
                if (SensorB1() && !SensorB2()) return EnumsAtuador.Recuado;
                if (!SensorB1() && SensorB2()) return EnumsAtuador.Avancado;
            }

            return EnumsAtuador.Indefinido;
        }

        public bool AguardarPosicao(Atuador atuador, EnumsAtuador posicaoDesejada, int timeoutMs = 5000)
        {
            var inicio = DateTime.Now;
            while ((DateTime.Now - inicio).TotalMilliseconds < timeoutMs)
            {
                if (ObterPosicao(atuador) == posicaoDesejada)
                    return true;

                Thread.Sleep(50);
            }
            return false;
        }

        private bool LerBitGet(int bitIndex)
        {
            try
            {
                string raw = _client.Request("get_0", 60000);
                if (int.TryParse(raw.Trim(), out int valor))
                {
                    return (valor & (1 << bitIndex)) != 0;
                }
            }
            catch { }
            return false;
        }

        private void EscreverBitSet(int bitIndex, bool estado)
        {
            try
            {
                if (estado)
                    _estadoSaidas |= (1 << bitIndex);
                else
                    _estadoSaidas &= ~(1 << bitIndex);

                _client.Poke("set_0", _estadoSaidas.ToString(), 60000);
            }
            catch { }
        }
    }
}