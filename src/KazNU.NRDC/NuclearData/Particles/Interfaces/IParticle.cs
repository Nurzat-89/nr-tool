namespace NuclearData
{
    /// <summary>
    /// Interface for particle
    /// </summary>
    public interface IParticle : INuclide
    {
        /// <summary>
        /// Short name of particle
        /// </summary>
        string Name { get; }
    }
}
