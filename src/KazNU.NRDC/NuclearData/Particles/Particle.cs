namespace NuclearData
{
    /// <inheritdoc/>
    public abstract class Particle : IParticle
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Z">Z number</param>
        /// <param name="A">Mass number</param>
        /// <param name="atomicMass">Aomic mass in a.e.m.</param>
        public Particle(int Z, int A, double atomicMass)
        {
            AtomicMass = atomicMass;
            this.Z = Z;
            this.A = A;
        }

        /// <inheritdoc/>
        public string Name { get; protected set; }

        /// <inheritdoc/>
        public int A { get; }

        /// <inheritdoc/>
        public int Z { get; }

        /// <inheritdoc/>
        public int ZAID => Z * 1000 + A;

        /// <inheritdoc/>
        public int MAT { get; protected set; }

        /// <inheritdoc/>
        public double AtomicMass { get; }
    }
}
