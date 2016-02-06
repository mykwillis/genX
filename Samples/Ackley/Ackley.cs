using System;
using genX;
using genX.Encoding;

namespace Ackley
{
	class Ackley
	{
        /// <summary>
        /// Calculates the objective based on Ackley's function.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>
        /// Ackley's function is given by:
        /// 
        ///             F(x) = 20 + e
        ///                    - 20*exp(-0.2*sqrt((1/n)*Sum(i=1,n) {x_i^2}))
        ///                    - exp((1/n)*Sum(i=1,n){cos(2*pi*x_i)}) 

        /// </remarks>
        public static double AckleyMinimizationObjective(Chromosome c)
        {
            int n = c.Genes.Length;
            DoubleGene[] x = new DoubleGene[c.Genes.Length];
            Array.Copy(c.Genes, x,c.Genes.Length);  // performs type casting for each element
            double f;
            double objective;


            // Split the equation into three parts:
            // F(x) = 
            //  (a)    20 + e
            //  (b)    - 20*exp(-0.2*sqrt((1/n)*Sum(i=1,n) {x_i^2}))
            //  (c)    - exp((1/n)*Sum(i=1,n){cos(2*pi*x_i)}) 


            //
            // Calculate the (c) part.
            //
            double f_c = 0;
            for(int i=0;i<n;i++)
            {
                f_c += Math.Cos(2*Math.PI*x[i].Value);
            }
            f_c *= ( 1.0/n );
            f_c = -Math.Exp( f_c );

            //
            // Calculate the (b) part
            //
            double f_b = 0;
            for(int i=0;i<n;i++)
            {
                double sum;
                sum = (x[i].Value * x[i].Value)/n;                
                f_b += sum;
            }
            f_b = Math.Sqrt( f_b );
            f_b *= -0.2;
            f_b = Math.Exp( f_b );
            f_b *= -20.0;

            //
            // Calculate the (a) part
            //
            double f_a = 0;
            f_a = 20.0 + Math.E;


            //
            // Calculate the total as (a) + (b) + (c)
            //
            f = f_a + f_b + f_c;

            objective = Math.Abs(f);

            return objective;
        }

		static void Main(string[] args)
		{
            GA ga;

            ga = new GA();
            ga.Homogeneous = true;
            ga.ChromosomeLength = 5;
            ga.EncodingType = EncodingType.Real;
            ga.MinDoubleValue = -30;
            ga.MaxDoubleValue = 30;

            ga.Objective            = new ObjectiveDelegate(AckleyMinimizationObjective);            
            ga.ObjectiveType        = ObjectiveType.MinimizeObjective;

            ga.Run();

            PopulationSummary ps = new PopulationSummary( ga, ga.Population );

            Console.WriteLine("Best Individual:");
            Console.WriteLine(ps.BestChromosome);
            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
 		}
	}
}
