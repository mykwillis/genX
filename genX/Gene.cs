using System;

namespace genX
{
    /// <summary>
    /// Represents a single gene on a chromosome.
    /// </summary>
    [Serializable]
    abstract public class Gene : ICloneable
    {
        /// <summary>
        /// Gets or sets the GeneDescriptor that defines this gene's purpose.
        /// </summary>
        public GeneDescriptor Descriptor
        {
            get { return descriptor; }
            set { descriptor = value; }
        }
        private GeneDescriptor descriptor;

        /// <summary>
        /// Gets or sets this gene's label.
        /// </summary>
        public int Label
        {
            get { return label; }
            set { label = value; }
        }
        int label;

        /// <summary>
        /// Gets this gene's value as an Object.
        /// </summary>
        public abstract object ObjectValue
        {
            get;
        }

        /// <summary>
        /// Creates a new Gene.
        /// </summary>
        public Gene(GeneDescriptor geneDescriptor) : this(geneDescriptor, 0) {}

        /// <summary>
        /// Creates a new gene.
        /// </summary>
        public Gene(GeneDescriptor geneDescriptor, int label)
        {
            this.Descriptor = geneDescriptor;
            this.label = label;
        }

        /// <summary>
        /// Creates a copy of the Gene that copies all values appropriately.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();            
        }
    }


    /// <summary>
    /// Describes a Gene.
    /// </summary>
    [Serializable]
    public abstract class GeneDescriptor
    {
        /// <summary>
        /// Gets or sets the gene's name.
        /// </summary>
        public string Name
        {   
            get { return name; }
            set { name = value; }
        }
        private string name;


        /// <summary>
        /// Gets or sets a description of this gene.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private string description;


        /// <summary>
        /// Randomizes the gene's value.
        /// </summary>
        public abstract Gene GetRandomAllele();   // implicitly virtual


        /// <summary>
        /// Gets or sets the default mutator for this gene.
        /// </summary>
        public ValueMutationDelegate Mutator
        {
            get { return mutator; }
            set { mutator = value; }
        }
        private ValueMutationDelegate mutator;


        /// <summary>
        /// Mutates a gene.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual Gene Mutate(Gene g)
        {
            if ( Mutator != null )
            {
                return Mutator(g);
            }
            else
            {
                Gene g2 = GetRandomAllele();
                g2.Label = g.Label;
                return g2;
            }
        }
    }




}
