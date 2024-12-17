using Accord;
using NuclearData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
        public IEnumerable<IIsotope> Isotopes => _isotopes;

        /// <inheritdoc/>
        public INeutronSpectra NeutronSpectra { get; }

        /// <inheritdoc/>
        public IMatrix<double> Matrix { get; }

        /// <inheritdoc/>
        public event Action<int> BurnupMatrixStatusChangedEvent;

        private void SetBurnMatrix()
        {
            Matrix.Zero();
            var dx = 100 / _isotopes.Count;
            double progress = 0.0;
            int currentProgress = 0;

            for (int i = 0; i < _isotopes.Count; i++)
            {
                int iAlfa = _isotopes.FindIndex(x => x.A == _isotopes[i].A + 4 && x.Z == _isotopes[i].Z + 2);  // alfa decay
                int iBeta = _isotopes.FindIndex(x => x.A == _isotopes[i].A && x.Z == _isotopes[i].Z - 1);      // beta decay
                int iEcup = _isotopes.FindIndex(x => x.A == _isotopes[i].A && x.Z == _isotopes[i].Z + 1);      // EC decay
                int iCapt = _isotopes.FindIndex(x => x.A == _isotopes[i].A - 1 && x.Z == _isotopes[i].Z);      // (n,g) 

                if (iAlfa != -1 && _isotopes[iAlfa].Decays.TryGetValue(Constants.RTYPE.ALFA, out IDecayData decayAlpha))
                {
                    double prob = decayAlpha.DecayProb;
                    Matrix.Array[i, iAlfa] += _isotopes[iAlfa].DecayConst * prob;
                }

                if (iBeta != -1 && _isotopes[iBeta].Decays.TryGetValue(Constants.RTYPE.BETA, out IDecayData decayBeta))
                {
                    double prob = decayBeta.DecayProb;
                    Matrix.Array[i, iBeta] += _isotopes[iBeta].DecayConst * prob;
                }

                if (iEcup != -1 && _isotopes[iEcup].Decays.TryGetValue(Constants.RTYPE.EC, out IDecayData decayEc))
                {
                    double prob = decayEc.DecayProb;
                    Matrix.Array[i, iEcup] += _isotopes[iEcup].DecayConst * prob;
                }

                if (iCapt != -1)
                {
                    Matrix.Array[i, iCapt] += NeutronSpectra.Flux * _isotopes[iCapt].AvgCs * Constants.barn;
                }
                Matrix.Array[i, i] += -NeutronSpectra.Flux * _isotopes[i].AvgCs * Constants.barn;

                if (!_isotopes[i].Stable)
                {
                    Matrix.Array[i, i] += -_isotopes[i].DecayConst;
                }

                progress += dx;
                if (currentProgress != (int)progress)
                {
                    currentProgress = (int)progress;
                    BurnupMatrixStatusChangedEvent?.Invoke(currentProgress);
                }
            }
        }
    }
}
