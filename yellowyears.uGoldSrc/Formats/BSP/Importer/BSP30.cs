using yellowyears.uGoldSrc.Formats.BSP.Lumps;

namespace yellowyears.uGoldSrc.Formats.BSP.Importer
{
    public class BSP30
    {
        public BSPHeader Header { get; private set; }

        public EntityLump EntityLump { get; private set; }
        public PlaneLump PlaneLump { get; private set; }
        public MipTextureLump MipTextureLump { get; private set; }
        public VertexLump VertexLump { get; private set; }
        public TextureInfoLump TextureInfoLump { get; private set; }
        public FaceLump FaceLump { get; private set; }
        public LightmapLump LightmapLump { get; private set; }
        public LeafLump LeafLump { get; private set; }
        public MarkSurfaceLump MarkSurfaceLump { get; private set; }
        public EdgeLump EdgeLump { get; private set; }
        public SurfEdgeLump SurfEdgeLump { get; private set; }
        public ModelLump ModelLump { get; private set; }

        public BSP30 (BSPHeader header, EntityLump entityLump, PlaneLump planeLump, MipTextureLump mipTextureLump, VertexLump vertexLump, TextureInfoLump textureInfoLump, FaceLump faceLump, LightmapLump lightmapLump, LeafLump leafLump, MarkSurfaceLump markSurfaceLump, EdgeLump edgeLump, SurfEdgeLump surfEdgeLump, ModelLump modelLump)
        {
            Header = header;

            EntityLump = entityLump;
            PlaneLump = planeLump;
            MipTextureLump = mipTextureLump;
            VertexLump = vertexLump;
            TextureInfoLump = textureInfoLump;
            FaceLump = faceLump;
            LightmapLump = lightmapLump;
            LeafLump = leafLump;
            MarkSurfaceLump = markSurfaceLump;
            EdgeLump = edgeLump;
            SurfEdgeLump = surfEdgeLump;
            ModelLump = modelLump;
        }
    }
}