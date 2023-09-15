using System.Collections.Generic;

namespace Harmony.CollisionSupport
{
    public interface IConvex<T>
    {
        public IReadOnlyList<T> ConvexVertex();
    }
}
