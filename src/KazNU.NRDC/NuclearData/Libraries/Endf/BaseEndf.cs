using System.Collections.Generic;

namespace NuclearData
{
    /// <summary>
    /// Base class for endf data file
    /// </summary>
    internal class BaseEndf : IEndf
    {
        private IList<IIsotope> _isotopes = new List<IIsotope>();
        private IList<INuclearDataReader> _nuclearData = new List<INuclearDataReader>();

        /// <inheritdoc/>
        public BaseEndf()
        {

        }

        /// <inheritdoc/>
        public IEnumerable<IIsotope> Isotopes => _isotopes;

        /// <inheritdoc/>
        public IEnumerable<INuclearDataReader> NuclearData => _nuclearData;

        /// <inheritdoc/>
        public IMacsEndf EndfMacs { get; }
    }
}
