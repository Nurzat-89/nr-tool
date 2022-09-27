using Accord;
using NuclearData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuclearCalculation
{
    /// <summary>
    /// Burnup
    /// </summary>
    public class BurnUp : IBurnUp
    {
        private List<IIsotope> _isotopes;
        
        /// <inheritdoc/>
        public BurnUp(IEnumerable<IIsotope> isotopes, INeutronSpectra neutronSpectra)
        {
            _isotopes = isotopes.ToList();
            NeutronSpectra = neutronSpectra;
            Matrix = new MatrixDouble(_isotopes.Count, _isotopes.Count);
            SetBurnMatrix();
        }

        /// <inheritdoc/>
        public IEnumerable<IIsotope> Isotopes { get; }

        /// <inheritdoc/>
        public INeutronSpectra NeutronSpectra { get; }

        /// <inheritdoc/>
        public IMatrix<double> Matrix { get; }

        private void SetBurnMatrix()
        {
            Matrix.Zero();

            for (int i = 0; i < Isotopes.Count(); i++)
            {
                int iAlfa = _isotopes.FindIndex(x => x.A == _isotopes[i].A + 4 && x.Z == _isotopes[i].Z + 2);  // alfa decay
                int iBeta = _isotopes.FindIndex(x => x.A == _isotopes[i].A && x.Z == _isotopes[i].Z - 1);      // beta decay
                int iEcup = _isotopes.FindIndex(x => x.A == _isotopes[i].A && x.Z == _isotopes[i].Z + 1);      // EC decay
                int iCapt = _isotopes.FindIndex(x => x.A == _isotopes[i].A - 1 && x.Z == _isotopes[i].Z);      // (n,g) 

                try
                {
                    if (iAlfa != -1)
                    {
                        double prob = _isotopes[iAlfa].Decays[Constants.RTYPE.ALFA].DecayProb;
                        Matrix.Array[i, iAlfa] += _isotopes[iAlfa].DecayConst * prob;
                    }
                }
                catch (Exception) { }
                try
                {
                    if (iBeta != -1)
                    {
                        double prob = _isotopes[iBeta].Decays[Constants.RTYPE.BETA].DecayProb;
                        Matrix.Array[i, iBeta] += _isotopes[iBeta].DecayConst * prob;
                    }
                }
                catch (Exception) { }
                try
                {
                    if (iEcup != -1)
                    {
                        double prob = _isotopes[iEcup].Decays[Constants.RTYPE.EC].DecayProb;
                        Matrix.Array[i, iEcup] += _isotopes[iEcup].DecayConst * prob;
                    }
                }
                catch (Exception) { }
                try
                {
                    if (iCapt != -1)
                    {
                        Matrix.Array[i, iCapt] += NeutronSpectra.Flux * _isotopes[iCapt].CrossSections[Constants.REACT.N_G].AvgCs * Constants.barn;
                    }
                }
                catch (Exception) { }

                try { Matrix.Array[i, i] += -NeutronSpectra.Flux * _isotopes[i].CrossSections[Constants.REACT.N_G].AvgCs * Constants.barn; } catch (Exception) { }
                Matrix.Array[i, i] += -_isotopes[i].DecayConst;
            }
        }
    }
}
