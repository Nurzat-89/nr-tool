using System;
using System.IO;
using System.Web;

namespace NuclearData
{
    internal class EndfHelper
    {
        private const int FIELD_WIDTH = 11;

        /// <summary>
        /// Get Record from line
        /// </summary>
        public static Record GetRecord(string line)
        {
            Record r = new Record();
            int p = 0;
            string s;

            s = line.Substring(p, FIELD_WIDTH); p += FIELD_WIDTH; r.c1 = ConvertToExp(s);
            s = line.Substring(p, FIELD_WIDTH); p += FIELD_WIDTH; r.c2 = ConvertToExp(s);
            s = line.Substring(p, FIELD_WIDTH); p += FIELD_WIDTH; r.l1 = ConvertToExp(s);
            s = line.Substring(p, FIELD_WIDTH); p += FIELD_WIDTH; r.l2 = ConvertToExp(s);
            s = line.Substring(p, FIELD_WIDTH); p += FIELD_WIDTH; r.n1 = ConvertToExp(s);
            s = line.Substring(p, FIELD_WIDTH); r.n2 = ConvertToExp(s);

            return (r);
        }

        /// <summary>
        /// Convert str to double exp value (e.g 1.2+6 -> 1.2E6)
        /// </summary>
        public static double ConvertToExp(string str)
        {
            /*** if blank, return zero */
            if (str == "           ") return (0.0);

            /*** search for E, if exists return the floating point number */
            bool found = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].ToString().ToUpper() == "E") found = true;
            }
            if (found) return double.Parse(str, System.Globalization.CultureInfo.InvariantCulture);

            int len = str.Length;
            int p1 = 0, p2 = 0, p3 = 0;

            /**** sign */
            int sig = 1;
            for (p1 = 0; p1 < len; p1++)
            {
                if (str[p1] == ' ') continue;
                else if (str[p1] == '+') sig = 1;
                else if (str[p1] == '-') sig = -1;
                else if ((str[p1] >= '0' && str[p1] <= '9') || str[p1] == '.') break;
            }

            /**** find + or - */
            found = false;
            char q = '+';
            for (p2 = p1; p2 < len; p2++)
            {
                if (str[p2] == '+' || str[p2] == '-')
                {
                    found = true;
                    q = str[p2];
                    break;
                }
                else if (str[p2] == ' ') continue;
            }

            /*** reconstruct the real number */
            char[] num = new char[20];
            int k = 0;
            for (int i = 0; i < (p2 - p1); i++)
            {
                if (str[i + p1] == ' ') continue;
                num[k++] = str[i + p1];
            }

            if (found)
            {
                num[k++] = 'e';
                num[k++] = q;
                for (p3 = p2 + 1; p3 < len; p3++)
                {
                    if (str[p3] == ' ') continue;
                    num[k++] = str[p3];
                }
            }
            num[k] = '\0';

            return sig * double.Parse(new string(num), System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// get line from stream reader
        /// </summary>
        public static bool GetLineFromStream(StreamReader streamReader, int MF, int MT, out string line) 
        {
            int mat, mfs, mts;
            string s;
            bool found = false;

            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Length < 75) continue;

                s = line.Substring(66, 4); mat = Convert.ToInt16(s);
                s = line.Substring(70, 2); mfs = Convert.ToInt16(s);
                s = line.Substring(72, 3); mts = Convert.ToInt16(s);

                if (mfs == MF && mts == MT)
                {
                    found = true;
                    break;
                }

                if ((mfs > MF) || ((mfs == MF) && (mts > MT)))
                {
                    break;
                }
            }

            return found;
        }
    }
}
