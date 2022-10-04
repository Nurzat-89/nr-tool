using System;

namespace NuclearCalculation.Matrix
{
    /// <summary>
    /// Padé approximation
    /// </summary>
    public class PADE : IMatrixExp
    {
        /// <inheritdoc/>
        public event Action<int> CalculationStatusChangedEvent;

        /// <inheritdoc/>
        public IMatrix<double> Calculate(IMatrix<double> a, IMatrix<double> n)
        {
            IMatrix<double> _exp = a.Clone();

            int counter = 0;
            do
            {
                _exp = (MatrixDouble)_exp / 2.0;
                counter++;
            } while (_exp.MaxValueAbs() >= 0.5);

            _exp = Exponent(_exp);
            var dx = 100.0 / counter;
            for (int i = 0; i < counter; i++)
            {
                if (double.IsInfinity(_exp.Array[9, 9]) || double.IsInfinity(_exp.Array[8, 8]) || double.IsInfinity(_exp.Array[7, 7]))
                {
                    Console.WriteLine("");
                }
                _exp = _exp.Pow(2);
                CalculationStatusChangedEvent?.Invoke((int)dx * (i + 1));
            }
            n = (MatrixDouble)_exp * n;
            return n;
        }

        private IMatrix<double> Exponent(IMatrix<double> a)
        {
            int p = 6;
            int q = 6;
            int col = a.Col;
            int row = a.Row;
            IMatrix<double> N_pq = new MatrixDouble(col, row);
            IMatrix<double> D_pq = new MatrixDouble(col, row);
            IMatrix<double> temp;
            double ff;
            for (int k = 0; k <= p; k++)
            {
                ff = Globals.Factorial(p + q - k) * Globals.Factorial(p) / (Globals.Factorial(p + q) * Globals.Factorial(k) * Globals.Factorial(p - k));
                temp = a.Pow(k);
                N_pq = (MatrixDouble)N_pq + ((MatrixDouble)temp * ff);
            }

            for (int k = 0; k <= q; k++)
            {
                ff = Globals.Factorial(p + q - k) * Globals.Factorial(q) / (Globals.Factorial(p + q) * Globals.Factorial(k) * Globals.Factorial(q - k));
                temp = (MatrixDouble)a * (-1.0);
                temp = temp.Pow(k);
                D_pq = (MatrixDouble)D_pq + ((MatrixDouble)temp * ff);
            }
            temp = (MatrixDouble)N_pq / D_pq;
            return temp;
        }
    }
}
