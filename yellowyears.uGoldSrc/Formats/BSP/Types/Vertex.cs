using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Vertex
    {
        public Vector3 VertexPosition { get; private set; }

        public const int TotalSize = 12;

        public Vertex(Vector3 vertexPosition, float mapScale)
        {
            VertexPosition = vertexPosition * mapScale;
        }
    }
}