using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NuclearData
{
    /// <summary>
    /// Class for Maxwellian Averaged Cross Section data reader 
    /// </summary>
    internal abstract class MacsDataReader : INuclearDataReader<IMacs>
    {
        /// <inheritdoc/>
        public int MF { get; }

        /// <inheritdoc/>
        public int MT { get; }

        /// <inheritdoc/>
        public Constants.FILETYP ReactionType => Constants.FILETYP.MACS;

        /// <inheritdoc/>
        public IEnumerable<IMacs> ReadData(int Z, int A, string fileName)
        {
            if (File.Exists(fileName))
            {
                return null;
            }

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            List<IMacs> macsDataList = new List<IMacs>();

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line = "";
                line = streamReader.ReadLine();
                while ((line = streamReader.ReadLine()) != null)
                {
                    var str = line.Split(';');
                    var s1 = str[0].Trim();
                    var s2 = str[1].Trim();
                    var s4 = str[3].Trim();
                    var s5 = str[4].Trim();
                    var za = s2.Split('-');
                    if (string.IsNullOrEmpty(za[2]) || za[2].ToUpper().Contains('M'))
                    {
                        continue;
                    }

                    int z = Convert.ToInt32(za[0]);
                    int a = Convert.ToInt32(za[2].Replace("G", ""));
                    var element = new Element(z, a);
                    var value = 0.0;
                    try
                    {
                        value = double.Parse(s5, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception) 
                    {
                        continue;
                    }
                    var macs = new Macs(element, value * 1.0E-3, s1, Convert.ToDouble(s4));
                    macsDataList.Add(macs);
                }

            }

            return macsDataList;
        }
    }
}
