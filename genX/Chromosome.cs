using System;
using genX;

namespace genX
{
    class LabelComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            Gene g1, g2;
            g1 = (Gene) x;
            g2 = (Gene) y;

            if ( g1.Label > g2.Label )      return 1;
            else if ( g1.Label < g2.Label ) return -1;
            else                            return 0;
        }
    }

    /// <summary>
    /// Represents a single chromosome in the population.
    /// </summary>
    [Serializable]
    public class Chromosome : ICloneable
    {
        /// <summary>
        /// Gets or sets the genes that make up the chromosome.
        /// </summary>
        public Gene[] Genes
        {
            get { return genes; }
            set { genes = value; }
        }
        private Gene[] genes;


        /// <summary>
        /// GenesByLabel
        /// </summary>
        public Gene[] GenesByLabel
        {
            get
            {
                Gene[] genesByLabel = new Gene[genes.Length];
                System.Array.Copy(genes, genesByLabel, genes.Length);
                System.Array.Sort(genesByLabel, new LabelComparer());                
                return genesByLabel;
            }
        }


        /// <summary>
        /// Gets the gene at the specified index.
        /// </summary>
        public Gene this[int i]
        {
            get { return genes[i]; }
            set { genes[i] = value; }
        }


        /// <summary>
        /// Initializes an empty chromosome with default values.
        /// </summary>
        /// <remarks>
        /// You must set the Genes property of the chromosome before it may
        /// be used.
        /// </remarks>
        public Chromosome() : this(null){}


        /// <summary>
        /// Initializes a chromosome with a given set of genes.
        /// </summary>
        /// <param name="genes">
        /// An array of Genes with which to initailize.  Each Gene in the array
        /// may be of any derived type.
        /// </param>
        public Chromosome(Gene[] genes)
        {
            this.Genes = genes;
            this.Parents = new Chromosome[2];
        }


        /// <summary>
        /// Specifies the crossoverpoint used with single point crossover.
        /// </summary>
        public int CrossoverPoint;  // BUGBUG: temp


        /// <summary>
        /// Gets the normalized objective value for this Chromosome.
        /// </summary>
        /// <remarks>
        /// <P>
        /// The <B>NormalizedObjective</B> property represents a normalized representation
        /// of how well a chromosome performed in the objective test.  While the
        /// <see href="genX.Chromosome.RawObjective" /> property may be negative
        /// or represent an error, <B>NormalizedObjective</B> is a "higher is better" non-
        /// negative value.
        /// </P>
        /// <P>
        /// Using a normalized objective value allows algorithms built to crafted
        /// more cleanly by dealing only with "higher is better" non-negative
        /// objectives.
        /// </P>
        /// <P>
        /// In general, a normalized objective value in one generation cannot be 
        /// compared directly to one in another generation, because the normalization
        /// is done specific to a generation; i.e., a NormalizedObjective of 20 in
        /// generation 1 might actually represent a more fit chromosome than one that
        /// has a NormalizedObjective of 30 in generation 3.
        /// </P>
        /// </remarks>
        public double NormalizedObjective
        {
            get { return normalizedObjective; }
            set { normalizedObjective = value; }
        }
        private double normalizedObjective;


        /// <summary>
        /// Gets or sets the chromosome's raw objective value.
        /// </summary>
        /// <remarks>
        /// <P>
        /// A chromosome's RawObjective is the unmodified value returned from 
        /// the objective function.  Depending on the implementation of the 
        /// objective function, this value may be positive or negative, and may
        /// represent either a measure of fitness or a measure of error for
        /// the chromosome.
        /// </P>
        /// <P>
        /// <B>RawObjective</B> is not used in the framework except for reporting
        /// purposes.  Instead, <see href="NormalizedObjective"/>
        /// is used, which represents a normalized expression of objective.
        /// </P>
        /// </remarks>
        public double RawObjective
        {
            get { return rawObjective; }
            set { rawObjective = value; }
        }
        private double rawObjective;
        

        /// <summary>
        /// Gets or sets the fitness value of this Chromosome.
        /// </summary>
        /// <remarks>
        /// <P>
        /// The fitness value is a typically a scaled function of this chromosome's
        /// <see cref="NormalizedObjective"/> value relative to the other
        /// chromosomes in the population.
        /// </P>
        /// <P>
        /// The Fitness value is supplied by the Scaler method, as opposed to the
        /// Objective method that furnishes the raw objective score for the chromosome.
        /// While Objective methods are typically specific to the problem being solved
        /// and representation used, the Fitness is typically calculated independently.
        /// </P>
        /// <P>
        /// Fitness represents the relative likelihood that this chromosome 
        /// will reproducing to form offspring in the next generation.  The
        /// fitness value may be any non-negative value.
        /// </P>
        /// </remarks>
        public double Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }
        private double fitness;


        /// <summary>
        /// Gets or sets a unique identifier for this chromosome.
        /// </summary>
        /// <remarks>
        /// The <B>ID</B> field is unique among all chromosomes, regardless of
        /// population.  
        /// </remarks>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        private int id;


        /// <summary>
        /// Gets or sets the number of times this chromosome was selected as
        /// a parent.
        /// </summary>
        public int SelectionCount
        {
            get { return selectionCount; }
            set { selectionCount = value; }
        }
        private int selectionCount;


        /// <summary>
        /// Provides information about this chromosome's parents and
        /// recombination method.
        /// </summary>
        public Chromosome[] Parents
        {
            get { return parents; }
            set { parents = value; }
        }
        private Chromosome[] parents;


        /// <summary>
        /// Creates a deep copy of the Chromosome.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            Chromosome c = (Chromosome) this.MemberwiseClone();
            c.Genes = new Gene[Genes.Length];
            for(int i=0;i<Genes.Length;i++)
            {
                c.Genes[i] = (Gene) Genes[i].Clone();
            }
            return c;
        }


        /// <summary>
        /// Returns this <B>Chromosome</B> instance as a string.
        /// </summary>
        /// <returns>
        /// Returns a string that contains each of the Genes in the chromosome
        /// seperated by a space.
        /// </returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach(Gene g in Genes)
            {
                //sb.Append("[" + g.Label + "]");
                sb.Append(g.ToString());
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }

#if NOTDEF
    public class ChromosomeDescriptor
    {
        System.Collections.ArrayList genes;
        bool mobileGenes;

        public void AddGene(GeneDescriptor geneDescriptor)
        {
            genes.Add(geneDescriptor);
        }
    }
#endif
}
