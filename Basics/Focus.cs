using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Harmony.Basics
{
    public class Focus<T>
    {
        public Focus(T value, Action<T> onRead = null, Action<T, Ref<T>> onWrite = null)
        {
            _value = value;
            if (onRead is not null)
            {
                OnRead += onRead;
            }
            if (onWrite is not null)
            {
                OnWrite += onWrite;
            }
        }
        public event Action<T> OnRead;
        public event Action<T, Ref<T>> OnWrite;
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
                Ref<T> v = new(value);
                OnWrite?.Invoke(_value, v);
                _value = v.Value;
            }
        }
        public static explicit operator FocusOverride<T>(Focus<T> Value)
        {
            return new(Value.Value, Value.OnRead, Value.OnWrite);
        }
    }
}