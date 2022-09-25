using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Interface for ENDF data file
    /// </summary>
    internal interface IEndf
    {
        /// <summary>
        /// Data library
        /// </summary>
        DATALIBS Library { get; }

        /// <summary>
        /// Get all isotopes from ENDF files
        /// </summary>
        /// <returns></returns>
        IEnumerable<IIsotope> GetIsotopes();

        /// <summary>
        /// Get isotopes between Z1 and Z2
        /// </summary>
        /// <param name="Z1">Lower Z number</param>
        /// <param name="Z2">Upper Z numer</param>
        /// <returns></returns>
        IEnumerable<IIsotope> GetIsotopes(int Z1, int Z2);

        /// <summary>
        /// Get isotopes between n1 and n2 elements
        /// </summary>
        /// <param name="n1">Name of lower element</param>
        /// <param name="n2">Name of upper element</param>
        /// <returns></returns>
        IEnumerable<IIsotope> GetIsotopes(string n1, string n2);

        /// <summary>
        /// Get isotopes of individual nuclides
        /// </summary>
        IEnumerable<IIsotope> GetIsotopes(params int[] zaids);
    }
}
