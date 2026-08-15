# Controle Eletropneumático

Projeto de automação para controle de ciclo eletropneumático integrado ao Festo FluidSIM via comunicação DDE.

- O ar comprimido do compressor entra pelas válvulas. Quando o código em C# manda o sinal elétrico via DDE, a válvula abre e o ar empurra a haste do pistão para fora. Ao chegar no fim do curso, o pistão bate num sensor físico que fecha um circuito de 24V, enviando a confirmação de volta pro programa em C# para que ele autorize o próximo movimento. Com essa leitura, o programa aciona a saída seguinte para inverter o ar na válvula, fazendo o pistão recuar e repetindo esse ciclo de encher e esvaziar as câmaras a cada peça que passa.

## Componentes
* Compressor
* Válvula solenóide de 5/2 vias
* Atuador pneumático
* Unidade de conservação de ar comprimido
* Sensor de posição do atuador
* Conexões elétricas (24V, 0V)
* Contatos NA
* Botão com trava
* Portas de entrada e saída para comunicação DDE

##
* Server: FLSIMP
* Topic: IOPanel
* Leitura: get_0 
* Escrita: set_0 
