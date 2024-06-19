using System.Collections.Generic;
using System.IO;
using yellowyears.uGoldSrc.Formats.RAD.Types;

namespace yellowyears.uGoldSrc.Formats.RAD.Importer
{
    public static class RADReader
    {
        public static RAD Read(string path)
        {
            var entries = new List<RADEntry>();

            var lines = File.ReadAllLines(path);
            if(lines.Length > 0)
            {
                foreach (var line in lines)
                {
                    if (line.StartsWith("//")) continue;

                    var splitLine = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    var textureName = splitLine[0];
                    var colour = new string[4] { splitLine[1], splitLine[2], splitLine[3], splitLine[4] };

                    var entry = new RADEntry(textureName, Utilities.GetLightColour(colour));
                    entries.Add(entry);
                }
            }

            var rad = new RAD(entries);

            return rad;
        }
    }
}