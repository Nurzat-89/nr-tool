namespace NuclearData
{
    /// <summary>
    /// Data reader for (n,el) reaction
    /// </summary>
    internal class NElectronDataReader : ReactionDataReader
    {
        /// <inheritdoc/>
        public override int MF => 3;

        /// <inheritdoc/>
        public override int MT => 2;

        /// <inheritdoc/>
        public override Constants.FILETYP ReactionType => Constants.FILETYP.NEUTRON;
    }
}
