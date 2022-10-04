using System;
using System.Numerics;

namespace NuclearCalculation.Matrix
{
    /// <summary>
    /// CRAM method to calculate matrix exponent (VTT in Finland)
    /// </summary>
    public class CRAM : IMatrixExp
    {
        /// <inheritdoc/>
        public event Action<int> CalculationStatusChangedEvent;

        /// <inheritdoc/>
        public IMatrix<double> Calculate(IMatrix<double> a, IMatrix<double> n)
        {
            IMatrix<Complex> U = new MatrixComplex(a.Col, a.Row);
            IMatrix<Complex> N = new MatrixComplex(n.Col, n.Row);
            //Matrix<Complex> N = n * Globals.Alpha[0].Real;           
            var aa = a.Cast<Complex>();
            var nn = n.Cast<Complex>();
            U = U.Unity();
            double dx = 100 / 7;
            for (int i = 1; i <= 7; i++)
            {
                var temp = (MatrixComplex)aa - ((MatrixComplex)U * Globals.Theta[i]);
                var temp1 = temp.Inverse();
                var _n = (MatrixComplex)((MatrixComplex)temp1 * nn) * Globals.Alpha[i];
                N = (MatrixComplex)N + _n;
                CalculationStatusChangedEvent?.Invoke((int)dx * i);
            }
            IMatrix<double> result = N.Cast<double>();
            result = (MatrixDouble)result * 2;
            result = (MatrixDouble)result + (MatrixDouble)((MatrixDouble)n * Globals.Alpha[0].Real);
            result.RemoveMinuses();
            return result;
        }
    }
}
