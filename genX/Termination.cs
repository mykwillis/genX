using System;
using System.ComponentModel;
using genX;

namespace genX.Termination
{
    /// <summary>
    /// Implements termination based on objective threshold.
    /// </summary>
    public class ObjectiveThresholdTerminator
    {
        /// <summary>
        /// The threshold value for the terminator.
        /// </summary>
        /// <remarks>
        /// If the GA is ObjectiveType.MinimizeObjective, the terminator will 
        /// terminate if the BestObjective of the population goes to or below
        /// this value.  
        /// </remarks>
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }
        double threshold;

        /// <summary>
        /// Terminates the algorithm if the threshold is met or exceeded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Terminate(object sender, CancelEventArgs e)
        {
            GA ga = (GA) sender;
            if ( ga.ObjectiveType == ObjectiveType.MinimizeObjective )
            {
                if ( ga.BestObjective <= threshold )
                {
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                if ( ga.BestObjective >= threshold )
                {
                    e.Cancel = true;
                    return;
                }
            }
        
            return;            
        }

        /// <summary>
        /// Creates a new objective threshold terminator with the given threshold.
        /// </summary>
        /// <param name="threshold"></param>
        public ObjectiveThresholdTerminator(double threshold)
        {
            this.threshold = threshold;
        }
    }


    /// <summary>
    /// Implements a clock-time based termination function.
    /// </summary>
    public class EvolutionTimeTerminator
    {
        TimeSpan timeSpan;
        DateTime startTime;
        DateTime endTime;

        /// <summary>
        /// Terminates the execution of the GA if the elapsed time
        /// of execution exceeds the timespan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Terminate(object sender, CancelEventArgs e)
        {
            if ( DateTime.Now > endTime )
            {
                e.Cancel = true;
            }
        }


        /// <summary>
        /// Initializes a new evolution time terminator, using the given
        /// time span as the time until termination.
        /// </summary>
        /// <param name="timeSpan"></param>
        public EvolutionTimeTerminator(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
            startTime = DateTime.Now;
            endTime = startTime.Add(timeSpan);
        }
    }
}