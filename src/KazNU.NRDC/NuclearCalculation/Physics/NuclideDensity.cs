using NuclearData;

namespace NuclearCalculation
{
    /// <inheritdoc/>
    public class NuclideDensity : INuclideDensity
    {
        /// <inheritdoc/>
        public NuclideDensity(IIsotope isotope, double density)
        {
            Isotope = isotope;
            Density = density;
        }

         /// <inheritdoc/>
        public string NuclideName => Isotope.Name;
        /// <inheritdoc/>
        
        public double AtomicWeight => Isotope.AtomicMass;
        /// <inheritdoc/>
        
        public double Density { get; set; }

        /// <inheritdoc/>
        public IIsotope Isotope { get; }
    }
}
