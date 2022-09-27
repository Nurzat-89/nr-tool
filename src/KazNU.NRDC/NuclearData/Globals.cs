using System;
using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    internal static class Globals
    {
        public static string LocalAppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static string RootDir => $"{LocalAppData}\\KazNRDC\\xsdir\\";

        public static Dictionary<FILETYP, string> FileTypeDir = new Dictionary<FILETYP, string>() {
               { FILETYP.DECAY, "decay/" },
               { FILETYP.NEUTRON, "neutron/" },
               { FILETYP.FISSION, "nfy/" },
        };

        public static Dictionary<FILETYP, string> FileTypeName = new Dictionary<FILETYP, string>() {
               { FILETYP.DECAY, "dec-" },
               { FILETYP.NEUTRON, "n-" },
               { FILETYP.FISSION, "nfy-" },
        };
    }
}
