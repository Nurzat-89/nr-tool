using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        /// <inheritdoc/>
        public double OneGroupCrossSection(ICrossSectionData crossSection)
        {
            double tot = 0.0;
            double totEn = 0.0;
            foreach (var cs in crossSection.CrossSectionValues)
            {
                var val = MaxwellBoltzmann(cs.EneV);
                tot += val * cs.CsBarn;
                totEn += val;
            }
            return tot / totEn;
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

        private double MaxwellBoltzmann(double en)
        {
            var res = 2 * Math.Sqrt(en / Math.PI) * Math.Pow(1 / kT, 3 / 2) * Math.Exp(-en / kT);
            return res;
        }
    }
}
