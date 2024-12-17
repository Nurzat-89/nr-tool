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
        public double AvgCs
        {
            get
            {
                CrossSections.TryGetValue(REACT.N_G, out ICrossSectionData crossSection);
                if (A == 59 && Z == 26)
                {
                    return 0.5;
                }
                if (A == 60 && Z == 26)
                {
                    return 0.5;
                
                }
                if (A == 59 && Z == 27)
                {
                    return 0.5;
                }
                if (A == 88 && Z == 38)
                {
                    return 0.5;
                }
                if (A == 86 && Z == 36)
                {
                    return 0.5;
                }
                if (A == 68 && Z == 30)
                {
                    return 0.5;
                }
                if (A == 67 && Z == 30)
                {
                    return 0.5;
                }
                if (A == 66 && Z == 30)
                {
                    return 0.5;
                }
                if (A == 64 && Z == 28)
                {
                    return 0.5;
                }
                if (A == 60 && Z == 28)
                {
                    return 0.5;
                }
                if (A == 61 && Z == 28)
                {
                    return 0.5;
                }
                if (A == 62 && Z == 28)
                {
                    return 0.5;
                }
                if (A == 63 && Z == 28)
                {
                    return 0.5;
                }
                if (A == 90 && Z == 40)
                {
                    return 0.5;
                }
                if (A == 94 && Z == 40)
                {
                    return 0.5;
                }
                if (A == 70 && Z == 30)
                {
                    return 0.5;
                }
                if (crossSection?.AvgCs == null && HalfLife > 1_000_000)
                {
                    //return 0.5;
                }
                return crossSection?.AvgCs ?? 0;
            }
            set
            {
                CrossSections.TryGetValue(REACT.N_G, out ICrossSectionData crossSection);
                if (crossSection != null)
                {
                    crossSection.AvgCs = value;
                }
            }
        }

        /// <inheritdoc/>
        public double AvgCalculatedCs { get; set; }

        /// <inheritdoc/>
        public ICrossSectionData GetCrossSection(REACT reactionType)
        {
            CrossSections.TryGetValue(reactionType, out ICrossSectionData crossSection);
            return crossSection;
        }

        /// <inheritdoc/>
        public IDecayData GetDecay(RTYPE decayType)
        {
            Decays.TryGetValue(decayType, out IDecayData decay);
            return decay;
        }
    }
}
