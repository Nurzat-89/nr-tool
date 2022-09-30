using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace NuclearData
{
    /// <inheritdoc/>
    public class NeutronSpectra : INeutronSpectra
    {
        /// <inheritdoc/>
        public NeutronSpectra(double kT, double flux)
        {
            this.kT = kT;
            Flux = flux;
        }

        /// <inheritdoc/>
        public double kT { get; }

        /// <inheritdoc/>
        public double Flux { get; }

        private static double OneGroupCrossSection(INeutronSpectra spectra, ICrossSectionData crossSection)
        {
            double tot = 0.0;
            double totEn = 0.0;
            foreach (var cs in crossSection.CrossSectionValues)
            {
                var val = MaxwellBoltzmann(cs.EneV, spectra.kT);
                tot += val * cs.CsBarn;
                totEn += val;
            }
            return tot / totEn;
        }

        /// <summary>
        /// Set Calculated Avg Cross Section
        /// </summary>
        public static void SetCalculatedAvgCrossSection(IEnumerable<IIsotope> isotopes, INeutronSpectra spectra)
        {
            foreach (var isotope in isotopes)
            {
                if (isotope.CrossSections.ContainsKey(Constants.REACT.N_G))
                {
                    isotope.AvgCalculatedCs = OneGroupCrossSection(spectra, isotope.GetCrossSection(Constants.REACT.N_G));
                }
            }
        }

        /// <summary>
        /// Set macs
        /// </summary>
        public static void SetMacsCrossSection(IEnumerable<IIsotope> isotopes, IEnumerable<IMacs> macsCollection) 
        {
            foreach (var isotope in isotopes)
            {
                var macs = macsCollection.FirstOrDefault(x => x.Element.ZAID == isotope.ZAID);
                if (macs == null) continue;
                isotope.AvgCs = macs.AvgCs;
            }
        }

        private static double MaxwellBoltzmann(double en, double kT)
        {
            var res = 2 * Math.Sqrt(en / Math.PI) * Math.Pow(1 / kT, 3 / 2) * Math.Exp(-en / kT);
            return res;
        }
    }
}
