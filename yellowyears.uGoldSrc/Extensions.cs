using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace yellowyears.uGoldSrc
{
    public static class Extensions
    {
        #region Binary Reader Extensions

        public static Vector3 ReadVector3(this BinaryReader reader, bool fixVector3 = true, bool readShorts = false)
        {
            Vector3 tempVector3;

            if (readShorts)
                tempVector3 = new Vector3(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
            else
                tempVector3 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            if (fixVector3)
                return Utilities.FixVector3(tempVector3);
            else
                return tempVector3;
        }

        public static uint[] ReadUInt32Array(this BinaryReader br, int count)
        {
            uint[] array = new uint[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = br.ReadUInt32();
            }

            return array;
        }

        public static char[] ReadChars(this BinaryReader br, int count, bool returnIfIllegalChar)
        {
            var readChars = br.ReadChars(count);

            List<char> filteredChars = new List<char>();

            for(int i = 0; i < readChars.Length; i++)
            {
                if (readChars[i] == '\0')
                {
                    if(returnIfIllegalChar)
                        return filteredChars.ToArray();
                    else
                        continue;
                }
                filteredChars.Add(readChars[i]);
            }

            return filteredChars.ToArray();
        }

        #endregion
    }
}