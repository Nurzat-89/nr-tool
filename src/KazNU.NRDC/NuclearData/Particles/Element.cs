namespace NuclearData
{
    public class Element
    {
        public int Z { get; }

        public int A { get; }

        public int ZAID => Z * 1000 + A;

        public string Name => Constants.ElementTableNames[Z];

        public Element(int z, int a)
        {
            Z = z;
            A = a;
        }
    }
}
