using System.Collections.Generic;

namespace NuclearData
{
    /// <summary>
    /// Interface for ENDF data file
    /// </summary>
    internal interface IEndf
    {
        /// <summary>
        /// List of isotopes
        /// </summary>
        IEnumerable<IIsotope> Isotopes { get; }

        /// <summary>
        /// List of nuclear data reader
        /// </summary>
        IEnumerable<INuclearDataReader<T>> NuclearData { get; }

        /// <summary>
        /// Maxwellian averaged cross section data
        /// </summary>
        IMacsEndf EndfMacs { get; }
    }
}
