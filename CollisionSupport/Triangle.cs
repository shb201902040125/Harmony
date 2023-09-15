using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Harmony.CollisionSupport
{
    public class Triangle : IConvex<Vector2>, ISAT<Vector2>, IEPA<Vector2>
    {
        private readonly Vector2[] vertex = new Vector2[3];
        public Triangle(Vector2[] vertex)
        {
            if(vertex.Length!=3)
            {
                throw new ArgumentException("vertex must be 3");
            }
            this.vertex = vertex;
        }
        public IReadOnlyList<Vector2> ConvexVertex()
        {
            return vertex.ToList();
        }
        public Vector2 FurthestPoint(Vector2 dir)
        {
            Vector2 center = (vertex[0] + vertex[1] + vertex[2]) / 3;
            Vector2 result = vertex[0];
            Vector2 Vr = result - center;
            float d = Vector2.Dot(Vr, dir);
            for (int i = 1; i < 3; i++)
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
        public List<Vector2> GetAxes()
        {
            return new List<Vector2>() { vertex[1] - vertex[0], vertex[2] - vertex[1], vertex[0] - vertex[2] };
        }
        public void Project(Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (Vector2 point in vertex)
            {
                float projection = Vector2.Dot(point, axis);
                if (projection < min)
                {
                    min = projection;
                }
                if (projection > max)
                {
                    max = projection;
                }
            }
        }
        public static explicit operator NormalConvex2D(Triangle triangle)
        {
            return new NormalConvex2D(triangle.ConvexVertex());
        }
    }
}
