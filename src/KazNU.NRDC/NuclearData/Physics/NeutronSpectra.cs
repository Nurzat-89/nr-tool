using System;

namespace NuclearData
{
    /// <inheritdoc/>
    public class NeutronSpectra : INeutronSpectra
    {
        /// <inheritdoc/>
        public NeutronSpectra(double kT)
        {
            this.kT = kT;
        }

        /// <inheritdoc/>
        public double kT { get; }

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

        private double MaxwellBoltzmann(double en)
        {
            var res = 2 * Math.Sqrt(en / Math.PI) * Math.Pow(1 / kT, 3 / 2) * Math.Exp(-en / kT);
            return res;
        }
    }
}
