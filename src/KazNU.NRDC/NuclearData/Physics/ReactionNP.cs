namespace NuclearData
{
    /// <summary>
    /// (n,p) reaction
    /// </summary>
    internal class ReactionNP : Reaction
    {
        public ReactionNP()
        {
            IncidentParticle = new Neutron();
            AddOutgoingParticle(new Proton());
        }

        /// <inheritdoc/>
        public override IIsotope React(IIsotope isotope)
        {
            return new Isotope(isotope.A, isotope.Z - 1, isotope.AtomicMass, isotope.HalfLife);
        }
    }
}
