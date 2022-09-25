namespace NuclearData
{
    /// <summary>
    /// Data reader for (n,2n) reaction 
    /// </summary>
    internal class N2NdataReader : ReactionDataReader
    {
        /// <inheritdoc/>
        public override int MF => 3;

        /// <inheritdoc/>
        public override int MT => 16;

        /// <inheritdoc/>
        public override Constants.FILETYP ReactionType => Constants.FILETYP.NEUTRON;
    }
}
