using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <summary>
    /// Base class for endf data file
    /// </summary>
    public abstract class BaseEndf : IEndf
    {
        private INuclearDataReader<IAtom> _atomicDataReader;
        private INuclearDataReader<IDecayData> _decayDataReader;
        private IEnumerable<INuclearDataReader<ICrossSectionValue>> _reactionDataReaderList;
                
        /// <inheritdoc/>
        public BaseEndf(DATALIBS library, string libFolder, string extention)
        {
            Library = library;
            LibFolder = libFolder;
            Extention = extention;
            _atomicDataReader = new AtomDataReader();
            _decayDataReader = new DecayDataReader();
            _reactionDataReaderList = new List<INuclearDataReader<ICrossSectionValue>>()
            {
                new N2NdataReader(),
                new NAlphaDataReader(),
                new NElectronDataReader(),
                new NGammaDataReader(),
                new NProtonDataReader()
            };
        }

        protected abstract string GetIsotopeFile(int Z, int A, FILETYP ftype);

        protected abstract IEnumerable<Element> GetAllElements(FILETYP fileType);

        protected string[] GetFileNames(FILETYP ftype)
        {
            string rtyp = Globals.FileTypeDir[ftype];
            string[] filePaths = Directory.GetFiles($"{Globals.RootDir}{LibFolder}{rtyp}", $"*{Extention}", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < filePaths.Length; i++)
            {
                filePaths[i] = Path.GetFileName(filePaths[i]);
            }
            return filePaths;
        }

        protected string LibFolder { get; }
        
        protected string Extention { get; }

        /// <inheritdoc/>
        public DATALIBS Library { get; }

        /// <inheritdoc/>
        public IEnumerable<IIsotope> GetIsotopes()
        {
            List<IIsotope> isotopes = new List<IIsotope>();
            var elementList = GetAllElements(FILETYP.DECAY);
            foreach (var element in elementList)
            {
                isotopes.Add(CreateIsotope(element.Z, element.A));
            }
            return isotopes;
        }

        /// <inheritdoc/>
        public IEnumerable<IIsotope> GetIsotopes(int Z1, int Z2)
        {
            if (Z1 >= Z2)
            {
                throw new ArgumentOutOfRangeException("Z1 must be less than Z2");
            }
            var elementList = GetAllElements(FILETYP.DECAY);
            List<IIsotope> isotopes = new List<IIsotope>();
            for (int z = Z1; z <= Z2; z++)
            {
                foreach (var element in elementList.Where(_ => _.Z == z))
                {
                    isotopes.Add(CreateIsotope(element.Z, element.A));
                }
            }
            return isotopes;
        }

        /// <inheritdoc/>
        public IEnumerable<IIsotope> GetIsotopes(string n1, string n2)
        {
            var z1 = ElementTableNames.ToList().IndexOf(n1);
            var z2 = ElementTableNames.ToList().IndexOf(n2);

            if (z1 == -1 || z2 == -1)
            {
                throw new ArgumentOutOfRangeException($"Unknown elemnt names: {n1} or {n2}");
            }

            return GetIsotopes(z1, z2);
        }

        /// <inheritdoc/>
        public IEnumerable<IIsotope> GetIsotopes(params int[] zaids)
        {
            List<IIsotope> isotopes = new List<IIsotope>();
            foreach (var zaid in zaids)
            {
                var za = EndfHelper.ConvertZaid(zaid);
                isotopes.Add(CreateIsotope(za.Item1, za.Item2));
            }
            return isotopes;
        }

        private IIsotope CreateIsotope(int z, int a) 
        {
            var nuclideData = _atomicDataReader.ReadData(z, a, GetIsotopeFile(z, a, FILETYP.DECAY));
            if (nuclideData == null)
            {
                return null;
            }

            var decayDataList = _decayDataReader.ReadData(z, a, GetIsotopeFile(z, a, FILETYP.DECAY));
            IIsotope isotope = new Isotope(z, a, nuclideData.FirstOrDefault().AtomicMass, nuclideData.FirstOrDefault().HalfLife);
            if (decayDataList != null && decayDataList.Any())
            {
                foreach (var decaData in decayDataList)
                {
                    if (isotope.Decays.ContainsKey(decaData.DecayType))
                    {
                        continue;
                    }
                    isotope.Decays.Add(decaData.DecayType, decaData);
                }
            }

            foreach (var reactionDataReader in _reactionDataReaderList)
            {
                var reactionDataList = reactionDataReader.ReadData(z, a, GetIsotopeFile(z, a, FILETYP.NEUTRON));
                if (reactionDataList != null && reactionDataList.Any())
                {
                    var crossSectionData = new CrossSectionData(reactionDataList.First().Id);
                    foreach (var reactionData in reactionDataList)
                    {
                        crossSectionData.AddValue(reactionData);
                    }
                    isotope.CrossSections.Add(reactionDataList.First().Type, crossSectionData);
                }
            }

            return isotope;
        }
    }
}
