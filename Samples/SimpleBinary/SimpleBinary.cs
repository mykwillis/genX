using System;
using System.ComponentModel;
using genX;
using genX.Encoding;
using genX.Termination;

namespace genX.Samples
{
    //
    // This sample demonstrates the most basic steps involved in using
    // genX to evolve solutions to a problem.  Note that this example
    // does not take advantage of the design-time capabilities that the 
    // genX GA component supports, it instantiates the component manually
    // using the new operator.  You can use either method in your 
    // application.
    // 
    // The problem being attacked with the algorithm is contrived; we
    // attempt to build chromosomes whose binary gene values alternate
    // between 0 and 1.
    //
    class SimpleBinary
    {
		static void Main()
		{
            //
            // Create the genetic algorithm component, and set the required 
            // properties.
            //
            GA ga = new GA();
         
            ga.EncodingType     = EncodingType.Binary;
            ga.ChromosomeLength = 10;
            ga.PopulationSize   = 50;
            ga.MaxGenerations   = 10;


            //
            // Set the objective function for the run, which provides feedback
            // to the algorithm as to the relative merit of candidate solutions.
            //
            ga.Objective = new ObjectiveDelegate( BinaryAlternateObjective );

            
            //
            // Set a "NewPopulation" event handler so that we get a callback on
            // each new generation.  During the callback, we print out the
            // generation's statistics and the best chromosome found so far.
            //
            ga.NewPopulation += new NewPopulationEventHandler( OnNewPopulation_ShowSummary );

            
            //
            // Run the algorithm.  It will stop after ga.MaxGenerations have elapsed.
            //
            ga.Run();


            //
            // Output the best individual that was found during the run.
            //
            Console.WriteLine("Best Individual:");
            PopulationSummary ps = new PopulationSummary( ga, ga.Population );
            Console.WriteLine(ps.BestChromosome);
            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
		}


        //
        /// Objective function that gives higher values for chromosomes
        /// with binary genes that alternate between 0 and 1.
        //
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

        //
        // Handler for the "NewPopulation" event that is raised by the GA
        // component each time a new population has been evaluated.  We take
        // the opportunity to print out some of the summary data for the
        // just-evaluated generation.
        //
        static void OnNewPopulation_ShowSummary(object o, NewPopulationEventArgs e)
        {
            Population oldPopulation = e.OldPopulation; // the just-evaluated population

            Console.WriteLine( "Generation: {0}", e.Generation );
            Console.WriteLine( "  Highest Objective: {0}", oldPopulation.Summary.HighestObjective );
            Console.WriteLine( "  Lowest Objective : {0}", oldPopulation.Summary.LowestObjective );
            Console.WriteLine( "  Best Individual  : {0}", oldPopulation.Summary.BestChromosome );
        }
	}
}
