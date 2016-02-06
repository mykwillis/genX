using System;
using System.Collections;
using genX;

namespace genX.Scaling
{
    /// <summary>
    /// Compares <see cref="genX.Chromosome"/> objects based on the value of
    /// their Objective property.
    /// </summary>
    /// <remarks>
    /// This comparer is used when sorting Chromosomes in a population.
    /// </remarks>
    class ObjectiveComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            Chromosome c1, c2;
            c1 = (Chromosome) x;
            c2 = (Chromosome) y;

            double Difference = c1.NormalizedObjective - c2.NormalizedObjective;
            if ( Difference > 0.0 )         return 1;
            else if ( Difference < 0.0 )    return -1;
            else                            return 0;
        }
    }


    /// <summary>
    /// A Fitness scaler that implements proportional fitness scaling.
    /// </summary>
    /// <remarks>
    /// Proportional fitness scaling simply assigns a fitness value that
    /// is identical to each chromosome's NormalizedObjective.  Proportional
    /// fitness scaling has been shown to lead to premature convergence,
    /// and so is not often used.
    /// </remarks>
    [Serializable]
    public class ProportionalFitnessScaler
    {
        /// <summary>
        /// Scales a collection of chromosome's objective values and puts
        /// the result in the Fitness property.
        /// </summary>
        /// <param name="Chromosomes">
        /// An array of chromosomes.
        /// </param>
        public static void Scale(Chromosome[] Chromosomes)
        {
            // In proportional scaling, the fitness of each individual is
            // simply that individual's objective value.
            foreach(Chromosome c in Chromosomes)
            {
                c.Fitness = c.NormalizedObjective;
            }
        }
    }


    /// <summary>
    /// Performs linear fitness scaling.
    /// </summary>
    /// <remarks>
    /// Linear fitness scaling is a function that assigns a scaled fitness
    /// value to each individual based on a linear relationship with the 
    /// original fitness value.
    /// <P>
    /// The parameters of this relationship are
    /// chosen in a manner to leave the fitness values of average individuals
    /// unchanged, while increasing the best individual's fitness by a
    /// multiple specified by FitnessMultiple.
    /// </P>
    /// </remarks>
    ///     f' = af + b
    /// Proportional fitness scaling can be seen as linear scaling with
    /// a=1 and b=0.
    /// a and b are chosen such that:
    ///     1. The average scaled fitness is equal to the average raw fitness;
    ///     2. The maximum scaled fitness is C times the average scaled fitness,
    ///         where C represents the desired number of copies of the best
    ///         individual.
    /// Each of these two conditions provides a point on the line represented
    /// by f' = ax+b, so it is straightforward to get values for a and b.
    /// 
    /// Because scaled fitness must be positive, we do have to check for cases
    /// where some raw fitness values would be scaled negative.  If this occurs,
    /// we adjust a and b such that the line crosses through the origin.
    /// 
    /// a is the line's slope.  b is its y-intercept.
    [Serializable]
    public class LinearFitnessScaler
    {   
        /// <summary>
        /// Specifies the fitness multiplier for the best individual in the
        /// population.
        /// </summary>
        /// <remarks>
        /// Values between 1.2 and 2.0 have been shown to perform well in
        /// small (50-100) populations.
        /// </remarks>
        public double FitnessMultiple
        {
            get { return fitnessMultiple; }
            set { fitnessMultiple = value; }
        }
        private double fitnessMultiple;
        /// <summary>
        /// Specifies the default fitness multiple.
        /// </summary>
        public const double DefaultFitnessMultiple = 1.5;

        double slope;
        double y_intercept;

        void PreScale(double minFitness, double maxFitness, double avgFitness)
        {
            double delta_x;
            double delta_y;

            //
            // Calculate a and b.  Assume that all raw fitness values will map
            // to positive values.
            //
            // Point 1: (avgFitness, avgFitness)
            // Point 2: (maxFitness, avgFitness*fitnessMultiple)
            //
            delta_y = ( avgFitness*fitnessMultiple ) - avgFitness;
            delta_x = ( maxFitness - avgFitness );

            // y = slope * x + b, so
            // b = y - (slope * x)

            slope       = delta_y / delta_x;
            y_intercept = avgFitness - (slope*avgFitness);  // use point (avgFitness, avgFitness)

            //
            // if the value of the minFitness is negative, we need to adjust this
            // so that minFitness has 0 scaled fitness.  So our points are:
            //
            // Point 1: (avgFitness, avgFitness)
            // Point 2: (minFitness, 0)
            //
            if ( slope * minFitness + y_intercept < 0 )
            {
                delta_y = ( 0 - avgFitness );
                delta_x = ( minFitness - avgFitness );

                slope = delta_y / delta_x;
                y_intercept = avgFitness - (slope*avgFitness);  // use point (avgFitness, avgFitness)
            }
        }

        /// <summary>
        /// Scales a collection of chromosome's objective values and puts
        /// the result in the Fitness property.
        /// </summary>
        /// <param name="chromosomes">
        /// An array of chromosomes.
        /// </param>
        public void Scale(Chromosome[] chromosomes)
        {
            //
            // First, we need to calculate the min, max, and average values for
            // the NormalizedObjectives.
            //
            double minObjective, maxObjective, totalObjective, avgObjective;
            minObjective = chromosomes[0].NormalizedObjective;
            maxObjective = chromosomes[0].NormalizedObjective;
            totalObjective = chromosomes[0].NormalizedObjective;
            for(int i=0;i<chromosomes.Length;i++)
            {
                totalObjective += chromosomes[i].NormalizedObjective;
                if ( chromosomes[i].NormalizedObjective < minObjective )
                {
                    minObjective = chromosomes[i].NormalizedObjective;
                }
                if ( chromosomes[i].NormalizedObjective > maxObjective )
                {
                    maxObjective = chromosomes[i].NormalizedObjective;
                }
            }
            avgObjective = totalObjective / chromosomes.Length;


            //
            // Use the calculated values to find the slope and y_intercept.
            //
            PreScale(minObjective, maxObjective, avgObjective);


            //
            // Apply the linear function to each of the chromosomes.
            //
            foreach(Chromosome c in chromosomes)
            {
                c.Fitness = (slope * c.NormalizedObjective) + y_intercept;
            }
        }


        /// <summary>
        /// Creates a new linear fitness scaler with the default fitness
        /// multiple.
        /// </summary>
        public LinearFitnessScaler() : this(DefaultFitnessMultiple) {}


        /// <summary>
        /// Creates a new linear fitness scaler with the given fitness
        /// multiple.
        /// </summary>
        /// <param name="fitnessMultiple">
        /// The value for the FitnessMultiple property.
        /// </param>
        public LinearFitnessScaler(double fitnessMultiple)
        {
            this.fitnessMultiple = fitnessMultiple;
        }
    }


    /// <summary>
    /// Performs linear ranked fitness scaling.
    /// </summary>
    /// <remarks>
    /// <P>
    /// Rank-based fitness scaling ignores the specific value of each 
    /// Chromosome's Objective, using instead the rank of the Chromosome when
    /// the population is sorted according to objective.
    /// </P>
    /// <P>
    /// Rank-based fitness scaling avoids the premature convergence problems
    /// of proportional fitness scaling in favor of a much more subtle bias
    /// toward higher-performing individuals.
    /// </P>
    /// </remarks>
    [Serializable]
    public class LinearRankedFitnessScaler
    {
        /// <summary>
        /// Specifies the default selective pressure.
        /// </summary>
        public const double DefaultSelectivePressure = 1.5;

        /// <summary>
        /// Gets or sets the selective pressure of this scaler.
        /// </summary>
        /// <remarks>
        /// The selective pressure may be any value between 1.0 and 2.0.
        /// </remarks>
        /// <exception cref="Exception">
        /// If the value is set to a value outside the range [1.0, 2.0].
        /// </exception>
        public double SelectivePressure
        {
            get { return selectivePressure; }
            set 
            {
                if ( value < 1.0 || value > 2.0 )
                {
                    throw new ArgumentOutOfRangeException(
                        "SelectivePressure",
                        (object)value,
                        "Selective Pressure must be in the range [1.0, 2.0]."
                        );
                }
                selectivePressure = value; 
            }
        }
        double selectivePressure;


        /// <summary>
        /// Creates a new linear ranked fitness scaler with the given selective pressure.
        /// </summary>
        public LinearRankedFitnessScaler(double selectivePressure)
        {
            this.selectivePressure = selectivePressure;
        }


        /// <summary>
        /// Creates a new linear ranked fitness scaler with the default selective pressure.
        /// </summary>
        public LinearRankedFitnessScaler() : this(DefaultSelectivePressure) {}



        /// <summary>
        /// Scales a collection of chromosome's objective values and puts
        /// the result in the Fitness property.
        /// </summary>
        /// <param name="Chromosomes">
        /// An array of chromosomes.
        /// </param>
        public void Scale(Chromosome[] Chromosomes)
        {
            //
            // We assume that the chromosomes have already been sorted
            // according to NormalizedObjective value.
            //

            //
            // Now, calculate each chromosome's fitness based on the 
            // formula...[This should be checked against Blickle & Thiel]
            //
            //  Fitness(Pos) = 2 - SP + 2·(SP - 1)·(Pos - 1) / (Nind - 1) 
            //
            for(int i=0;i<Chromosomes.Length;i++)
            {                
                Chromosomes[i].Fitness = (
                    (2 - SelectivePressure)         + 
                    ( (2 * (SelectivePressure - 1)) * (i) )
                    / (Chromosomes.Length - 1)
                    );
            }
        }
    }
 
    /// <summary>
    /// Performs pre-scaling operations on a population.
    /// </summary>
    [Serializable]
    public class SigmaTruncation
    {
        /// <summary>
        /// Constant multiple.
        /// </summary>
        /// <remarks>
        /// Should generally be between 1 and 3
        /// </remarks>
        public double C
        {
            get { return c; }
            set { c = value; }
        }
        double c;   // between 1.0 and 3.0
        const double defaultC = 1.0;

        /// <summary>
        /// Truncates the Fitness value 
        /// </summary>
        /// <param name="chromosomes"></param>
        public void Truncate(Chromosome[] chromosomes)
        {
            //
            //
            // This code ripped from PopulationSummary for now.
            // >>>>
            //
            double totalObjective;            
            double meanObjective;
            double variance;
            double standardDeviation;

            totalObjective = 0;

            foreach( Chromosome c in chromosomes )
            {
                totalObjective += c.NormalizedObjective;
            }
            meanObjective = totalObjective / chromosomes.Length;


            //
            // Variance is the average squared deviation from the mean.
            // Standard Deviation is just the square root of this.
            //
            double sum = 0;
            foreach(Chromosome c in chromosomes)
            {
                double deviation;
                double squaredDeviation;

                deviation = c.NormalizedObjective - meanObjective;
                squaredDeviation = deviation * deviation;

                sum += squaredDeviation;
            }
            variance = sum / (chromosomes.Length-1);
            standardDeviation = Math.Sqrt( variance );

            //
            //
            // This code ripped from PopulationSummary for now.
            // <<<<
            //

            
            //
            // Fitness = NormalizedObjective - (meanObjective - c * standardDeviation)
            //
            foreach(Chromosome c in chromosomes)
            {
                c.NormalizedObjective = c.NormalizedObjective - (meanObjective - (C * standardDeviation));
                if ( c.NormalizedObjective  < 0 )
                {
                    c.NormalizedObjective = 0;
                }
            }
        }

        /// <summary>
        /// Initialize sigma truncation with a default constant.
        /// </summary>
        public SigmaTruncation() : this(defaultC) {}

        /// <summary>
        /// Initialize sigma truncation with a given constant.
        /// </summary>
        public SigmaTruncation(double c)
        {
            this.c = c;
        }
    }


    /// <summary>
    /// Performs power law scaling.
    /// </summary>
    /// <remarks>
    /// Power law scaling simply sets the fitness of each individual to some
    /// constant power of the objective.
    /// </remarks>
    [Serializable]
    public class PowerLawScaling
    {
        /// <summary>
        /// Gets or sets the power to which the objective will be raised to
        /// obtain the fitness.
        /// </summary>
        public double Power
        {
            get { return power; }
            set { power = value; }
        }
        double power;
        const double defaultPower = 1.005;

        /// <summary>
        /// Creates a new power law scaling object with default Power.
        /// </summary>
        public PowerLawScaling() : this(defaultPower) {}

        /// <summary>
        /// Creates a new power law scaling object with the given Power.
        /// </summary>
        /// <param name="power">The value of the Power property.</param>
        public PowerLawScaling(double power)
        {
            this.power = power;
        }        

        /// <summary>
        /// Scales a collection of chromosome's objective values and puts
        /// the result in the Fitness property.
        /// </summary>
        /// <param name="chromosomes">
        /// An array of chromosomes.
        /// </param>
        public void Scale(Chromosome[] chromosomes)
        {
            foreach(Chromosome c in chromosomes)
            {
                c.Fitness = Math.Pow( c.NormalizedObjective, power );
            }
        }
    }
}