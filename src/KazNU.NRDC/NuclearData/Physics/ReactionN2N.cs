namespace NuclearData
{
    /// <summary>
    /// (n,2n) reaction
    /// </summary>
    internal class ReactionN2N : Reaction
    {
        public ReactionN2N()
        {
            IncidentParticle = new Neutron();
            AddOutgoingParticle(new Neutron());
            AddOutgoingParticle(new Neutron());
        }

        /// <inheritdoc/>
        public override IIsotope React(IIsotope isotope)
        {
            return new Isotope(isotope.A - 1, isotope.Z);
        }
    }
}
