using System;
using System.Collections;
using genX;
using genX.Encoding;

//
// There are three primary types of mutation:
//  value mutation   - changes to a gene's value (randomize, bitflip, etc.)
//  order mutation  - changes to the order of genes (swap, invert, reorder)
//  label mutation  - changes to a gene's purpose.  (swap_label, invert_label, reorder_labels, change_label_value)

namespace genX.Mutation
{
    /// <summary>
    /// Implements bitwise mutation.
    /// </summary>
    public class BitwiseMutation
    {
        /// <summary>
        /// Mutates a gene by bitwise mutation.
        /// </summary>
        /// <param name="gene"></param>
        /// <returns></returns>
        public static Gene Mutate(Gene gene)
        {
            if ( gene is BinaryGene )
            {
                BinaryGene g = (BinaryGene) gene.Clone();
                g.Value = !(BinaryGene)gene;
                return g;
            }
            else if ( gene is DoubleGene )
            {
                DoubleGene g = (DoubleGene) gene.Clone();
                byte[] bytes = BitConverter.GetBytes(g.Value);
                BitArray ba = new BitArray(bytes);
                int p = Utils.Rand.Next( ba.Length );
                ba.Set(p, !ba[p]);
                ba.CopyTo(bytes, 0);
                g.Value = BitConverter.ToDouble(bytes,0);
                return g;
            }
            else if ( gene is IntegerGene )
            {
                IntegerGene g = (IntegerGene) gene.Clone();
                byte[] bytes = BitConverter.GetBytes(g.Value);
                BitArray ba = new BitArray(bytes);
                int p = Utils.Rand.Next( ba.Length );
                ba.Set(p, !ba[p]);
                ba.CopyTo(bytes, 0);
                g.Value = BitConverter.ToInt32(bytes,0);
                return g;
            }
            return (Gene) gene.Clone(); // default
        }
    }


    /// <summary>
    /// Implements boundary mutation.
    /// </summary>
    public class BoundaryMutation
    {
        /// <summary>
        /// Mutates a gene by applying boundary mutation.
        /// </summary>
        /// <param name="gene">An IntegerGene or DoubleGene to be mutated.</param>
        /// <returns></returns>
        public static Gene Mutate(Gene gene)
        {
            if ( gene is IntegerGene )
            {
                IntegerGene ig = (IntegerGene)gene;
                if ( genX.Utils.Rand.NextDouble() < 0.5 )
                {
                    ig.Value = ig.Descriptor.MaxValue;
                }
                else
                {
                    ig.Value = ig.Descriptor.MinValue;
                }
            }
            else if ( gene is DoubleGene )
            {
                DoubleGene dg = (DoubleGene)gene;
                if ( genX.Utils.Rand.NextDouble() < 0.5 )
                {
                    dg.Value = dg.Descriptor.MaxValue;
                }
                else
                {
                    dg.Value = dg.Descriptor.MinValue;
                }
            }
            else
            {
                throw new ArgumentException(
                    "Boundary mutation may only apply to IntegerGene or DoubleGene genes.",
                    "gene"
                    );
            }
            return gene;
        }
    }
}