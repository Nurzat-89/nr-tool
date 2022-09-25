using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NuclearData.Libraries
{
    /// <summary>
    /// Nuclide data reader
    /// </summary>
    internal class AtomDataReader : INuclearDataReader<IAtom>
    {
        /// <inheritdoc/>
        public int MF => 8;

        /// <inheritdoc/>
        public int MT => 457;

        /// <inheritdoc/>
        public Constants.FILETYP ReactionType => Constants.FILETYP.ATOM;

        /// <inheritdoc/>
        public IEnumerable<IAtom> ReadData(int Z, int A, string fileName)
        {
            if (File.Exists(fileName))
            {
                return null;
            }

            IAtom atomicData;

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                EndfHelper.GetLineFromStream(streamReader, MF, MT, out string line);
                Record r = EndfHelper.GetRecord(line);
                
                line = streamReader.ReadLine();
                r = EndfHelper.GetRecord(line);
                atomicData = new Atom(r.c2, r.c1 == 0.0 ? Constants.STABLE : r.c1);
            }
            return new List<IAtom>() { atomicData };
        }
    }
}
