using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Cross section value
    /// </summary>
    public interface ICrossSectionValue
    {
        /// <summary>
        /// Id of reaction type
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Reaction type
        /// </summary>
        REACT Type { get; }

        /// <summary>
        /// Energy of particle in eV
        /// </summary>
        double EneV { get; }

        /// <summary>
        /// Energy of particle in Joule
        /// </summary>
        double EnJ { get; }
        
        /// <summary>
        /// Cross section in barn
        /// </summary>
        double CsBarn { get; }

        /// <summary>
        /// Cross section in cm2
        /// </summary>
        double Cssm2 { get; }
    }
}
