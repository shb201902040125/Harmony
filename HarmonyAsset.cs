using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Harmony
{
    public class HarmonyAsset<T> where T : class
    {
        static HarmonyAsset()
        {
            Harmony.OnUnload_MainThread += delegate
            {
                foreach (var asset in dic.Values)
                {
                    if (asset?.IsDisposed ?? false)
                    {
                        continue;
                    }
                    asset.Dispose();
                }
                dic.Clear();
                dic = null;
            };
        }

        private static Dictionary<string, Asset<T>> dic = new();
        public bool IsSure
        {
            get
            {
                return dic.TryGetValue(Name, out var asset) && asset.State == AssetState.Loaded;
            }
        }
        private bool saveCache;
        public bool SaveCache
        {
            get => saveCache;
            set
            {
                CachedValue = saveCache && value ? CachedValue : null;
                saveCache = value;
            }
        }
        public T CachedValue { get; private set; }
        public readonly string Name;
        public HarmonyAsset(string name, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
        {
            Name = name;
            dic[name] = ModContent.Request<T>(name, mode);
        }
        private static Asset<T> SureLoad(HarmonyAsset<T> harmonyAsset)
        {
            if (dic.TryGetValue(harmonyAsset.Name, out Asset<T> asset))
            {
                if (asset.State == AssetState.Loaded)
                {
                    return asset;
                }
                else if (asset.State == AssetState.Loading)
                {
                    return null;
                }
                else
                {
                    if (Harmony.IsMainThread)
                    {
                        dic[asset.Name] = ModContent.Request<T>(harmonyAsset.Name, AssetRequestMode.ImmediateLoad);
                        return asset;
                    }
                    else
                    {
                        dic[asset.Name] = ModContent.Request<T>(harmonyAsset.Name, AssetRequestMode.AsyncLoad);
                        return null;
                    }
                }
            }
            else
            {
                if (Harmony.IsMainThread)
                {
                    dic[harmonyAsset.Name] = ModContent.Request<T>(harmonyAsset.Name, AssetRequestMode.ImmediateLoad);
                    return asset;
                }
                else
                {
                    dic[harmonyAsset.Name] = ModContent.Request<T>(harmonyAsset.Name, AssetRequestMode.AsyncLoad);
                    return null;
                }
            }
        }
        public void WhenSure(Action<T> action)
        {
            T value = SaveCache && CachedValue is not null ? CachedValue : SureLoad(this)?.Value ?? null;
            if (value is not null)
            {
                action(value);
            }
        }
        public void WhenSure(Action<Asset<T>> action)
        {
            Asset<T> asset;
            if ((asset = SureLoad(this)) is not null)
            {
                action(asset);
            }
        }
        public static implicit operator Asset<T>(HarmonyAsset<T> harmonyAsset)
        {
            return SureLoad(harmonyAsset);
        }
        public static implicit operator T(HarmonyAsset<T> harmonyAsset)
        {
            return SureLoad(harmonyAsset)?.Value ?? default;
        }
    }
}
