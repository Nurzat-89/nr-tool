using System.Collections.Generic;

namespace NuclearData
{
    /// <inheritdoc/>
    internal abstract class Reaction : IReaction
    {
        private IList<IParticle> _outgoingParticles;

        public Reaction()
        {
            _outgoingParticles = new List<IParticle>();
        }

        /// <inheritdoc/>
        public int Id { get; }

        /// <inheritdoc/>
        public IParticle IncidentParticle { get; protected set; }

        /// <inheritdoc/>
        public IEnumerable<IParticle> OutgoingParticles => _outgoingParticles;

        /// <inheritdoc/>
        public abstract IIsotope React(IIsotope isotope);

        protected void AddOutgoingParticle(IParticle particle) 
        {
            _outgoingParticles.Add(particle);
        }
    }
}
