using System;
using System.Collections.Generic;
using System.Linq;

namespace NuclearData
{
    /// <summary>
    /// Jendl data file (Japanese)
    /// </summary>
    public class Jendl : BaseEndf
    {
        private const string libFolder = "JENDL/";
        private const string extention = ".dat";

        /// <inheritdoc/>
        public Jendl() : base(Constants.DATALIBS.JENDL, libFolder, extention) { }

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
            string filePath = $"{Globals.RootDir}{LibFolder}{Globals.FileTypeDir[ftype]}/{name}{Extention}";
            filePath = filePath.Replace("/", $"\\").Replace("\\\\", $"\\");
            return filePath;
        }
    }
}
