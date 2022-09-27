using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static NuclearData.Constants;

namespace NuclearData
{
    /// <inheritdoc/>
    internal abstract class ReactionDataReader : INuclearDataReader<ICrossSectionValue>
    {
        /// <inheritdoc/>
        public abstract int MF { get; }

        /// <inheritdoc/>
        public abstract int MT { get; }

        /// <inheritdoc/>
        public abstract FILETYP ReactionType { get; }

        /// <inheritdoc/>
        public IEnumerable<ICrossSectionValue> ReadData(int Z, int A, string fileName)
        {
            int mat, mfs, mts;
            string s;

            if (!File.Exists(fileName))
            {
                return null;
            }
            List<ICrossSectionValue> crossSectionValueList = new List<ICrossSectionValue>();

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                if (EndfHelper.GetLineFromStream(streamReader, MF, MT, out string line))
                {
                    int i = 0;
                    line = streamReader.ReadLine();
                    line = streamReader.ReadLine();
                    Record r = EndfHelper.GetRecord(line);
                    int ns = Convert.ToInt16(r.c1);
                    i = -1;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Length < 75) continue;

                        s = line.Substring(66, 4); mat = Convert.ToInt16(s);
                        s = line.Substring(70, 2); mfs = Convert.ToInt16(s);
                        s = line.Substring(72, 3); mts = Convert.ToInt16(s);

                        r = EndfHelper.GetRecord(line);
                        i++; crossSectionValueList.Add(new CrossSectionValue(MT, r.c1, r.c2)); if (i >= ns) break;
                        i++; crossSectionValueList.Add(new CrossSectionValue(MT, r.l1, r.l2)); if (i >= ns) break;
                        i++; crossSectionValueList.Add(new CrossSectionValue(MT, r.n1, r.n2)); if (i >= ns) break;

                        if (mfs != MF || mts == 2) break;
                    }
                }
                return crossSectionValueList;
            }
        }
    }
}
