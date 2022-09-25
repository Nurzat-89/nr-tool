using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Interface for decay data
    /// </summary>
    public interface IDecayData
    {
        /// <summary>
        /// Decay type
        /// </summary>
        RTYPE DecayType { get; }
       
        /// <summary>
        /// Id of decay
        /// </summary>
        double Id { get; }
        
        /// <summary>
        /// Decay energy
        /// </summary>
        double DecayEnergy { get; }
        
        /// <summary>
        /// Decay probobility normalized
        /// </summary>
        double DecayProb { get; }

        /// <summary>
        /// Decay probobility in percent
        /// </summary>
        double DecayProbPerc { get; }
    }
}
