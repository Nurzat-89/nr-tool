using System.Collections.Generic;
using System.Numerics;
using System;

namespace NuclearCalculation
{
    internal class Globals
    {
        public static Dictionary<Type, Type> MatrixTypes = new Dictionary<Type, Type>() {
            {typeof(double), typeof(MatrixDouble) },
            {typeof(Complex), typeof(MatrixComplex) },
        };
    }
}
