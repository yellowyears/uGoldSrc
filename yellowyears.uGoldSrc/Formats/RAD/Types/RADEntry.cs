using System;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.Common.Types;

namespace yellowyears.uGoldSrc.Formats.RAD.Types
{
    [Serializable]
    public class RADEntry
    {
        public string TextureName;
        public Color LightColour; // TODO: The alpha is capped at 255 in the inspector but stores the brightness which can be a lot higher

        public RADEntry(string textureName, Color lightColour)
        {
            TextureName = textureName;
            LightColour = lightColour;
        }

        public RADEntry(MipTexture mipTexture, Color lightColour)
        {
            TextureName = mipTexture.TextureName;
            LightColour = lightColour;
        }
    }
}