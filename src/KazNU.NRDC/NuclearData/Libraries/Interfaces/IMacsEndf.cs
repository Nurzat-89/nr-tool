using System.Collections.Generic;

namespace NuclearData
{
    internal interface IMacsEndf
    {
        IEnumerable<IMacs> MacsList { get; }

        IEnumerable<IMacs> GetMacsList(Constants.DATALIBS dataLib, double kt);
    }
}
