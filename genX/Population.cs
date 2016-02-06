using System;

namespace genX
{
    /// <summary>
    /// Provides summary statistics for a <see cref="genX.Population"/>.
    /// </summary>
    [Serializable]
    public class PopulationSummary
    {
        /// <summary>
        /// The highest objective value present in the population.
        /// </summary>
        public double HighestObjective
        {
            get { return highestObjective; }
        }
        double highestObjective;


        /// <summary>
        /// The lowest objective value present in the population.
        /// </summary>
        public double LowestObjective
        {
            get { return lowestObjective; }
        }
        double lowestObjective;


        /// <summary>
        /// The mean objective value of the population.
        /// </summary>
        public double MeanObjective
        {
            get { return meanObjective; }
        }
        double meanObjective;


        /// <summary>
        /// Gets the <see cref="genX.Chromosome"/> in the population with the
        /// highest objective value.
        /// </summary>
        public Chromosome BestChromosome
        {
            get { return bestChromosome; }
            set { bestChromosome = value; }
        }
        Chromosome bestChromosome;


        /// <summary>
        /// Gets the average squared deviation from the mean.
        /// </summary>
        /// <remarks>
        /// "average squared deviation from the mean" means that Variance
        /// represents a measure of how 'clumped up' a population is around
        /// the average (mean) objective score.
        /// </remarks>
        public double Variance
        {
            get { return variance; }
        }
        double variance;


        /// <summary>
        /// Gets the square root of the Variance.
        /// </summary>
        public double StandardDeviation
        {
            get { return standardDeviation; }
        }
        double standardDeviation;


        /// <summary>
        /// Gets the difference between the highest and lowest objectives.
        /// </summary>
        public double Range
        {
            get { return highestObjective - lowestObjective; }
        }


        /*
         * mode - most frequently occuring score.         
         * median - midpoint of distribution, above which have of the scores fall, and below
         *   which the other half fall.
         */

        /// <summary>
        /// Creates a PopulationSummary from a Population.
        /// </summary>
        /// <param name="ga">The GA.</param>
        /// <param name="Population">The Population for which the 
        /// PopulationSummary will provide summary information.
        /// </param>
        public PopulationSummary( GA ga, Population Population )
        {
            double totalObjective;

            lowestObjective = System.Double.MaxValue;
            highestObjective = System.Double.MinValue;
            totalObjective = 0;

            foreach( Chromosome c in Population.Chromosomes )
            {
                totalObjective += c.RawObjective;
                if ( c.RawObjective < LowestObjective )
                {
                    lowestObjective = c.RawObjective;
                    if ( ga.ObjectiveType == ObjectiveType.MinimizeObjective )
                    {
                        bestChromosome = c;
                    }
                }
                if ( c.RawObjective > HighestObjective )
                {
                    highestObjective = c.RawObjective;
                    if ( ga.ObjectiveType == ObjectiveType.MaximizeObjective )
                    {
                        bestChromosome = c;
                    }
                }
            }
            meanObjective = totalObjective / Population.Chromosomes.Length;


            //
            // Variance is the average squared deviation from the mean.
            // Standard Deviation is just the square root of this.
            //
            double sum = 0;
            foreach(Chromosome c in Population.Chromosomes)
            {
                double deviation;
                double squaredDeviation;

                deviation = c.RawObjective - meanObjective;
                squaredDeviation = deviation * deviation;

                sum += squaredDeviation;
            }
            variance = sum / (Population.Chromosomes.Length-1);
            standardDeviation = Math.Sqrt( variance );
        }


        /// <summary>
        /// Dumps a text representation of the PopulationSummary.
        /// </summary>
        /// <param name="textWriter">
        /// Specifies the TextWriter to where the summary will be
        /// written.
        /// </param>
        /// <example>
        /// This example shows sample output from this method:
        /// <code>
        ///     HighestObjective: 95
        ///     LowestObjective:  92
        ///     MeanObjective:    94
        ///     Variance:         1.0
        ///     StandardDeviation:1.0
        /// </code>
        /// </example>
        public void Write2(System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("HighestObjective: {0}", HighestObjective);
            textWriter.WriteLine("LowestObjective:  {0}", LowestObjective);
            textWriter.WriteLine("MeanObjective:    {0}", MeanObjective);
            textWriter.WriteLine("Variance:         {0}", Variance);
            textWriter.WriteLine("StandardDeviation:{0}", StandardDeviation);
        }

        /// <summary>
        /// Writes the population summary to a text writer.
        /// </summary>
        /// <param name="textWriter"></param>
        public void Write(System.IO.TextWriter textWriter)
        {
            textWriter.Write( "O: {0:0.###}, o: {1}, u: {2}, v: {3:0.000}, sd: {4:0.###}",
                HighestObjective, LowestObjective, MeanObjective, Variance, StandardDeviation  );
        }

        /// <summary>
        /// Puts the contents of the Write method in a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            Write( sw );
            return sw.ToString();
        }
    }


    /// <summary>
    /// Represents a single Population of Chromosomes.
    /// </summary>
    [Serializable]
    public class Population
    {
        /// <summary>
        /// Gets the <see cref="genX.Chromosome"/> objects in the Population.
        /// </summary>
        public Chromosome[] Chromosomes
        {
            get { return chromosomes; }
        }
        Chromosome[] chromosomes;


        /// <summary>
        /// Gets or sets the size of the population.
        /// </summary>
        public int PopulationSize
        {
            get { return populationSize;    }
 //           set { populationSize = value;   }
        }
        int populationSize;


        private GA ga;

        /// <summary>
        /// Creates a new Population of Chromosomes.
        /// </summary>        
        /// <param name="ga">The GA.</param>
        /// <param name="populationSize">The size of the population.</param>
        public Population( GA ga, int populationSize )
        {
            this.ga = ga;
            this.populationSize = populationSize;
            chromosomes = new Chromosome[ PopulationSize ];
        }

        /// <summary>
        /// Gets a PopulationSummary for this Population.
        /// </summary>
        public PopulationSummary Summary
        {
            get
            {
                if ( summary == null ) summary = new PopulationSummary(this.ga, this);
                return summary;
            }
        }
        PopulationSummary summary;
   }
}