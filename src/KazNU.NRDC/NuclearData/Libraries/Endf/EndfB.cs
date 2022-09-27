using System;
using System.Collections.Generic;

namespace NuclearData
{
    /// <summary>
    /// Brookhaven ENDF-B data file (USA)
    /// </summary>
    public class EndfB : BaseEndf
    {
        private const string libFolder = "ENDFB-VIII/";
        private const string extention = ".endf";

        /// <inheritdoc/>
        public EndfB() : base(Constants.DATALIBS.ENDFB_VIII, libFolder, extention) { }

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
                var str = files[i].Split('_');
                int z = Convert.ToInt32(str[0]);
                if (str[2].Contains("m")) continue;
                int a = Convert.ToInt32(str[2]);
                elements.Add(new Element(z, a));
            }
            return elements;
        }

        /// <inheritdoc/>
        protected override string GetIsotopeFile(int Z, int A, Constants.FILETYP ftype)
        {
            string sz = Z.ToString("D3");
            string sa = A.ToString("D3");
            var name = sz + "_" + Constants.ElementNames[Z] + "_" + sa;
            string filePath = $"{Globals.RootDir}{LibFolder}{Globals.FileTypeDir[ftype]}/{Globals.FileTypeName[ftype]}{name}{Extention}";
            filePath = filePath.Replace("/", $"\\").Replace("\\\\", $"\\");
            return filePath;
        }
    }
}
