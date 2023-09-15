using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Harmony.Basics
{
    public class FocusOverride<T>
    {
        private readonly T _value;
        private T _overrideValue;
        public event Action<T> OnRead;
        public event Action<T, Ref<T>> OnWrite;
        public FocusOverride(T OrigValue, Action<T> onRead = null, Action<T, Ref<T>> onWrite = null)
        {
            _value = _overrideValue = OrigValue;
            if (onRead is not null)
            {
                OnRead += onRead;
            }
            if (onWrite is not null)
            {
                OnWrite += onWrite;
            }
        }
        public T Value
        {
            get
            {
                T ret = _overrideValue ?? _value;
                OnRead?.Invoke(ret);
                return ret;
            }
            set
            {
                Ref<T> v = new(value);
                OnWrite?.Invoke(_value, v);
                _overrideValue = v.Value;
            }
        }
        public bool IsOrig => _overrideValue is null || _overrideValue.Equals(_value);
        public void Reset()
        {
            _overrideValue = _value;
        }
        public static implicit operator T(FocusOverride<T> @override)
        {
            return @override.Value;
        }
        public static explicit operator FocusOverride<T>(T Value)
        {
            return new(Value);
        }
        public static implicit operator Override<T>(FocusOverride<T> focusOverride)
        {
            return new Override<T>(focusOverride._value) { Value = focusOverride.Value };
        }
        public static implicit operator Focus<T>(FocusOverride<T> focusOverride)
        {
            return new Focus<T>(focusOverride.Value, focusOverride.OnRead, focusOverride.OnWrite) { Value = focusOverride.Value };
        }
    }
}
