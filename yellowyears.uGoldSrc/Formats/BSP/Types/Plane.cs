using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Plane
    {

        /// <summary>
        /// The plane's normal vector
        /// </summary
        public Vector3 Normal { get; private set; }

        /// <summary>
        /// Plane equation is: vNormal * X = fDist
        /// </summary>
        public float Dist { get; private set; }

        /// <summary>
        /// Plane type
        /// </summary>
        public int Type { get; private set; }

        public const int TotalSize = 20;


        public Plane(Vector3 normal, float dist, int type)
        {
            Normal = -normal; // Inverted for GoldSrc
            Dist = dist;
            Type = type;
        }
    }
}