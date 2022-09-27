using System;
using System.Numerics;

namespace NuclearCalculation
{
    /// <summary>
    /// Complex matrix class
    /// </summary>
    [Serializable]
    public class MatrixComplex : Matrix<Complex>
    {
        /// <inheritdoc/>
        public MatrixComplex(int col, int row) : base(col, row)
        {

        }
        /// <inheritdoc/>
        public MatrixComplex()
        {

        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Pow(int p)
        {
            IMatrix<Complex> result = new MatrixComplex(Col, Row);

            result = result.Unity();

            if (p == 0) { }
            else if (p == 1) result = Clone();
            else
            {
                result = Clone();
                for (int i = 1; i < p; i++)
                    result = (MatrixComplex) result * this;
            }
            return result;

        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Add(IMatrix<Complex> A)
        {
            MatrixComplex result = new MatrixComplex(A.Col, A.Row);
            if (A.Col != this.Col || A.Row != this.Row)
            {
                throw new Exception("These matrices is not equal");
            }
            else
            {
                for (int i = 0; i < A.Col; i++)
                    for (int j = 0; j < A.Row; j++)
                        result.Array[i, j] = this.Array[i, j] + A.Array[i, j];
                return result;
            }
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Devide(Complex k)
        {
            MatrixComplex result = new MatrixComplex(Col, Row);
            for (int i = 0; i < Col; i++)
                for (int j = 0; j < Row; j++)
                    result.Array[i, j] = Array[i, j] / k;
            return result;
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Multiply(IMatrix<Complex> B)
        {
            MatrixComplex result = new MatrixComplex(this.Col, B.Row);
            if (this.Row == B.Col)
            {
                for (int i = 0; i < this.Col; i++)
                    for (int j = 0; j < B.Row; j++)
                    {
                        Complex sum = 0.0;
                        for (int k = 0; k < this.Row; k++)
                        {
                            sum += this.Array[i, k] * B.Array[k, j];
                        }
                        result.Array[i, j] = sum;
                    }
                return result;
            }
            else
            {
                throw new Exception("These matrices cannot be multiplied");
            }
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Multiply(Complex k)
        {
            MatrixComplex result = new MatrixComplex(Col, Row);
            for (int i = 0; i < Col; i++)
                for (int j = 0; j < Row; j++)
                    result.Array[i, j] = Array[i, j] * k;
            return result;
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Substract(IMatrix<Complex> B)
        {
            MatrixComplex result = new MatrixComplex(B.Col, B.Row);
            if (B.Col != this.Col || B.Row != this.Row)
            {
                throw new Exception("These matrices is not equal");
            }
            else
            {
                for (int i = 0; i < B.Col; i++)
                    for (int j = 0; j < B.Row; j++)
                        result.Array[i, j] = this.Array[i, j] - B.Array[i, j];
                return result;
            }
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Unity()
        {
            var unity = new MatrixComplex(this.Col, this.Row);
            for (int i = 0; i < Col; i++)
            {
                unity.Array[i, i] = new Complex(1.0, 0.0);
            }
            return unity;
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Devide(IMatrix<Complex> A)
        {
            var result = A.Inverse();
            result = this * result;
            return result;
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Inverse()
        {
            Matrix<Complex> X = new MatrixComplex(Col, Row);
            MathNet.Numerics.LinearAlgebra.MatrixBuilder<Complex> matBuild = MathNet.Numerics.LinearAlgebra.Matrix<Complex>.Build;
            var matrix = matBuild.DenseOfArray(Array).Inverse();
            X.Array = matrix.ToArray();
            return X;
        }

        /// <inheritdoc/>
        public override void Zero()
        {
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    Array[i, j] = new Complex(0.0, 0.0);
                }
            }
        }

        /// <inheritdoc/>
        public override Complex MaxValueAbs()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Complex MinValueAbs()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IMatrix<Complex> Clone()
        {
            Matrix<Complex> result = new MatrixComplex(Col, Row);
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    result.Array[i, j] = Array[i, j];
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public override Complex Sum()
        {
            Complex sum = 0.0;
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    sum += Array[i, j];
                }
            }
            return sum;
        }

        /// <inheritdoc/>
        public override void Normalize()
        {
            var sum = Sum();
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    Array[i, j] = Array[i, j] / sum;
                }
            }
        }

        /// <inheritdoc/>
        public override void RemoveMinuses()
        {
            throw new NotImplementedException();
        }
    }
}
