using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Harmony.CollisionSupport
{
    public class Rectangle : IConvex<Vector2>, ISAT<Vector2>, IEPA<Vector2>
    {
        private Vector2 leftTop;
        private float width, height;
        public Vector2 LeftTop => leftTop;
        public Vector2 RightTop => leftTop + new Vector2(width, 0);
        public Vector2 LeftBottom => leftTop + new Vector2(0, height);
        public Vector2 RightBottom => leftTop + new Vector2(width, height);
        public Rectangle(Vector2 pos, float width, float height)
        {
            leftTop = pos;
            this.width = width;
            this.height = height;
        }
        public Vector2 FurthestPoint(Vector2 dir)
        {
            Vector2 center = leftTop + new Vector2(width, height) / 2;
            Vector2 result = leftTop;
            Vector2 Vr = result - center;
            float d = Vector2.Dot(Vr, dir);
            foreach (Vector2 v in ConvexVertex())
            {
                Vector2 Vi = v - center;
                float di = Vector2.Dot(Vi, dir);
                if (di > d)
                {
                    d = di;
                    result = v;
                }
            }
            return result;
        }
        public List<Vector2> GetAxes()
        {
            return new List<Vector2>()
            {
                RightTop-LeftTop,
                RightBottom-RightTop,
                LeftBottom-RightBottom,
                LeftTop-LeftBottom
            };
        }
        public void Project(Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (Vector2 point in ConvexVertex())
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
        public IReadOnlyList<Vector2> ConvexVertex()
        {
            return new List<Vector2>() { LeftTop, RightTop, RightBottom, LeftBottom };
        }
        public static explicit operator Microsoft.Xna.Framework.Rectangle(Rectangle rect)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)rect.leftTop.X, (int)rect.leftTop.Y, (int)rect.width, (int)rect.height);
        }
        public static explicit operator NormalConvex2D(Rectangle rect)
        {
            return new NormalConvex2D(new Vector2[] { rect.LeftTop, rect.RightTop, rect.RightBottom, rect.LeftBottom });
        }
    }
}
