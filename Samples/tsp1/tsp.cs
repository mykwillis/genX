using System;
using Galib;

namespace GaTest
{
    class TravellingSalesman1dObjective : Galib.Evaluation.IObjective
    {
        int[] cities;

        public TravellingSalesman1dObjective(int[] cities)
        {
            this.cities = cities;
        }

        public double GetObjective(Chromosome c)
        {
            IntegerChromosome ic = (IntegerChromosome) c;

            double distance = 0;
            int currentPosition = 0;
            int initialPosition = 0;
            for(int i=0;i<ic.Genes.Length;i++)
            {
                if ( i==0 )
                {
                    initialPosition = cities[ic.Genes[i]];
                    currentPosition = cities[ic.Genes[i]];
                }
                else
                {
                    distance += Math.Abs( cities[ic.Genes[i]] - currentPosition );
                    currentPosition = cities[ic.Genes[i]];
                }
            }
            distance += Math.Abs( cities[ic.Genes[ic.Genes.Length-1]] - initialPosition );

            return distance;
        }
    }


    class GaTest
    {
        static void Main(string[] args)
        {
            GA Ga;
            TravellingSalesman1dObjective Objective;
            Galib.Mutation.IntegerSwapMutator Mutator;
            IntegerChromosomeSpecifier ChromosomeSpecifier;
            
            //
            // Create the objective.
            //
            int[] cities = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
            Objective = new TravellingSalesman1dObjective(cities);
            
            //
            // Create the mutator
            //
            Mutator = new Galib.Mutation.IntegerSwapMutator();

            //
            // Create the chromosome specifier.
            //
            ChromosomeSpecifier = new IntegerChromosomeSpecifier();
            ChromosomeSpecifier.Length = numberOfCities;
            ChromosomeSpecifier.MinValue = 0;
            ChromosomeSpecifier.MaxValue = numberOfCities;
            ChromosomeSpecifier.PositionDependent = true;

            //
            // Create the recombinator
            //
            Galib.Recombination.PartiallyMatchedRecombinator Recombinator;
            Recombinator = new Galib.Recombination.PartiallyMatchedRecombinator();

            Ga = new GA();

            Ga.PopulationSize       = 100;
            Ga.Mutator              = Mutator;
            Ga.ChromosomeSpecifier  = ChromosomeSpecifier;
            Ga.Objective            = Objective;
            Ga.InvertedObjective    = true;
            Ga.Recombinator         = Recombinator;
            Ga.MaxGenerations       = 500;

            Ga.Run();

            Console.WriteLine("Best Individual:");
            Console.WriteLine( Ga.Population.BestChromosome );
            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
        }
    }
}
