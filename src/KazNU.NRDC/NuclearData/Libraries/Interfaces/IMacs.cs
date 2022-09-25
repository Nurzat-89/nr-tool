namespace NuclearData
{
    /// <summary>
    /// Maxwellian average cross section data
    /// </summary>
    public interface IMacs
    {
        /// <summary>
        /// Element
        /// </summary>
        Element Element { get; }

        /// <summary>
        /// Average cross section value in barn
        /// </summary>
        double AvgCs { get; }

        /// <summary>
        /// Data library
        /// </summary>
        string DataLib { get; }

        /// <summary>
        /// Temperature in ev Kt
        /// </summary>
        double kT { get; }
    }
}
