using System;
using genX;

namespace genX.Encoding
{
    /// <summary>
    /// Describes the constraints for a <B>DoubleGene</B>, and produces 
    /// <B>DoubleGene</B> objects based on those constraints.
    /// </summary>
    /// <remarks>
    /// The <see cref="MinValue"/> and <see cref="MaxValue"/> properties
    /// specify the range of values that may be taken on by double gene
    /// alleles.
    /// </remarks>
    public class DoubleGeneDescriptor : GeneDescriptor
    {
        /// <summary>
        /// The default minimum value.
        /// </summary>
        public const double DefaultMinValue = -1000;

        /// <summary>
        /// The default maximum value.
        /// </summary>
        public const double DefaultMaxValue = 1000;

        /// <summary>
        /// Gets or sets the minimum value that may be taken on by a gene.
        /// </summary>
        public double MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        private double minValue;

        /// <summary>
        /// Gets or sets the maximum value that may be taken on by a gene.
        /// </summary>
        public double MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
        private double maxValue;

        /// <summary>
        /// Creates a new IntegerGeneConstraints with the default values.
        /// </summary>
        public DoubleGeneDescriptor() : this(DefaultMinValue, DefaultMaxValue){}

        /// <summary>
        /// Creates a new IntegerGeneConstraints with the given minimum and
        /// maximum values.
        /// </summary>
        public DoubleGeneDescriptor(double minValue, double maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        /// <summary>
        /// Returns a random allele based on the constraints.
        /// </summary>
        override public Gene GetRandomAllele()
        {
            DoubleGene gene = new DoubleGene(this);            
            double newDouble;
            newDouble = genX.Utils.Rand.NextDouble();
            newDouble *= (MaxValue - MinValue);
            newDouble += MinValue;
            gene.Value = newDouble;
            return gene;
        }

        /// <summary>
        /// Represents the default IntegerGeneConstraints.
        /// </summary>
        public static readonly DoubleGeneDescriptor Default = new DoubleGeneDescriptor();
    }


    /// <summary>
    /// Represents a gene with integer type.
    /// </summary>
    public class DoubleGene : Gene
    {
        /// <summary>
        /// Gets or sets this gene's value.
        /// </summary>
        public double Value
        {
            get { return val; }
            set { val = value; }
        }
        private double val;

        
        /// <summary>
        /// 
        /// </summary>
        public override object ObjectValue
        {
            get { return Value; }
        }


        /// <summary>
        /// Gets or sets the GeneDescriptor that defines this gene's purpose.
        /// </summary>
        public new DoubleGeneDescriptor Descriptor
        {
            get { return (DoubleGeneDescriptor) base.Descriptor; }
            set { base.Descriptor = value; }
        }


        /// <summary>
        /// Creates an IntegerGene with default constraints and a random value.
        /// </summary>
        public DoubleGene() : this( DoubleGeneDescriptor.Default ){}


        /// <summary>
        /// Creates an IntegerGene with the specified constraints.
        /// </summary>
        public DoubleGene(DoubleGeneDescriptor descriptor) : base(descriptor)
        {            
            this.Descriptor = descriptor;            
        }


        /// <summary>
        /// Creates an IntegerGene with the specified constraints and an initial value.
        /// </summary>
        public DoubleGene(DoubleGeneDescriptor descriptor, int value) : base(descriptor)
        {
            this.Descriptor = descriptor;
            this.Value = value;
        }


        /// <summary>
        /// Gets a string representation of this Gene.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
