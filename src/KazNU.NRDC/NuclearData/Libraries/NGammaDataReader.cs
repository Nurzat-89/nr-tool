namespace NuclearData
{
    /// <summary>
    /// Data reader for (n,g) reaction
    /// </summary>
    internal class NGammaDataReader : ReactionDataReader
    {
        /// <inheritdoc/>
        public override int MF => 3;

        /// <inheritdoc/>
        public override int MT => 102;

        /// <inheritdoc/>
        public override Constants.FILETYP ReactionType => Constants.FILETYP.NEUTRON;
    }
}
