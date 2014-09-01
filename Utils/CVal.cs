using System;

namespace Utils
{
    public class CVal<T>
    {
        public Func<T> GetValue { get; set; }

        public CVal(Func<T> getValue)
        {
            GetValue = getValue;
        }

        public CVal(T value)
        {
            GetValue = () => value;
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        public static implicit operator T(CVal<T> value)
        {
            if (value.GetValue != null)
            {
                return value.GetValue();
            }
            else
            {
                return default(T);
            }
        }

        public static implicit operator CVal<T>(T value)
        {
            return new CVal<T>(value);
        }
    }
}