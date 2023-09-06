using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Harmony
{
    public class HarmonyCache<TKey, TValue>
    {
        static Dictionary<Mod, HarmonyCache<TKey, TValue>> caches = new();
        static HarmonyCache()
        {
            Harmony.OnUnload += delegate
            {
                caches.Clear();
            };
        }
        internal static HarmonyCache<TKey, TValue> GetOrCreate(Mod mod)
        {
            if(!caches.TryGetValue(mod, out var cache))
            {
                cache = new HarmonyCache<TKey, TValue>(mod);
                caches.Add(mod, cache);
            }
            return cache;
        }
        Dictionary<TKey, TValue> cache = new();
        Mod _mod;
        HarmonyCache(Mod mod)
        {
            _mod = mod;
        }
        public TValue this[TKey key]
        {
            get
            {
                if(cache.TryGetValue(key, out var value))
                {
                    return value;
                }
                return default;
            }
            set
            {
                cache[key] = value;
            }
        }
        public int Count => cache.Count;
        public void Add(TKey key, TValue value)
        {
            cache.Add(key, value);
        }
        public bool Remove(TKey key)
        {
            return cache.Remove(key);
        }
        public bool ContainsKey(TKey key)
        {
            return cache.ContainsKey(key);
        }
        public bool ContainsValue(TValue value)
        {
            return cache.ContainsValue(value);
        }
        public void Clear()
        {
            cache.Clear();
        }
        public TValue Get(TKey key)
        {
            return cache.TryGetValue(key, out var value) ? value : default;
        }
        public bool TryGet(TKey key, out TValue value)
        {
            return cache.TryGetValue(key, out value);
        }
        public void ForEach(Action<TKey, TValue> action)
        {
            foreach(var key in cache.Keys)
            {
                action(key, cache[key]);
            }
        }
        public void Dispose(Action<Dictionary<TKey,TValue>> special=null)
        {
            special?.Invoke(cache);
            caches.Remove(_mod);
        }
    }
}
