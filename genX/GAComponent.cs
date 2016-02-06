using System;
using System.ComponentModel;
using genX;
using genX.Encoding;
using genX.Mutation;
//using genX.Recombination;
using genX.Scaling;
using genX.Selection;
using genX.Termination;

namespace genX
{
    /// <summary>
    /// Specifies the type of the genes used in a chromosome.
    /// </summary>
    public enum EncodingType
    {
        /// <summary>
        /// Specifies that chromosomes are strings of binary digits.
        /// </summary>
        Binary,

        /// <summary>
        /// Specifies that chromosomes are strings of integers.
        /// </summary>
        Integer,

        /// <summary>
        /// Specifies that chromosomes are strings of real numbers 
        /// (System.Double datatype).
        /// </summary>
        Real,

        /// <summary>
        /// Specifies that chromosomes are made up of genes of custom 
        /// data types.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies a method by which chromosomes are selected for reproduction.
    /// </summary>
    public enum SelectionMethod
    {
        /// <summary>
        /// Roulette selection, a.k.a. stochastic selection with replacement.
        /// </summary>
        Roulette,

#if NOTDEF
        /// <summary>
        /// Tournament selection.
        /// </summary>
        Tournament,
#endif

        /// <summary>
        /// Stochastic remainder selection without replacement.
        /// </summary>
        StochasticRemainderWithoutReplacement,

        /// <summary>
        /// A custom selection method is used.
        /// </summary>
        Custom
    }


    /// <summary>
    /// Specifies an operator to use for gene value mutation.
    /// </summary>
    public enum MutationOperator
    {
        /// <summary>
        /// "Bit-flipping" mutation.
        /// </summary>
        Bitwise,

        /// <summary>
        /// Boundary mutation randomly sets a gene to the low or high
        /// boundary.
        /// </summary>
        Boundary,
#if NOTDEF
        /// <summary>
        /// Uniform mutation randomly changes the value of a gene to another
        /// value.
        /// </summary>
        Uniform
#endif
        /// <summary>
        /// Mutation is implemented using mutation operators defined by 
        /// the affected gene's associated GeneDescriptor.
        /// </summary>
        GeneSpecific,

        /// <summary>
        /// A custom mutation method is used.
        /// </summary>
        Custom,
    }


    /// <summary>
    /// Specifies an operator used for recombination.
    /// </summary>
    public enum RecombinationOperator
    {
        /// <summary>
        /// A single point is chosen on the chromosome, and offspring receive genes
        /// before that point from one parent and after that gene from the other.
        /// </summary>
        SinglePointCrossover,

        /// <summary>
        /// Two points are chosen on the chromosome, and offspring receive the genes
        /// between those points from one parent and other genes from the other parent.
        /// </summary>
        TwoPointCrossover,

        /// <summary>
        /// Offspring are produced by randomly selecting each gene from one parent or
        /// the other.
        /// </summary>
        Uniform,

        /// <summary>
        /// A custom recombination method is used.
        /// </summary>
        Custom
    }


    /// <summary>
    /// Specifies a method to use for scaling normalized objectives to fitness values.
    /// </summary>
    public enum FitnessScaling
    {
        /// <summary>
        /// The fitness value is the normalized objective.
        /// </summary>
        Proportional,
//        None = Proportional,

        /// <summary>
        /// Fitness value is scaled linearly.
        /// </summary>
        Linear,

        /// <summary>
        /// Fitness value is assigned according to the ranking of each individual
        /// in order of normalized objective.
        /// </summary>
        LinearRanked,

        /// <summary>
        /// A custom fitness scaler is used.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Specifies one or more methods to use for termination.
    /// </summary>
    /// <remarks>
    /// Termination based on MaxGenerations is always active.
    /// </remarks>
    public enum TerminationFlags
    {
        /// <summary>
        /// Termination when the objective of the best individual in a population
        /// meets or exceeds a given threshold.
        /// </summary>
        ObjectiveThreshold,

#if LATER
        /// <summary>
        /// Terminates when a given amount of time has passed since the run
        /// began.
        /// </summary>
        EvolutionTime,
#endif

        /// <summary>
        /// A custom termination method is used.
        /// </summary>
        Custom,
    }

#if NOTDEF
    /// <summary>
    /// Specifies an operator to apply prior to scaling.
    /// </summary>
    public enum PreScaling
    {
        /// <summary>
        /// No prescaling operation.
        /// </summary>
        None,

        /// <summary>
        /// Sigma truncation.
        /// </summary>
        SigmaTrunctation,

        /// <summary>
        /// Each fitness value is raised to a constant power value.
        /// </summary>
        PowerLaw
    }
#endif

}
