using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


/// <summary>
/// O jogo conta com 2 tipos de dados (dados de 6 faces, dado de 8 faces, dado de 12 faces). 
/// No inicio do jogo o jogador escolhe um dos tipos de dados e segue utilizando ele até o final do jogo.
/// Durante cada rodada 2 dados do tipo escolhido anteriormente são jogados e dependendo da soma das faces resultados diferentes são obtidos: 
///
///     * Se a soma das faces for igual a 10 ou 12 o jogador ganha
///     * Se a soma das faces for igual a 3, 4, 5, 16, 17 ou 18  o jogador perde 
///     * Se o total for igual a qualquer outro resultado o jogador relança os dados
///     
/// </summary>
namespace Simulacao2
{
    static class Constants
    {
        public const int qtdJogos = 5000;
        public const int qtdRodadas = 200;
    }

    public enum ResultadoRodada { empty, win, lose, reroll};
    public enum TipoDado { d6Faces, d8Faces, d12Faces, lenght };

    class Program
    {
        static void Main(string[] args)
        {
            List<Jogo> jogos = new List<Jogo>();

            for (int i = 1; i <= Constants.qtdJogos; i++)
            {
                Console.WriteLine("Jogo #" + i + "\n");
                Jogo jogo = new Jogo();
                jogos.Add(jogo);
                jogo.init();

                String nomePlanilha = "jogo" + i + ".csv";
                panilhaRodadas(jogo, nomePlanilha);
            }
            analiseDados(jogos);
            panilhaJogos(jogos);
            Console.ReadKey();
        }

        static void panilhaRodadas(Jogo jogo, String nomePlanilha)
        {
            using (StreamWriter writer = new StreamWriter(nomePlanilha))
            {
                writer.WriteLine("{0}, {1}, {2}, {3}", "Dado 1", "Dado 2", "Soma dos Dados", "Resultado");
                foreach (var rodada in jogo.Rodadas)
                {
                    writer.WriteLine("{0}, {1}, {2}, {3}", rodada.dado1, rodada.dado2, rodada.somaDados, rodada.result);
                }
            }
            //Console.WriteLine("arquivo gerado!");
        }

        static void panilhaJogos(List<Jogo> jogos) { 
        
            using (StreamWriter writer = new StreamWriter("jogos.csv"))
            {
                writer.WriteLine("{0}, {1}, {2}", "Dado Escolhido", "Numero Rodadas", "Resultado");
                foreach (var j in jogos)
                {
                    writer.WriteLine("{0}, {1}, {2}", j.tipoDado, j.rodadaFinal, j.resultadoRodadaFinal);
                }
            }
        }

        static void analiseDados(List<Jogo> jogos) {
            var ordenados = jogos.OrderBy(x => x.rodadaFinal).ToList();
            ordenados.ForEach(x => Console.WriteLine("resultado apos: " + x.rodadaFinal + " jogadas"));

            //rodadas necessarias para chegar a um resultado
            var grupos = ordenados.GroupBy(j => j.rodadaFinal).Select(grp => new { numeroRodada = grp.Key, total = grp.Count() });
            foreach (var g in grupos)
            {
                Console.WriteLine("Indice da Rodada Final: " + g.numeroRodada, " Total: " + g.total);
            }

            //contado o numero de vitorias e derrotas
            Console.WriteLine("# Resultado dos jogos");
            var resultados = jogos.GroupBy(x => x.resultadoRodadaFinal).Select(grp => new { resultadoJogo = grp.Key, total = grp.Count() });
            foreach (var r in resultados)
            {
                Console.WriteLine("Resultado: " + r.resultadoJogo + " Total: " + r.total);
            }

        }

        class Jogo
        {
            private static Random rnd = new Random();
            public struct Rodada
            {
                public int dado1, dado2;
                public int numeroRodada;
                public int somaDados;
                public ResultadoRodada result;

                public Rodada(int numeroRodada, int d1, int d2)
                {
                    this.dado1 = d1;
                    this.dado2 = d2;

                    this.somaDados = d1 + d2;
                    this.numeroRodada = numeroRodada;
                    this.result = ResultadoRodada.empty;
                }
            }
            //numero da rodada
            public int rodadaFinal;
            public ResultadoRodada resultadoRodadaFinal;
            //rodadas
            private List<Rodada> rodadas = new List<Rodada>();

            internal List<Rodada> Rodadas { get => rodadas; set => rodadas = value; }
            public TipoDado tipoDado;

            public void init()
            {
                //sorteia o do tipo do dado que o jogador ira jogar
                tipoDado = (TipoDado)rnd.Next(0, (int)TipoDado.lenght);

                int numeroFaces = 0;
                //identifica quantas faces o dado escolhido tem
                switch (tipoDado)
                {
                    case TipoDado.d6Faces:
                        numeroFaces = 6;
                        break;
                    case TipoDado.d8Faces:
                        numeroFaces = 8;
                        break;
                    case TipoDado.d12Faces:
                        numeroFaces = 12;
                        break;
                }

                Console.WriteLine("tipo do dado escolhido: " + tipoDado);
                for (int i = 1; i <= Constants.qtdRodadas; i++)
                {
                    Rodada rodada = jogarDados(i, numeroFaces);
                    Console.WriteLine(rodada.dado1 + " " + rodada.dado2);
                    rodada.result = calcularResultado(rodada);
                    Rodadas.Add(rodada);
                    Console.WriteLine(rodada.result);
                    if (rodada.result == ResultadoRodada.win || rodada.result == ResultadoRodada.lose)
                    {
                        rodadaFinal = i;
                        resultadoRodadaFinal = rodada.result;
                        break;
                    }
                }

            }
            public Rodada jogarDados(int numeroRodada, int numeroFaces)
            {
                int max = numeroFaces + 1;
                Rodada r = new Rodada(numeroRodada, rnd.Next(1, max), rnd.Next(1, max));
                return r;
            }

            public ResultadoRodada calcularResultado(Rodada r)
            {
                int somaDados = r.somaDados;

                if (somaDados == 10 || somaDados == 12)
                {
                    return ResultadoRodada.win;
                }
                else if ((somaDados >= 3 && somaDados <= 5) || (somaDados >= 16 && somaDados <= 18))
                {
                    return ResultadoRodada.lose;
                }
                else
                {
                    return ResultadoRodada.reroll;
                }
            }
        }
       
    }


}
