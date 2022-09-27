using NuclearData;

namespace NuclearCalculation
{
    /// <summary>
    /// Nuclide density
    /// </summary>
    public interface INuclideDensity
    {
        /// <summary>
        /// Name of isotope
        /// </summary>
        string NuclideName { get; }

        /// <summary>
        /// Atomic weight in a.e.m.
        /// </summary>
        double AtomicWeight { get; }

        /// <summary>
        /// Density
        /// </summary>
        double Density { get; }

        /// <summary>
        /// Isotope
        /// </summary>
        IIsotope Isotope { get; }
    }
}
