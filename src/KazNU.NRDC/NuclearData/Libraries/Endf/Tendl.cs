using System;
using System.Collections.Generic;
using System.Linq;

namespace NuclearData
{
    /// <summary>
    /// Tendl data file (A.Koning)
    /// </summary>
    public class Tendl : BaseEndf
    {
        private const string libFolder = "TENDL/";
        private const string extention = ".tendl";

        /// <inheritdoc/>
        public Tendl() : base(Constants.DATALIBS.TENDL, libFolder, extention) { }

        /// <inheritdoc/>
        protected override IEnumerable<Element> GetAllElements(Constants.FILETYP fileType)
        {
            var elements = new List<Element>();
            var dir = Globals.FileTypeName[fileType];
            var files = GetFileNames(fileType);
            for (int i = 0; i < files.Length; i++)
            {
                var name = files[i].Replace(dir, "").Replace(Extention, "");
                if (name[name.Length - 1] == 'm' || name[name.Length - 2] == 'm') continue;
                var a = name.Substring(name.Length - 3, 3);
                var _a = Convert.ToInt32(a);
                name = name.Replace(a, "");
                var z = Constants.ElementNames.ToList().IndexOf(name);
                elements.Add(new Element(z, _a));
            }
            return elements;
        }

        /// <inheritdoc/>
        protected override string GetIsotopeFile(int Z, int A, Constants.FILETYP ftype)
        {
            var a = A.ToString("D3");
            var name = $"{Constants.ElementNames[Z]}{a}";
            string filePath = $"{Globals.RootDir}{LibFolder}{Globals.FileTypeDir[ftype]}/{Globals.FileTypeName[ftype]}{name}{Extention}";
            filePath = filePath.Replace("/", $"\\").Replace("\\\\", $"\\");
            return filePath;
        }
    }
}
