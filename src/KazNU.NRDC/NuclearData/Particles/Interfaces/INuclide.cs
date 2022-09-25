namespace NuclearData
{
    /// <summary>
    /// Inteface for nuclide
    /// </summary>
    public interface INuclide
    {
        /// <summary>
        /// Mass number
        /// </summary>
        int A { get; }

        /// <summary>
        /// Z number
        /// </summary>
        int Z { get; }

        /// <summary>
        /// ZAID A+Z
        /// </summary>
        int ZAID { get; }

        /// <summary>
        /// MAT number for ENDF
        /// </summary>
        int MAT { get; }
    }
}
