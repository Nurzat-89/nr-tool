namespace NuclearCalculation
{
    /// <summary>
    /// Interface for Matrix manipulation
    /// </summary>
    /// <typeparam name="T">Type of elements in matrix</typeparam>
    public interface IMatrix<T> where T : struct
    {
        /// <summary>
        /// Matrix row number
        /// </summary>
        int Row { get; }

        /// <summary>
        /// Matrix column number
        /// </summary>
        int Col { get; }

        /// <summary>
        /// Two dimension matrix array 
        /// </summary>
        T[,] Array { get; set; }

        /// <summary>
        /// Method to clone matrix returns new object
        /// </summary>
        IMatrix<T> Clone();

        /// <summary>
        /// Method to case matrix elements
        /// </summary>
        IMatrix<T1> Cast<T1>() where T1 : struct;
        
        /// <summary>
        /// Set all elemnts in matrix to zero
        /// </summary>
        void Zero();

        /// <summary>
        /// Get maximun value of matrix elements
        /// </summary>
        T MaxValueAbs();

        /// <summary>
        /// Get minimum value of matrix elements
        /// </summary>
        T MinValueAbs();

        /// <summary>
        /// Get sum of elements
        /// </summary>
        T Sum();

        /// <summary>
        /// Mormalize by one
        /// </summary>
        void Normalize();

        /// <summary>
        /// Set negative values to zero of elements
        /// </summary>
        void RemoveMinuses();

        /// <summary>
        /// Get Unity matrix
        /// </summary>
        IMatrix<T> Unity();
        
        /// <summary>
        /// Power of matrix by p value
        /// </summary>
        IMatrix<T> Pow(int p);
        
        /// <summary>
        /// Method to inverse matrix
        /// </summary>
        IMatrix<T> Inverse();

        IMatrix<T> Add(IMatrix<T> A);

        IMatrix<T> Substract(IMatrix<T> A);

        IMatrix<T> Multiply(IMatrix<T> A);

        IMatrix<T> Multiply(T k);

        IMatrix<T> Devide(T k);

        IMatrix<T> Devide(IMatrix<T> A);


    }
}
