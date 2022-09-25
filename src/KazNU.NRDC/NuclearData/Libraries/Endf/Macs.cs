namespace NuclearData
{
        /// <inheritdoc/>
    internal class Macs : IMacs
    {
        public Macs(Element element, double avgCs, string dataLib, double kT)
        {
            Element = element;
            AvgCs = avgCs;
            DataLib = dataLib;
            this.kT = kT;
        }

        /// <inheritdoc/>
        public Element Element { get; }

        /// <inheritdoc/>
        public double AvgCs { get; }

        /// <inheritdoc/>
        public string DataLib { get; }

        /// <inheritdoc/>
        public double kT { get; }
    }
}
