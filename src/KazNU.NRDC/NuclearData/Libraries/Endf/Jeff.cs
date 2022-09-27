using System;
using System.Collections.Generic;

namespace NuclearData
{
    /// <summary>
    /// Jeff nuclear data file (OECD France)
    /// </summary>
    public class Jeff : BaseEndf
    {
        private const string libFolder = "Jeff/";
        private const string extention = ".jeff33";

        /// <inheritdoc/>
        public Jeff() : base(Constants.DATALIBS.JEFF, libFolder, extention) { }

        /// <inheritdoc/>
        protected override IEnumerable<Element> GetAllElements(Constants.FILETYP fileType)
        {
            var elements = new List<Element>();
            var dir = Globals.FileTypeName[fileType];
            var files = GetFileNames(fileType);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace(dir, "");
                files[i] = files[i].Replace(Extention, "");
                var str = files[i].Split('-');
                int z = Convert.ToInt32(str[0]);
                if (str[2].Contains("m")) continue;
                int a = Convert.ToInt32(str[2].Replace("g", ""));
                elements.Add(new Element(z, a));
            }
            return elements;
        }

        /// <inheritdoc/>
        protected override string GetIsotopeFile(int Z, int A, Constants.FILETYP ftype)
        {
            var name = Z + "-" + Constants.ElementNames[Z] + "-" + A + "g";
            string filePath = $"{Globals.RootDir}{LibFolder}{Globals.FileTypeDir[ftype]}/{name}{Extention}";
            filePath = filePath.Replace("/", $"\\").Replace("\\\\", $"\\");
            return filePath;
        }
    }
}
