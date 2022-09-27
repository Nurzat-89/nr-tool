using NuclearData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NuclearCalculation
{
    /// <inheritdoc/>
    public class BurnUpProcess : IBurnUpProcess
    {
        private int _nuclidesCount;
        private List<INuclideDensity> _initialDensities;

        /// <inheritdoc/>
        public BurnUpProcess(IBurnUp burnUp, IMatrixExp matrixExp, params INuclideDensity[] initialDensities)
        {
            BurnUp = burnUp;
            MatrixExp = matrixExp;
            _initialDensities = new List<INuclideDensity>();
            foreach (var isotope in BurnUp.Isotopes)
            {
                var density = initialDensities.FirstOrDefault(x => x.Isotope.ZAID == isotope.ZAID);
                _initialDensities.Add(new NuclideDensity(isotope, density?.Density ?? 0));
            }
            _nuclidesCount = _initialDensities.Count;
        }

        /// <inheritdoc/>
        public IBurnUp BurnUp { get; }

        /// <inheritdoc/>
        public IMatrixExp MatrixExp { get; }

        /// <inheritdoc/>
        public IEnumerable<INuclideDensity> InitialDensities => _initialDensities;

        /// <inheritdoc/>
        public IEndf EndfLibrary { get; }

        /// <inheritdoc/>
        public IEnumerable<INuclideDensity> Calculate(TimeSpan time)
        {
            var matrix = BurnUp.Matrix;
            var density = MatrixExp.Calculate((MatrixDouble)matrix * time.TotalSeconds, DensityToMatix(InitialDensities));
            density.Normalize();
            return MatrixToDensity(density);
        }

        private IMatrix<double> DensityToMatix(IEnumerable<INuclideDensity> densities) 
        {
            int i = 0;
            IMatrix<double> matrix = new MatrixDouble(_nuclidesCount, 1);
            foreach (var density in densities)
            {
                matrix.Array[i, 0] = density.Density;
                i++;
            }
            return matrix;
        }

        private IEnumerable<INuclideDensity> MatrixToDensity(IMatrix<double> matrix)
        {
            var densityList = new List<INuclideDensity>();
            for (int i = 0; i < matrix.Col; i++)
            {
                densityList.Add(new NuclideDensity(_initialDensities[i].Isotope, matrix.Array[i, 0]));
            }
            return densityList;
        }
    }
}
