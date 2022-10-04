using System;

namespace NuclearCalculation.Matrix
{
    /// <summary>
    /// MMPA method to calculate matrix exponent (Hokkaido University)
    /// </summary>
    public class MMPA : IMatrixExp
    {
        /// <inheritdoc/>
        public event Action<int> CalculationStatusChangedEvent;

        /// <inheritdoc/>
        public IMatrix<double> Calculate(IMatrix<double> a, IMatrix<double> n)
        {
            int order = 12;
            IMatrix<double> U = new MatrixDouble(a.Col, a.Row);
            IMatrix<double> S = new MatrixDouble(a.Col, a.Row);
            U = U.Unity();
            S = S.Unity();
            U = (MatrixDouble)U * 11.88124;
            var Inv = ((MatrixDouble)a - U).Inverse();
            var At = (MatrixDouble)((MatrixDouble)a + U) * Inv;
            var N = (MatrixDouble)n * Globals.MMPA_a12[0];
            var dx = 100.0 / (order + 1.0);
            for (int i = 1; i <= order; i++)
            {
                S = (MatrixDouble)At * S;
                N = (MatrixDouble)N + (MatrixDouble)((MatrixDouble)S * n) * Globals.MMPA_a12[i];
                CalculationStatusChangedEvent?.Invoke((int)dx * i);
            }
            N.RemoveMinuses();

            return N;
        }
    }
}
