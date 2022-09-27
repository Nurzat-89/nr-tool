using NuclearCalculation;
using NuclearCalculation.Matrix;
using NuclearData;
using System;
using System.Linq;

namespace TestNuclearData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BurnUpTest();

            Console.ReadLine();
        }

        private static void BurnUpTest() 
        {
            IEndf endf = new EndfB();
            IMacsEndf endfMacs = new EndfBMacs();
            var isoopes = endf.GetIsotopes(81206, 81207, 82204, 82205, 82206, 82207, 82208, 82209, 82210, 82211, 83209, 83210, 83211, 84210, 84211);
            INeutronSpectra spectra = new NeutronSpectra(30000, 1E16);
            NeutronSpectra.SetMacsCrossSection(isoopes, endfMacs.GetMacsData());
            IBurnUp burnUp = new BurnUp(isoopes, spectra);


            IMatrixExp matrixExp = new PADE();
            var initial = isoopes.FirstOrDefault(x => x.Z == 83 && x.A == 209);
            IBurnUpProcess burnUpProcess = new BurnUpProcess(burnUp, matrixExp, new NuclideDensity(initial, 1.0));
            burnUpProcess.SetAvgCrossSections(endfMacs);

            foreach (var item in burnUp.Isotopes)
            {
                Console.WriteLine($"{item.Name}\t{item.GetCrossSection(Constants.REACT.N_G)?.AvgCs ?? 0}");
            }

            for (int i = 0; i < burnUp.Matrix.Col; i++)
            {
                for (int j = 0; j < burnUp.Matrix.Row; j++)
                {
                    Console.Write($"{burnUp.Matrix.Array[i, j]}\t\t");
                }
                Console.WriteLine();
            }

            var finalDensities = burnUpProcess.Calculate(TimeSpan.FromDays(600000));

            foreach (var dens in finalDensities)
            {
                if (dens.Density == 0) continue;
                Console.WriteLine($"{dens.Isotope.Name} - {dens.Density.ToString("F5")}");
            }
        }

        private void NuclearDataTest() 
        {
            IEndf endf = new EndfB();
            IMacsEndf endfMacs = new EndfBMacs();
            var macsData = endfMacs.GetMacsData();
            var isoopes = endf.GetIsotopes(82, 84);
            INeutronSpectra spectra = new NeutronSpectra(30000, 10E14);
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
                if (avg != 0 || avgMacs != 0) Console.WriteLine($"{isotope.Name}-{avg}-{avgMacs}");
                //Console.WriteLine($"{isotope.Name}\t{isotope.A}\t{string.Join(" : ", isotope.Decays.Keys)}\t{avg.ToString("F3")} - {avgMacs.ToString("F3")}\t{isotope.HalfLife}");
            }

            Console.ReadLine();
        }

        private static void InverseMatrixTest() 
        {
            IMatrix<double> matrix = new MatrixDouble(4, 4);
            matrix.Array[0, 0] = 7; matrix.Array[0, 1] = 8; matrix.Array[0, 2] = 9; matrix.Array[0, 3] = 5;
            matrix.Array[1, 0] = 6; matrix.Array[1, 1] = 9; matrix.Array[1, 2] = 8; matrix.Array[1, 3] = 2;
            matrix.Array[2, 0] = 4; matrix.Array[2, 1] = 3; matrix.Array[2, 2] = 6; matrix.Array[2, 3] = 8;
            matrix.Array[3, 0] = 1; matrix.Array[3, 1] = 9; matrix.Array[3, 2] = 3; matrix.Array[3, 3] = 7;
            var inv = matrix.Inverse();

            for (int i = 0; i < inv.Col; i++)
            {
                for (int j = 0; j < inv.Row; j++)
                {
                    Console.Write($"{inv.Array[i, j]}\t\t");
                }
                Console.WriteLine();
            }

        }
    }
}
