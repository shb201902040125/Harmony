using System.Collections.Generic;

namespace Harmony
{
    public class Override<T>
    {
        private readonly T _value;
        private T _overrideValue;
        private readonly List<T> _history = new();
        public Override(T OrigValue)
        {
            _value = _overrideValue = OrigValue;
        }
        public T Value
        {
            get
            {
                return _overrideValue;
            }
            set
            {
                _history.Add(_overrideValue);
                _overrideValue = value;
            }
        }
        public bool IsOrig => _overrideValue is null || _overrideValue.Equals(_value);
        public IReadOnlyList<T> OverrideHistory => _history;
        public void Reset()
        {
            _overrideValue = _value;
            _history.Clear();
        }
        public static implicit operator T(Override<T> @override)
        {
            return @override.Value;
        }
        public static explicit operator Override<T>(T Value)
        {
            return new(Value);
        }
    }
}
