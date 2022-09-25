namespace NuclearData
{
    /// <inheritdoc/>
    internal class CrossSectionValue : ICrossSectionValue
    {
        /// <inheritdoc/>
        public CrossSectionValue(int id, double en, double cs)
        {
            Id = id;
            EneV = en;
            CsBarn = cs;
        }

        /// <inheritdoc/>
        public int Id { get; }

        /// <inheritdoc/>
        public double EneV { get; }

        /// <inheritdoc/>
        public double EnJ => EneV * Constants.q_electron;

        /// <inheritdoc/>
        public double CsBarn { get; }

        /// <inheritdoc/>
        public double Cssm2 => CsBarn * Constants.barn;

        /// <inheritdoc/>
        public Constants.REACT Type => Constants.REACTIONTYPE[Id];
    }
}
