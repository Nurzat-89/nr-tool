using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NuclearData
{
    /// <summary>
    /// Data reader decay
    /// </summary>
    internal class DecayDataReader : INuclearDataReader<IDecayData>
    {
        /// <inheritdoc/>
        public int MF => 8;

        /// <inheritdoc/>
        public int MT => 457;

        /// <inheritdoc/>
        public Constants.FILETYP ReactionType => Constants.FILETYP.DECAY;

        /// <inheritdoc/>
        public IEnumerable<IDecayData> ReadData(int Z, int A, string fileName)
        {
            if (File.Exists(fileName))
            {
                return null;
            }

            List<IDecayData> decayDataList = new List<IDecayData>();
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                EndfHelper.GetLineFromStream(streamReader, MF, MT, out string line);                
                Record r = EndfHelper.GetRecord(line);

                line = streamReader.ReadLine();                
                line = streamReader.ReadLine();
                line = streamReader.ReadLine();
                r = EndfHelper.GetRecord(line);
                int rt = Convert.ToInt16(r.n2);

                for (int i = 0; i < rt; i++)
                {
                    line = streamReader.ReadLine();
                    r = EndfHelper.GetRecord(line);
                    var decay = new DecayData((int)r.c1, r.l1, r.n1);
                    decayDataList.Add(decay);
                }
            }
            return decayDataList;
        }
    }
}
