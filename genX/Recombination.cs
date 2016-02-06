using System;

namespace genX.Recombination
{
    /// <summary>
    /// Implements single point crossover.
    /// </summary>
    public class SinglePointCrossover
    {
        /// <summary>
        /// Recombines two Chromosomes according to single point crossover.
        /// </summary>
        /// <param name="parent1">The first parent chromosome.</param>
        /// <param name="parent2">The second parent chromosome.</param>
        /// <returns>
        /// An array of length two containing the offspring from the 
        /// recombination operation.
        /// </returns>
        /// <remarks>
        /// Single-point crossover generates offspring that have the first half of 
        /// their genes inherited from one parent and the second half inherited
        /// from the other parent.  The point at which the switch from one parent's
        /// genes to another's is made is called the crossover point.
        /// </remarks>
        public static Chromosome[] Recombine(Chromosome parent1, Chromosome parent2)
        {
            Chromosome[] Offspring = new Chromosome[2];
            Chromosome c1 = parent1;
            Chromosome c2 = parent2;

            int NumberOfGenes;

            NumberOfGenes = c1.Genes.Length;
            int CrossoverPoint = Utils.Rand.Next( NumberOfGenes );

            Offspring[0] = new Chromosome();
            Offspring[0].Genes = new Gene[c1.Genes.Length];
            Offspring[0].CrossoverPoint = CrossoverPoint;
            for(int i=0;i<CrossoverPoint;i++)
            {
                Offspring[0].Genes[i] = (Gene) c1.Genes[i].Clone();
            }
            for(int i=CrossoverPoint;i<NumberOfGenes;i++)
            {
                Offspring[0].Genes[i] = (Gene) c2.Genes[i].Clone();
            }

            Offspring[1] = new Chromosome();
            Offspring[1].Genes = new Gene[c1.Genes.Length];
            Offspring[1].CrossoverPoint = CrossoverPoint;
            for(int i=0;i<CrossoverPoint;i++)
            {
                Offspring[1].Genes[i] = (Gene) c2.Genes[i].Clone();
            }
            for(int i=CrossoverPoint;i<NumberOfGenes;i++)
            {
                Offspring[1].Genes[i] = (Gene) c1.Genes[i].Clone();
            }

            return Offspring;
        }
    }
    
    /// <summary>
    /// Implements two-point crossover.
    /// </summary>
    public class TwoPointCrossover
    {
        /// <summary>
        /// Recombines two Chromosomes according to 2 point crossover.
        /// </summary>
        /// <param name="parent1">The first parent chromosome.</param>
        /// <param name="parent2">The second parent chromosome.</param>
        /// <returns>
        /// An array of length two containing the offspring from the 
        /// recombination operation.
        /// </returns>
        public static Chromosome[] Recombine(Chromosome parent1, Chromosome parent2)
        {
            Chromosome[] Offspring = new Chromosome[2];
            Chromosome c1 = parent1;
            Chromosome c2 = parent2;

            int NumberOfGenes;

            NumberOfGenes = c1.Genes.Length;
            int CrossoverPoint1 = Utils.Rand.Next( NumberOfGenes );
            int CrossoverPoint2 = Utils.Rand.Next( NumberOfGenes );
            if ( CrossoverPoint1 > CrossoverPoint2 )
            {
                int tmp = CrossoverPoint1;
                CrossoverPoint1 = CrossoverPoint2;
                CrossoverPoint2 = tmp;
            }

            Offspring[0] = new Chromosome();
            Offspring[0].Genes = new Gene[c1.Genes.Length];
            for(int i=0;i<CrossoverPoint1;i++)
            {
                Offspring[0].Genes[i] = (Gene) c1.Genes[i].Clone();
            }
            for(int i=CrossoverPoint1;i<CrossoverPoint2;i++)
            {
                Offspring[0].Genes[i] = (Gene) c2.Genes[i].Clone();
            }
            for(int i=CrossoverPoint2;i<NumberOfGenes;i++)
            {
                Offspring[0].Genes[i] = (Gene) c1.Genes[i].Clone();
            }

            Offspring[1] = new Chromosome();
            Offspring[1].Genes = new Gene[c1.Genes.Length];
            for(int i=0;i<CrossoverPoint1;i++)
            {
                Offspring[1].Genes[i] = (Gene) c2.Genes[i].Clone();
            }
            for(int i=CrossoverPoint1;i<CrossoverPoint2;i++)
            {
                Offspring[1].Genes[i] = (Gene) c1.Genes[i].Clone();
            }
            for(int i=CrossoverPoint2;i<NumberOfGenes;i++)
            {
                Offspring[1].Genes[i] = (Gene) c2.Genes[i].Clone();
            }

            return Offspring;            
        }
    }
    
