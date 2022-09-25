using System.Collections.Generic;

namespace NuclearData
{
    /// <summary>
    /// Interface for reaction (n,g), (n,2n), (n,a) etc
    /// </summary>
    public interface IReaction
    {
        /// <summary>
        /// Id of reaction
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// Incident particle (neutron, proton etc.)
        /// </summary>
        IParticle IncidentParticle { get; }
        
        /// <summary>
        /// List of outgoing particles
        /// </summary>
        IEnumerable<IParticle> OutgoingParticles { get; }

        /// <summary>
        /// Method to react incident particle with isotope
        /// </summary>
        /// <param name="isotope"></param>
        IIsotope React(IIsotope isotope);
    }
}
