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
                    if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line)) continue;

                    var splitLine = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    var textureName = splitLine[0];

                    string[] colour = new string[splitLine.Length - 1];
                    for (int i = 1; i < colour.Length; i++)
                    {
                        colour[i - 1] = splitLine[i];
                    }

                    var entry = new RADEntry(textureName, Utilities.GetLightColour(colour));
                    entries.Add(entry);
                }
            }

            var rad = new RAD(entries);

            return rad;
        }
    }
}