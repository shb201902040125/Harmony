using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Contents.Interfaces
{
    public interface ILifeMeasurable<T> where T :IComparable<T>
    {
        public abstract T LifeMax { get; protected set; }
        public abstract T Life { get; protected set; }
        public virtual bool DrawLife(SpriteBatch sprite)
        {
            return false;
        }
    }
}
