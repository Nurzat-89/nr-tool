using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Base class for macs endf data
    /// </summary>
    public abstract class BaseMacsEndf : IMacsEndf
    {
        /// <inheritdoc/>
        public BaseMacsEndf(DATALIBS library)
        {
            Library = library;
        }

        /// <inheritdoc/>
        public DATALIBS Library { get; }

        /// <inheritdoc/>
        public abstract IEnumerable<IMacs> GetMacsData();
    }
}
