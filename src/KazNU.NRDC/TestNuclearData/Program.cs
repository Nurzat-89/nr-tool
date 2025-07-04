﻿using NuclearCalculation;
using NuclearCalculation.Matrix;
using NuclearData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestNuclearData
{
    internal class Program
    {
        static void Main(string[] args)
        {

            double val = 1000;

            var str = val.ToString("E1");

            double[] fluxes = new double[] { 1E8, 1E10, 1E12, 1E14, 1E16, 1E18, 1E20, 1E22, 1E24, 1E26, 1E28 };

            var dir = "E://CatalysisResults";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            IEndf endf = new EndfB();
            IMacsEndf endfMacs = new EafMacs();
            var isotopes = endf.GetIsotopes(81206, 81207, 82204, 82205, 82206, 82207, 82208, 82209, 82210, 82211, 83209, 83210, 83211, 84210, 84211);
            var macsList = endfMacs.GetMacsData(isotopes.ToList());

            HeatDensityTest();

            //foreach (var flux in fluxes)
            //{
            //    TimeMeshTest(flux, $"{dir}/time_mesh_density_endfb_{flux:E2}.txt");
            //}

            //TimeMeshTest();
            //HeatDensityTest();
            Console.WriteLine("Finish");
            Console.ReadLine();
        }

        private static void HeatDensityTest() 
        {
            //double[] fluxes = new double[] { 1E8, 1E10, 1E12, 1E14, 1E16, 1E18, 1E20, 1E22, 1E24, 1E26, 1E28 };

            Dictionary<double, double> fluxWithTime = new Dictionary<double, double>()
            {
                { 1E8, 1E20 },
                { 1E10, 1E18 },
                { 1E12, 1E16 },
                { 1E14, 1E14 },
                { 1E16, 1E12 },
                { 1E18, 1E10 },
                { 1E20, 1E8 },
                { 1E22, 1E8 },
                { 1E24, 1E8 },
                { 1E26, 1E8 },
                { 1E28, 1E8 },
            };

            IEndf endf = new EndfB();
            IMacsEndf endfMacs = new EafMacs();
            var isotopes = endf.GetIsotopes(81206, 81207, 82204, 82205, 82206, 82207, 82208, 82209, 82210, 82211, 83209, 83210, 83211, 84210, 84211);
            var macsList = endfMacs.GetMacsData(isotopes.ToList());
            var initial = isotopes.FirstOrDefault(x => x.Z == 82 && x.A == 206);

            IMatrixExp matrixExp = new MMPA();

            foreach (var flux in fluxWithTime)
            {
                INeutronSpectra spectra = new NeutronSpectra(0.0253, flux.Key);
                NeutronSpectra.SetMacsCrossSection(isotopes, macsList, spectra);
                IBurnUp burnUp = new BurnUp(isotopes, spectra);

                IBurnUpProcess burnUpProcess = new BurnUpProcess(burnUp, matrixExp, new NuclideDensity(initial, 1.0, Constants.NaturalLeadDensity, 208));
                burnUpProcess.SetAvgCrossSections(endfMacs);
                var finalDensities = burnUpProcess.Calculate(flux.Value);

                foreach (var dens in finalDensities)
                {
                    dens.CalculateHeat(Constants.NaturalLeadDensity, 208);
                    if (dens.Isotope.A == 211 && dens.Isotope.Z == 84)
                    {
                        Console.WriteLine($"{dens.Isotope.Name}\t{dens.Density}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
                var heat = finalDensities.Sum(x => x.HeatDensity);// * 1.6E-19;
                Console.WriteLine($"{flux.Key}\t{heat}");

                foreach (var final in finalDensities)
                {
                    //Console.WriteLine($"\t{final.NuclideName}-{final.Density}");
                }
                Console.WriteLine("");
                System.IO.File.AppendAllText($"E://CatalysisResults/heat_density_long_jeff.txt", $"{flux.Key}\t{heat}\n");
            }
        }

        public static void TimeMeshTest(double flux, string filename)
        {
            int intervalCount = 20;
            Dictionary<double, IEnumerable<INuclideDensity>> zaidDensities = new Dictionary<double, IEnumerable<INuclideDensity>>();

            //double flux = 1E12;
            double time = 1;
            //var filename = $"E://mesh_density_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.txt";
            Console.WriteLine($"Started for flux: {flux}");
            for (int i = 0; i < intervalCount; i++)
            {
                IEndf endf = new EndfB();
                IMacsEndf endfMacs = new EndfBMacs();
                var isoopes = endf.GetIsotopes(81206, 81207, 82204, 82205, 82206, 82207, 82208, 82209, 82210, 82211, 83209, 83210, 83211, 84210, 84211);
                INeutronSpectra spectra = new NeutronSpectra(0.0253, flux);
                NeutronSpectra.SetMacsCrossSection(isoopes, endfMacs.GetMacsData(), spectra);
                IBurnUp burnUp = new BurnUp(isoopes, spectra);

                IMatrixExp matrixExp = new MMPA();
                var initial = isoopes.FirstOrDefault(x => x.Z == 82 && x.A == 206);
                IBurnUpProcess burnUpProcess = new BurnUpProcess(burnUp, matrixExp, new NuclideDensity(initial, 1.0, Constants.NaturalLeadDensity, 206));
                burnUpProcess.SetAvgCrossSections(endfMacs);
                var currTime = time * Math.Pow(10, i);
                var finalDensities = burnUpProcess.Calculate(currTime);
                zaidDensities.Add(i, finalDensities);
                
                foreach (var nuclideDensity in finalDensities)
                {
                    if (i == 19)
                    {
                        Console.WriteLine($"{nuclideDensity.Isotope.Name}\tDensity={nuclideDensity.Density}");
                    }
                }
                Console.WriteLine($"\tTime: {currTime}");
            }
            Console.WriteLine($"Finished flux: {flux}");
            Console.WriteLine();

            System.IO.File.AppendAllText(filename, $"Time sec");
            foreach (var item in zaidDensities.Values.SelectMany(x => x).Select(x => x.NuclideName).Distinct())
            {
                
                System.IO.File.AppendAllText(filename, $"\t{item}");
            }

            System.IO.File.AppendAllText(filename, $"\n");
            for (int i = 0; i < intervalCount; i++)
            {
                var currTime = time * Math.Pow(10, i);
                System.IO.File.AppendAllText(filename, $"{currTime}");
                foreach (var item in zaidDensities[i])
                {
                    System.IO.File.AppendAllText(filename, $"\t{item.Density}");
                }
                System.IO.File.AppendAllText(filename, $"\n");
            }
        }

        private static void BurnUpTest() 
        {
            IEndf endf = new EndfB();
            IMacsEndf endfMacs = new EndfBMacs();
            var isoopes = endf.GetIsotopes(81206, 81207, 82204, 82205, 82206, 82207, 82208, 82209, 82210, 82211, 83209, 83210, 83211, 84210, 84211);
            INeutronSpectra spectra = new NeutronSpectra(30000, 1E16);
            NeutronSpectra.SetMacsCrossSection(isoopes, endfMacs.GetMacsData(), spectra);
            IBurnUp burnUp = new BurnUp(isoopes, spectra);


            IMatrixExp matrixExp = new PADE();
            var initial = isoopes.FirstOrDefault(x => x.Z == 83 && x.A == 209);
            IBurnUpProcess burnUpProcess = new BurnUpProcess(burnUp, matrixExp, new NuclideDensity(initial, 1.0, Constants.NaturalLeadDensity, 208));
            burnUpProcess.SetAvgCrossSections(endfMacs);

            foreach (var item in burnUp.Isotopes)
            {
                Console.WriteLine($"{item.Name}\t{item.AvgMacsCs}");
            }

            for (int i = 0; i < burnUp.Matrix.Col; i++)
            {
                for (int j = 0; j < burnUp.Matrix.Row; j++)
                {
                    Console.Write($"{burnUp.Matrix.Array[i, j]}\t\t");
                }
                Console.WriteLine();
            }

            var finalDensities = burnUpProcess.Calculate((long)TimeSpan.FromDays(600000).TotalSeconds);

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
