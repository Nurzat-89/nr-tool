namespace NuclearData
{
    /// <summary>
    /// Interface for neutron spectra
    /// </summary>
    public interface INeutronSpectra
    {
        /// <summary>
        /// Temperature kT (eV)
        /// </summary>
        double kT { get; }

        /// <summary>
        /// Neutron flux in 1/cm2/sec
        /// </summary>
        double Flux { get; }
    }
}
