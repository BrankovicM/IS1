using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp1
{
    public enum Znak
    {
        Tref,
        Pik,
        Srce,
        Kocka,
        Trougao,
        Krug,
        Karo,
        Smajli
    }
    class Program
    {
        static int KombinacijaLen = 5;
        static List<Znak> Kombinacija = new List<Znak>() { (Znak)1, (Znak)2, (Znak)3, (Znak)1, (Znak)6 };


        static void Main(string[] args)
        {

            //int tableDimension = 8;

            var chromosome = new MyChromosome(KombinacijaLen);
            var fitness = new MyFitness();

            var mutation = new ReverseSequenceMutation();
            //var mutation = new DisplacementMutation();
            //ICrossover crossover = new TwoPointCrossover();
            ICrossover crossover = new VotingRecombinationCrossover(4, 2);
            var selection = new EliteSelection();
            var population = new Population(50,50, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new FitnessThresholdTermination(KombinacijaLen * 5) //25 max
            };


            //sluzi za nadgledanje hromozoma nakon generisanja jedne generacije
            ga.GenerationRan += delegate
            {
                var bestChromosome = ga.Population.BestChromosome;

                Console.WriteLine("Generacija: " + ga.Population.CurrentGeneration.Number);
                Console.WriteLine("Fitness najboljeg hromozoma: {0}", bestChromosome.Fitness);

                Console.WriteLine("Geni: {0}", string.Join("", bestChromosome.GetGenes().ToList().Select(x=>x.Value)));

                Console.WriteLine();
                //Thread.Sleep(1000);

            };

            Console.WriteLine("GA krece");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ga.Start();

            Console.WriteLine("Najbolje resenje ima {0} vrednost za fitness.", ga.BestChromosome.Fitness);
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Algoritam je trajao {0} sekundi. ", elapsedTime);

            Console.Read();
        }




        public class MyFitness : IFitness
        {
            //za svaku kraljicu proveravamo da li se sudara sa nekom od onih koje se nalaze u narednim redovima
            public double Evaluate(IChromosome chromosome)
            {

                int result = 0;
                var genes = chromosome.GetGenes();

                //var geneList = genes.Select(x => x.Value);

                List<Znak> geneList = genes.Select(x => (Znak)x.Value).ToList();


                for (int i = 0; i < genes.Length; i++)
                {
                    if ((Znak)genes[i].Value == Kombinacija[i])
                    {
                        result = result + 5;
                    }
                    else if (geneList.Where(x => x == (Znak)genes[i].Value).Count() == Kombinacija.Where(x => x == (Znak)genes[i].Value).Count())
                    {
                        result = result + 2;
                    }
                    else if (Kombinacija.Contains((Znak)genes[i].Value))
                    {
                        result = result + 1;
                    }
                    else if (geneList.Where(x => x == (Znak)genes[0].Value).Count() != Kombinacija.Where(x => x == (Znak)genes[0].Value).Count())
                    {
                        result = result - 1;
                    }
                    else
                    {
                        result = result - 2;
                    }
                }

                return result;
            }
        }

        public class MyChromosome : ChromosomeBase
        {
            public override IChromosome CreateNew()
            {
                return new MyChromosome(Length);
            }

            public MyChromosome(int length) : base(length)
            {
                CreateGenes();
            }
            public override Gene GenerateGene(int geneIndex)
            {
                var rnd = RandomizationProvider.Current;

                return new Gene(rnd.GetInt(0,9));
            }

        }
           


    }


}