    /// <summary>
    /// Implements Partially Matched Crossover (PMX).
    /// </summary>
    public class PartiallyMatchedCrossover
    {
        /// <summary>
        /// Recombines two Chromosomes according to partially-matched 
        /// recombination (PMX).
        /// </summary>
        /// <param name="parent1">The first parent chromosome.</param>
        /// <param name="parent2">The second parent chromosome.</param>
        /// <returns>
        /// An array of length two containing the offspring from the 
        /// recombination operation.
        /// </returns>
        public virtual Chromosome[] Recombine(Chromosome parent1, Chromosome parent2)
        {
            Chromosome[] Offspring = new Chromosome[2];
            Chromosome c1 = parent1;
            Chromosome c2 = parent2;
            int NumberOfGenes;

            NumberOfGenes = c1.Genes.Length;
            int CrossoverPoint1 = Utils.Rand.Next( NumberOfGenes );
            int CrossoverPoint2 = Utils.Rand.Next( NumberOfGenes );

            if ( CrossoverPoint1 > CrossoverPoint2 )
            {
                int tmp = CrossoverPoint1;
                CrossoverPoint1 = CrossoverPoint2;
                CrossoverPoint2 = tmp;
            }

            Offspring[0] = new Chromosome();
            Offspring[0].Genes = (Gene[]) c1.Genes.Clone(); // BUGBUG
            for(int i=CrossoverPoint1;i<CrossoverPoint2;i++)
            {
                //
                // Replace gene i with the gene i from c2.  Whatever label
                // we're replacing needs to be swapped.
                //
                Gene oldAllele = Offspring[0].Genes[i]; // BUGBUG
                Gene newAllele = c2.Genes[i];

                for(int j=0;j<NumberOfGenes;j++)
                {
                    if ( Offspring[0].Genes[j].Label == newAllele.Label )
                    {
                        Offspring[0].Genes[j] = oldAllele;
                    }
                }
                Offspring[0].Genes[i] = newAllele;
            }

            Offspring[1] = new Chromosome();
            Offspring[1].Genes = (Gene[]) c2.Genes.Clone();
            for(int i=CrossoverPoint1;i<CrossoverPoint2;i++)
            {
                Gene oldAllele = Offspring[1].Genes[i];
                Gene newAllele = c1.Genes[i];

                for(int j=0;j<NumberOfGenes;j++)
                {
                    if ( Offspring[1].Genes[j].Label == newAllele.Label )
                    {
                        Offspring[1].Genes[j] = oldAllele;
                    }
                }
                Offspring[1].Genes[i] = newAllele;
            }

            return Offspring;
        }
    }
    
    /// <summary>
    /// Implements uniform crossover.
    /// </summary>
    public class UniformCrossover
    {
        const double mixingRatio = 0.5;

        /// <summary>
        /// Recombines two Chromosomes according to uniform crossover 
        /// </summary>
        /// <remarks>
        /// Uniform crossover randomly chooses one gene from either of the
        /// parents at each position in the offspring.
        /// </remarks>
        /// <param name="parent1">The first parent chromosome.</param>
        /// <param name="parent2">The second parent chromosome.</param>
        /// <returns>
        /// An array of length two containing the offspring from the 
        /// recombination operation.
        /// </returns>
        public static Chromosome[] Recombine(Chromosome parent1, Chromosome parent2)
        {
            Chromosome[] Offspring = new Chromosome[2];            
            Chromosome c1 = parent1;
            Chromosome c2 = parent2;

            int NumberOfGenes;


            Offspring[0] = new Chromosome();
            Offspring[0].Genes = (Gene[]) c1.Genes.Clone();
            Offspring[1] = new Chromosome();
            Offspring[1].Genes = (Gene[]) c2.Genes.Clone();

            NumberOfGenes = c1.Genes.Length;
            for(int i=0;i<NumberOfGenes;i++)
            {
                if ( Utils.Rand.NextDouble() < mixingRatio )
                {
                    Offspring[0].Genes[i] = (Gene) c1.Genes[i].Clone();
                    Offspring[1].Genes[i] = (Gene) c2.Genes[i].Clone();
                }
                else
                {
                    Offspring[0].Genes[i] = (Gene) c2.Genes[i].Clone();
                    Offspring[1].Genes[i] = (Gene) c1.Genes[i].Clone();
                }                
            }

            return Offspring;
        }
    }
}