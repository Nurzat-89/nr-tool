using NuclearData;
using System;
using System.Linq;

namespace TestNuclearData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IEndf endf = new EndfB();
            IMacsEndf endfMacs = new EndfBMacs();
            var macsData = endfMacs.GetMacsData();
            var isoopes = endf.GetIsotopes(82,84);
            INeutronSpectra spectra = new NeutronSpectra(30000);
            foreach (var isotope in isoopes)
            {
                if (isotope == null)
                    continue;
                double avg = 0;
                double avgMacs = 0;
                //Console.WriteLine($"{isotope.Element.Name}\t{isotope.kT}\t{isotope.AvgCs}");
                if (isotope.CrossSections.Any(x => x.Key == Constants.REACT.N_G))
                {
                    avg = spectra.OneGroupCrossSection(isotope.GetCrossSection(Constants.REACT.N_G));
                    avgMacs = macsData.FirstOrDefault(x => x.Element.ZAID == isotope.ZAID)?.AvgCs ?? 0;
                    //isotope.CrossSections.FirstOrDefault(x => x.Key == Constants.REACT.N_G).Value.AvgCs = macsData.FirstOrDefault(x => x.Element.ZAID == isotope.ZAID)?.AvgCs ?? 0;

                }
                if(avg!=0 || avgMacs !=0) Console.WriteLine($"{isotope.Name}-{avg}-{avgMacs}");
                //Console.WriteLine($"{isotope.Name}\t{isotope.A}\t{string.Join(" : ", isotope.Decays.Keys)}\t{avg.ToString("F3")} - {avgMacs.ToString("F3")}\t{isotope.HalfLife}");
            }

            Console.ReadLine();
        }
    }
}
