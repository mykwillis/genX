using System;
using System.ComponentModel;
using System.Collections;
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
    /// Specifies whether an objective function should be maximized or minimized.
    /// </summary>
    public enum ObjectiveType
    {
        /// <summary>
        /// Specifies that the objective represents a cost function, and should
        /// be minimized.
        /// </summary>
        MinimizeObjective,

        /// <summary>
        /// Specifies that the objective represents a profit function, and should
        /// be maximized.
        /// </summary>
        MaximizeObjective
    }

    /// <summary>
    /// Provides utility functions required throughout the code.
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// A common instance of <see cref="System.Random"/> to be used for
        /// obtaining random values.
        /// </summary>
        public static System.Random Rand = new System.Random();
    }

    /// <summary>
    /// Implements the primary control of the genetic algorithm's execution.
    /// </summary>
    [LicenseProvider(typeof(licX.LicXLicenseProvider))]
    [Designer(typeof(GADesigner))]
    public class GA : System.ComponentModel.Component
//    public class GA : System.Windows.Forms.UserControl
    {
        private int nextChromosomeID;

        /// <summary>
        /// Gets the current population of chromosomes.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Population Population
        {
            get { return population; }
            set { population = value; }
        }
        private Population population;        

        #region Delegates

        /// <summary>
        /// Gets or sets the object implementing the objective methods.
        /// </summary>
        /// <remarks>
        /// The objective object determines the RawObjective of each Chromosome
        /// each generation.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectiveDelegate Objective
        {
            get { return objective; }
            set { objective = value; }            
        }
        private ObjectiveDelegate objective;


        /// <summary>
        /// Gets or sets the object implementing the Scaler methods.
        /// </summary>
        /// <remarks>
        /// The Scaler object determines the Fitness of each Chromosome
        /// each generation, based on the Chromosome's NormalizedObjective.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScalingDelegate Scaler
        {
            get { return scaler; }
            set { scaler = value; }
        }
        private ScalingDelegate scaler;


        /// <summary>
        /// Gets or sets the object implementing the selector methods.
        /// </summary>
        /// <remarks>
        /// The selector object chooses the Chromosomes that are to participate
        /// in populating the next generation.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectionDelegate Selector
        {
            get { return selector; }
            set { selector = value; }
        }
        private SelectionDelegate selector;


        /// <summary>
        /// Gets or sets the object implementing the recombinator methods.
        /// </summary>
        /// <remarks>
        /// The recombinator object builds offspring Chromosomes from parents.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RecombinationDelegate Recombinator
        {
            get { return recombinator; }
            set { recombinator = value; }
        }
        private RecombinationDelegate recombinator;


        /// <summary>
        /// Gets or sets the function that is called when a gene is to be
        /// mutated.
        /// </summary>
        /// <remarks>
        /// <P>
        /// Each gene has a default mutation function specified by its
        /// associated GeneDescriptor.  In most cases, this default mutation
        /// function is appropriate.
        /// </P>
        /// <P>
        /// ValueMutator functions are generally built to handle mutation of
        /// a specific type of gene.  If the gene population is homogeneous
        /// (that is, all genes are of the same type), the <B>ValueMutator</B>
        /// property can safely be set to a mutator suitable for that type.
        /// </P>
        /// <P>
        /// If, however, the gene population is heterogeneous, you must be sure
        /// that the ValueMutator specified can deal appropriately with all gene
        /// types in the genome.
        /// </P>
        /// <P>
        /// Set this value to null to use the value mutator specified as the
        /// default for a gene.
        /// </P>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ValueMutationDelegate ValueMutator
        {
            get { return valueMutator; }
            set { valueMutator = value; }
        }
        private ValueMutationDelegate valueMutator;

#if NOTDEF
        /// <summary>
        /// Gets or sets the function that is called when a chromosome is to be
        /// reordered.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OrderMutationDelegate OrderMutator
        {
            get { return orderMutator; }
            set { orderMutator = value; }
        }
        private OrderMutationDelegate orderMutator;
#endif

        /// <summary>
        /// Gets or sets a value that specifies whether the algorithm will
        /// attempt to maximize or minimize the objective value.
        /// </summary>
        /// <remarks>
        /// If the objective function specified by <see cref="Objective"/> is
        /// a profit function, <B>ObjectiveType</B> should be set to
        /// <B>MaximizeObjective</B>.  If instead the objective function is a
        /// measure of cost, specify <B>MinimizeObjective</B> for the objective
        /// type.
        /// </remarks>        
        [Category("General"), DefaultValue(defaultObjectiveType)]
        public ObjectiveType ObjectiveType
        {
            get { return objectiveType; }
            set { objectiveType = value; }
        }
        private ObjectiveType objectiveType = defaultObjectiveType;
        private const ObjectiveType defaultObjectiveType = ObjectiveType.MaximizeObjective;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the probability of a mutation occuring on a gene.
        /// </summary>
        /// <remarks>
        /// The probability must be in the range [0,1.0].
        /// </remarks>        
        [Category("Mutation"),DefaultValue(DefaultGeneMutationProbability)]
        public double GeneMutationProbability
        {
            get { return geneMutationProbability; }
            set 
            { 
                if ( value < 0.0 || value > 1.0 )
                {
                    throw new ArgumentOutOfRangeException(
                        "GeneMutationProbability",
                        (object)value,
                        "Probabilities must be between 0.0 and 1.0 inclusive."
                        );
                }
                geneMutationProbability = value; 
            }
        }
        private double geneMutationProbability = DefaultGeneMutationProbability;
        /// <summary>
        /// Specifies the default gene mutation probability.
        /// </summary>
        public const double DefaultGeneMutationProbability = 0.005;

#if NOTDEF
        /// <summary>
        /// Gets or sets the probability of a mutation occuring in the
        /// order of genes on a chromosome.
        /// </summary>
        /// <remarks>
        /// The probability must be in the range [0,1.0].
        /// </remarks>        
        [Category("Mutation"),DefaultValue(DefaultOrderMutationProbability)]
        public double OrderMutationProbability
        {
            get { return orderMutationProbability; }
            set 
            { 
                if ( value < 0.0 || value > 1.0 )
                {
                    throw new ArgumentOutOfRangeException(
                        "OrderMutationProbability",
                        (object)value,
                        "Probabilities must be between 0.0 and 1.0 inclusive."
                        );
                }
                orderMutationProbability = value; 
            }
        }
        private double orderMutationProbability = DefaultOrderMutationProbability;
        /// <summary>
        /// Specifies the default gene mutation probability.
        /// </summary>
        public const double DefaultOrderMutationProbability = 0.005;
#endif

        /// <summary>
        /// Gets or sets the probability of recombination occuring.
        /// </summary>
        /// <remarks>
        /// The probability must be in the range [0,1.0].
        /// </remarks>        
        [Category("Recombination"),DefaultValue(DefaultRecombinationProbability)]
        public double RecombinationProbability
        {
            get { return recombinationProbability; }
            set 
            { 
                if ( value < 0.0 || value > 1.0 )
                {
                    throw new ArgumentOutOfRangeException(
                        "RecombinationProbability",
                        (object)value,
                        "Probabilities must be between 0.0 and 1.0 inclusive."
                        );
                }
                recombinationProbability = value; 
            }
        }
        private double recombinationProbability = DefaultRecombinationProbability;
        /// <summary>
        /// Specifies the default recombination probability.
        /// </summary>
        public const double DefaultRecombinationProbability = 0.7;

        /// <summary>
        /// Gets or sets the size of the population.
        /// </summary>
        [Category("General"),DefaultValue(defaultPopulationSize)]
        public int PopulationSize
        {
            get { return populationSize; }
            set { populationSize = value; }
        }
        private int populationSize = defaultPopulationSize;
        private const int defaultPopulationSize = 500;


        /// <summary>
        /// Gets or sets the maximum number of generations that the algorithm
        /// will be allowed to run.
        /// </summary>
        [Category("General"),DefaultValue(defaultMaxGenerations)]
        public int MaxGenerations
        {
            get { return maxGenerations; }
            set { maxGenerations = value; }
        }
        private int maxGenerations = defaultMaxGenerations;
        private const int defaultMaxGenerations = 2000;

        
#if NOTDEF
        /// <summary>
        /// Gets or sets a value that determines whether or not parents will
        /// be propogated to the next generation.
        /// </summary>
        /// <remarks>
        /// By default, parents selected for reproduction are not reintroduced
        /// into the next generation.  Setting this value to <B>true</B> causes
        /// the parents, as well as their children, to be added to the next
        /// generation.
        /// </remarks>
        [Category("Recombination"),DefaultValue(defaultPropogateParents)]
        public bool PropogateParents
        {
            get { return propogateParents; }
            set { propogateParents = value; }
        }
        private bool propogateParents;
        private const bool defaultPropogateParents = false;


        /// <summary>
        /// Gets or sets a value that determines whether propogated parents will
        /// have a chance of being mutated.
        /// </summary>
        /// <remarks>
        /// This property only has meaning if 
        /// <see cref="genX.GA.PropogateParents"/> is true.  In this case, this
        /// property determines whether or not parents will be subject to the same
        /// mutation rate and probability as 'normal' offspring.  If it is false,
        /// the selected parents will be copied verbatim from one generation to the 
        /// next.
        /// </remarks>
        [Category("Recombination"),DefaultValue(defaultMutatePropogatedParents)]
        public bool MutatePropogatedParents
        {
            get { return mutatePropogatedParents; }
            set { mutatePropogatedParents = value; }
        }
        private bool mutatePropogatedParents;
        private const bool defaultMutatePropogatedParents = false;
#endif        
        #endregion

        #region Statistics

        /// <summary>
        /// Gets the highest objective yet to be found.
        /// </summary>
        [Browsable(false)]        
        public double HighestObjective
        {   get { return highestObjective; } }
        private double highestObjective;


        /// <summary>
        /// Gets the lowest objective yet to be found.
        /// </summary>
        [Browsable(false)]
        public double LowestObjective
        {   get { return lowestObjective; } }
        private double lowestObjective;
		private licX.LicXLicenseComponent licXLicenseComponent1;


        /// <summary>
        /// Gets the best objective yet to be found.
        /// </summary>
        /// <remarks>
        /// The best objective may be either the HighestObjective or the
        /// LowestObjective, depending on whether or not IntertedObjective
        /// is true.
        /// </remarks>
        [Browsable(false)]
        public double BestObjective
        {
            get { return (objectiveType == ObjectiveType.MinimizeObjective) ? lowestObjective : highestObjective; }        
        }


        /// <summary>
        /// Gets the current generation.
        /// </summary>
        /// <remarks>
        /// The first generation is generation 0.
        /// </remarks>
        [Browsable(false)]
        public int Generation
        {   get { return generation; } }
        private int generation;

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when a gene is mutated.
        /// </summary>
        public event MutatedEventHandler Mutated;

        /// <summary>
        /// Event that is raised when a new population is ready to be tested.
        /// </summary>
        public event NewPopulationEventHandler NewPopulation;

        /// <summary>
        /// Event that is raised when the genetic algorithm is testing for 
        /// termination.
        /// </summary>
        public event TerminateEventHandler Terminate;

        /// <summary>
        /// Event that is raised when the objective for a chromosome is
        /// to be calculated.
        /// </summary>
        public event CalculateObjectiveEventHandler CalculateObjective;
        

        /// <summary>
        /// Raises the Mutated event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMutated(MutatedEventArgs e)
        {
            if ( Mutated != null )
            {
                Mutated(this, e);
            }
        }


        /// <summary>
        /// Raises the NewPopulation event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewPopulation(NewPopulationEventArgs e)
        {
            if ( NewPopulation != null )
            {
                NewPopulation(this, e);
            }
        }


        /// <summary>
        /// Raises the Terminate event.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual void OnTerminate(CancelEventArgs e)
        {
            if ( Terminate != null )
            {
                Terminate(this, e);
            }
        }

        private void InitializeComponent()
        {
			this.licXLicenseComponent1 = new licX.LicXLicenseComponent();
			// 
			// licXLicenseComponent1
			// 
			this.licXLicenseComponent1.ContactUrl = "http://www.wanderlust-software.com";
			this.licXLicenseComponent1.ExpirationDate = new System.DateTime(2003, 12, 31, 0, 0, 0, 0);
			this.licXLicenseComponent1.MasterLicensePassword = "genX License Password";
			this.licXLicenseComponent1.ProductName = "genX Genetic Algorithm Library";
			this.licXLicenseComponent1.VendorName = "Wanderlust Software, LLC";

		}


        /// <summary>
        /// Raises the CalculateObjective event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCalculateObjective(CalculateObjectiveEventArgs e)
        {
            if ( CalculateObjective != null )
            {
                CalculateObjective(this, e);
            }
        }

        #endregion
        
        #region Encoding

        /// <summary>
        /// Gets or sets the chromosome length (the number of genes in each
        /// chromosome).
        /// </summary>
        /// <remarks>
        /// The chromosome length may only be set with this property if the
        /// chromosomes being used are homogenous.  
        /// </remarks>
        [Category("Encoding"), DefaultValue(defaultChromosomeLength)]
        public int ChromosomeLength
        {
            get { return chromosomeLength; }
            set { chromosomeLength = value;}
        }
        private int chromosomeLength = defaultChromosomeLength;
        private const int defaultChromosomeLength = 50;       

#if NOTDEF
        /// <summary>
        /// Specifies whether the chromosomes are homogeneous.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Homogeneous
        {
            get { return homogeneous; }
//            set { homogeneous = value; }
        }
        bool homogeneous = defaultHomogeneous;
        const bool defaultHomogeneous = true;


        /// <summary>
        /// Gets or sets the gene descriptors.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GeneDescriptor[] GeneDescriptors
        {
            get { return geneDescriptors;   }
            set { geneDescriptors = value;  }
        }
        private GeneDescriptor[] geneDescriptors;
#endif
        /// <summary>
        /// Gets or sets the gene descriptors.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GeneDescriptor GeneDescriptor
        {
            get { return geneDescriptor;   }
            set { geneDescriptor = value;  }
        }
        private GeneDescriptor geneDescriptor;

        #endregion

        #region The Meat

        void Initialize()
        {
            Population = new Population( this, PopulationSize );
            
            for(int i=0;i<PopulationSize;i++)
            {
                Population.Chromosomes[i]       = new Chromosome();
                Population.Chromosomes[i].Genes = new Gene[ChromosomeLength];
                Population.Chromosomes[i].ID    = nextChromosomeID++;

                //GeneDescriptor geneDescriptor = geneDescriptors[0];
                bool[] usedLabels = new bool[ChromosomeLength];
                for(int j=0;j<ChromosomeLength;j++)
                {
//                    if ( !Homogeneous )
//                    {
//                        geneDescriptor = geneDescriptors[j];
//                    }
                    Population.Chromosomes[i].Genes[j] = geneDescriptor.GetRandomAllele();
//
                    int label;

                    do
                    {
                        label = Utils.Rand.Next(ChromosomeLength);
                    }
                    while ( usedLabels[label] );

                    usedLabels[label] = true;
                    Population.Chromosomes[i].Genes[j].Label = label;
                }                    
            }
        }
        
        void Evaluate()
        {
            //
            // Calculate the normalized objective value of each chromosome.  If we have
            // an objective value that is actually a measure of error (Inverted
            // objective), we negate it before passing it to the Scaler.
            //
            foreach(Chromosome c in Population.Chromosomes)
            {
                //
                // Give the Objective delegate first crack at supplying the objective,
                // then call any attached event handlers.  This way, an event handler
                // can be used to supply the RawObjective (which is friendly from a
                // visual designer point of view), or the delegate can be installed
                // manually to avoid the performance overhead of all of those events
                // being raised.
                //
                if ( Objective != null )
                {
                    c.RawObjective = Objective(c);
                }
                if ( CalculateObjective != null )
                {
                    CalculateObjectiveEventArgs e = new CalculateObjectiveEventArgs();
                    e.Chromosome = c;
                    OnCalculateObjective( e );
                }

                if ( c.RawObjective > highestObjective )
                {                    
                    highestObjective = c.RawObjective;
                }
                if ( c.RawObjective < lowestObjective )
                {
                    lowestObjective = c.RawObjective;
                }
            }

            double bias = (objectiveType == ObjectiveType.MinimizeObjective) ? Math.Abs(highestObjective) : Math.Abs(lowestObjective);
            foreach(Chromosome c in Population.Chromosomes)
            {
                //
                // If the objective is a measure of error, invert it.
                //
                c.NormalizedObjective = (objectiveType == ObjectiveType.MinimizeObjective) ? -c.RawObjective : c.RawObjective;

                //
                // Now add in the constant bias that forces all[*] values to 
                // be positive.
                //                
                c.NormalizedObjective += bias;
            }

            //
            // Sort the chromosomes by objective value.
            //
            Array.Sort( Population.Chromosomes, new Scaling.ObjectiveComparer() );            

            //
            // Now that all the objective values have been calculated,
            // determine the fitness of each individual.
            //
            Scaler(Population.Chromosomes);

            // TODO: Track the total fitness here so that it can be passed to
            // the selection routine.
        }


        Chromosome[] Select()
        {
            Chromosome[] parents = Selector(Population.Chromosomes, Population.PopulationSize);
            foreach(Chromosome c in parents)
            {
                c.SelectionCount++;
            }            
            return parents;
        }


        // There's at least two major problems here.
        // First, there's the problem that the Parents array is coming back such
        // that highly fit individuals will be consecutive in the array; it's essentially
        // a sorted array (at least in the case of StachasticRemainderSelectionWtihoutReplacement).
        // The real problem is that we allow chromosomes to mate with themselves to breed a
        // new generation.  This is dumb, I think.  We allow the recombination probability
        // to be less than 1.0 to allow for some individuals to make it through unscatche,
        // so when we do recombine, we have to make sure the parents are different.

        // I Fixed the selector to randomize the order of chromosomes.  The issue of
        // mating one chromosome with another remains.

        Chromosome[] Recombine(Chromosome[] Parents)
        {
            Chromosome[] children;
            Chromosome[] newPopulation = new Chromosome[PopulationSize];

            for(int i=0;i<PopulationSize-1;i++)
            {
                if ( Utils.Rand.NextDouble() < RecombinationProbability )
                {
                    // HACK: each parent reproduces at least once, but its mate
                    // HACK: is chosen from the others in the selection pool.
                    //Chromosome parent2 = Parents[ Utils.Rand.Next(PopulationSize) ];
                    // Uncomment this line to get previous behavior:
                    Chromosome parent2 = Parents[i+1];
                    
                    children = Recombinator(Parents[i], parent2);
                    Chromosome[] parents = new Chromosome[] { Parents[i], parent2 };
                    children[0].Parents[0] = Parents[i];
                    children[0].Parents[1] = parent2;
                    children[1].Parents[0] = Parents[i];
                    children[1].Parents[1] = parent2;
                }
                else
                {
                    children = new Chromosome[] { (Chromosome)Parents[i].Clone(), (Chromosome)Parents[i+1].Clone() };
                    children[0].Parents[0] = Parents[i];
                    children[0].Parents[1] = null;
                    children[1].Parents[0] = Parents[i+1];
                    children[1].Parents[1] = null;
                }
                newPopulation[i] = children[0];
                newPopulation[i+1] = children[1];
            }
            return newPopulation;
        }


        void Mutate(Chromosome[] cs)
        {
            foreach(Chromosome c in cs)
            {
                //
                // Mutate a gene's value in this chromosome.
                //
                if ( Utils.Rand.NextDouble() < GeneMutationProbability )
                {
                    int mutationPoint = Utils.Rand.Next( c.Genes.Length );
                    Gene gene = c.Genes[mutationPoint];

                    //
                    // A globally installed valuemutator takes precedence over the
                    // default gene mutator.
                    //
                    if ( ValueMutator != null )
                    {
                        c.Genes[mutationPoint] = ValueMutator(gene);
                    }
                    else
                    {
                        // this is the Custom case
                        c.Genes[mutationPoint] = gene.Descriptor.Mutate(gene);
                    }

                    //
                    // Raise the Mutated event.
                    //
                    MutatedEventArgs e = new MutatedEventArgs();
                    e.Chromosome = c;
                    e.MutationPoint = mutationPoint;
                    e.OldGene = gene;
                    e.NewGene = c.Genes[mutationPoint];

                    OnMutated( e );
                }
#if NOTDEF
                if ( Utils.Rand.NextDouble() < OrderMutationProbability )
                {
                    if ( OrderMutator != null )
                    {
                        OrderMutator(c);
                    }
                }
#endif
            }
        }


        bool Done()
        {
            if ( Generation > MaxGenerations )
            {
                return true;
            }

            CancelEventArgs e = new CancelEventArgs();
            OnTerminate(e);

            if ( e.Cancel )
            {
                return true;
            }

            return false;
        }

        void Replace(Chromosome[] children)
        {
            Population nextPopulation = new Population( this, population.PopulationSize );
            for(int i=0;i<populationSize;i++)
            {
                //
                // Since the recombination record for a chromosome may contain a
                // pointer to chromosome's in the previous population, it has to be
                // cleared out.  This prevents us from getting an unbroken reference
                // chain between the current population's chromosomes all the way back
                // to the first population (which would prevent the garbage collector
                // from reclaiming any chromosome memory).
                // (There is probably some way we can configure how deep we want
                // to remember recombination records.  But that might be overkill
                // if we get everything serializing nicely.)
                //
                population.Chromosomes[i].Parents = null;
                nextPopulation.Chromosomes[i] = children[i];
                nextPopulation.Chromosomes[i].ID = nextChromosomeID++;
            }            
            

            NewPopulationEventArgs e = new NewPopulationEventArgs();
            e.OldPopulation = population;
            e.NewPopulation = nextPopulation;
            e.Generation    = generation;
            OnNewPopulation( e );

            population = nextPopulation;    // BUGBUG: should move this before the call out
        }


        /// <summary>
        /// Runs the genetic algorithm for a single generation.
        /// </summary>
        public void Step()
        {
            Chromosome[] Parents;
            Chromosome[] Children;

            //
            // Repopulate the population, or initialize on first pass.
            //
            if ( Generation == 0 )  Initialize();
            else
            {
                //
                // Select parents.
                //
                Parents = Select();                    


                //
                // Produce offspring by recombination.
                //
                Children = Recombine( Parents );


                //
                // Mutate the offspring.
                //
                Mutate( Children );


                //
                // Replace the old population with the new.
                //
                Replace( Children );
            }
            
            generation++;


            //
            // Evaluate each of the chromosomes.
            //
            Evaluate();
        }


        /// <summary>
        /// Runs the genetic algorithm to completion.
        /// </summary>
        /// <remarks>
        /// The algorithm will be run continuously until one of the 
        /// termination criteria have been met, or until the number of elapsed
        /// generations exceeds the value set by <see cref="genX.GA.MaxGenerations"/>.
        /// </remarks>
        public void Run()
        {
            while ( !Done() )
            {
                Step();
            }
        }

        /// <summary>
        /// Runs the genetic algorithm for a given number of generations.
        /// </summary>
        /// <remarks>
        /// The algorithm will be run continuously until on of the following
        /// three things happen: the specified number of generations pass;
        /// the <see cref="MaxGenerations"/> has been exceeded, or at least 
        /// one termination event handler signals completion.
        /// </remarks>
        public void Run(int numberOfGenerations)
        {
            while( !Done() && (numberOfGenerations-- > 0) )
            {
                Step();
            }
        }

        #endregion

        License license;

        /// <summary>
        /// Creates a new GA object with default values.
        /// </summary>
        public GA()
        {
            InitializeComponent();

            LicenseManager.Validate( this.GetType(), this );

            Objective	= null;
            Scaler	    = new ScalingDelegate       ( new Scaling.LinearFitnessScaler().Scale );
            Selector	= new SelectionDelegate     ( Selection.RouletteSelector.Select );
            Recombinator= new RecombinationDelegate ( Recombination.SinglePointCrossover.Recombine );

            //geneDescriptors = new GeneDescriptor[1];
            //geneDescriptors[0] = new BinaryGeneDescriptor();
            geneDescriptor = new BinaryGeneDescriptor();

            highestObjective    = System.Int32.MinValue;
            lowestObjective     = System.Int32.MaxValue;

            license = LicenseManager.Validate(typeof(GA), this);

            encodingType    = defaultEncodingType;
            selectionMethod = defaultSelectionMethod;
            recombinationOperator = defaultRecombinationOperator;
            mutationOperator = defaultMutationOperator;
            //PreScaler
            fitnessScaling = defaultFitnessScaling;
        }

        #region Encoding

        private BinaryGeneDescriptor binaryGeneDescriptor   = new BinaryGeneDescriptor();
        private IntegerGeneDescriptor integerGeneDescriptor = new IntegerGeneDescriptor();
        private DoubleGeneDescriptor doubleGeneDescriptor   = new DoubleGeneDescriptor();

        /// <summary>
        /// Specifies the encoding type for the chromosome.
        /// </summary>
        [Category("Encoding"), DefaultValue(defaultEncodingType)]
        //[RefreshProperties(RefreshProperties.All)]    // doesn't seem to work...
        public EncodingType EncodingType
        {
            get { return encodingType; }
            set 
            {
                encodingType = value;
                switch(encodingType)
                {
                    case EncodingType.Binary:
                        geneDescriptor = binaryGeneDescriptor;
//                        homogeneous = true;
                        break;
                    case EncodingType.Integer:
                        geneDescriptor = integerGeneDescriptor;
//                        homogeneous = true;
                        break;
                    case EncodingType.Real:
                        geneDescriptor = doubleGeneDescriptor;
//                        homogeneous = true;
                        break;
                    case EncodingType.Custom:
//                        homogeneous = false;
                        break;
                    default:
                        break;
                }

                //
                // Thanks to Andrew Stevenson [andrewseven@hotmail.com], 18 
                // months after I posted a query online, it seems the solution for
                // selectively hiding attributes is to implement PreFilterProperties
                // and then to call TypeDescriptor.Refresh(this.Component) when
                // I want to change the set of visible attributes.
                //
            }
        }
        EncodingType encodingType;
        private const EncodingType defaultEncodingType = EncodingType.Binary;


        /// <summary>
        /// Specifies the minimum integer value used when <B>EncodingType</B>
        /// is <B>EncodingType.Integer</B>.
        /// </summary>
        [Category("Encoding"), DefaultValue(IntegerGeneDescriptor.DefaultMinValue)]
        public int MinIntValue
        {
            get { return integerGeneDescriptor.MinValue; }
            set { integerGeneDescriptor.MinValue = value; }
        }


        /// <summary>
        /// Specifies the maximum integer value used when <B>EncodingType</B>
        /// is <B>EncodingType.Integer</B>.
        /// </summary>
        [Category("Encoding"), DefaultValue(IntegerGeneDescriptor.DefaultMaxValue)]
        public int MaxIntValue
        {
            get { return integerGeneDescriptor.MaxValue; }
            set { integerGeneDescriptor.MaxValue = value; }
        }


        /// <summary>
        /// Specifies the minimum double value used when <B>EncodingType</B>
        /// is <B>EncodingType.Double</B>.
        /// </summary>
        [Category("Encoding"), DefaultValue(DoubleGeneDescriptor.DefaultMinValue)]
        public double MinDoubleValue
        {
            get { return doubleGeneDescriptor.MinValue; }
            set { doubleGeneDescriptor.MinValue = value; }
        }


        /// <summary>
        /// Specifies the maximum double value used when <B>EncodingType</B>
        /// is <B>EncodingType.Double</B>.
        /// </summary>
        [Category("Encoding"), DefaultValue(DoubleGeneDescriptor.DefaultMaxValue)]
        public double MaxDoubleValue
        {
            get { return doubleGeneDescriptor.MaxValue; }
            set { doubleGeneDescriptor.MaxValue = value; }
        }

        #endregion


        #region Selection

        
        //      private const int defaultTourSize = 10;

        //        private TournamentSelector tournamentSelector = new TournamentSelector();

        /// <summary>
        /// Specifies the selection method.
        /// </summary>        
        [Category("Selection"), DefaultValue(defaultSelectionMethod)]
        public SelectionMethod SelectionMethod
        {
            get { return selectionMethod; }
            set 
            { 
                selectionMethod = value; 
                switch(selectionMethod)
                {
                    case SelectionMethod.Roulette:
                        Selector = new SelectionDelegate(RouletteSelector.Select);
                        break;
                        //case SelectionMethod.Tournament:
                        //Selector = new SelectionDelegate(tournamentSelector.Select);
                        //break;
                    case SelectionMethod.StochasticRemainderWithoutReplacement:
                        Selector = new SelectionDelegate(StochasticRemainderSelectionWithoutReplacement.Select);
                        break;
                }
            }
        }
        SelectionMethod selectionMethod;
        private const SelectionMethod defaultSelectionMethod = SelectionMethod.Roulette;

#if NOTDEF
        /// <summary>
        /// Specifies the size of the tour when <see cref="SelectionMethod"/>
        /// is <see cref="SelectionMethod.Tournament"/>.
        /// </summary>
        [
        Category("Selection"), DefaultValue(defaultTourSize),
        Description("Specifies the size of the tour when tournament selection is used.")
        ]
        public int TourSize
        {
            get { return tournamentSelector.TourSize; }
            set { tournamentSelector.TourSize = value; }
        }
        int tourSize = defaultTourSize;
#endif
        #endregion

        
        #region Recombination
            
        /// <summary>
        /// Specifies the recombination method to use.
        /// </summary>        
        [Category("Recombination"), DefaultValue(defaultRecombinationOperator)]
        public RecombinationOperator RecombinationOperator
        {
            get { return recombinationOperator; }
            set 
            { 
                recombinationOperator = value; 
                switch(recombinationOperator)
                {
                    case RecombinationOperator.SinglePointCrossover:
                        Recombinator = new RecombinationDelegate( Recombination.SinglePointCrossover.Recombine );
                        break;
                    case RecombinationOperator.TwoPointCrossover:
                        Recombinator = new RecombinationDelegate( Recombination.TwoPointCrossover.Recombine );
                        break;
                    case RecombinationOperator.Uniform:
                        Recombinator = new RecombinationDelegate( Recombination.UniformCrossover.Recombine );
                        break;
                }
            }
        }
        RecombinationOperator recombinationOperator;
        private const RecombinationOperator defaultRecombinationOperator = RecombinationOperator.SinglePointCrossover;

        #endregion


        #region Mutation

        /// <summary>
        /// Specifies the mutation operator to use.
        /// </summary>
        [Category("Mutation"), DefaultValue(defaultMutationOperator)]
        public MutationOperator MutationOperator
        {
            get { return mutationOperator; }
            set 
            { 
                mutationOperator = value; 
                switch(mutationOperator)
                {
                    case MutationOperator.Bitwise:
                        ValueMutator = new ValueMutationDelegate( BitwiseMutation.Mutate );
                        break;
                    case MutationOperator.Boundary:
                        ValueMutator = new ValueMutationDelegate( BoundaryMutation.Mutate );
                        break;
                        //                    case MutationOperator.Uniform:
                        //                        break;
                    case MutationOperator.GeneSpecific:
                        ValueMutator = null;
                        break;
                }
            }
        }
        MutationOperator mutationOperator;
        private const MutationOperator defaultMutationOperator = MutationOperator.Bitwise;

        #endregion


        #region Scaling


        LinearFitnessScaler linearFitnessScaler = new LinearFitnessScaler();
        LinearRankedFitnessScaler linearRankedFitnessScaler = new LinearRankedFitnessScaler();

#if NOTDEF
        /// <summary>
        /// Specifies a pre-scaling operator.
        /// </summary>
        [Category("Scaling"), DefaultValue(defaultPreScaling)]
        public PreScaling PreScaling
        {
            get { return preScaling; }
            set 
            { 
                preScaling = value;
                switch(preScaling)
                {
                    case PreScaling.PowerLaw:
                        break;
                    case PreScaling.SigmaTrunctation:
                        break;
                    case PreScaling.None:
                        break;                        
                }
            }
        }
        PreScaling preScaling;
        private const PreScaling defaultPreScaling = PreScaling.None;
#endif

        /// <summary>
        /// Specifies a scaling operator.
        /// </summary>
        [Category("Scaling"), DefaultValue(defaultFitnessScaling)]
        public FitnessScaling FitnessScaling
        {
            get { return fitnessScaling; }
            set 
            {
                fitnessScaling = value;
                switch(fitnessScaling)
                {
                    case FitnessScaling.Linear:
                        Scaler = new ScalingDelegate( linearFitnessScaler.Scale );                        
                        break;
                    case FitnessScaling.LinearRanked:
                        Scaler = new ScalingDelegate( linearRankedFitnessScaler.Scale );
                        break;
                    case FitnessScaling.Proportional:
                        Scaler = new ScalingDelegate( ProportionalFitnessScaler.Scale );
                        break;
                }
            }
        }
        FitnessScaling fitnessScaling;
        private const FitnessScaling defaultFitnessScaling = FitnessScaling.Proportional;

        /// <summary>
        /// Specifies the fitness multiple for linear fitness scaling.
        /// </summary>
        [
        Category("Scaling"), DefaultValue(LinearFitnessScaler.DefaultFitnessMultiple),
        Description("Specifies the fitness multiple to use when linear fitness scaling is used.")
        ]
        public double FitnessMultiple
        {
            get { return linearFitnessScaler.FitnessMultiple; }
            set { linearFitnessScaler.FitnessMultiple = value; }
        }


        /// <summary>
        /// Specifies the selective pressure for linear ranked fitness scaling.
        /// </summary>
        [
        Category("Scaling"), DefaultValue(LinearRankedFitnessScaler.DefaultSelectivePressure),
        Description("Specifies the selective pressure to use when linear ranked fitness scaling is used.")
        ]
        public double SelectivePressure
        {
            get { return linearRankedFitnessScaler.SelectivePressure; }
            set { linearRankedFitnessScaler.SelectivePressure = value; }
        }

        #endregion
	}
}

