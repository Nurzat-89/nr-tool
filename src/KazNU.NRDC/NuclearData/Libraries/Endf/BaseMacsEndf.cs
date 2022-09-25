using System.Collections.Generic;
using System.Linq;

namespace NuclearData
{
    /// <summary>
    /// Base class for macs endf data
    /// </summary>
    internal abstract class BaseMacsEndf : IMacsEndf
    {
        private IEnumerable<IMacs> _macsList;

        /// <inheritdoc/>
        public BaseMacsEndf()
        {
            _macsList = DataRead();
        }

        protected abstract string FileName { get; }

        /// <inheritdoc/>
        public IEnumerable<IMacs> MacsList => _macsList;

        /// <inheritdoc/>
        public IEnumerable<IMacs> GetMacsList(Constants.DATALIBS dataLib, double kt)
        {
            var lib = Constants.DataCenters[dataLib];
            return _macsList.Where(x => x.DataLib == lib && x.kT == kt);
        }

        protected abstract IEnumerable<IMacs> DataRead();
    }
}
