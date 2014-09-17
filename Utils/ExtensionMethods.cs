using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace Utils
{
    public static class ExtensionMethods
    {
        #region Maybe
        // For class Foo { public int Bar { get; set; } }
        // and Foo foo = null, you can write
        // foo.Maybe(f => f.Bar) and get back null rather than a null reference exception.

        // NOTE: In order to keep the compiler happy, there are a series of overloads here in order to handle cases
        // where the input and the output can be either a class or a nullable struct.  There are also overloads which
        // take a default value that may be used for cases where the output type is a non-nullable struct.

        public static TMember Maybe<TParent, TMember>(this TParent parent, Func<TParent, TMember> selector,
                                                      TMember defaultValue = default(TMember))
            where TParent : class
        {
            return parent == null ? defaultValue : selector(parent);
        }

        public static TMember Maybe<TParent, TMember>(this TParent? parent, Func<TParent?, TMember> selector,
                                                      TMember defaultValue = default(TMember))
            where TParent : struct
        {
            return parent == null ? defaultValue : selector(parent);
        }

        public static T Maybe<T>(this T? nullable, T defaultValue = default(T))
            where T : struct
        {
            return nullable.HasValue ? nullable.Value : defaultValue;
        }
        #endregion Maybe

        #region MaybeAction

        // Call a side-effecting method if a variable is non-null.  Otherwise silently skip.

        public static void MaybeAction<TParent>(this TParent parent, Action<TParent> action)
            where TParent : class
        {
            if (parent != null) action(parent);
        }

        public static void MaybeAction<TParent>(this TParent? parent, Action<TParent> action)
            where TParent : struct
        {
            if (parent != null) action(parent.Value);
        }

        #endregion MaybeAction

        #region Maybe for dictionaries

        // For Dictionary<int, string> foo
        // and some integer (say 5) which isn't a key in the dictionary, you can write
        // foo.Maybe(5) and get back null rather than a key not found exception

        // NOTE: There are overloads which handle default values rather than null and which combine with the maybe
        // for member access so that you can write something like foo.Maybe(5, v => v.Bar) which will give you the
        // value of a member bar of the thing found in the dictionary or null if it's not found in the dictionary

        public static TValue Maybe<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
                                                 TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null || (!typeof(TKey).IsValueType && key == null)) return defaultValue;

            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static TMember Maybe<TKey, TValue, TMember>(this Dictionary<TKey, TValue> dictionary,
                                                           TKey key, Func<TValue, TMember> selector,
                                                           TMember defaultValue = default(TMember))
            where TValue : class
        {
            return dictionary.Maybe(key).Maybe(selector, defaultValue);
        }

        public static TMember Maybe<TKey, TValue, TMember>(this Dictionary<TKey, TValue?> dictionary,
                                                           TKey key, Func<TValue?, TMember> selector,
                                                           TMember defaultValue = default(TMember))
            where TValue : struct
        {
            return dictionary.Maybe(key).Maybe(selector, defaultValue);
        }

        #endregion Maybe for dictionaries
    }
}