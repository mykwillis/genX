using System;
using genX;

namespace genX.Encoding
{
    /// <summary>
    /// Describes the constraints for an <B>IntegerGene</B>, and produces 
    /// <B>IntegerGene</B> objects based on those constraints.
    /// </summary>
    /// <remarks>
    /// The <see cref="MinValue"/> and <see cref="MaxValue"/> properties
    /// specify the range of values that may be taken on by integer gene
    /// alleles.
    /// </remarks>
    public class IntegerGeneDescriptor : GeneDescriptor
    {
        /// <summary>
        /// The default minimum value.
        /// </summary>
        public const int DefaultMinValue = -1000;

        /// <summary>
        /// The default maximum value.
        /// </summary>
        public const int DefaultMaxValue = 1000;

        
        /// <summary>
        /// Gets or sets the minimum value that may be taken on by a gene.
        /// </summary>
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        private int minValue;


        /// <summary>
        /// Gets or sets the maximum value that may be taken on by a gene.
        /// </summary>
        public int MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
        private int maxValue;


        /// <summary>
        /// Creates a new <B>IntegerGeneConstraints</B> with default property 
        /// values.
        /// </summary>
        public IntegerGeneDescriptor() : this(DefaultMinValue, DefaultMaxValue){}


        /// <summary>
        /// Creates a new <B>IntegerGeneConstraints</B> with the given minimum 
        /// and maximum values.
        /// </summary>
        public IntegerGeneDescriptor(int minValue, int maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }


        /// <summary>
        /// Returns a random allele based on the constraints.
        /// </summary>
        override public Gene GetRandomAllele()
        {
            IntegerGene gene = new IntegerGene(this);
            gene.Value = genX.Utils.Rand.Next( MinValue, MaxValue+1 );
            return gene;
        }

        /// <summary>
        /// Represents the default IntegerGeneConstraints.
        /// </summary>
        public static readonly IntegerGeneDescriptor Default = new IntegerGeneDescriptor();

    }


    /// <summary>
    /// Represents a gene with integer type.
    /// </summary>
    public class IntegerGene : Gene
    {
        /// <summary>
        /// Gets or sets this gene's value.
        /// </summary>
        public int Value
        {
            get { return val; }
            set { val = value; }
        }
        private int val;

        /// <summary>
        /// Gets the IntegerGene's value as an object.
        /// </summary>
        /// <remarks>
        /// This property exists to allow the object value to be accessible
        /// to code written against the Gene object.  Code written against
        /// IntegerGene should use the Value property to avoid casts.
        /// </remarks>
        public override object ObjectValue
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets or sets the GeneDescriptor that defines this gene's purpose.
        /// </summary>
        public new IntegerGeneDescriptor Descriptor
        {
            get { return (IntegerGeneDescriptor) base.Descriptor; }
            set { base.Descriptor = value; }
        }


        /// <summary>
        /// Creates an IntegerGene with default constraints and a random value.
        /// </summary>
        public IntegerGene() : this( IntegerGeneDescriptor.Default ){}


        /// <summary>
        /// Creates an IntegerGene with the specified constraints.
        /// </summary>
        public IntegerGene(IntegerGeneDescriptor descriptor) : base(descriptor)
        {            
            this.Descriptor = descriptor;            
        }


        /// <summary>
        /// Creates an IntegerGene with the specified constraints and an initial value.
        /// </summary>
        public IntegerGene(IntegerGeneDescriptor descriptor, int value) : base(descriptor)
        {
            this.Descriptor = descriptor;
            this.Value = value;
        }


        /// <summary>
        /// Gets a string representation of this gene.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
