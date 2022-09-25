namespace NuclearData
{
    /// <summary>
    /// Data reader for (n,p) reaction 
    /// </summary>
    internal class NProtonDataReader : ReactionDataReader
    {
        /// <inheritdoc/>
        public override int MF => 3;

        /// <inheritdoc/>
        public override int MT => 103;

        /// <inheritdoc/>
        public override Constants.FILETYP ReactionType => Constants.FILETYP.NEUTRON;
    }
}
