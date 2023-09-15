using System.Collections.Generic;

namespace Harmony.Basics
{
    public class Override<T>
    {
        private readonly T _value;
        private T _overrideValue;
        public Override(T OrigValue)
        {
            _value = _overrideValue = OrigValue;
        }
        public T Value
        {
            get
            {
                return _overrideValue ?? _value;
            }
            set
            {
                _overrideValue = value;
            }
        }
        public bool IsOrig => _overrideValue is null || _overrideValue.Equals(_value);
        public void Reset()
        {
            _overrideValue = _value;
        }
        public static implicit operator T(Override<T> @override)
        {
            return @override.Value;
        }
        public static explicit operator Override<T>(T Value)
        {
            return new(Value);
        }
        public static explicit operator FocusOverride<T>(Override<T> Value)
        {
            return new(Value.Value, null, null);
        }
    }
}
