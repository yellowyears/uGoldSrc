using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Model
    {

        public Vector3 BoxMin { get; private set; }
        public Vector3 BoxMax { get; private set; }
        public Vector3 Origin { get; private set; }
        public int[] Nodes { get; private set; } = new int[4];
        public int NumVisNodes { get; private set; }
        public int FirstFace { get; private set; }
        public int NumFaces { get; private set; }

        public const int TotalSize = 64;

        public Model(Vector3 boxMin, Vector3 boxMax, Vector3 origin, int[] nodes, int numVisNodes, int firstFace, int numFaces)
        {
            BoxMin = boxMin;
            BoxMax = boxMax;
            Origin = origin;
            Nodes = nodes;
            NumVisNodes = numVisNodes;
            FirstFace = firstFace;
            NumFaces = numFaces;
        }
    }
}