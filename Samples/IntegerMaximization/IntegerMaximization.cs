using System;
using genX;
using genX.Encoding;
using genX.Mutation;

namespace genX.Samples
{
    //
    // This sample demonstrates the use of IntegerGenes with constraints.
    // 
    // The problem being attacked with the algorithm is contrived; we
    // attempt to build chromosomes whose integer gene values are maximized
    // while being constrained to the range [-10,10].
    //
	class IntegerMaximization
	{
		static void Main()
		{
            //
            // Describe the coding of our problem as an array of 10 integers
            // in the range [-10,10].
            // 
            GA ga = new GA();

            ga.ChromosomeLength = 10;
            ga.PopulationSize   = 100;
            ga.MaxGenerations   = 100;

            ga.EncodingType     = EncodingType.Integer;
            ga.MinIntValue      = -10;
            ga.MaxIntValue      = 10;            

            ga.Objective        = new ObjectiveDelegate( IntegerMaximizationObjective );
            ga.MutationOperator = MutationOperator.GeneSpecific;
         
            //
            // Set a "NewPopulation" event handler so that we get a callback on
            // each new generation.  During the callback, we print out the
            // generation's statistics and the best chromosome found so far.
            //
            ga.NewPopulation += new NewPopulationEventHandler( OnNewPopulation_ShowSummary );

   
            //
            // Let it run until default termination criteria are met, or
            // MaxGenerations has elapsed.
            //
            ga.Run();


            //
            // Output the best individual that was found during the run.
            //
            Console.WriteLine("Best Individual:");
            PopulationSummary ps = new PopulationSummary( ga, ga.Population );
            Console.WriteLine( ps.BestChromosome );
            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
		}


        //
        // Objective function that rewards higher-valued integer Genes.  The
        // return value of this function is a value that is the arithmetic sum 
        // of all integers in the Chromosome.
        //
        public static double IntegerMaximizationObjective(Chromosome chromosome)
        {
            double objective = 0;
            foreach(IntegerGene g in chromosome.Genes)
            {
                objective += g.Value;
            }
            return objective;
        }


        //
        // Handler for the "NewPopulation" event that is raised by the GA
        // component each time a new population has been evaluated.  We take
        // the opportunity to print out information about the best chromosome
        // in the just-evaluated generation.
        //
        static void OnNewPopulation_ShowSummary(object o, NewPopulationEventArgs e)
        {            
            Chromosome bestChromosome = e.OldPopulation.Summary.BestChromosome;

            Console.WriteLine( "{0}: {1}  ({2})", 
                e.Generation, 
                bestChromosome, 
                bestChromosome.RawObjective
                );
        }
	}
}
