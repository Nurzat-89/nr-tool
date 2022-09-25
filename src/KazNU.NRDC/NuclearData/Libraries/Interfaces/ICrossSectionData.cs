using System.Collections.Generic;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Interface model for cross section data by type
    /// </summary>
    public interface ICrossSectionData
    {
        /// <summary>
        /// Id of cross section 
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// List of cross section values
        /// </summary>
        IEnumerable<ICrossSectionValue> CrossSectionValues { get; }
               
        /// <summary>
        /// Reaction type
        /// </summary>
        REACT Type { get; }
        
        /// <summary>
        /// Name of cross section
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Average cross section in mb
        /// </summary>
        double AvgCs { get; }
        
        /// <summary>
        /// Cross section reaction
        /// </summary>
        IReaction Reaction { get; }

        /// <summary>
        /// Method to add cross section values
        /// </summary>
        void AddValue(ICrossSectionValue crossSectionValue);
    }
}
