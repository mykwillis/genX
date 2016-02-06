using System;
using System.IO;
using System.Runtime.Serialization;

namespace genX
{
    /// <summary>
    /// Provides support for persisting population data to disk.
    /// </summary>
    internal class Persistance
    {
        int count;
        const string filenameBase = "population";
        const string filenameExt = ".bin";

        void NewPopulationHandler(object sender, NewPopulationEventArgs e)        
        {
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Stream stream = new FileStream(filenameBase + count + filenameExt, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, e.NewPopulation);
            stream.Close();
            count++;
        }


        /// <summary>
        /// Gets or sets the associated GA.
        /// </summary>
        public GA GA
        {
            get { return ga; }
            set
            {
                if ( ga != null )
                {
                    ga.NewPopulation -= new NewPopulationEventHandler( this.NewPopulationHandler );
                }                
                ga = value;
                ga.NewPopulation += new NewPopulationEventHandler( this.NewPopulationHandler );
            }
        }
        private GA ga;
    }
}
