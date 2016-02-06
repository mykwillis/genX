using System;
using System.ComponentModel;
using genX;

namespace genX
{
    /// <summary>
    /// Arguments passed to a Mutated event handler.
    /// </summary>
    public class MutatedEventArgs : EventArgs
    {
        /// <summary>
        /// The chromosome that was mutated.
        /// </summary>
        public Chromosome Chromosome
        {
            get { return chromosome; }
            set { chromosome = value; }
        }
        Chromosome chromosome;

        /// <summary>
        /// The index of the gene at which the mutation occured.
        /// </summary>
        public int MutationPoint
        {
            get { return mutationPoint; }
            set { mutationPoint = value; }
        }
        int mutationPoint;

        /// <summary>
        /// The value of the gene before the mutation.
        /// </summary>
        public Gene OldGene
        {
            get { return oldGene; }
            set { oldGene = value; }
        }
        Gene oldGene;

        /// <summary>
        /// The value of the gene after the mutation.
        /// </summary>
        public Gene NewGene
        {
            get { return newGene; }
            set { newGene = value; }
        }
        Gene newGene;
    }


    /// <summary>
    /// Arguments passed to a NewPopulation event handler.
    /// </summary>
    public class NewPopulationEventArgs : EventArgs
    {
        /// <summary>
        /// The old population.
        /// </summary>
        public Population OldPopulation
        {
            get { return oldPopulation; }
            set { oldPopulation = value; }
        }
        Population oldPopulation;

        /// <summary>
        /// The new population.
        /// </summary>
        public Population NewPopulation
        {
            get { return newPopulation; }
            set { newPopulation = value; }
        }
        Population newPopulation;

        /// <summary>
        /// The generation most recently evaluated.
        /// </summary>
        public int Generation
        {
            get { return generation; }
            set { generation = value; }
        }
        int generation;
    }


    /// <summary>
    /// Arguments passed to a CalculateObjective event handler.
    /// </summary>
    public class CalculateObjectiveEventArgs : EventArgs
    {
        /// <summary>
        /// The Chromosome whose objective is to be calculated.
        /// </summary>
        public Chromosome Chromosome
        {
            get { return chromosome; }
            set { chromosome = value; }
        }
        Chromosome chromosome;
    }


    /// <summary>
    /// Defines the objective function.
    /// </summary>
    /// <remarks>
    /// <P>
    /// The objective function is responsible for testing how well a chromosome
    /// performs its objective.  In the case of function optimization problems, 
    /// the objective typically calculates the function value based on the 
    /// chromosome's genes.  In other problems, the objective function may subject
    /// the chromosome to a set of random tests to measure its worthiness.
    /// </P>
    /// <P>
    /// The objective function is called once for each chromosome in the population.
    /// The value it returns is stored in that chromosome's RawObjective property.
    /// </P>
    /// <P>
    /// Although the term 'fitness function' is often used to describe the
    /// problem-specific function that measures the worth of individuals in the
    /// population, it's important to understand that the Fitness property of
    /// the Chromosome is not necessarily the same as the RawObjective.  See
    /// <see cref="ScalingDelegate"/> for a description of the process used
    /// to set the Fitness property.
    /// </P>
    /// </remarks>
    public delegate double ObjectiveDelegate(Chromosome c);


    /// <summary>
    /// Defines the fitness scaling function.
    /// </summary>
    /// <remarks>
    /// <P>
    /// Fitness scaling functions are responsible for setting the Fitness
    /// property of each chromosome in the population.  Typically this is done
    /// as some function of the Chromosome's RawObjective value, though the
    /// fitness scaling function is free to assign a value as it sees fit.
    /// </P>
    /// <P>
    /// Fitness values may be any non-negative value, and represent the
    /// relative liklihood of a chromosome being selected for reproduction.
    /// </P>
    /// </remarks>
    /// <param name="chromosomes">An array of Chromosomes for which the Fitness
    /// property is to be calculated.  The passed-in array is sorted according
    /// to ascending Objective values.  The NormalizedObjective property of each Chromosome
    /// is normalized and offset from the value returned by the objective function
    /// such that it may be assumed to be non-negative.
    /// </param>
    public delegate void ScalingDelegate(Chromosome[] chromosomes);


    /// <summary>
    /// Defines the selection delegate.
    /// </summary>
    /// <remarks>
    /// The selection function is responsible for selecting chromosomes
    /// for reproduction.
    /// </remarks>
    public delegate Chromosome[] SelectionDelegate(Chromosome[] c, int i);

    
    /// <summary>
    /// RecombinationDelegate.
    /// </summary>
    public delegate Chromosome[] RecombinationDelegate(Chromosome c1, Chromosome c2);


    /// <summary>
    /// ValueMutationDelegate
    /// </summary>
    public delegate Gene ValueMutationDelegate(Gene g);


    /// <summary>
    /// OrderMutationDelegate
    /// </summary>
    public delegate void OrderMutationDelegate(Chromosome g);


    /// <summary>
    /// Handler for the Mutated event.
    /// </summary>
    public delegate void MutatedEventHandler(object sender, MutatedEventArgs e);


    /// <summary>
    /// Handler for the NewPopulation event.
    /// </summary>
    public delegate void NewPopulationEventHandler(object sender, NewPopulationEventArgs e);


    /// <summary>
    /// Handler for the Terminate event.
    /// </summary>
    public delegate void TerminateEventHandler(object sender, CancelEventArgs e);

    /// <summary>
    /// Handler for the CalculateObjective event.
    /// </summary>
    public delegate void CalculateObjectiveEventHandler(object sender, CalculateObjectiveEventArgs e);
}
