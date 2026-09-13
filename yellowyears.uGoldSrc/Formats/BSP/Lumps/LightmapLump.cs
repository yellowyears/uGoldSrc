using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class LightmapLump : BSPLump
    {
        public List<Lightmap> Lightmaps { get; private set; } = new List<Lightmap>();

        public Texture2D LightmapAtlas { get; private set; }

        public LightmapLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_LIGHTING;
        }

        private const int AtlasWidth = 2048;
        private const int Padding = 1;

        public void PackLightmapAtlas(FaceLump faceLump, string modName, string mapName)
        {
            if (Lightmaps.Count == 0) return;

            var ordered = Lightmaps.OrderByDescending(b => b.Height).ToList();
            var placements = new Dictionary<Lightmap, (int x, int y)>();

            int x = 0, y = 0, rowHeight = 0;
            foreach (var lightmap in ordered)
            {
                int paddedW = lightmap.Width + Padding * 2;
                int paddedH = lightmap.Height + Padding * 2;

                if (x + paddedW > AtlasWidth)
                {
                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }

                placements[lightmap] = (x, y);
                x += paddedW;
                rowHeight = Mathf.Max(rowHeight, paddedH);
            }

            int atlasHeight = Mathf.NextPowerOfTwo(y + rowHeight);
            var buffer = new Color32[AtlasWidth * atlasHeight];

            void SetPixelSafe(int px, int py, Color32 c)
            {
                if (px < 0 || py < 0 || px >= AtlasWidth || py >= atlasHeight) return;
                buffer[py * AtlasWidth + px] = c;
            }

            foreach (var lightmap in ordered)
            {
                var (bx, by) = placements[lightmap];
                int originX = bx + Padding;
                int originY = by + Padding;

                // Copy the block's own pixels in
                for (int py = 0; py < lightmap.Height; py++)
                    for (int px = 0; px < lightmap.Width; px++)
                        SetPixelSafe(originX + px, originY + py, lightmap.Pixels[py * lightmap.Width + px]);

                // Duplicate edge pixels outward into the padding border — this is what
                // stops hardware bilinear filtering from ever blending in a neighbor's texels
                for (int p = 1; p <= Padding; p++)
                {
                    for (int px = 0; px < lightmap.Width; px++)
                    {
                        SetPixelSafe(originX + px, originY - p, lightmap.Pixels[px]); // above
                        SetPixelSafe(originX + px, originY + lightmap.Height - 1 + p, lightmap.Pixels[(lightmap.Height - 1) * lightmap.Width + px]); // below
                    }
                    for (int py = 0; py < lightmap.Height; py++)
                    {
                        SetPixelSafe(originX - p, originY + py, lightmap.Pixels[py * lightmap.Width]); // left
                        SetPixelSafe(originX + lightmap.Width - 1 + p, originY + py, lightmap.Pixels[py * lightmap.Width + lightmap.Width - 1]); // right
                    }
                    // corners
                    SetPixelSafe(originX - p, originY - p, lightmap.Pixels[0]);
                    SetPixelSafe(originX + lightmap.Width - 1 + p, originY - p, lightmap.Pixels[lightmap.Width - 1]);
                    SetPixelSafe(originX - p, originY + lightmap.Height - 1 + p, lightmap.Pixels[(lightmap.Height - 1) * lightmap.Width]);
                    SetPixelSafe(originX + lightmap.Width - 1 + p, originY + lightmap.Height - 1 + p, lightmap.Pixels[(lightmap.Height - 1) * lightmap.Width + lightmap.Width - 1]);
                }

                // Remap this face's local (0..1 within its own block) UVs into atlas space
                var face = faceLump.Faces[lightmap.FaceIndex];
                var atlasUVs = new List<Vector2>(lightmap.UVs.Count);
                foreach (var uv in lightmap.UVs)
                {
                    atlasUVs.Add(new Vector2(
                        (originX + uv.x * lightmap.Width) / AtlasWidth,
                        (originY + uv.y * lightmap.Height) / atlasHeight));
                }
                face.LightmapUVs = atlasUVs;
                face.LightmapIndex = 0; // all faces now share the one atlas
            }

            var atlas = new Texture2D(AtlasWidth, atlasHeight, TextureFormat.RGB24, false, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            atlas.SetPixels32(buffer);
            atlas.Apply();

            var savedAtlas = Utilities.SaveLightmapAtlas(atlas, Path.Combine("Assets/_uGoldSrc", "assets", modName, "lightmaps"), mapName);

            LightmapAtlas = savedAtlas;
        }
    }
}