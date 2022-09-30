using System;
using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Interface for reading the nuclear data
    /// </summary>
    public interface INuclearDataReader<T> where T : class
    {
        /// <summary>
        /// MF number
        /// </summary>
        int MF { get; }

        /// <summary>
        /// MT number
        /// </summary>
        int MT { get; }

        /// <summary>
        /// Reaction type
        /// </summary>
        FILETYP ReactionType { get; }

        /// <summary>
        /// Method to create isotope from file
        /// </summary>
        /// <param name="Z">Z number of particle</param>
        /// <param name="A">A number of particle</param>
        /// <param name="fileName">Filename to read</param>
        IEnumerable<T> ReadData(int Z, int A, string fileName);
    }
}
