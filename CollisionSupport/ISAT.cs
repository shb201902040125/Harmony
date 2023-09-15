using System.Collections.Generic;

namespace Harmony.CollisionSupport
{
    public interface ISAT<T>
    {
        public List<T> GetAxes();
        public void Project(T axis, out float min, out float max);
    }
}
