using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NuclearData
{
    /// <summary>
    /// Macs data file EAF
    /// </summary>
    public class EafMacs : BaseMacsEndf
    {
        private const string fileName = "eaf2010.txt";

        public EafMacs() : base(Constants.MACSDATALIBS.EAF2010) { }

        public override IEnumerable<IMacs> GetMacsData()
        {
            var fullFilePath = $"{Globals.RootDir}MACS/{fileName}";
            var macsCollection = new List<IMacs>();

            if (File.Exists(fullFilePath))
            {
                FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);

                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line = "";
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var str = line.Split(',');
                        var s1 = str[1].Trim();
                        var s4 = str[4].Trim();
                        var s5 = str[5].Trim();
                        var s6 = str[6].Trim();
                        var za = s4.Split('-');
                        if (string.IsNullOrEmpty(za[1]) || za[1].ToUpper().Contains('M') || za[1].ToUpper().Contains('N')) continue;
                        string elname = za[0];
                        int z = Constants.ElementNames.Select(x => x.ToUpper()).ToList().IndexOf(elname);
                        int a = Convert.ToInt32(za[1].Replace("G", ""));
                        var element = new Element(z, a);
                        var kt = 0.0;
                        try
                        {
                            kt = double.Parse(s5, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            kt = 0.0;
                        }
                        var avgCs = 0.0;
                        try
                        {
                            avgCs = double.Parse(s6, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        var macs = new Macs(element, avgCs, s1, kt);
                        macsCollection.Add(macs);
                    }
                }
            }
            return macsCollection;
        }

        public override IEnumerable<IMacs> GetMacsData(ICollection<IIsotope> isotopes)
        {
            var fullFilePath = $"{Globals.RootDir}MACS/{fileName}";
            var macsCollection = new List<IMacs>();

            if (File.Exists(fullFilePath))
            {
                FileStream fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);

                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line = "";
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var str = line.Split(',');
                        var s1 = str[1].Trim();
                        var s4 = str[4].Trim();
                        var s5 = str[5].Trim();
                        var s6 = str[6].Trim();
                        var za = s4.Split('-');
                        if (string.IsNullOrEmpty(za[1]) || za[1].ToUpper().Contains('M') || za[1].ToUpper().Contains('N')) continue;
                        string elname = za[0];
                        int z = Constants.ElementNames.Select(x => x.ToUpper()).ToList().IndexOf(elname);
                        int a = Convert.ToInt32(za[1].Replace("G", ""));
                        if (!isotopes.Any(x => x.A == a && x.Z == z))
                        {
                            continue;
                        }
                        var element = new Element(z, a);
                        var kt = 0.0;
                        try
                        {
                            kt = double.Parse(s5, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            kt = 0.0;
                        }
                        var avgCs = 0.0;
                        try
                        {
                            avgCs = double.Parse(s6, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        var macs = new Macs(element, avgCs, s1, kt);
                        macsCollection.Add(macs);
                    }
                }
            }
            return macsCollection;
        }
    }
}
