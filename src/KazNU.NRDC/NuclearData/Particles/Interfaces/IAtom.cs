namespace NuclearData
{
    /// <summary>
    /// Interface for atom
    /// </summary>
    public interface IAtom
    {
        /// <summary>
        /// Half life in sec
        /// </summary>
        double HalfLife { get; }

        /// <summary>
        /// Atomic mass in a.e.m.
        /// </summary>
        double AtomicMass { get; }
    }
}
