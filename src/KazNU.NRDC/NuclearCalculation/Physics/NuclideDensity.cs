using MathNet.Numerics.Statistics.Mcmc;
using NuclearData;
using System.Linq;

namespace NuclearCalculation
{
    /// <inheritdoc/>
    public class NuclideDensity : INuclideDensity
    {
        /// <inheritdoc/>
        public NuclideDensity(IIsotope isotope, double density, double materialDensity, double atomicMass)
        {
            Isotope = isotope;
            Density = density;
            MaterialDensity = materialDensity;
            AtomicMass = atomicMass;
        }

         /// <inheritdoc/>
        public string NuclideName => Isotope.Name;
        /// <inheritdoc/>
        
        public double AtomicWeight => Isotope.AtomicMass;
        /// <inheritdoc/>
        
        public double Density { get; set; }

        /// <inheritdoc/>
        public IIsotope Isotope { get; }

        /// <inheritdoc/>
        public double HeatDensity { get; private set; }

        /// <inheritdoc/>
        public double HeatDensityMeV => HeatDensity / 1E6;

        /// <inheritdoc/>
        public double MaterialDensity { get; }

        /// <inheritdoc/>
        public double AtomicMass { get; }

        public void CalculateHeat(double materialDensity, double atomicMass)
        {
            HeatDensity = CalculateHeatDensity(materialDensity, atomicMass);
        }

        private double CalculateHeatDensity(double materialDensity, double atomicMass)
        {
            double concen = materialDensity * Constants.N_Avogadro / atomicMass;
            var decayEnergy = Isotope.Decays.Select(x => x.Value.DecayEnergy * x.Value.DecayProb).Sum();
            return decayEnergy * concen * Density * Constants.q_electron * Isotope.DecayConst;
        }
    }
}
