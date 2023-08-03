using System;
using System.Security.Cryptography;
using Terraria.ModLoader;

namespace Harmony
{
    public static class Utils
    {
        private static MD5 md5 = MD5.Create();
        static Utils()
        {
            Harmony.OnUnload += delegate
            {
                md5 = null;
            };
        }
        public static TOutput[] Convert<TInput, TOutput>(this TInput[] inputs, Func<TInput, TOutput> convert)
        {
            TOutput[] output = new TOutput[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                output[i] = convert(inputs[i]);
            }
            return output;
        }
        public static bool CompareArray<T>(T[] array1, T[] array2,Func<T,T,bool> comparer)
        {
            if(comparer is null)
            {
                return false;
            }
            if(array1.Length != array2.Length)
            {
                return false;
            }
            int l = array1.Length;
            for(int i=0; i < l; i++)
            {
                if (!comparer(array1[i], array2[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public static HarmonyCache<TKey, TValue> Cache<TKey, TValue>(this Mod mod)
        {
            return HarmonyCache<TKey, TValue>.GetOrCreate(mod);
        }
        public static bool IsClassOrSubClass(this Type type,Type other)
        {
            return type.TypeHandle.Equals(other.TypeHandle) || type.IsSubclassOf(other);
        }
        public static T ModInstance<T>() where T:Mod
        {
            if(!HarmonyCache<Type,Mod>.GetOrCreate(Harmony.Instance).TryGet(typeof(T),out Mod mod))
            {
                HarmonyCache<Type, Mod>.GetOrCreate(Harmony.Instance)[typeof(T)] = mod = ModContent.GetInstance<T>();
            }
            return mod as T;
        }
    }
}
