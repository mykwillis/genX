using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using genX;
using genX.Encoding;
using genX.Termination;


namespace TravelingSalesmanWin
{
    public class TspLibDocument
    {
        public Point[] cities;

        public int maxX;
        public int maxY;

        public TspLibDocument(string fileName)
        {
            //
            // Create the objective.
            //
            System.Collections.ArrayList cities2 = new System.Collections.ArrayList();
            System.IO.StreamReader sr = System.IO.File.OpenText(fileName);

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

                if ( x > maxX ) maxX = x;
                if ( y > maxY ) maxY = y;

                s = sr.ReadLine();
            }
            sr.Close();

            cities = (Point[]) cities2.ToArray(typeof(Point));                
        }
    }

	public class Form1 : System.Windows.Forms.Form
	{
        GA Ga;
        Point[] cities;

        int maxX=0, maxY=0;        
        int BorderSize = 5;
        private System.Windows.Forms.Panel graphPanel;
        private AxMSChart20Lib.AxMSChart axMSChart1;
        private genX.GA ga1;

        private System.Timers.Timer timer1;

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt( (p2.X-p1.X)*(p2.X-p1.X) + (p2.Y-p1.Y)*(p2.Y-p1.Y) );
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
                    distance += Distance(cities[g.Label], currentPosition);
                    currentPosition = cities[g.Label];
                }
            }
            Gene lastGene = c.Genes[c.Genes.Length-1];
            distance += Distance(cities[lastGene.Label],initialPosition);

            return distance;
        }

        genX.PopulationSummary currentPopulationSummary;
        short generation=0;
        void OnNewPopulation(object o, NewPopulationEventArgs e)
        {
            //System.Threading.Monitor.Enter(this);
            currentPopulationSummary = e.OldPopulation.Summary;
            //System.Threading.Monitor.Exit(this);

            axMSChart1.RowCount++;
            axMSChart1.DataGrid.SetData(
                ++generation, 
                1, 
                currentPopulationSummary.BestChromosome.RawObjective, 
                0
                );
            
            graphPanel.Invalidate();            
        }

        void gaThread()
        {
            Ga.Run();
        }

        void InitializeGa()
        {        
            TspLibDocument doc = new TspLibDocument("../../../TravellingSalesman/xpf131.tsp");
//            TspLibDocument doc = new TspLibDocument("../../../TravellingSalesman/xit1083.tsp");
            //TspLibDocument doc = new TspLibDocument("../../../TravellingSalesman/tspbayg29.tsp");
            cities = doc.cities;
            maxX = doc.maxX;
            maxY = doc.maxY;

            Ga = new GA();
            //Ga.GeneDescriptors = new GeneDescriptor[] { new IntegerGeneDescriptor() };
            Ga.GeneDescriptors = new GeneDescriptor[] { new DoubleGeneDescriptor() };
            Ga.Homogeneous = true;
            Ga.ChromosomeLength = cities.Length;            

            Ga.Recombinator = new RecombinationDelegate( new genX.Recombination.PartiallyMatchedCrossover().Recombine );
            //Ga.OrderMutator = new OrderMutationDelegate( genX.Reordering.Swap.Reorder );
            Ga.OrderMutator = new OrderMutationDelegate( genX.Reordering.Inversion.Reorder );
            Ga.ValueMutator = new ValueMutationDelegate( genX.Mutation.BitwiseMutation.Mutate );
            Ga.Scaler = new ScalingDelegate( new genX.Scaling.LinearRankedFitnessScaler(2.5).Scale );

            Ga.RecombinationProbability = 0.8;
            Ga.OrderMutationProbability = 0.15;

            Ga.PopulationSize       = 500;
            Ga.Objective            = new ObjectiveDelegate( GetObjective );
            Ga.ObjectiveType        = ObjectiveType.MinimizeObjective;
            Ga.MaxGenerations       = 1000000;

            Ga.NewPopulation += new NewPopulationEventHandler( OnNewPopulation );

            t = new System.Threading.Thread( new System.Threading.ThreadStart( gaThread ) );
            t.Start();            
        }

        System.Threading.Thread t;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            axMSChart1.ColumnCount = 1;
            axMSChart1.RowCount = 1;
            axMSChart1.chartType = MSChart20Lib.VtChChartType.VtChChartType2dLine;
            axMSChart1.RandomFill = true;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            InitializeGa();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
            this.timer1 = new System.Timers.Timer();
            this.graphPanel = new System.Windows.Forms.Panel();
            this.axMSChart1 = new AxMSChart20Lib.AxMSChart();
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMSChart1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.SynchronizingObject = this;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
            // 
            // graphPanel
            // 
            this.graphPanel.Location = new System.Drawing.Point(8, 8);
            this.graphPanel.Name = "graphPanel";
            this.graphPanel.Size = new System.Drawing.Size(408, 248);
            this.graphPanel.TabIndex = 0;
            this.graphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.graphPanel_Paint);
            // 
            // axMSChart1
            // 
            this.axMSChart1.DataSource = null;
            this.axMSChart1.Location = new System.Drawing.Point(440, 8);
            this.axMSChart1.Name = "axMSChart1";
            this.axMSChart1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMSChart1.OcxState")));
            this.axMSChart1.Size = new System.Drawing.Size(232, 248);
            this.axMSChart1.TabIndex = 1;
            this.axMSChart1.ChartSelected += new AxMSChart20Lib._DMSChartEvents_ChartSelectedEventHandler(this.axMSChart1_ChartSelected);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(688, 266);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.axMSChart1,
                                                                          this.graphPanel});
            this.Name = "Form1";
            this.Text = "Form1";
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMSChart1)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if ( Ga != null )
            {
                //Ga.Step();
                //Invalidate();
            }
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
        }

        private void Form1_Closed(object sender, System.EventArgs e)
        {
            t.Abort();
        }

        private void graphPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            int Width = p.ClientRectangle.Width - (BorderSize*2);
            int Height = p.ClientRectangle.Height - (BorderSize*2);
            if ( Width < 0 ) Width = 0;
            if ( Height < 0 ) Height = 0;

            double dx = (double)Width/maxX;
            double dy = (double)Height/maxY;

            //            Chromosome c = Ga.Population.Summary.BestChromosome;
            if ( currentPopulationSummary == null ) return;
            //System.Threading.Monitor.Enter(this);
            Chromosome c = currentPopulationSummary.BestChromosome;
            Point[] points = new Point[c.Genes.Length];
            int i=0;
            foreach(Gene g in c.Genes)
            {
                points[i] = cities[g.Label];
                points[i].X = (int) ((double)points[i].X * dx) + BorderSize;
                points[i].Y = (int) ((double)points[i].Y * dy) + BorderSize;
                i++;                
            }
            //System.Threading.Monitor.Exit(this);
            e.Graphics.DrawLines(System.Drawing.Pens.Blue, points);
            e.Graphics.DrawString( Ga.Population.Summary.ToString(), new Font("Arial", 8), new SolidBrush(Color.Black), 0, 0);
        }

        private void axMSChart1_ChartSelected(object sender, AxMSChart20Lib._DMSChartEvents_ChartSelectedEvent e)
        {
        
        }
	}
}
