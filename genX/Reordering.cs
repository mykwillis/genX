using System;

namespace genX.Reordering
{
    /// <summary>
    /// Implements the inversion reordering operator.
    /// </summary>
    public class Inversion
    {
        //int maxLength;    // maximum length of string to invert.

        /// <summary>
        /// Reorders the genes in the chromosome.
        /// </summary>
        /// <param name="c">The chromosome to be reordered.</param>
        public static void Reorder(Chromosome c)
        {
            // This is like two-point crossover, in that we pick two
            // random points on the chromosome.  We then reverse the order
            // of the genes contained between those two points.
            int NumberOfGenes;

            NumberOfGenes = c.Genes.Length;
            int CrossoverPoint1 = Utils.Rand.Next( NumberOfGenes );
            int CrossoverPoint2 = Utils.Rand.Next( NumberOfGenes );
            if ( CrossoverPoint1 > CrossoverPoint2 )
            {
                int tmp = CrossoverPoint1;
                CrossoverPoint1 = CrossoverPoint2;
                CrossoverPoint2 = tmp;
            }
 
            for (
                int i=CrossoverPoint1, j=CrossoverPoint2; 
                (i - CrossoverPoint1) < (CrossoverPoint2-CrossoverPoint1)/2;
                i++,j--
                )
            {
                Gene g = c.Genes[i];
                c.Genes[i] = c.Genes[j];
                c.Genes[j] = g;
            }
        }
    }

    /// <summary>
    /// Implements a swap reordering operator.
    /// </summary>
    public class Swap
    {
        /// <summary>
        /// Reorders the genes in the chromosome.
        /// </summary>
        /// <param name="c">The chromosome to be reordered.</param>
        public static void Reorder(Chromosome c)
        {
            //
            // Pick two genes and swap their position.
            //
            int NumberOfGenes;

            NumberOfGenes = c.Genes.Length;
            int g1 = Utils.Rand.Next( NumberOfGenes );
            int g2 = Utils.Rand.Next( NumberOfGenes );

            Gene tmp = c.Genes[g1];
            c.Genes[g1] = c.Genes[g2];
            c.Genes[g2] = tmp;
        }
    }
}