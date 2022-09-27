using System;
using System.Numerics;

namespace NuclearCalculation
{
    /// <summary>
    /// Base abstract class for matrix
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    [Serializable]
    public abstract class Matrix<T> : IMatrix<T> where T : struct
    {

        #region constructors

        /// <inheritdoc/>
        public Matrix(int col, int row)
        {
            Col = col;
            Row = row;
            Array = new T[col, row];
        }

        /// <inheritdoc/>
        public Matrix() { }

        #endregion

        #region public properties

        /// <inheritdoc/>
        public T[,] Array { get; set; }

        /// <inheritdoc/>
        public int Row { get; private set; }

        /// <inheritdoc/>
        public int Col { get; private set; }

        #endregion

        #region public methods

        /// <inheritdoc/>
        public abstract IMatrix<T> Clone();

        /// <inheritdoc/>
        public abstract void Zero();

        /// <inheritdoc/>
        public abstract T MaxValueAbs();

        /// <inheritdoc/>
        public abstract T MinValueAbs();

        /// <inheritdoc/>
        public abstract T Sum();

        /// <inheritdoc/>
        public abstract void Normalize();

        /// <inheritdoc/>
        public abstract void RemoveMinuses();

        /// <inheritdoc/>
        public abstract IMatrix<T> Unity();
        
        /// <inheritdoc/>
        public abstract IMatrix<T> Pow(int p);
        
        /// <inheritdoc/>
        public abstract IMatrix<T> Inverse();

        /// <inheritdoc/>
        public abstract IMatrix<T> Add(IMatrix<T> A);

        /// <inheritdoc/>
        public abstract IMatrix<T> Substract(IMatrix<T> A);

        /// <inheritdoc/>
        public abstract IMatrix<T> Multiply(IMatrix<T> A);

        /// <inheritdoc/>
        public abstract IMatrix<T> Multiply(T k);

        /// <inheritdoc/>
        public abstract IMatrix<T> Devide(T k);

        /// <inheritdoc/>
        public abstract IMatrix<T> Devide(IMatrix<T> A);

        /// <inheritdoc/>
        public Matrix<T2> Cast<T2>() where T2 : struct
        {
            var matrixType = Globals.MatrixTypes[typeof(T2)];
            var instance = Activator.CreateInstance(matrixType) as Matrix<T2>;
            instance.SetMatrix(Col, Row);
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    dynamic value = null;
                    if (typeof(T2) == typeof(Complex))
                    {
                        value = new Complex((double)Convert.ChangeType(Array[i, j], typeof(double)), 0.0);
                    }
                    if (typeof(T2) == typeof(double))
                    {
                        value = ((Complex)Convert.ChangeType(Array[i, j], typeof(Complex))).Real;
                    }
                    if (value != null)
                        instance.Array[i, j] = (T2)Convert.ChangeType(value, typeof(T2));
                }
            }
            return instance;
        }

        private void SetMatrix(int col, int row)
        {
            Col = col;
            Row = row;
            Array = new T[col, row];
        }

        #endregion

        #region static methods

        public static IMatrix<T> operator +(Matrix<T> A, IMatrix<T> B)
        {
            return A.Add(B);
        }
        
        public static IMatrix<T> operator -(Matrix<T> A, IMatrix<T> B)
        {
            return A.Substract(B);
        }
        
        public static IMatrix<T> operator *(Matrix<T> A, IMatrix<T> B)
        {
            return A.Multiply(B);
        }
        
        public static IMatrix<T> operator /(Matrix<T> A, IMatrix<T> B)
        {
            return A.Devide(B);
        }

        public static IMatrix<T> operator *(Matrix<T> A, T k)
        {
            return A.Multiply(k);
        }
        
        public static IMatrix<T> operator /(Matrix<T> A, T k)
        {
            return A.Devide(k);
        }

        #endregion

    }
}
