namespace NuclearData
{
    /// <inheritdoc/>
    internal class CrossSectionValue : ICrossSectionValue
    {
        /// <inheritdoc/>
        public CrossSectionValue(Constants.REACT type, double en, double cs)
        {
            Type = type;
            EneV = en;
            CsBarn = cs;
        }

        /// <inheritdoc/>
        public double EneV { get; }

        /// <inheritdoc/>
        public double EnJ => EneV * Constants.q_electron;

        /// <inheritdoc/>
        public double CsBarn { get; }

        /// <inheritdoc/>
        public double Cssm2 => CsBarn * Constants.barn;

        /// <inheritdoc/>
        public Constants.REACT Type { get; }
    }
}
