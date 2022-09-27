using System;

namespace NuclearCalculation
{
    /// <summary>
    /// Double matrix class
    /// </summary>
    [Serializable]
    public class MatrixDouble : Matrix<double>
    {

        /// <inheritdoc/>
        public MatrixDouble(int col, int row) : base(col, row)
        {

        }

        /// <inheritdoc/>
        public MatrixDouble() { }

        /// <inheritdoc/>
        public override IMatrix<double> Add(IMatrix<double> A)
        {
            MatrixDouble result = new MatrixDouble(A.Col, A.Row);
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
        public override IMatrix<double> Devide(double k)
        {
            MatrixDouble result = new MatrixDouble(this.Col, this.Row);
            for (int i = 0; i < Col; i++)
                for (int j = 0; j < Row; j++)
                    result.Array[i, j] = Array[i, j] / k;
            return result;
        }

        /// <inheritdoc/>
        public override IMatrix<double> Multiply(IMatrix<double> B)
        {
            MatrixDouble result = new MatrixDouble(this.Col, B.Row);
            if (this.Row == B.Col)
            {
                result.Array = Accord.Math.Matrix.Dot(Array, B.Array);
                return result;
            }
            else
            {
                throw new Exception("These matrices cannot be multiplied");
            }
        }

        /// <inheritdoc/>
        public override IMatrix<double> Multiply(double k)
        {
            MatrixDouble result = new MatrixDouble(Col, Row);
            for (int i = 0; i < Col; i++)
                for (int j = 0; j < Row; j++)
                    result.Array[i, j] = Array[i, j] * k;
            return result;
        }

        /// <inheritdoc/>
        public override IMatrix<double> Substract(IMatrix<double> B)
        {
            MatrixDouble result = new MatrixDouble(B.Col, B.Row);
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
        public override IMatrix<double> Unity()
        {
            var unity = new MatrixDouble(this.Col, this.Row);
            for (int i = 0; i < Col; i++)
            {
                unity.Array[i, i] = 1.0;
            }
            return unity;
        }

        /// <inheritdoc/>
        public override IMatrix<double> Pow(int p)
        {
            IMatrix<double> result = new MatrixDouble(Col, Row);

            result = result.Unity();

            if (p == 0) { }
            else if (p == 1) result = Clone();
            else
            {
                result = Clone();
                for (int i = 1; i < p; i++)
                    result = (MatrixDouble) result * this;
            }
            return result;
        }

        /// <inheritdoc/>
        public override IMatrix<double> Devide(IMatrix<double> A)
        {
            var result = A.Inverse();
            result = this * result;
            return result;
        }

        /// <inheritdoc/>
        public override IMatrix<double> Inverse()
        {
            Matrix<double> X = new MatrixDouble(Col, Row);
            X.Array = Accord.Math.Matrix.Inverse(Array);
            return X;
        }

        /// <inheritdoc/>
        public override void Zero()
        {
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    Array[i, j] = 0.0;
                }
            }
        }

        /// <inheritdoc/>
        public override double MaxValueAbs()
        {
            double max = Math.Abs(Array[0, 0]);
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    if (Math.Abs(Array[i, j]) > max)
                        max = Array[i, j];
                }
            }
            return max;
        }

        /// <inheritdoc/>
        public override double MinValueAbs()
        {
            double min = Math.Abs(Array[0, 0]);
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    if (Math.Abs(Array[i, j]) < min)
                        min = Array[i, j];
                }
            }
            return min;
        }

        /// <inheritdoc/>
        public override IMatrix<double> Clone()
        {
            Matrix<double> result = new MatrixDouble(Col, Row);
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
        public override double Sum()
        {
            double sum = 0.0;
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
            for (int i = 0; i < Col; i++)
            {
                for (int j = 0; j < Row; j++)
                {
                    if (Array[i, j] < 0.0) Array[i, j] = 0.0;
                }
            }
        }
    }
}
