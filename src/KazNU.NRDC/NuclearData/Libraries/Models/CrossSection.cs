using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <inheritdoc/>
    internal class CrossSectionData : ICrossSectionData
    {
        private IList<ICrossSectionValue> _crossSectionValues = new List<ICrossSectionValue>();

        public CrossSectionData(int id)
        {
            Id = id;
        }

        /// <inheritdoc/>
        public int Id { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<ICrossSectionValue> CrossSectionValues => _crossSectionValues;

        /// <inheritdoc/>
        public REACT Type => REACTIONTYPE[Id];

        /// <inheritdoc/>
        public string Name => Type.ToString();

        /// <inheritdoc/>
        public double AvgCs { get; set; }

        /// <inheritdoc/>
        public IReaction Reaction { get; }

        /// <inheritdoc/>
        public void AddValue(ICrossSectionValue crossSectionValue)
        {
            _crossSectionValues.Add(crossSectionValue);
        }
    }
}
