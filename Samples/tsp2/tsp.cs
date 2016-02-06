using System;
using Galib;

namespace GaTest
{
    class TravellingSalesmanObjective : Galib.Evaluation.IObjective
    {
        struct City
        {
            public int id;
            public int x;
            public int y;
        };

        int numberOfCities;
        City[] cities;
        int maxX;
        int maxY;

        public TravellingSalesmanObjective(int numberOfCities)
        {
            this.numberOfCities = numberOfCities;
            cities = new City[numberOfCities];

            maxX = 100;
            maxY = 100;

            for(int i=0;i<numberOfCities;i++)
            {
                cities[i].id = i;
                cities[i].x = Galib.Utils.Rand.Next(maxX);
                cities[i].y = Galib.Utils.Rand.Next(maxY);
            }
        }

        public double GetObjective(Chromosome c)
        {
            IntegerChromosome ic = (IntegerChromosome) c;

            double distance = 0;    // actually distance^2
            City currentCity;
            City nextCity;                

            currentCity = cities[ic.Genes[0]];
            for(int i=0;i<numberOfCities+1;i++)
            {
                //
                // The next city to be visited is given by the current gene.
                // If we've already gone through all of the genes, the next city
                // is the one from which we started.
                //
                if ( i < numberOfCities )
                {
                    nextCity = cities[ic.Genes[i]];
                }
                else
                {
                    nextCity = cities[ic.Genes[0]];
                }

                //
                // Calculate the distance^2 between the currentCity and the next.
                // (the sqrt() of the distance is unnecessary)
                //
                int dx = nextCity.x - currentCity.x;    // change in x
                int dy = nextCity.y - currentCity.y;    // change in y
                int z = (dx*dx) + (dy*dy);              // dist^2 = dx^2 + dy^2

                distance += z;

                //
                // Advance to the next city.
                //
                currentCity = nextCity;
            }

            return distance;
        }
    }

    class GaTest
    {
        static void Main(string[] args)
        {
            GA Ga;
            TravellingSalesmanObjective Objective;
            Galib.Mutation.IntegerSwapMutator Mutator;
            IntegerChromosomeSpecifier ChromosomeSpecifier;

            
            //
            // Create the objective objective.
            //
            int numberOfCities = 10;
            Objective = new TravellingSalesmanObjective(numberOfCities);
            
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
