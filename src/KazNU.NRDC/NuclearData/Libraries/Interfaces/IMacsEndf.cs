using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Macs data file
    /// </summary>
    public interface IMacsEndf
    {
        /// <summary>
        /// Data library
        /// </summary>
        DATALIBS Library { get; }

        /// <summary>
        /// Get list of MACS data
        /// </summary>
        IEnumerable<IMacs> GetMacsData();
    }
}
