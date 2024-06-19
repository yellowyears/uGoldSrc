using System.Collections.Generic;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.RAD.Types;

namespace yellowyears.uGoldSrc.Formats.RAD.Importer
{
    [CreateAssetMenu(fileName = "Lights", menuName = "Half-Lab/RAD")]
    public class RAD : ScriptableObject
    {
        public List<RADEntry> Entries;

        public RAD(List<RADEntry> entries)
        {
            Entries = entries;
        }
    }
}