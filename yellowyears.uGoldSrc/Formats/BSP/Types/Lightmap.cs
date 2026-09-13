using System.Collections.Generic;
using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Lightmap
    {
        public int FaceIndex { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Color32[] Pixels { get; private set; }
        public List<Vector2> UVs; // Local to this lightmap 


        public Lightmap(int faceIndex, int width, int height, Color32[] pixels, List<Vector2> uvs)
        {
            FaceIndex = faceIndex;
            Width = width;
            Height = height;
            Pixels = pixels;
            UVs = uvs;
        }
    }
}