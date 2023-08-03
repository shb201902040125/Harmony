using System;
using Terraria;
using Terraria.ModLoader;

namespace Harmony
{
    public class Harmony : Mod
    {
        private static int? _mainThreadID = null;
        internal static event Action OnUnload;
        internal static event Action OnUnload_MainThread;
        internal static Harmony Instance;
        public static bool IsMainThread => _mainThreadID.HasValue && _mainThreadID == Environment.CurrentManagedThreadId;
        public override void Load()
        {
            Instance = this;
            Main.QueueMainThreadAction(() =>
            {
                _mainThreadID = Environment.CurrentManagedThreadId;
            });
            OnUnload += delegate { _mainThreadID = null; };
        }
        public override void Unload()
        {
            bool wait = true;
            if (OnUnload_MainThread is not null)
            {
                Main.QueueMainThreadAction(() =>
                {
                    OnUnload_MainThread();
                });
            }
            else
            {
                wait = false;
            }
            OnUnload?.Invoke();
            while (wait)
            {
                ;
            }
            Instance = null;
        }
    }
}