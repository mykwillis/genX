using System;
using genX;

namespace genX.Selection
{
    /// <summary>
    /// Performs selection with the roulette method.
    /// </summary>
    public class RouletteSelector
    {
        /// <summary>
        /// Selects a single chromosome for reproduction based on roulette
        /// selection.
        /// </summary>
        /// <param name="chromosomes">The array of candidate chromosomes.</param>
        /// <param name="n">The number of chromosomes to select.</param>
        /// <returns>
        /// A Chromosome that should be used for reproduction.
        /// </returns>
        public static Chromosome[] Select(Chromosome[] chromosomes, int n)
        {
            // "stochastic sampling with replacement"
            Chromosome[] cs = new Chromosome[n];

            /*
                The individuals are mapped to contiguous segments of a line, such 
                that each individual's segment is equal in size to its fitness. 
                A random number is generated and the individual whose segment 
                spans the random number is selected. The process is repeated until 
                the desired number of individuals is obtained (called mating 
                population). This technique is analogous to a roulette wheel with 
                each slice proportional in size to the fitness. 
                */
            double TotalFitness = 0;
            foreach(Chromosome c in chromosomes)
            {
                TotalFitness += c.Fitness;
            }

            for(int i=0;i<n;i++)
            {
                double d = Utils.Rand.NextDouble() * TotalFitness;

                double CumulativeFitness = 0;
                int j;
                // OPTIMIZE: this should work from the end and go back, because the
                // low fitness individuals are at the beginning of the array.
                for(j=0;j<chromosomes.Length & d > CumulativeFitness;j++)
                {
                    CumulativeFitness += chromosomes[j].Fitness;
                }
                if (j!=0)
                {
                    cs[i] = chromosomes[j-1];
                }
                else 
                {
                    cs[i] = chromosomes[0];
                }
            }
            return cs;
        }
    }


    /// <summary>
    /// Performs tournament selection.
    /// </summary>
    /// <remarks>
    /// Tournament selection operators by choosing a random subpopulation
    /// and selecting the best individual from this subpopulation to reproduce.
    /// </remarks>
    public class TournamentSelector
    {
        /// <summary>
        /// Gets or sets the size of each tour.
        /// </summary>
        /// <remarks>
        /// The TourSize property determines how many chromosomes will be
        /// selected from the population for comparison in each tour.  The
        /// output of each tour is the chromosome in the tour that had the
        /// highest fitness.
        /// </remarks>
        public int TourSize
        {
            get { return tourSize; }
            set { tourSize = value; }
        }
        private int tourSize;


        /// <summary>
        /// Selects a single chromosome for reproduction based on tournament
        /// selection.
        /// </summary>
        /// <param name="chromosomes">The array of candidate chromosomes.</param>
        /// <returns>
        /// A Chromosome that should be used for reproduction.
        /// </returns>
        public Chromosome Select(Chromosome[] chromosomes)
        {
            if ( TourSize > chromosomes.Length || TourSize < 1 )
            {
                throw new ArgumentOutOfRangeException(
                    "TourSize",
                    TourSize,
                    "TourSize must be between 1 and the population size."                    
                    );
            }

            Chromosome c;
            Chromosome highestChromosome=null;
            int highestFitness = 0;
            for(int i=0;i<TourSize;i++)
            {
                c = chromosomes[ genX.Utils.Rand.Next(TourSize) ];
                if ( c.Fitness > highestFitness )
                {
                    highestChromosome = c;
                }
            }

            return highestChromosome;
        }
    }


    // TODO: This requires that it set the entire mating population in
    // one call.

    /// <summary>
    /// Performs stochastic remainder selection without replacement.
    /// </summary>
    /// <remarks>
    /// Each individual is selected deterministically according to the integer
    /// portion of their expected selection count.  Thereafter, individuals are
    /// chosen according to roulette selection based on the fractional part of
    /// the expected selection count.
    /// </remarks>
    public class StochasticRemainderSelectionWithoutReplacement
    {
        /// <summary>
        /// Selects a number of individuals for reproduction.
        /// </summary>
        /// <param name="chromosomes"></param>
        /// <param name="nc"></param>
        /// <returns></returns>
        public static Chromosome[] Select(Chromosome[] chromosomes, int nc)
        {
            Chromosome[] mates = new Chromosome[nc];
            int n=0;

            double[] expectedSelectionCounts = new double[nc];
            double[] fractionalExpectedSelectionCounts = new double[nc];

            double totalFitness=0;
            foreach(Chromosome c in chromosomes)
            {
                totalFitness += c.Fitness;
            }
            for(int i=0;i<nc;i++)
            {
                Chromosome c = chromosomes[i];

                expectedSelectionCounts[i] = (c.Fitness / totalFitness) * chromosomes.Length;
                double sc = expectedSelectionCounts[i];
                while ( sc - 1.0 > 0 && n < nc)
                {
                    mates[n++] = c;
                    sc -= 1.0;
                }
                fractionalExpectedSelectionCounts[i] = sc;
            }

            // calculate total fractional expected count
            double totalFractionalExpectedSelectionCount=0;
            for(int i=0;i<nc;i++)
            {
                totalFractionalExpectedSelectionCount += fractionalExpectedSelectionCounts[i];
            }

            // do roulette selection based on fractional expected count

            int diff = nc - n;
            for(int i=n;i<nc;i++)
            {
                double d = Utils.Rand.NextDouble() * totalFractionalExpectedSelectionCount;

                double CumulativeCount = 0;
                int j;
                for(j=0;j<nc && d > CumulativeCount;j++)
                {
                    CumulativeCount += fractionalExpectedSelectionCounts[j];
                }
                if (j!=0)
                {
                    mates[i] = chromosomes[j-1];
                }
                else 
                {
                    mates[i] = chromosomes[0];
                }
            }

            // randomize the results, since Recombine will mate each adjacent
            // pair.
            Chromosome[] randomMates = new Chromosome[nc];
            for(int i=0;i<nc;i++)
            {
                do
                {
                    int r = Utils.Rand.Next(nc);
                    randomMates[i] = mates[ r ];
                    mates[r] = null;
                }
                while ( randomMates[i] == null );
            }            
            return randomMates;
        }
    }
}