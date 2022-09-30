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
        public BaseMacsEndf(MACSDATALIBS library)
        {
            Library = library;
        }

        /// <inheritdoc/>
        public MACSDATALIBS Library { get; }

        /// <inheritdoc/>
        public abstract IEnumerable<IMacs> GetMacsData();
    }
}
