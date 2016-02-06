using System;
using genX;
using genX.Encoding;
using genX.Termination;

namespace genX.Samples
{
    class LinearDiophantineSolver
    {
        // 1a + 2b + 3c + 4d = 5
        int[] Coefficients;
        int Result;

        public double LinearDiophantineObjective(Chromosome c)
        {
            double r = 0;
            for(int i=0;i<Coefficients.Length;i++)
            {
                IntegerGene g = (IntegerGene) c.Genes[i];
                r += ((double)Coefficients[i] * (double)g.Value);
            }

            return System.Math.Abs((double)Result - r);
        }        

        public void Run()
        {
            GA Ga;

            Ga = new GA();

            Ga.EncodingType     = EncodingType.Integer;
            Ga.MinIntValue      = -5000;
            Ga.MaxIntValue      = 5000;
            Ga.ChromosomeLength = Coefficients.Length;
            Ga.PopulationSize   = Coefficients.Length * 10;

            Ga.Objective            = new ObjectiveDelegate( this.LinearDiophantineObjective );
            Ga.ObjectiveType        = ObjectiveType.MinimizeObjective;
            Ga.Terminate            += new TerminateEventHandler( new ObjectiveThresholdTerminator(0).Terminate );
            Ga.Terminate            += new TerminateEventHandler( new EvolutionTimeTerminator( new TimeSpan(0,0,20) ).Terminate );
            Ga.NewPopulation        += new NewPopulationEventHandler( OnNewPopulation_ShowSummary );
            Ga.FitnessScaling       = FitnessScaling.LinearRanked;

            Ga.Run();

            Console.WriteLine("Best Individual: (obj={0})", Ga.BestObjective);
            PopulationSummary ps = new PopulationSummary( Ga, Ga.Population );
            Console.WriteLine(ps.BestChromosome);
            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            LinearDiophantineSolver solver = new LinearDiophantineSolver();
            
            solver.Coefficients = new int[] { 31, 12, 3, 64, 45 };;
            solver.Result = 6345;

            solver.Run();
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
