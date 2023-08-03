using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony
{
    internal class Debugger
    {
        static HashSet<string> errorInfos = new(), warnInfos = new(), logInfos = new();
        static Debugger()
        {
            Harmony.OnUnload += delegate
            {
                errorInfos.Clear();
                warnInfos.Clear();
                logInfos.Clear();
            };
        }
        public static void Error(Exception e, bool onlyOnce = true)
        {
            string msg = $"[HarmonyDebug:Error]\n\t{e.Message}";
            if (onlyOnce && errorInfos.Contains(msg))
            {
                return;
            }
            Harmony.Instance.Logger.Error(msg);
        }
        public static void Warn(string message, bool onlyOnce = true)
        {
            string msg = $"[HarmonyDebug:Warn]\n\t{message}";
            if (onlyOnce && warnInfos.Contains(message))
            {
                return;
            }
            Harmony.Instance.Logger.Warn(msg);
        }
        public static void Log(string message, bool onlyOnce = true)
        {
            string msg = $"[HarmonyDebug:Log]\n\t{message}";
            if (onlyOnce && logInfos.Contains(message))
            {
                return;
            }
            Harmony.Instance.Logger.Warn(msg);
        }
    }
}
