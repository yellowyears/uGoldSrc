using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Lightmap
    {
        public Color32[] Pixels { get; private set; }
        public Texture Texture { get; private set; }


        public Lightmap(Color32[] pixels, Texture texture)
        {
            Pixels = pixels;
            Texture = texture;
        }
    }
}