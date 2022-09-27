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
        /// Method to calculate Maxwellian averaged cross section for given cross section vaules
        /// </summary>
        double OneGroupCrossSection(ICrossSectionData crossSection);
    }
}
