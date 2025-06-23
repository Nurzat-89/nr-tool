using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <inheritdoc/>
    public class Isotope : Particle, IIsotope
    {
        /// <inheritdoc/>
        public Isotope(int Z, int A, double atmoicMass, double halfLife) : base(Z, A, atmoicMass)
        {
            Name = $"{Constants.ElementTableNames[Z]}-{A}";
            HalfLife = halfLife;
        }

        /// <inheritdoc/>
        public int DecayTypes => Decays.Count;

        /// <inheritdoc/>
        public string ElementName => Constants.ElementTableNames[Z];

        /// <inheritdoc/>
        public bool Stable => Decays.Count == 0;

        /// <inheritdoc/>
        public double HalfLife { get; set; }

        /// <inheritdoc/>
        public double DecayConst => ln2 / HalfLife;

        /// <inheritdoc/>
        public IDictionary<RTYPE, IDecayData> Decays { get; } = new Dictionary<RTYPE, IDecayData>();

        /// <inheritdoc/>
        public IDictionary<REACT, ICrossSectionData> CrossSections { get; } = new Dictionary<REACT, ICrossSectionData>();

        /// <inheritdoc/>
        //public double AvgCs
        //{
        //    get
        //    {
        //        CrossSections.TryGetValue(REACT.N_G, out ICrossSectionData crossSection);
        //        return crossSection?.AvgCs ?? 0;
        //    }
        //    set
        //    {
        //        CrossSections.TryGetValue(REACT.N_G, out ICrossSectionData crossSection);
        //        //if (crossSection != null)
        //        {
        //            crossSection.AvgCs = value;
        //        }
        //    }
        //}
        public double AvgMacsCs => HasMacsCs ? AvgDataMacsCs : AvgCalculatedCs;

        /// <inheritdoc/>
        public double AvgCalculatedCs { get; set; }

        /// <inheritdoc/>
        public ICrossSectionData GetCrossSection(REACT reactionType)
        {
            CrossSections.TryGetValue(reactionType, out ICrossSectionData crossSection);
            return crossSection;
        }

        public double AvgDataMacsCs { get; set; }

        public bool HasMacsCs => AvgDataMacsCs != 0;

        /// <inheritdoc/>
        public IDecayData GetDecay(RTYPE decayType)
        {
            Decays.TryGetValue(decayType, out IDecayData decay);
            return decay;
        }
    }
}
