using static NuclearData.Constants;

namespace NuclearData
{
    /// <inheritdoc/>
    internal class DecayData : IDecayData
    {
        /// <inheritdoc/>
        public DecayData(int id, double decayEnergy, double decayProb)
        {
            Id = id;
            DecayEnergy = decayEnergy;
            DecayProb = decayProb;
        }

        /// <inheritdoc/>
        public double Id { get; }

        /// <inheritdoc/>
        public double DecayEnergy { get; }

        /// <inheritdoc/>
        public double DecayProb { get; }

        /// <inheritdoc/>
        public double DecayProbPerc => DecayProb * 100.0;

        /// <inheritdoc/>
        public RTYPE DecayType => DECAYTYPE[Id];
    }
}
