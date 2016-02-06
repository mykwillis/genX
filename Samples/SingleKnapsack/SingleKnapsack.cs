using System;
using genX;
using genX.Encoding;

namespace SingleKnapsack
{
    class SingleKnapsackObjective
    {
        int[] costs;
        int[] values;
        int maxCost;
        int numberOfItems;

        public SingleKnapsackObjective(int numberOfItems)
        {
            this.numberOfItems = numberOfItems;
            costs = new int[numberOfItems];
            values = new int[numberOfItems];

            System.Random rand = new System.Random();

            for(int i=0;i<numberOfItems;i++)
            {
                costs[i]  = rand.Next(100);
                values[i] = rand.Next(100);
            }
            maxCost = 250;
        }

        public int GetCost(Chromosome c)
        {
            int cost=0;
            for(int i=0;i<numberOfItems;i++)
            {
                if ( (BinaryGene)c.Genes[i] )
                {
                    cost += costs[i];
                }
            }
            return cost;
        }

        public int GetValue(Chromosome c)
        {
            int val=0;
            for(int i=0;i<numberOfItems;i++)
            {
                if ( (BinaryGene)c.Genes[i] )
                {
                    val += values[i];
                }
            }
            return val;
        }

        public double GetObjective(Chromosome c)
        {
            double value;
            int cost;

            cost = 0;
            value = 0;
            for(int i=0;i<numberOfItems;i++)
            {
                if ( (BinaryGene)c.Genes[i] )
                {
                    cost += costs[i];
                    value += values[i];
                }
            }

            if ( cost > maxCost )
            {
                return maxCost-cost;    // always negative
            }
            else
            {
                return value;
            }
        }

        public void Dump()
        {
            Console.WriteLine("---------------------------------------");
            Console.Write("Costs:   ");
            for(int i=0;i<numberOfItems;i++)
            {
                Console.Write( "{0,2} ", costs[i] );
            }
            Console.WriteLine();

            Console.Write("Values:  ");
            for(int i=0;i<numberOfItems;i++)
            {
                Console.Write( "{0,2} ", values[i] );
            }
            Console.WriteLine();
            Console.WriteLine("MaxCost: 250");
            Console.WriteLine("---------------------------------------");
        }
    }

    class SingleKnapsackExample
    {
        static SingleKnapsackObjective Objective = new SingleKnapsackObjective(10);

        static void OnNewPopulation(object o, NewPopulationEventArgs e)
        {
            Chromosome c = e.OldPopulation.Summary.BestChromosome;

            Console.WriteLine("Best of Generation: {0}", e.OldPopulation.Summary.BestChromosome);
            Console.WriteLine("  cost      = {0}", Objective.GetCost(c));
            Console.WriteLine("  value     = {0}", Objective.GetValue(c));
            Console.WriteLine("  objective = {0}", c.RawObjective );
        }

        static void Main(string[] args)
        {
            GA Ga;

            Ga = new GA();

            Ga.PopulationSize       = 25;
            Ga.EncodingType         = EncodingType.Binary;
            Ga.ChromosomeLength     = 10;
            Ga.NewPopulation        += new NewPopulationEventHandler( OnNewPopulation );
            Ga.Objective            = new ObjectiveDelegate( Objective.GetObjective );
            Ga.FitnessScaling       = FitnessScaling.LinearRanked;
            Ga.MaxGenerations       = 35;
            Ga.RecombinationProbability = 0.6;

            Objective.Dump();

            Ga.Run();            

            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
        }    
    }

}
