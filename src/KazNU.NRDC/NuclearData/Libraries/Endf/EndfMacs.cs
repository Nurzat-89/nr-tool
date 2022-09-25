using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NuclearData
{
    /// <summary>
    /// Macs data for ENDF data file
    /// </summary>
    internal class EndfMacs : BaseMacsEndf
    {
        /// <inheritdoc/>
        protected override string FileName => "ngamma.txt";

        /// <inheritdoc/>
        protected override IEnumerable<IMacs> DataRead()
        {
            var fn = $"{Globals.RootDir}MACS/{FileName}";

            FileStream fileStream;
            try
            {
                fileStream = new FileStream(fn, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            var macsList = new List<IMacs>();
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
                    if (string.IsNullOrEmpty(za[2]) || za[2].ToUpper().Contains('M')) continue;
                    int z = Convert.ToInt32(za[0]);
                    int a = Convert.ToInt32(za[2].Replace("G", ""));
                    var element = new Element(z, a);
                    var value = 0.0;
                    try
                    {
                        value = double.Parse(s5, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception) { }
                    var macs = new Macs(element, value * 1.0E-3, s1, Convert.ToDouble(s4));

                    macsList.Add(macs);
                }
            }
            return macsList;
        }
    }
}
