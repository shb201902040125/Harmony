using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Harmony.CollisionSupport
{
    public class NormalConvex2D : IConvex<Vector2>, IEPA<Vector2>
    {
        public NormalConvex2D(IEnumerable<Vector2> vs)
        {
            vertex = Collision_Vector2.QuickHull(vs);
        }
        protected readonly List<Vector2> vertex;
        public IReadOnlyList<Vector2> ConvexVertex()
        {
            return vertex;
        }
        public Vector2 FurthestPoint(Vector2 dir)
        {
            Vector2 center = Vector2.Zero;
            vertex.ForEach(v => center += v);
            center /= vertex.Count;
            Vector2 result = vertex[0];
            Vector2 Vr = result - center;
            float d = Vector2.Dot(Vr, dir);
            for (int i = 1; i < vertex.Count; i++)
            {
                Vector2 Vi = vertex[i] - center;
                float di = Vector2.Dot(Vi, dir);
                if (di > d)
                {
                    d = di;
                    result = vertex[i];
                }
            }
            return result;
        }
    }
}
