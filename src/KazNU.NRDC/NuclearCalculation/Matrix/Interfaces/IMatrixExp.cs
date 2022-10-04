using System;

namespace NuclearCalculation
{
    /// <summary>
    /// Interface for matrix exponential calculation
    /// </summary>
    public interface IMatrixExp
    {
        /// <summary>
        /// Method to calculate matrix exponent
        /// </summary>
        /// <param name="a">A square matrix</param>
        /// <param name="n">N vector</param>
        IMatrix<double> Calculate(IMatrix<double> a, IMatrix<double> n);

        /// <summary>
        /// Event triggered when calculation progress was changed
        /// </summary>
        event Action<int> CalculationStatusChangedEvent;
    }
}
