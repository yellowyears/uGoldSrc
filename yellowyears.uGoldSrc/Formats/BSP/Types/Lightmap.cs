using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Lightmap
    {
        public Color32[] Pixels { get; private set; }

        public Lightmap(Color32[] pixels)
        {
            Pixels = pixels;
        }
    }
}