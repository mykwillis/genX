using System;
using System.ComponentModel;
using genX;
using genX.Encoding;
using genX.Termination;

//
// Solutions to the 1D objective will be tours that travel back and forth
// across the cities exactly once, i.e., they go from one side to the other
// and back again.  It doesn't matter which cities they visit in each direction,
// so long as they only back-track once.
//

namespace TravelingSalesman
{
    public struct Point
    {
        int x;
        int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public double Distance(Point p)
        {
            return Math.Sqrt( (x-p.x)*(x-p.x) + (y-p.y)*(y-p.y) );
        }
    }

    class TravelingSalesman2dObjective
    {
        Point[] cities;
        public TravelingSalesman2dObjective(Point[] cities)
        {
            this.cities = cities;
        }

        public double GetObjective(Chromosome c)
        {
            double distance = 0;
            Point currentPosition = cities[0];
            Point initialPosition = cities[0];
            for(int i=0;i<c.Genes.Length;i++)
            {
                Gene g = c.Genes[i];
                if ( i==0 )
                {
                    initialPosition = cities[g.Label];
                    currentPosition = cities[g.Label];
                }
                else
                {
                    distance += cities[g.Label].Distance(currentPosition);
                    currentPosition = cities[g.Label];
                }
            }
            Gene lastGene = c.Genes[c.Genes.Length-1];
            distance += cities[lastGene.Label].Distance(initialPosition);

            return distance;
        }
    }


    class TravellingSalesman1dObjective
    {
        int[] cities;

        public TravellingSalesman1dObjective(int[] cities)
        {
            this.cities = cities;
        }

        public double GetObjective(Chromosome c)
        {
            double distance = 0;
            int currentPosition = 0;
            int initialPosition = 0;
            for(int i=0;i<c.Genes.Length;i++)
            {
                IntegerGene ig = (IntegerGene) c.Genes[i];
                if ( i==0 )
                {
                    initialPosition = cities[ig.Label];
                    currentPosition = cities[ig.Label];
                }
                else
                {
                    distance += Math.Abs( cities[ig.Label] - currentPosition );
                    currentPosition = cities[ig.Label];
                }
            }
            IntegerGene lastGene = (IntegerGene) c.Genes[c.Genes.Length-1];
            distance += Math.Abs( cities[lastGene.Label] - initialPosition );

            return distance;
        }
    }


    class GaTest
    {
        static int gen=0;
        static void OnNewPopulation(object o, NewPopulationEventArgs e)
        {
            //Console.WriteLine("Gen {0}: {1}", gen++, e.OldPopulation.Summary.BestChromosome);
            Console.WriteLine(e.OldPopulation.Summary);
        }

        static void Main(string[] args)
        {
            GA Ga;

            TravellingSalesman1dObjective Objective;
            TravelingSalesman2dObjective Objective2;

            int[] cities = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 1400, 15000};
            Objective = new TravellingSalesman1dObjective(cities);

        
            //
            // Create the objective.
            //
            System.Collections.ArrayList cities2 = new System.Collections.ArrayList();
            //System.IO.StreamReader sr = System.IO.File.OpenText("../../xpf131.tsp");
            System.IO.StreamReader sr = System.IO.File.OpenText("../../xit1083.tsp");
            string s;
            bool gotime = false;
            s = sr.ReadLine();
            while ( s != null )
            {
                s.Trim();

                if ( !gotime )
                {
                    if (s == "NODE_COORD_SECTION" )
                    {
                        gotime = true;
                    }
                    s = sr.ReadLine();
                    continue;
                }
                if ( s == "EOF" )
                {
                    break;
                }
                
                string[] sa = s.Split(' ', '\t');
                if ( sa.Length != 3 )
                {
                    throw new Exception();
                }
                int city = Int32.Parse(sa[0]);
                int x = Int32.Parse(sa[1]);
                int y = Int32.Parse(sa[2]);

                cities2.Add( new Point(x, y) );
                s = sr.ReadLine();
            }
            sr.Close();

            //Point[] cities2 = new Point[] { };
            Objective2 = new TravelingSalesman2dObjective( (Point[]) cities2.ToArray(typeof(Point)));        


            //
            // Create the mutator
            //
            

            Ga = new GA();
            Ga.GeneDescriptors = new GeneDescriptor[] { new IntegerGeneDescriptor() };
            Ga.Homogeneous = true;
            //Ga.ChromosomeLength = cities.Length;
            Ga.ChromosomeLength = cities2.Count;

            Ga.Recombinator = new RecombinationDelegate( new genX.Recombination.PartiallyMatchedCrossover().Recombine );
            Ga.OrderMutator = new OrderMutationDelegate( genX.Reordering.Swap.Reorder );
            Ga.ValueMutator = null;
            Ga.Scaler = new ScalingDelegate( new genX.Scaling.LinearRankedFitnessScaler(1.6).Scale );
            Ga.RecombinationProbability = 0.7;

            Ga.PopulationSize       = 500;
//            Ga.Objective            = new ObjectiveDelegate( Objective.GetObjective );
            Ga.Objective            = new ObjectiveDelegate( Objective2.GetObjective );
            Ga.ObjectiveType        = ObjectiveType.MinimizeObjective;
            Ga.MaxGenerations       = 1000000;

            Ga.NewPopulation += new NewPopulationEventHandler( OnNewPopulation );

            Ga.Run();

            Console.WriteLine("Best Individual:");
            Console.WriteLine( Ga.Population.Summary.BestChromosome );
            Console.WriteLine("Finished.  Hit return.");
            Console.ReadLine();
        }
    }
}