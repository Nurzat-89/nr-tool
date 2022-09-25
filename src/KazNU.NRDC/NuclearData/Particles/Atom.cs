namespace NuclearData
{
    /// <inheritdoc/>
    internal class Atom : IAtom
    {
        /// <inheritdoc/>
        public Atom(double atomicMass, double halfLife)
        {
            AtomicMass = atomicMass;
            HalfLife = halfLife;
        }

        /// <inheritdoc/>
        public double HalfLife { get; }

        /// <inheritdoc/>
        public double AtomicMass { get; }
    }
}
