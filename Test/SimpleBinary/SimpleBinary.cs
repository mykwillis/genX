using System;
using System.ComponentModel;
using genX;
using genX.Encoding;
using genX.Termination;

namespace genX.Test
{
    class SimpleBinary
    {
        static double BinaryMaximizationObjective(Chromosome chromosome)
        {
            int objective = 0;
            foreach(BinaryGene g in chromosome.Genes)
            {
                if ( g.Value ) objective++;
            }
            return objective;
        }

        static double BinaryAlternateObjective(Chromosome chromosome)
        {
            int objective = 0;
            bool matchValue = false;
            foreach(BinaryGene g in chromosome.Genes)
            {
                if ( g.Value == matchValue ) objective++;
                matchValue = !matchValue;
            }
            return objective;
        }

        static void OnMutated(object o, MutatedEventArgs e)
        {
            Console.WriteLine("Chromosome was mutated:");
            Console.WriteLine( e.Chromosome.ToString() );
            for(int i=0;i<e.MutationPoint;i++)
            {
                Console.Write("  ");
            }
            Console.WriteLine("^");
        }

        static void OnNewPopulation(object o, NewPopulationEventArgs e)
        {
            Console.WriteLine();
            for(int i=0;i<e.NewPopulation.PopulationSize;i++)
            {
                Console.WriteLine(
                    "{0,2}) {1}   {2,2}) {3}",
                    e.OldPopulation.Chromosomes[i].ID,
                    e.OldPopulation.Chromosomes[i],
                    e.NewPopulation.Chromosomes[i].ID,
                    e.NewPopulation.Chromosomes[i]
                    );
            }            
        }

        static void OnNewPopulation_ShowParents(object o, NewPopulationEventArgs e)
        {
            //
            //     00101110
            // +   10100010
            //     ----^---
            // =   00100010
            //
            foreach(Chromosome c in e.NewPopulation.Chromosomes)
            {
                Console.WriteLine( "   {0}", c.Parents[0]);
                Console.WriteLine( "+  {0}", c.Parents[1]);
                Console.Write(     "   ");
                for(int i=0;i<c.CrossoverPoint;i++)
                {
                    Console.Write("--");
                }
                Console.Write("^-");
                for(int i=c.CrossoverPoint+1;i<c.Genes.Length;i++)
                {
                    Console.Write("--");
                }
                Console.WriteLine();
                Console.WriteLine( "=  {0}", c);
                Console.WriteLine();
            }
        }

        static void OnNewPopulation_ShowSummary(object o, NewPopulationEventArgs e)
        {
            Console.WriteLine( e.OldPopulation.Summary.ToString() );
        }

        static double optimalObjective;

        static void OnTerminate_CheckForOptimal(object o, CancelEventArgs e)
        {
            GA ga = (GA) o;
            if ( ga.BestObjective == optimalObjective )
            {
                Console.WriteLine("--> Optimal objective has been achieved!");
                e.Cancel = true;
            }
        }

        static void Main()
        {
            //
            // Create the genetic algorithm, and set the required properties.
            //
            GA ga = new GA();            
         
            ga.EncodingType     = EncodingType.Binary;
            ga.ChromosomeLength = 25;

            //ga.Objective            = new ObjectiveDelegate( BinaryMaximizationObjective );
            ga.Objective            = new ObjectiveDelegate( BinaryAlternateObjective );
            ga.PopulationSize       = 25;
            //ga.Mutated += new MutatedEventHandler( OnMutated );
            //ga.NewPopulation += new NewPopulationEventHandler( OnNewPopulation );
            //ga.NewPopulation += new NewPopulationEventHandler( OnNewPopulation_ShowParents );
            ga.NewPopulation += new NewPopulationEventHandler( OnNewPopulation_ShowSummary );
            //ga.NewPopulation += new NewPopulationEventHandler( new BinaryPersist().NewPopulationHandler );
            //ga.NewPopulation += new NewPopulationEventHandler( new BinaryPersist().NewPopulationHandlerSoap );
            optimalObjective = 25;
            //ga.Terminate += new TerminateEventHandler( OnTerminate_CheckForOptimal );
            ga.Terminate += new TerminateEventHandler( new ObjectiveThresholdTerminator(optimalObjective).Terminate );
            ga.FitnessScaling = FitnessScaling.LinearRanked;
            ga.GeneMutationProbability = 0.05;
            ga.Run(50);

            //
            // Output the best individual that was found during the run.
            //
            Console.WriteLine("Best Individual:");
            PopulationSummary ps = new PopulationSummary( ga, ga.Population );
            Console.WriteLine(ps.BestChromosome);
            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
        }
    }
}
