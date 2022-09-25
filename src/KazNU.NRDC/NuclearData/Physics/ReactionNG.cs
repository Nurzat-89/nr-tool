namespace NuclearData
{
    /// <summary>
    /// (n,g) reaction
    /// </summary>
    internal class ReactionNG : Reaction
    {
        public ReactionNG()
        {
            IncidentParticle = new Neutron();
            AddOutgoingParticle(new Photon());
        }

        /// <inheritdoc/>
        public override IIsotope React(IIsotope isotope)
        {
            return new Isotope(isotope.A + 1, isotope.Z);
        }
    }
}
