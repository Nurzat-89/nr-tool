namespace NuclearData
{
    /// <summary>
    /// Data reader for (n,a) reaction 
    /// </summary>
    internal class NAlphaDataReader : ReactionDataReader
    {
        /// <inheritdoc/>
        public override int MF => 3;

        /// <inheritdoc/>
        public override int MT => 107;

        /// <inheritdoc/>
        public override Constants.FILETYP ReactionType => Constants.FILETYP.NEUTRON;
    }
}
