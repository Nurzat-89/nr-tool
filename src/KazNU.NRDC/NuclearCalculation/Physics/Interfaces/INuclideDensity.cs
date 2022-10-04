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
        double Density { get; set; }

        /// <summary>
        /// Material density g/cm3
        /// </summary>
        double MaterialDensity { get; }
        
        /// <summary>
        /// Atomic mass
        /// </summary>
        double AtomicMass { get; }

        /// <summary>
        /// Isotope
        /// </summary>
        IIsotope Isotope { get; }

        /// <summary>
        /// HeatDensity eV/sec/cm3
        /// </summary>
        double HeatDensity { get; }

        /// <summary>
        /// HeatDensity MeV/sec/cm3
        /// </summary>
        double HeatDensityMeV { get; }

        /// <summary>
        /// CalculateHeat
        /// </summary>
        void CalculateHeat(double materialDensity, double atomicMass); 
    }
}
