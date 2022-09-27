using NuclearData;
using System.Collections.Generic;

namespace NuclearCalculation
{
    /// <summary>
    /// Interface for burnup matrix
    /// </summary>
    public interface IBurnUp
    {
        /// <summary>
        /// List of isotopes
        /// </summary>
        IEnumerable<IIsotope> Isotopes { get; }

        /// <summary>
        /// Neutron spectra
        /// </summary>
        INeutronSpectra NeutronSpectra { get; }

        /// <summary>
        /// Burnup matrix (n x n)
        /// </summary>
        IMatrix<double> Matrix { get; }
    }
}
