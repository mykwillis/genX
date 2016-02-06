using System;

namespace genX.Encoding
{
    /// <summary>
    /// Describes the constraints for a <B>BinaryGene</B>, and produces 
    /// <B>BinaryGene</B> objects based on those constraints.
    /// </summary>
    /// <remarks>
    /// Binary genes are by their nature extremely simple, and thus have
    /// no constraints.
    /// </remarks>    
    [Serializable]
    public class BinaryGeneDescriptor : GeneDescriptor
    {
        /// <summary>
        /// Returns a randomized <B>BinaryGene</B> object.
        /// </summary>
        override public Gene GetRandomAllele()
        {
            BinaryGene gene = new BinaryGene();
            gene.Value = genX.Utils.Rand.Next( 0, 2 ) == 1 ? true : false;
            return gene;
        }

        /// <summary>
        /// Represents the default BinaryGeneDescriptor.
        /// </summary>
        public static readonly BinaryGeneDescriptor Default = new BinaryGeneDescriptor();
    }


    /// <summary>
    /// Represents a gene with binary type.
    /// </summary>
    [Serializable]
    public class BinaryGene : Gene
    {
        /// <summary>
        /// Gets or sets this gene's value.
        /// </summary>
        public bool Value
        {
            get { return val; }
            set { val = value; }
        }
        private bool val;

        /// <summary>
        /// 
        /// </summary>
        public override object ObjectValue
        {
            get { return Value; }
        }

        /// <summary>
        /// Casts a BinaryGene to a bool.
        /// </summary>
        /// <param name="gene">The gene that is to be cast.</param>
        /// <returns>
        /// The value of BinaryGene.Value.
        /// </returns>
        public static implicit operator bool(BinaryGene gene)
        {
            return gene.val;
        }


        /// <summary>
        /// Gets or sets the GeneDescriptor that defines this gene's purpose.
        /// </summary>
        public new BinaryGeneDescriptor Descriptor
        {
            get { return (BinaryGeneDescriptor) base.Descriptor; }
            set { base.Descriptor = value; }
        }

        /// <summary>
        /// Creates an IntegerGene with default constraints and a random value.
        /// </summary>
        public BinaryGene() : this( BinaryGeneDescriptor.Default ){}

        /// <summary>
        /// Creates an BinaryGene with the specified constraints.
        /// </summary>
        public BinaryGene(BinaryGeneDescriptor descriptor) : base(descriptor)
        {            
            this.Descriptor = descriptor;            
        }

        /// <summary>
        /// Creates a BinaryGene with the specified constraints and an initial value.
        /// </summary>
        public BinaryGene(BinaryGeneDescriptor descriptor, bool val) : base(descriptor)
        {
            this.Descriptor = descriptor;
            this.Value = val;
        }

        /// <summary>
        /// Gets a string representation of this gene.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value ? "1" : "0";
        }
    }
}
