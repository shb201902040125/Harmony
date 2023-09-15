using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace Harmony.CollisionSupport
{
    public class Circle : IConvex<Vector2>, ISAT<Vector2>, IEPA<Vector2>
    {
        private uint samplingNumber = 8;
        private float radius;
        public Vector2 Center;
        public uint SamplingNumber
        {
            get => samplingNumber;
            set => samplingNumber = Math.Max(value, 3);
        }
        public float Radius
        {
            get => radius;
            set => radius = Math.Max(value, float.Epsilon);
        }
        public IReadOnlyList<Vector2> ConvexVertex()
        {
            List<Vector2> result = new();
            for (int i = 0; i < samplingNumber; i++)
            {
                float f = (MathHelper.TwoPi / samplingNumber * i);
                result.Add(Center + new Vector2((float)Math.Cos(f), (float)Math.Sin(f)) * radius);
            }
            return result;
        }
        public Vector2 FurthestPoint(Vector2 dir)
        {
            return Center + Vector2.Normalize(dir) * radius;
        }
        public List<Vector2> GetAxes()
        {
            var samplings = ConvexVertex();
            List<Vector2> axes = new();
            for (int i = 1; i < samplingNumber; i++)
            {
                axes.Add(samplings[i] - samplings[i - 1]);
            }
            axes.Add(samplings[0] - samplings[^1]);
            return axes;
        }
        public void Project(Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            Vector2[] vs = new Vector2[2] { Center + Vector2.Normalize(axis) * radius, Center - Vector2.Normalize(axis) * radius };
            foreach (Vector2 point in vs)
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
        public static explicit operator NormalConvex2D(Circle circle)
        {
            return new NormalConvex2D(circle.ConvexVertex());
        }
    }
}