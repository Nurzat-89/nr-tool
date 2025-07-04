﻿using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Interface for Isotope
    /// </summary>
    public interface IIsotope : IAtom, INuclide
    {
        /// <summary>
        /// Number of decay types
        /// </summary>
        int DecayTypes { get; }

        /// <summary>
        /// Isotope name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Full name of element
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// True - if isotope is stable
        /// </summary>
        bool Stable { get; }

        /// <summary>
        /// Decay const
        /// </summary>
        double DecayConst { get; }

        /// <summary>
        /// Dictionary list of decays
        /// </summary>
        IDictionary<RTYPE, IDecayData> Decays { get; }

        /// <summary>
        /// Dictionary list of cross sections
        /// </summary>
        IDictionary<REACT, ICrossSectionData> CrossSections { get; }

        /// <summary>
        /// Get cross section data via reaction type
        /// </summary>
        ICrossSectionData GetCrossSection(REACT reactionType);

        /// <summary>
        /// Averaged cross section for (n,g) reaction cs
        /// </summary>
        double AvgDataMacsCs { get; set; }

        /// <summary>
        /// Averaged cross section for (n,g) reaction cs
        /// </summary>
        double AvgMacsCs { get; }

        /// <summary>
        /// Averaged calculated cross section for (n,g) reaction cs
        /// </summary>
        double AvgCalculatedCs { get; set; }

        /// <summary>
        /// HasMacsCs
        /// </summary>
        bool HasMacsCs { get; }

        /// <summary>
        /// Get decay data via decay type
        /// </summary>
        IDecayData GetDecay(RTYPE decayType);
    }
}
