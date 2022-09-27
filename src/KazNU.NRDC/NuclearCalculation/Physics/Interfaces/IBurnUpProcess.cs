using NuclearData;
using System;
using System.Collections.Generic;

namespace NuclearCalculation
{
    /// <summary>
    /// Interface for calculating s and r processes in start, neutron irradiation
    /// </summary>
    public interface IBurnUpProcess
    {
        /// <summary>
        /// Burnup
        /// </summary>
        IBurnUp BurnUp { get; }

        /// <summary>
        /// Method to calculate matix exponenet
        /// </summary>
        IMatrixExp MatrixExp { get; }

        /// <summary>
        /// Initial nuclide density (t=0)
        /// </summary>
        IEnumerable<INuclideDensity> InitialDensities { get; }

        /// <summary>
        /// Endf library
        /// </summary>
        IEndf EndfLibrary { get; }

        /// <summary>
        /// Method to calculate nuclide number density after time
        /// </summary>
        IEnumerable<INuclideDensity> Calculate(TimeSpan time);
    }
}
