using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Basics
{
    public class Focus<T>
    {
        public Focus(T value, Action onRead = null, Action<T, T> onWrite = null)
        {
            _value = value;
        }
        public event Action<T> OnRead;
        public event Action<T, T> OnWrite;
        bool writeLock;
        T _value;
        public T Value
        {
            get
            {
                OnRead?.Invoke(_value);
                return _value;
            }
            set
            {
                if (writeLock)
                {
                    return;
                }
                writeLock = true;
                OnWrite?.Invoke(_value, value);
                _value = value;
                writeLock = false;
            }
        }
    }
}